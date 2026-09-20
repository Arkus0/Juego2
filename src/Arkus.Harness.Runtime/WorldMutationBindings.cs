using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.Validation;
using Arkus.Game.World;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    /// <summary>
    /// Marker for effective public handlers that can commit canonical authorable state.
    /// It is executable-surface metadata, not a second capability registry.
    /// </summary>
    public interface ITransactionalMutationHandler : ICanonicalCapabilityHandler
    {
    }

    internal static class WorldMutationBindings
    {
        public static IReadOnlyList<CapabilityRoute> CreateRoutes(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            // Plan/dry-run and HK08B recovery reads share the accepted attenuated facade whose
            // runtime object is not the authoritative session. Only apply retains commit authority.
            var planner = new WorldMutationPlannerView(service);
            var committer = CanonicalWorldMutationAuthority.Bind(service);
            var recovery = new WorldConflictRecovery(planner);
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldChangePlanHandler(planner, recovery)),
                CapabilityRoute.FromHandler(new WorldChangeDryRunHandler(planner, recovery)),
                CapabilityRoute.FromHandler(new WorldChangeApplyHandler(committer, recovery))
            }.AsReadOnly();
        }
    }

    /// <summary>
    /// Capability attenuation boundary: exposes only non-committing authoring operations and
    /// deliberately does not expose or implement canonical commit authority even when the wrapped
    /// service does. HK05/HK06A/HK06B/HK08B reuse the same accepted attenuation boundary for
    /// validation, provenance, portability and recovery reads.
    /// </summary>
    internal sealed class WorldMutationPlannerView :
        IWorldMutationService,
        IWorldValidationService,
        IWorldProvenanceService,
        IWorldPortabilityService,
        IWorldConflictRecoverySource
    {
        private readonly IWorldMutationService _service;
        private readonly IWorldValidationService _validation;
        private readonly IWorldProvenanceService _provenance;
        private readonly IWorldPortabilityService _portability;
        private readonly IWorldStateSource? _stateSource;

        public WorldMutationPlannerView(IWorldMutationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _validation = service as IWorldValidationService ?? new UnavailableWorldValidationService();
            _provenance = service as IWorldProvenanceService ?? new UnavailableWorldProvenanceService();
            _portability = service as IWorldPortabilityService ?? new UnavailableWorldPortabilityService();
            _stateSource = service as IWorldStateSource;
        }

        public CapabilityInvocationResult Plan(IReadOnlyDictionary<string, object?> request)
        {
            return _service.Plan(request);
        }

        public CapabilityInvocationResult DryRun(IReadOnlyDictionary<string, object?> request)
        {
            return _service.DryRun(request);
        }

        public CapabilityInvocationResult ValidateCurrent(IReadOnlyDictionary<string, object?> request)
        {
            return _validation.ValidateCurrent(request);
        }

        public CapabilityInvocationResult ValidateProposed(IReadOnlyDictionary<string, object?> request)
        {
            return _validation.ValidateProposed(request);
        }

        public CapabilityInvocationResult ReadJournal(IReadOnlyDictionary<string, object?> request)
        {
            return _provenance.ReadJournal(request);
        }

        public CapabilityInvocationResult CompareSnapshots(IReadOnlyDictionary<string, object?> request)
        {
            return _portability.CompareSnapshots(request);
        }

        public CapabilityInvocationResult ExportSnapshot(IReadOnlyDictionary<string, object?> request)
        {
            return _portability.ExportSnapshot(request);
        }

        bool IWorldConflictRecoverySource.TryGetCurrent(out WorldState state)
        {
            if (_stateSource == null)
            {
                state = null!;
                return false;
            }

            state = _stateSource.Current;
            return true;
        }
    }

    internal abstract class WorldMutationHandlerBase : ICanonicalCapabilityHandler
    {
        protected WorldMutationHandlerBase(IWorldMutationService service, WorldConflictRecovery recovery)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
            Recovery = recovery ?? throw new ArgumentNullException(nameof(recovery));
        }

        protected IWorldMutationService Service { get; }
        protected WorldConflictRecovery Recovery { get; }

        public abstract CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request);

        protected static void Validate(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldMutationContract.PlanName, "1.0")]
    internal sealed class WorldChangePlanHandler : WorldMutationHandlerBase
    {
        public WorldChangePlanHandler(IWorldMutationService service, WorldConflictRecovery recovery)
            : base(service, recovery) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Recovery.Enrich(Service.Plan(request), request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldMutationContract.DryRunName, "1.0")]
    internal sealed class WorldChangeDryRunHandler : WorldMutationHandlerBase
    {
        public WorldChangeDryRunHandler(IWorldMutationService service, WorldConflictRecovery recovery)
            : base(service, recovery) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Recovery.Enrich(Service.DryRun(request), request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldMutationContract.ApplyName, "1.0")]
    internal sealed class WorldChangeApplyHandler : ITransactionalMutationHandler
    {
        private readonly ICanonicalWorldMutationCommitter _committer;
        private readonly WorldConflictRecovery _recovery;

        public WorldChangeApplyHandler(ICanonicalWorldMutationCommitter committer, WorldConflictRecovery recovery)
        {
            _committer = committer ?? throw new ArgumentNullException(nameof(committer));
            _recovery = recovery ?? throw new ArgumentNullException(nameof(recovery));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
            return _recovery.Enrich(_committer.Apply(request, context.ResourceBudget), request);
        }
    }
}
