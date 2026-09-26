using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Arkus.Game.Validation;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Game.Authoring
{
    /// <summary>
    /// Engine-neutral authoritative authoring session. Canonical commit authority is internal;
    /// public callers can inspect current state and use plan/dry-run only.
    /// </summary>
    public sealed class TransactionalWorldAuthoringSession :
        IWorldStateSource,
        IWorldMutationService,
        IWorldValidationService,
        IWorldProvenanceService,
        ICanonicalWorldMutationCommitter
    {
        private const string FingerprintVersion = "arkus-world-mutation-request-v2";
        private const string PlanVersion = "arkus-world-mutation-plan-v2";
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        private readonly object _gate = new object();
        private readonly AuthoredWorldAnchor _journalBase;
        private AuthoringState _state;

        public TransactionalWorldAuthoringSession(WorldState initialState)
        {
            if (initialState == null) throw new ArgumentNullException(nameof(initialState));
            WorldStateValidator.ValidateOrThrow(initialState);
            _journalBase = AuthoredWorldAnchor.FromState(initialState);
            _state = AuthoringState.Initial(initialState);
        }

        public WorldState Current
        {
            get
            {
                lock (_gate)
                {
                    return _state.Current;
                }
            }
        }

        public CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Count != 0)
            {
                return InvalidProvenanceRequest("Authored journal read takes an empty request.");
            }

            lock (_gate)
            {
                var entries = new List<object?>();
                for (var index = 0; index < _state.Journal.Count; index++)
                {
                    entries.Add(_state.Journal[index].ToData());
                }

                return CapabilityInvocationResult.Succeeded(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["schemaId"] = WorldProvenanceContract.JournalSchemaId,
                    ["entrySchemaId"] = WorldProvenanceContract.EntrySchemaId,
                    ["base"] = _journalBase.ToData(),
                    ["current"] = AuthoredWorldAnchor.FromState(_state.Current).ToData(),
                    ["entryCount"] = _state.Journal.Count,
                    ["entries"] = entries.AsReadOnly()
                }));
            }
        }

        public CapabilityInvocationResult ValidateCurrent(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Count != 0) return InvalidRequest("$", "Current-state validation takes an empty request.");
            return CapabilityInvocationResult.Succeeded(WorldValidationEngine.ValidateCurrent(Capture().State).ToData());
        }

        public CapabilityInvocationResult ValidateProposed(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var parseError = ParseRequest(request, out var parsed);
            if (parseError != null) return parseError;

            var candidateError = BuildCandidate(Capture(), parsed, out _, out var validation);
            return candidateError ?? CapabilityInvocationResult.Succeeded(validation!.ToData());
        }

        public CapabilityInvocationResult Plan(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var parseError = ParseRequest(request, out var parsed);
            if (parseError != null) return parseError;

            var snapshot = Capture();
            var planError = BuildPlan(snapshot, parsed, out var plan);
            return planError ?? Success("plan", false, false, plan!);
        }

        public CapabilityInvocationResult DryRun(IReadOnlyDictionary<string, object?> request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var parseError = ParseRequest(request, out var parsed);
            if (parseError != null) return parseError;

            var snapshot = Capture();
            var planError = BuildPlan(snapshot, parsed, out var plan);
            return planError ?? Success("dry-run", false, false, plan!);
        }

        CapabilityInvocationResult ICanonicalWorldMutationCommitter.Apply(
            IReadOnlyDictionary<string, object?> request,
            InvocationResourceBudget resourceBudget)
        {
            return Apply(request, resourceBudget);
        }

        internal CapabilityInvocationResult Apply(
            IReadOnlyDictionary<string, object?> request,
            InvocationResourceBudget resourceBudget)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (resourceBudget == null) throw new ArgumentNullException(nameof(resourceBudget));
            var parseError = ParseRequest(request, out var parsed);
            if (parseError != null) return parseError;

            lock (_gate)
            {
                if (_state.Receipts.TryGetValue(parsed.IdempotencyKey, out var existing))
                {
                    return ReplayOrConflict(parsed, existing);
                }
            }

            var snapshot = Capture();
            var planError = BuildPlan(snapshot, parsed, out var plan);
            if (planError != null) return planError;

            lock (_gate)
            {
                if (_state.Receipts.TryGetValue(parsed.IdempotencyKey, out var racedReceipt))
                {
                    return ReplayOrConflict(parsed, racedReceipt);
                }

                if (_state.Journal.Count >= H0ResourceEnvelope.MaximumSessionTransactions)
                {
                    return CapabilityInvocationResult.Failed(H0ResourceDiagnostics.Exceeded(
                        "resource.session_transactions_exceeded",
                        "The H0 authored session reached its bounded transaction-history limit.",
                        "$",
                        "sessionTransactions",
                        H0ResourceEnvelope.MaximumSessionTransactions,
                        _state.Journal.Count + 1L,
                        "Export a canonical checkpoint and begin a new local lineage before authoring further changes."));
                }

                var actualHash = CanonicalWorldStateCodec.ComputeContentHash(_state.Current);
                if (_state.Current.Revision != plan!.BaseRevision ||
                    !string.Equals(actualHash, plan.BaseHash, StringComparison.Ordinal))
                {
                    return StaleWrite(plan.BaseRevision, plan.BaseHash, _state.Current.Revision, actualHash);
                }

                var provenanceError = BuildProvenanceEntry(
                    _state.Journal.Count + 1L,
                    parsed,
                    plan,
                    _state.Journal,
                    out var entry);
                if (provenanceError != null) return provenanceError;

                var nextJournal = new List<WorldMutationProvenanceEntry>(_state.Journal) { entry };
                var nextReceipts = new Dictionary<string, IdempotencyReceipt>(_state.Receipts, StringComparer.Ordinal)
                {
                    [parsed.IdempotencyKey] = new IdempotencyReceipt(parsed.Fingerprint, plan)
                };

                if (!resourceBudget.TryBeginPublication("canonical-mutation", out var budgetError))
                    return CapabilityInvocationResult.Failed(budgetError!);

                // Publish authored state, mutation receipt and journal append as one immutable
                // session-state reference while holding the accepted HK04 commit lock.
                _state = new AuthoringState(
                    plan.CandidateState,
                    nextReceipts,
                    nextJournal.AsReadOnly());
                resourceBudget.MarkPublicationCommitted();
            }

            return Success("apply", true, false, plan!);
        }

        private WorldSnapshot Capture()
        {
            lock (_gate)
            {
                return new WorldSnapshot(
                    _state.Current,
                    CanonicalWorldStateCodec.ComputeContentHash(_state.Current));
            }
        }

        private static CapabilityInvocationResult? BuildPlan(
            WorldSnapshot snapshot,
            ParsedMutationRequest request,
            out MutationPlan? plan)
        {
            plan = null;
            var candidateError = BuildCandidate(snapshot, request, out var candidate, out var validation);
            if (candidateError != null) return candidateError;
            if (!validation!.Valid) return InvalidCandidate(validation);

            var changes = BuildChanges(snapshot.State, candidate!);
            if (changes.Count == 0)
            {
                return Failure(
                    "world.change.no_effect",
                    "The accepted operations produce no net authorable-state change.",
                    "$.operations",
                    EmptyContext(),
                    false,
                    "Remove cancelling/no-op operations or change at least one canonical authorable value.");
            }

            var coverageIssues = WorldMutationCoverage.FindMismatches(snapshot.State, candidate!, changes);
            if (coverageIssues.Count != 0)
            {
                return Failure(
                    "world.change.plan_incomplete",
                    "The deterministic change set does not cover the candidate state's effective semantic changes.",
                    "$.operations",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["firstMismatch"] = coverageIssues[0]
                    },
                    false,
                    "Treat this as a canonical planner invariant failure; do not apply the request.");
            }

            var resultHash = CanonicalWorldStateCodec.ComputeContentHash(candidate!);
            var planId = ComputePlanId(snapshot, candidate!, resultHash, changes);
            plan = new MutationPlan(
                planId,
                snapshot.State.Id.Value,
                snapshot.State.SchemaVersion,
                snapshot.State.Revision,
                snapshot.Hash,
                candidate!,
                resultHash,
                changes);
            return null;
        }

        private static CapabilityInvocationResult? BuildCandidate(
            WorldSnapshot snapshot,
            ParsedMutationRequest request,
            out WorldState? candidate,
            out WorldValidationResult? validation)
        {
            candidate = null;
            validation = null;
            if (request.ExpectedRevision != snapshot.State.Revision)
            {
                return Failure(
                    "world.change.stale_revision",
                    "The mutation expected revision is stale.",
                    "$.expectedRevision",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["expected"] = request.ExpectedRevision,
                        ["actual"] = snapshot.State.Revision,
                        ["actualHash"] = snapshot.Hash
                    },
                    true,
                    "Refresh world.summary and rebuild the mutation against the current revision/hash.");
            }

            if (!string.Equals(request.ExpectedHash, snapshot.Hash, StringComparison.Ordinal))
            {
                return Failure(
                    "world.change.stale_hash",
                    "The mutation expected content hash is stale.",
                    "$.expectedHash",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["expected"] = request.ExpectedHash,
                        ["actual"] = snapshot.Hash,
                        ["revision"] = snapshot.State.Revision
                    },
                    true,
                    "Refresh world.summary and rebuild the mutation against the current revision/hash.");
            }

            if (snapshot.State.Revision == long.MaxValue)
            {
                return Failure(
                    "world.change.revision_exhausted",
                    "The canonical revision counter cannot advance further.",
                    "$.expectedRevision",
                    EmptyContext(),
                    false,
                    "Create a new canonical world lineage before applying further mutations.");
            }

            var objects = new Dictionary<string, WorldObject>(StringComparer.Ordinal);
            for (var index = 0; index < snapshot.State.Objects.Count; index++)
            {
                var value = snapshot.State.Objects[index];
                objects.Add(value.Id.Value, value);
            }

            var extensions = new Dictionary<string, WorldExtensionData>(StringComparer.Ordinal);
            for (var index = 0; index < snapshot.State.Extensions.Count; index++)
            {
                var value = snapshot.State.Extensions[index];
                extensions.Add(WorldMutationCoverage.ExtensionKey(value.Owner, value.SchemaVersion, value.SubjectId), value);
            }

            for (var index = 0; index < request.Operations.Count; index++)
            {
                var operationError = ApplyOperation(request.Operations[index], index, objects, extensions);
                if (operationError != null)
                {
                    return operationError;
                }
            }

            var objectValues = new List<WorldObject>(objects.Values);
            objectValues.Sort((left, right) => left.Id.CompareTo(right.Id));
            var extensionValues = new List<WorldExtensionData>(extensions.Values);
            extensionValues.Sort(CompareExtensions);

            var rawCandidate = new WorldStateCandidate(
                snapshot.State.Id,
                snapshot.State.Revision + 1,
                objectValues,
                extensionValues,
                snapshot.State.SchemaVersion);
            validation = WorldValidationEngine.ValidateCandidate(rawCandidate);
            if (!validation.Valid) return null;

            try
            {
                candidate = new WorldState(
                    snapshot.State.Id,
                    snapshot.State.Revision + 1,
                    objectValues,
                    extensionValues,
                    snapshot.State.SchemaVersion);
                var resourceError = WorldResourceLimits.ValidateState(candidate, "$.operations", out _);
                if (resourceError != null)
                {
                    candidate = null;
                    return CapabilityInvocationResult.Failed(resourceError);
                }
            }
            catch (WorldStateException exception)
            {
                return InternalValidationFailure(exception.MachineCode, exception.Message, exception.Path);
            }
            catch (ArgumentException exception)
            {
                return InternalValidationFailure("world.invalid_value", exception.Message, "$.operations");
            }
            return null;
        }

        private static CapabilityInvocationResult? ApplyOperation(
            MutationOperation operation,
            int operationIndex,
            IDictionary<string, WorldObject> objects,
            IDictionary<string, WorldExtensionData> extensions)
        {
            switch (operation.Kind)
            {
                case MutationOperationKind.PutObject:
                    objects[operation.Id!] = new WorldObject(
                        new WorldObjectId(operation.Id!),
                        new WorldTypeId(operation.TypeId!),
                        operation.ContainerId == null ? (WorldObjectId?)null : new WorldObjectId(operation.ContainerId),
                        BuildReferences(operation.References));
                    return null;

                case MutationOperationKind.RemoveObject:
                    if (!objects.Remove(operation.Id!))
                    {
                        return MissingResource(operationIndex, "world.object:" + operation.Id);
                    }

                    return null;

                case MutationOperationKind.PutExtension:
                    extensions[WorldMutationCoverage.ExtensionKey(
                        operation.Owner!,
                        operation.SchemaVersion,
                        ToObjectId(operation.SubjectId))] = new WorldExtensionData(
                            operation.Owner!,
                            operation.SchemaVersion,
                            operation.Payload!,
                            ToObjectId(operation.SubjectId),
                            BuildReferences(operation.Dependencies));
                    return null;

                case MutationOperationKind.RemoveExtension:
                    var key = WorldMutationCoverage.ExtensionKey(
                        operation.Owner!,
                        operation.SchemaVersion,
                        ToObjectId(operation.SubjectId));
                    if (!extensions.Remove(key))
                    {
                        return MissingResource(operationIndex, "world.extension:" + key);
                    }

                    return null;

                default:
                    throw new InvalidOperationException("Unknown parsed mutation operation.");
            }
        }

        private static IReadOnlyList<WorldReference> BuildReferences(IReadOnlyList<ReferenceValue> values)
        {
            var references = new List<WorldReference>();
            for (var index = 0; index < values.Count; index++)
            {
                references.Add(new WorldReference(
                    new WorldReferenceKind(values[index].Kind),
                    new WorldObjectId(values[index].TargetId)));
            }

            return references.AsReadOnly();
        }

        private static WorldObjectId? ToObjectId(string? value)
        {
            return value == null ? (WorldObjectId?)null : new WorldObjectId(value);
        }

        private static IReadOnlyList<WorldMutationChange> BuildChanges(WorldState before, WorldState after)
        {
            var changes = new List<WorldMutationChange>();
            var beforeObjects = ObjectsById(before);
            var afterObjects = ObjectsById(after);
            var objectIds = new SortedSet<string>(beforeObjects.Keys, StringComparer.Ordinal);
            objectIds.UnionWith(afterObjects.Keys);

            foreach (var id in objectIds)
            {
                beforeObjects.TryGetValue(id, out var oldValue);
                afterObjects.TryGetValue(id, out var newValue);
                var fields = new List<string>();
                if (oldValue == null || newValue == null) fields.Add("existence");
                if (oldValue == null || newValue == null || oldValue.TypeId != newValue.TypeId) fields.Add("typeId");
                if (oldValue == null || newValue == null || oldValue.ContainerId != newValue.ContainerId) fields.Add("containerId");

                var oldReferences = ReferenceSet(oldValue);
                var newReferences = ReferenceSet(newValue);
                var added = Difference(newReferences, oldReferences);
                var removed = Difference(oldReferences, newReferences);
                if (added.Count != 0 || removed.Count != 0) fields.Add("references");

                if (fields.Count != 0 || added.Count != 0 || removed.Count != 0)
                {
                    changes.Add(new WorldMutationChange(
                        "world.object:" + id,
                        oldValue == null ? "create" : newValue == null ? "remove" : "update",
                        fields,
                        added,
                        removed));
                }
            }

            var beforeExtensions = ExtensionsByKey(before);
            var afterExtensions = ExtensionsByKey(after);
            var extensionKeys = new SortedSet<string>(beforeExtensions.Keys, StringComparer.Ordinal);
            extensionKeys.UnionWith(afterExtensions.Keys);
            foreach (var key in extensionKeys)
            {
                beforeExtensions.TryGetValue(key, out var oldValue);
                afterExtensions.TryGetValue(key, out var newValue);
                var fields = new List<string>();
                if (oldValue == null || newValue == null) fields.Add("existence");
                if (oldValue == null || newValue == null || !PayloadEquals(oldValue, newValue)) fields.Add("payload");
                var oldDependencies = ExtensionDependencySet(oldValue);
                var newDependencies = ExtensionDependencySet(newValue);
                var added = Difference(newDependencies, oldDependencies);
                var removed = Difference(oldDependencies, newDependencies);
                if (added.Count != 0 || removed.Count != 0) fields.Add("dependencies");
                if (fields.Count != 0 || added.Count != 0 || removed.Count != 0)
                {
                    changes.Add(new WorldMutationChange(
                        "world.extension:" + key,
                        oldValue == null ? "create" : newValue == null ? "remove" : "update",
                        fields,
                        added,
                        removed));
                }
            }

            return changes.AsReadOnly();
        }

        private static CapabilityInvocationResult? ParseRequest(
            IReadOnlyDictionary<string, object?> request,
            out ParsedMutationRequest parsed)
        {
            parsed = null!;
            if (!TryGetString(request, "idempotencyKey", out var idempotencyKey) || !IsStableToken(idempotencyKey))
            {
                return InvalidRequest("$.idempotencyKey", "idempotencyKey must be a 1-128 character stable token.");
            }

            if (!TryGetInteger(request, "expectedRevision", out var expectedRevision) || expectedRevision < 0)
            {
                return InvalidRequest("$.expectedRevision", "expectedRevision must be a non-negative integer.");
            }

            if (!TryGetString(request, "expectedHash", out var expectedHash) || !IsCanonicalHash(expectedHash))
            {
                return InvalidRequest("$.expectedHash", "expectedHash must be a lowercase 64-character SHA-256 hex string.");
            }

            if (!request.TryGetValue("operations", out var rawOperations) ||
                !(rawOperations is IReadOnlyList<object?> operationValues) ||
                operationValues.Count == 0)
            {
                return InvalidRequest(
                    "$.operations",
                    "operations must contain between 1 and " + WorldMutationContract.MaximumOperations.ToString(CultureInfo.InvariantCulture) + " entries.");
            }

            if (operationValues.Count > WorldMutationContract.MaximumOperations)
            {
                return CapabilityInvocationResult.Failed(H0ResourceDiagnostics.Exceeded(
                    "resource.batch_operations_exceeded",
                    "The mutation batch exceeds the measured H0 atomic operation envelope.",
                    "$.operations",
                    "batchOperations",
                    WorldMutationContract.MaximumOperations,
                    operationValues.Count,
                    "Keep the coherent intent at or below the advertised 96-operation atomic envelope."));
            }

            var operations = new List<MutationOperation>();
            for (var index = 0; index < operationValues.Count; index++)
            {
                if (!(operationValues[index] is IReadOnlyDictionary<string, object?> operationData))
                {
                    return InvalidRequest(OperationPath(index), "Each operation must be an object from the declared mutation grammar.");
                }

                var operationError = ParseOperation(operationData, index, out var operation);
                if (operationError != null) return operationError;
                operations.Add(operation!);
            }

            long payloadBytes = 0;
            for (var index = 0; index < operations.Count; index++)
            {
                if (operations[index].Payload != null)
                    payloadBytes = checked(payloadBytes + operations[index].Payload!.Length);
            }
            if (payloadBytes > H0ResourceEnvelope.MaximumBatchPayloadBytes)
            {
                return CapabilityInvocationResult.Failed(H0ResourceDiagnostics.Exceeded(
                    "resource.batch_payload_exceeded",
                    "The mutation batch exceeds the H0 decoded-payload byte limit.",
                    "$.operations",
                    "decodedBatchPayloadBytes",
                    H0ResourceEnvelope.MaximumBatchPayloadBytes,
                    payloadBytes,
                    "Reduce opaque payload volume without splitting the accepted coherent authoring intent."));
            }

            var fingerprint = ComputeRequestFingerprint(expectedRevision, expectedHash, operations);
            parsed = new ParsedMutationRequest(
                idempotencyKey,
                expectedRevision,
                expectedHash,
                operations.AsReadOnly(),
                fingerprint);
            return null;
        }

        private static CapabilityInvocationResult? ParseOperation(
            IReadOnlyDictionary<string, object?> data,
            int index,
            out MutationOperation? operation)
        {
            operation = null;
            if (!TryGetString(data, "kind", out var kind))
            {
                return InvalidRequest(OperationPath(index) + ".kind", "Operation kind is required.");
            }

            switch (kind)
            {
                case "put-object":
                    if (!OnlyFields(data, PutObjectFields))
                        return GrammarViolation(data, index, kind, PutObjectFields, PutObjectRequired);
                    if (!TryStableToken(data, "id", out var id))
                        return InvalidRequest(OperationPath(index) + ".id", "put-object id must be a stable token.");
                    if (!TryStableToken(data, "typeId", out var typeId))
                        return InvalidRequest(OperationPath(index) + ".typeId", "put-object typeId must be a stable token.");
                    string? containerId = null;
                    if (data.ContainsKey("containerId"))
                    {
                        if (!TryStableToken(data, "containerId", out var parsedContainer))
                            return InvalidRequest(OperationPath(index) + ".containerId", "containerId must be a stable token.");
                        containerId = parsedContainer;
                    }

                    var referenceError = ParseReferences(data, index, "references", out var references);
                    if (referenceError != null) return referenceError;
                    operation = MutationOperation.PutObject(id, typeId, containerId, references);
                    return null;

                case "remove-object":
                    if (!OnlyFields(data, RemoveObjectFields))
                        return GrammarViolation(data, index, kind, RemoveObjectFields, RemoveObjectRequired);
                    if (!TryStableToken(data, "id", out var removeId))
                        return InvalidRequest(OperationPath(index) + ".id", "remove-object id must be a stable token.");
                    operation = MutationOperation.RemoveObject(removeId);
                    return null;

                case "put-extension":
                    if (!OnlyFields(data, PutExtensionFields))
                        return GrammarViolation(data, index, kind, PutExtensionFields, PutExtensionRequired);
                    if (!TryStableToken(data, "owner", out var owner))
                        return InvalidRequest(OperationPath(index) + ".owner", "Extension owner must be a stable token.");
                    if (!TryGetInteger(data, "schemaVersion", out var schemaVersion) || schemaVersion <= 0 || schemaVersion > int.MaxValue)
                        return InvalidRequest(OperationPath(index) + ".schemaVersion", "Extension schemaVersion must be a positive 32-bit integer.");
                    if (!TryGetString(data, "payloadBase64", out var payloadText) || !TryCanonicalBase64(payloadText, out var payload))
                        return InvalidRequest(OperationPath(index) + ".payloadBase64", "payloadBase64 must use canonical Base64 encoding.");
                    if (payload.Length > H0ResourceEnvelope.MaximumExtensionPayloadBytes)
                    {
                        return CapabilityInvocationResult.Failed(H0ResourceDiagnostics.Exceeded(
                            "resource.extension_payload_exceeded",
                            "One extension payload exceeds the H0 per-resource byte limit.",
                            OperationPath(index) + ".payloadBase64",
                            "extensionPayloadBytes",
                            H0ResourceEnvelope.MaximumExtensionPayloadBytes,
                            payload.Length,
                            "Reduce this extension payload before retrying the same coherent transaction."));
                    }
                    string? subjectId = null;
                    if (data.ContainsKey("subjectId"))
                    {
                        if (!TryStableToken(data, "subjectId", out var parsedSubject))
                            return InvalidRequest(OperationPath(index) + ".subjectId", "subjectId must be a stable token.");
                        subjectId = parsedSubject;
                    }

                    var dependencyError = ParseReferences(data, index, "dependencies", out var dependencies);
                    if (dependencyError != null) return dependencyError;
                    operation = MutationOperation.PutExtension(owner, (int)schemaVersion, subjectId, dependencies, payload);
                    return null;

                case "remove-extension":
                    if (!OnlyFields(data, RemoveExtensionFields))
                        return GrammarViolation(data, index, kind, RemoveExtensionFields, RemoveExtensionRequired);
                    if (!TryStableToken(data, "owner", out var removeOwner))
                        return InvalidRequest(OperationPath(index) + ".owner", "Extension owner must be a stable token.");
                    if (!TryGetInteger(data, "schemaVersion", out var removeVersion) || removeVersion <= 0 || removeVersion > int.MaxValue)
                        return InvalidRequest(OperationPath(index) + ".schemaVersion", "Extension schemaVersion must be a positive 32-bit integer.");
                    string? removeSubjectId = null;
                    if (data.ContainsKey("subjectId"))
                    {
                        if (!TryStableToken(data, "subjectId", out var parsedSubject))
                            return InvalidRequest(OperationPath(index) + ".subjectId", "subjectId must be a stable token.");
                        removeSubjectId = parsedSubject;
                    }

                    operation = MutationOperation.RemoveExtension(removeOwner, (int)removeVersion, removeSubjectId);
                    return null;

                default:
                    return InvalidRequest(OperationPath(index) + ".kind", "Unknown mutation operation kind.");
            }
        }

        private static CapabilityInvocationResult? ParseReferences(
            IReadOnlyDictionary<string, object?> data,
            int operationIndex,
            string fieldName,
            out IReadOnlyList<ReferenceValue> references)
        {
            references = Array.Empty<ReferenceValue>();
            if (!data.TryGetValue(fieldName, out var raw)) return null;
            if (!(raw is IReadOnlyList<object?> values))
            {
                return InvalidRequest(OperationPath(operationIndex) + "." + fieldName, fieldName + " must be an array.");
            }

            if (values.Count > H0ResourceEnvelope.MaximumRelationsPerResource)
            {
                return CapabilityInvocationResult.Failed(H0ResourceDiagnostics.Exceeded(
                    "resource.relation_count_exceeded",
                    "One authored resource exceeds the H0 relation-count limit.",
                    OperationPath(operationIndex) + "." + fieldName,
                    "relationsPerResource",
                    H0ResourceEnvelope.MaximumRelationsPerResource,
                    values.Count,
                    "Reduce the relations attached to this resource and retry."));
            }

            var parsed = new List<ReferenceValue>();
            for (var index = 0; index < values.Count; index++)
            {
                if (!(values[index] is IReadOnlyDictionary<string, object?> reference) ||
                    !OnlyFields(reference, "kind", "targetId") ||
                    !TryStableToken(reference, "kind", out var kind) ||
                    !TryStableToken(reference, "targetId", out var targetId))
                {
                    return InvalidRequest(
                        OperationPath(operationIndex) + "." + fieldName + "[" + index.ToString(CultureInfo.InvariantCulture) + "]",
                        "Each reference requires only stable-token kind and targetId fields.");
                }

                parsed.Add(new ReferenceValue(kind, targetId));
            }

            parsed.Sort((left, right) =>
            {
                var comparison = StringComparer.Ordinal.Compare(left.Kind, right.Kind);
                return comparison != 0
                    ? comparison
                    : StringComparer.Ordinal.Compare(left.TargetId, right.TargetId);
            });
            references = parsed.AsReadOnly();
            return null;
        }

        private static CapabilityInvocationResult ReplayOrConflict(
            ParsedMutationRequest request,
            IdempotencyReceipt receipt)
        {
            if (!string.Equals(request.Fingerprint, receipt.Fingerprint, StringComparison.Ordinal))
            {
                return Failure(
                    "world.change.idempotency_conflict",
                    "This idempotency key was already accepted for different mutation semantics.",
                    "$.idempotencyKey",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["idempotencyKey"] = request.IdempotencyKey
                    },
                    false,
                    "Use the original request for this key or allocate a new idempotency key for different semantics.");
            }

            return Success("apply", true, true, receipt.Plan);
        }

        private static CapabilityInvocationResult StaleWrite(
            long expectedRevision,
            string expectedHash,
            long actualRevision,
            string actualHash)
        {
            return Failure(
                "world.change.concurrent_update",
                "Canonical state advanced after planning; the mutation was not committed.",
                "$.expectedRevision",
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["expectedRevision"] = expectedRevision,
                    ["expectedHash"] = expectedHash,
                    ["actualRevision"] = actualRevision,
                    ["actualHash"] = actualHash
                },
                true,
                "Refresh state, rebuild the plan against the new revision/hash and retry with a new idempotency key.");
        }

        private static CapabilityInvocationResult Success(string mode, bool persisted, bool replayed, MutationPlan plan)
        {
            return CapabilityInvocationResult.Succeeded(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["mode"] = mode,
                ["persisted"] = persisted,
                ["replayed"] = replayed,
                ["plan"] = PlanData(plan)
            }));
        }

        private CapabilityInvocationResult? BuildProvenanceEntry(
            long sequence,
            ParsedMutationRequest request,
            MutationPlan plan,
            IReadOnlyList<WorldMutationProvenanceEntry> currentJournal,
            out WorldMutationProvenanceEntry entry)
        {
            entry = null!;
            var baseAnchor = new AuthoredWorldAnchor(
                plan.WorldId,
                plan.SchemaVersion,
                plan.BaseRevision,
                plan.BaseHash);
            var resultHash = CanonicalWorldStateCodec.ComputeContentHash(plan.CandidateState);
            var resultAnchor = new AuthoredWorldAnchor(
                plan.WorldId,
                plan.SchemaVersion,
                plan.CandidateState.Revision,
                resultHash);

            var previous = currentJournal.Count == 0
                ? _journalBase
                : currentJournal[currentJournal.Count - 1].Result;
            if (!AnchorsEqual(previous, baseAnchor) ||
                request.ExpectedRevision != baseAnchor.Revision ||
                !string.Equals(request.ExpectedHash, baseAnchor.Hash, StringComparison.Ordinal) ||
                plan.CandidateState.Revision != plan.BaseRevision + 1 ||
                !string.Equals(plan.ResultHash, resultHash, StringComparison.Ordinal))
            {
                return Failure(
                    "world.provenance.commit_mismatch",
                    "The mutation journal entry disagrees with the canonical state transition and was not committed.",
                    "$",
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["sequence"] = sequence,
                        ["baseRevision"] = plan.BaseRevision,
                        ["resultRevision"] = plan.CandidateState.Revision
                    },
                    false,
                    "Treat this as an Arkus authored-state/provenance invariant failure; do not report mutation success.");
            }

            var affected = new SortedSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < plan.Changes.Count; index++)
            {
                affected.Add(plan.Changes[index].Resource);
            }

            if (affected.Count == 0)
            {
                return Failure(
                    "world.provenance.empty_effect",
                    "A persisted mutation must identify at least one affected authored resource.",
                    "$.operations",
                    EmptyContext(),
                    false,
                    "Treat this as an Arkus mutation-plan/provenance invariant failure; do not commit the request.");
            }

            var affectedResources = new List<string>(affected);
            var entryId = ComputeProvenanceEntryId(
                sequence,
                request,
                baseAnchor,
                resultAnchor,
                affectedResources);
            entry = new WorldMutationProvenanceEntry(
                entryId,
                sequence,
                WorldMutationContract.ApplyName,
                WorldMutationContract.ContractVersionText,
                request.IdempotencyKey,
                request.Fingerprint,
                NormalizedRequestData(request),
                baseAnchor,
                resultAnchor,
                affectedResources.AsReadOnly());
            return null;
        }

        private static bool AnchorsEqual(AuthoredWorldAnchor left, AuthoredWorldAnchor right)
        {
            return string.Equals(left.WorldId, right.WorldId, StringComparison.Ordinal) &&
                left.StateSchemaVersion == right.StateSchemaVersion &&
                left.Revision == right.Revision &&
                string.Equals(left.Hash, right.Hash, StringComparison.Ordinal);
        }

        private static string ComputeProvenanceEntryId(
            long sequence,
            ParsedMutationRequest request,
            AuthoredWorldAnchor baseAnchor,
            AuthoredWorldAnchor resultAnchor,
            IReadOnlyList<string> affectedResources)
        {
            var builder = new StringBuilder();
            AppendIdentityField(builder, WorldProvenanceContract.EntrySchemaId);
            AppendIdentityField(builder, sequence.ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(builder, WorldMutationContract.ApplyName);
            AppendIdentityField(builder, WorldMutationContract.ContractVersionText);
            AppendIdentityField(builder, request.IdempotencyKey);
            AppendIdentityField(builder, request.Fingerprint);
            AppendIdentityField(builder, baseAnchor.WorldId);
            AppendIdentityField(builder, baseAnchor.StateSchemaVersion.ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(builder, baseAnchor.Revision.ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(builder, baseAnchor.Hash);
            AppendIdentityField(builder, resultAnchor.Revision.ToString(CultureInfo.InvariantCulture));
            AppendIdentityField(builder, resultAnchor.Hash);
            for (var index = 0; index < affectedResources.Count; index++)
            {
                AppendIdentityField(builder, affectedResources[index]);
            }

            return Sha256(builder.ToString());
        }

        private static void AppendIdentityField(StringBuilder builder, string value)
        {
            builder.Append(value.Length.ToString(CultureInfo.InvariantCulture))
                .Append(':')
                .Append(value)
                .Append('\n');
        }

        private static IReadOnlyDictionary<string, object?> NormalizedRequestData(ParsedMutationRequest request)
        {
            var operations = new List<object?>();
            for (var index = 0; index < request.Operations.Count; index++)
            {
                operations.Add(NormalizedOperationData(request.Operations[index]));
            }

            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["idempotencyKey"] = request.IdempotencyKey,
                ["expectedRevision"] = request.ExpectedRevision,
                ["expectedHash"] = request.ExpectedHash,
                ["operations"] = operations.AsReadOnly()
            });
        }

        private static IReadOnlyDictionary<string, object?> NormalizedOperationData(MutationOperation operation)
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal);
            switch (operation.Kind)
            {
                case MutationOperationKind.PutObject:
                    data["kind"] = "put-object";
                    data["id"] = operation.Id;
                    data["typeId"] = operation.TypeId;
                    if (operation.ContainerId != null) data["containerId"] = operation.ContainerId;
                    data["references"] = ReferenceData(operation.References);
                    break;
                case MutationOperationKind.RemoveObject:
                    data["kind"] = "remove-object";
                    data["id"] = operation.Id;
                    break;
                case MutationOperationKind.PutExtension:
                    data["kind"] = "put-extension";
                    data["owner"] = operation.Owner;
                    data["schemaVersion"] = operation.SchemaVersion;
                    if (operation.SubjectId != null) data["subjectId"] = operation.SubjectId;
                    data["dependencies"] = ReferenceData(operation.Dependencies);
                    data["payloadBase64"] = Convert.ToBase64String(operation.Payload!);
                    break;
                case MutationOperationKind.RemoveExtension:
                    data["kind"] = "remove-extension";
                    data["owner"] = operation.Owner;
                    data["schemaVersion"] = operation.SchemaVersion;
                    if (operation.SubjectId != null) data["subjectId"] = operation.SubjectId;
                    break;
                default:
                    throw new InvalidOperationException("Unknown parsed mutation operation.");
            }

            return ReadOnly(data);
        }

        private static IReadOnlyList<object?> ReferenceData(IReadOnlyList<ReferenceValue> references)
        {
            var values = new List<object?>();
            for (var index = 0; index < references.Count; index++)
            {
                values.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["kind"] = references[index].Kind,
                    ["targetId"] = references[index].TargetId
                }));
            }

            return values.AsReadOnly();
        }

        private static IReadOnlyDictionary<string, object?> PlanData(MutationPlan plan)
        {
            var changes = new List<object?>();
            for (var index = 0; index < plan.Changes.Count; index++)
            {
                var change = plan.Changes[index];
                changes.Add(ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["resource"] = change.Resource,
                    ["action"] = change.Action,
                    ["fields"] = ToObjectList(change.Fields),
                    ["referencesAdded"] = ToObjectList(change.ReferencesAdded),
                    ["referencesRemoved"] = ToObjectList(change.ReferencesRemoved)
                }));
            }

            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["planId"] = plan.PlanId,
                ["base"] = WorldData(plan.WorldId, plan.SchemaVersion, plan.BaseRevision, plan.BaseHash),
                ["result"] = WorldData(plan.WorldId, plan.SchemaVersion, plan.CandidateState.Revision, plan.ResultHash),
                ["changes"] = changes.AsReadOnly(),
                ["preconditions"] = new List<object?>
                {
                    Condition("world.expected-revision-match", "$.expectedRevision"),
                    Condition("world.expected-hash-match", "$.expectedHash")
                }.AsReadOnly(),
                ["postconditions"] = new List<object?>
                {
                    Condition("world.state-valid", "$.operations"),
                    Condition("world.revision-advanced", "$.expectedRevision"),
                    Condition("world.change-set-covers-effects", "$.operations")
                }.AsReadOnly()
            });
        }

        private static IReadOnlyDictionary<string, object?> WorldData(
            string worldId,
            int schemaVersion,
            long revision,
            string hash)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["worldId"] = worldId,
                ["schemaVersion"] = schemaVersion,
                ["revision"] = revision,
                ["hash"] = hash
            });
        }

        private static IReadOnlyDictionary<string, object?> Condition(string code, string path)
        {
            return ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["code"] = code,
                ["path"] = path,
                ["satisfied"] = true
            });
        }

        private static string ComputeRequestFingerprint(
            long expectedRevision,
            string expectedHash,
            IReadOnlyList<MutationOperation> operations)
        {
            var builder = new StringBuilder();
            builder.Append(FingerprintVersion).Append('\n');
            builder.Append(expectedRevision.ToString(CultureInfo.InvariantCulture)).Append('\n');
            builder.Append(expectedHash).Append('\n');
            for (var index = 0; index < operations.Count; index++)
            {
                operations[index].AppendFingerprint(builder);
            }

            return Sha256(builder.ToString());
        }

        private static string ComputePlanId(
            WorldSnapshot snapshot,
            WorldState candidate,
            string resultHash,
            IReadOnlyList<WorldMutationChange> changes)
        {
            var builder = new StringBuilder();
            builder.Append(PlanVersion).Append('\n');
            builder.Append(snapshot.State.Id.Value).Append('\n');
            builder.Append(snapshot.State.Revision.ToString(CultureInfo.InvariantCulture)).Append('\n');
            builder.Append(snapshot.Hash).Append('\n');
            builder.Append(candidate.Revision.ToString(CultureInfo.InvariantCulture)).Append('\n');
            builder.Append(resultHash).Append('\n');
            for (var index = 0; index < changes.Count; index++)
            {
                var change = changes[index];
                builder.Append(change.Resource).Append('\t').Append(change.Action).Append('\t');
                builder.Append(string.Join(",", change.Fields)).Append('\t');
                builder.Append(string.Join(",", change.ReferencesAdded)).Append('\t');
                builder.Append(string.Join(",", change.ReferencesRemoved)).Append('\n');
            }

            return Sha256(builder.ToString());
        }

        private static string Sha256(string value)
        {
            byte[] digest;
            using (var sha = SHA256.Create())
            {
                digest = sha.ComputeHash(StrictUtf8.GetBytes(value));
            }

            const string alphabet = "0123456789abcdef";
            var characters = new char[digest.Length * 2];
            for (var index = 0; index < digest.Length; index++)
            {
                characters[index * 2] = alphabet[digest[index] >> 4];
                characters[index * 2 + 1] = alphabet[digest[index] & 0x0f];
            }

            return new string(characters);
        }

        private static Dictionary<string, WorldObject> ObjectsById(WorldState state)
        {
            var values = new Dictionary<string, WorldObject>(StringComparer.Ordinal);
            for (var index = 0; index < state.Objects.Count; index++)
                values.Add(state.Objects[index].Id.Value, state.Objects[index]);
            return values;
        }

        private static Dictionary<string, WorldExtensionData> ExtensionsByKey(WorldState state)
        {
            var values = new Dictionary<string, WorldExtensionData>(StringComparer.Ordinal);
            for (var index = 0; index < state.Extensions.Count; index++)
            {
                var value = state.Extensions[index];
                values.Add(WorldMutationCoverage.ExtensionKey(value.Owner, value.SchemaVersion, value.SubjectId), value);
            }

            return values;
        }

        private static HashSet<string> ReferenceSet(WorldObject? value)
        {
            var references = new HashSet<string>(StringComparer.Ordinal);
            if (value == null) return references;
            for (var index = 0; index < value.References.Count; index++)
            {
                var current = value.References[index];
                references.Add(current.Kind.Value + "->" + current.TargetId.Value);
            }

            return references;
        }

        private static IReadOnlyList<string> Difference(HashSet<string> left, HashSet<string> right)
        {
            var values = new List<string>();
            foreach (var value in left)
            {
                if (!right.Contains(value)) values.Add(value);
            }

            values.Sort(StringComparer.Ordinal);
            return values.AsReadOnly();
        }

        private static bool PayloadEquals(WorldExtensionData left, WorldExtensionData right)
        {
            var first = left.GetPayloadCopy();
            var second = right.GetPayloadCopy();
            if (first.Length != second.Length) return false;
            for (var index = 0; index < first.Length; index++)
            {
                if (first[index] != second[index]) return false;
            }

            return true;
        }

        private static HashSet<string> ExtensionDependencySet(WorldExtensionData? value)
        {
            var dependencies = new HashSet<string>(StringComparer.Ordinal);
            if (value == null) return dependencies;
            for (var index = 0; index < value.Dependencies.Count; index++)
            {
                var current = value.Dependencies[index];
                dependencies.Add(current.Kind.Value + "->" + current.TargetId.Value);
            }

            return dependencies;
        }

        private static int CompareExtensions(WorldExtensionData left, WorldExtensionData right)
        {
            return left.Identity.CompareTo(right.Identity);
        }

        private static CapabilityInvocationResult MissingResource(int index, string resource)
        {
            return Failure(
                "world.change.resource_missing",
                "A remove operation targeted a resource that does not exist in the planned state.",
                OperationPath(index),
                new Dictionary<string, object?>(StringComparer.Ordinal) { ["resource"] = resource },
                false,
                "Refresh state or remove the invalid delete operation before retrying.");
        }

        private static CapabilityInvocationResult InvalidCandidate(WorldValidationResult validation)
        {
            var path = validation.Diagnostics.Count == 0 ? "$.operations" : validation.Diagnostics[0].Path;
            return Failure(
                "world.change.invalid_candidate",
                "The proposed operations violate canonical world invariants.",
                path,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["validation"] = validation.ToData()
                },
                false,
                "Use the ordered validation diagnostics to repair the complete proposed state before retrying.");
        }

        private static CapabilityInvocationResult InternalValidationFailure(string sourceCode, string message, string path)
        {
            return Failure(
                "world.validation.internal_error",
                "Validated candidate materialization disagreed with the canonical invariant evaluator.",
                path,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["sourceCode"] = sourceCode,
                    ["detail"] = message
                },
                false,
                "Treat this as an Arkus validator defect; do not commit the candidate.");
        }

        private static CapabilityInvocationResult InvalidRequest(string path, string message)
        {
            return Failure(
                "world.change.invalid_request",
                message,
                path,
                EmptyContext(),
                false,
                "Use only the typed mutation envelope declared by system.describe.");
        }

        // Per-kind operation grammar. The public request schema is a flattened union of these kinds, so a grammar
        // violation names the kind's own allowed/required fields and the offending ones (WP-HK-05 reopen 1).
        private static readonly string[] PutObjectFields = { "kind", "id", "typeId", "containerId", "references" };
        private static readonly string[] PutObjectRequired = { "kind", "id", "typeId" };
        private static readonly string[] RemoveObjectFields = { "kind", "id" };
        private static readonly string[] RemoveObjectRequired = { "kind", "id" };
        private static readonly string[] PutExtensionFields = { "kind", "owner", "schemaVersion", "subjectId", "dependencies", "payloadBase64" };
        private static readonly string[] PutExtensionRequired = { "kind", "owner", "schemaVersion", "payloadBase64" };
        private static readonly string[] RemoveExtensionFields = { "kind", "owner", "schemaVersion", "subjectId" };
        private static readonly string[] RemoveExtensionRequired = { "kind", "owner", "schemaVersion" };

        private static CapabilityInvocationResult GrammarViolation(
            IReadOnlyDictionary<string, object?> data,
            int index,
            string kind,
            IReadOnlyList<string> allowed,
            IReadOnlyList<string> required)
        {
            var allowedSet = new HashSet<string>(allowed, StringComparer.Ordinal);
            var unexpected = new List<string>();
            foreach (var key in data.Keys)
            {
                if (!allowedSet.Contains(key)) unexpected.Add(key);
            }

            unexpected.Sort(StringComparer.Ordinal);
            return Failure(
                "world.change.invalid_request",
                kind + " contains fields outside its declared grammar.",
                OperationPath(index),
                ReadOnly(new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["operationKind"] = kind,
                    ["allowedFields"] = ToObjectList(allowed),
                    ["requiredFields"] = ToObjectList(required),
                    ["unexpectedFields"] = ToObjectList(unexpected)
                }),
                false,
                "Remove unexpectedFields; each operation kind accepts only its own allowedFields. Object data (id, typeId, containerId, references) and extension data (owner, schemaVersion, subjectId, dependencies, payloadBase64) are separate put-object and put-extension operations in the same transaction.");
        }

        private static CapabilityInvocationResult InvalidProvenanceRequest(string message)
        {
            return Failure(
                "world.provenance.invalid_request",
                message,
                "$",
                EmptyContext(),
                false,
                "Use the empty journal-read request declared by system.describe.");
        }

        private static CapabilityInvocationResult Failure(
            string code,
            string message,
            string path,
            IReadOnlyDictionary<string, object?> context,
            bool retryable,
            string repairHint)
        {
            return CapabilityInvocationResult.Failed(new StructuredError(
                code,
                message,
                path,
                context,
                retryable,
                repairHint));
        }

        private static IReadOnlyDictionary<string, object?> EmptyContext()
        {
            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal));
        }

        private static IReadOnlyDictionary<string, object?> ReadOnly(IDictionary<string, object?> values)
        {
            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(values, StringComparer.Ordinal));
        }

        private static IReadOnlyList<object?> ToObjectList(IReadOnlyList<string> values)
        {
            var result = new List<object?>();
            for (var index = 0; index < values.Count; index++) result.Add(values[index]);
            return result.AsReadOnly();
        }

        private static bool TryStableToken(IReadOnlyDictionary<string, object?> data, string key, out string value)
        {
            return TryGetString(data, key, out value) && IsStableToken(value);
        }

        private static bool IsStableToken(string value)
        {
            if (value.Length == 0 || value.Length > 128) return false;
            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= 'a' && character <= 'z') ||
                      (character >= '0' && character <= '9') ||
                      character == '.' || character == '_' || character == '-'))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsCanonicalHash(string value)
        {
            if (value.Length != 64) return false;
            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (!((character >= '0' && character <= '9') || (character >= 'a' && character <= 'f')))
                    return false;
            }

            return true;
        }

        private static bool TryCanonicalBase64(string value, out byte[] payload)
        {
            payload = Array.Empty<byte>();
            try
            {
                payload = Convert.FromBase64String(value);
                return string.Equals(Convert.ToBase64String(payload), value, StringComparison.Ordinal);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static bool TryGetString(IReadOnlyDictionary<string, object?> data, string key, out string value)
        {
            value = string.Empty;
            if (!data.TryGetValue(key, out var raw) || !(raw is string text)) return false;
            value = text;
            return true;
        }

        private static bool TryGetInteger(IReadOnlyDictionary<string, object?> data, string key, out long value)
        {
            value = 0;
            if (!data.TryGetValue(key, out var raw)) return false;
            switch (raw)
            {
                case sbyte current: value = current; return true;
                case byte current: value = current; return true;
                case short current: value = current; return true;
                case ushort current: value = current; return true;
                case int current: value = current; return true;
                case uint current: value = current; return true;
                case long current: value = current; return true;
                case ulong current when current <= long.MaxValue: value = (long)current; return true;
                default: return false;
            }
        }

        private static bool OnlyFields(IReadOnlyDictionary<string, object?> data, params string[] allowed)
        {
            var set = new HashSet<string>(allowed, StringComparer.Ordinal);
            foreach (var key in data.Keys)
            {
                if (!set.Contains(key)) return false;
            }

            return true;
        }

        private static string OperationPath(int index)
        {
            return "$.operations[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        }

        private sealed class WorldSnapshot
        {
            public WorldSnapshot(WorldState state, string hash)
            {
                State = state;
                Hash = hash;
            }

            public WorldState State { get; }
            public string Hash { get; }
        }

        private sealed class AuthoringState
        {
            public AuthoringState(
                WorldState current,
                IReadOnlyDictionary<string, IdempotencyReceipt> receipts,
                IReadOnlyList<WorldMutationProvenanceEntry> journal)
            {
                Current = current ?? throw new ArgumentNullException(nameof(current));
                if (receipts == null) throw new ArgumentNullException(nameof(receipts));
                if (journal == null) throw new ArgumentNullException(nameof(journal));

                Receipts = new ReadOnlyDictionary<string, IdempotencyReceipt>(
                    new Dictionary<string, IdempotencyReceipt>(receipts, StringComparer.Ordinal));
                Journal = new List<WorldMutationProvenanceEntry>(journal).AsReadOnly();
            }

            public WorldState Current { get; }
            public IReadOnlyDictionary<string, IdempotencyReceipt> Receipts { get; }
            public IReadOnlyList<WorldMutationProvenanceEntry> Journal { get; }

            public static AuthoringState Initial(WorldState state)
            {
                return new AuthoringState(
                    state,
                    new ReadOnlyDictionary<string, IdempotencyReceipt>(
                        new Dictionary<string, IdempotencyReceipt>(StringComparer.Ordinal)),
                    Array.Empty<WorldMutationProvenanceEntry>());
            }
        }

        private sealed class ParsedMutationRequest
        {
            public ParsedMutationRequest(
                string idempotencyKey,
                long expectedRevision,
                string expectedHash,
                IReadOnlyList<MutationOperation> operations,
                string fingerprint)
            {
                IdempotencyKey = idempotencyKey;
                ExpectedRevision = expectedRevision;
                ExpectedHash = expectedHash;
                Operations = operations;
                Fingerprint = fingerprint;
            }

            public string IdempotencyKey { get; }
            public long ExpectedRevision { get; }
            public string ExpectedHash { get; }
            public IReadOnlyList<MutationOperation> Operations { get; }
            public string Fingerprint { get; }
        }

        private sealed class MutationPlan
        {
            public MutationPlan(
                string planId,
                string worldId,
                int schemaVersion,
                long baseRevision,
                string baseHash,
                WorldState candidateState,
                string resultHash,
                IReadOnlyList<WorldMutationChange> changes)
            {
                PlanId = planId;
                WorldId = worldId;
                SchemaVersion = schemaVersion;
                BaseRevision = baseRevision;
                BaseHash = baseHash;
                CandidateState = candidateState;
                ResultHash = resultHash;
                Changes = changes;
            }

            public string PlanId { get; }
            public string WorldId { get; }
            public int SchemaVersion { get; }
            public long BaseRevision { get; }
            public string BaseHash { get; }
            public WorldState CandidateState { get; }
            public string ResultHash { get; }
            public IReadOnlyList<WorldMutationChange> Changes { get; }
        }

        private sealed class IdempotencyReceipt
        {
            public IdempotencyReceipt(string fingerprint, MutationPlan plan)
            {
                Fingerprint = fingerprint;
                Plan = plan;
            }

            public string Fingerprint { get; }
            public MutationPlan Plan { get; }
        }

        private enum MutationOperationKind
        {
            PutObject,
            RemoveObject,
            PutExtension,
            RemoveExtension
        }

        private sealed class ReferenceValue
        {
            public ReferenceValue(string kind, string targetId)
            {
                Kind = kind;
                TargetId = targetId;
            }

            public string Kind { get; }
            public string TargetId { get; }
        }

        private sealed class MutationOperation
        {
            private MutationOperation(
                MutationOperationKind kind,
                string? id,
                string? typeId,
                string? containerId,
                IReadOnlyList<ReferenceValue>? references,
                string? owner,
                int schemaVersion,
                string? subjectId,
                IReadOnlyList<ReferenceValue>? dependencies,
                byte[]? payload)
            {
                Kind = kind;
                Id = id;
                TypeId = typeId;
                ContainerId = containerId;
                References = references ?? Array.Empty<ReferenceValue>();
                Owner = owner;
                SchemaVersion = schemaVersion;
                SubjectId = subjectId;
                Dependencies = dependencies ?? Array.Empty<ReferenceValue>();
                Payload = payload;
            }

            public MutationOperationKind Kind { get; }
            public string? Id { get; }
            public string? TypeId { get; }
            public string? ContainerId { get; }
            public IReadOnlyList<ReferenceValue> References { get; }
            public string? Owner { get; }
            public int SchemaVersion { get; }
            public string? SubjectId { get; }
            public IReadOnlyList<ReferenceValue> Dependencies { get; }
            public byte[]? Payload { get; }

            public static MutationOperation PutObject(
                string id,
                string typeId,
                string? containerId,
                IReadOnlyList<ReferenceValue> references)
            {
                return new MutationOperation(MutationOperationKind.PutObject, id, typeId, containerId, references, null, 0, null, null, null);
            }

            public static MutationOperation RemoveObject(string id)
            {
                return new MutationOperation(MutationOperationKind.RemoveObject, id, null, null, null, null, 0, null, null, null);
            }

            public static MutationOperation PutExtension(
                string owner,
                int schemaVersion,
                string? subjectId,
                IReadOnlyList<ReferenceValue> dependencies,
                byte[] payload)
            {
                return new MutationOperation(
                    MutationOperationKind.PutExtension,
                    null,
                    null,
                    null,
                    null,
                    owner,
                    schemaVersion,
                    subjectId,
                    dependencies,
                    (byte[])payload.Clone());
            }

            public static MutationOperation RemoveExtension(string owner, int schemaVersion, string? subjectId)
            {
                return new MutationOperation(
                    MutationOperationKind.RemoveExtension,
                    null,
                    null,
                    null,
                    null,
                    owner,
                    schemaVersion,
                    subjectId,
                    null,
                    null);
            }

            public void AppendFingerprint(StringBuilder builder)
            {
                builder.Append((int)Kind).Append('\t');
                switch (Kind)
                {
                    case MutationOperationKind.PutObject:
                        builder.Append(Id).Append('\t').Append(TypeId).Append('\t').Append(ContainerId ?? "-").Append('\t');
                        for (var index = 0; index < References.Count; index++)
                        {
                            builder.Append(References[index].Kind).Append("->").Append(References[index].TargetId).Append(';');
                        }
                        break;
                    case MutationOperationKind.RemoveObject:
                        builder.Append(Id);
                        break;
                    case MutationOperationKind.PutExtension:
                        builder.Append(Owner).Append('\t').Append(SchemaVersion.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(SubjectId == null ? "global" : "object:" + SubjectId.Length.ToString(CultureInfo.InvariantCulture) + ":" + SubjectId).Append('\t');
                        for (var index = 0; index < Dependencies.Count; index++)
                        {
                            builder.Append(Dependencies[index].Kind).Append("->").Append(Dependencies[index].TargetId).Append(';');
                        }

                        builder.Append('\t').Append(Convert.ToBase64String(Payload!));
                        break;
                    case MutationOperationKind.RemoveExtension:
                        builder.Append(Owner).Append('\t').Append(SchemaVersion.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(SubjectId == null ? "global" : "object:" + SubjectId.Length.ToString(CultureInfo.InvariantCulture) + ":" + SubjectId);
                        break;
                }

                builder.Append('\n');
            }
        }
    }
}
