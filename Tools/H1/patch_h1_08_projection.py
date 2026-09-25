from pathlib import Path

scene_path = Path('Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.cs')
component_path = Path('Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1ComponentProjection.cs')

text = scene_path.read_text()
if 'public H1ValidationResult validation;' not in text:
    start = text.index('        public static string Execute(string payload)')
    end = text.index('        private static ProjectionObservation Materialize', start)
    execute = r'''        public static string Execute(string payload)
        {
            var request = JsonUtility.FromJson<ProjectionRequest>(payload);
            if (request == null || request.schemaId != "arkus.h1-projection-worker-request@1" ||
                request.sceneLogicalId != SceneId || request.plan == null ||
                request.plan.schemaId != "arkus.h1-managed-scene-plan@1" ||
                request.plan.sceneLogicalId != SceneId || request.plan.nodes == null || request.plan.nodes.Length > 128 ||
                !IsHash(request.plan.inputDigest) || !IsHash(request.plan.canonicalHash) || !IsHash(request.plan.catalogueFingerprint))
                throw new InvalidDataException("projection.invalid-worker-plan");
            if (request.mode != "materialize" && request.mode != "observe" &&
                request.mode != "validate-proposed" && request.mode != "validate-current")
                throw new InvalidDataException("projection.invalid-worker-mode");

            try
            {
                if (request.mode == "materialize" || request.mode == "validate-proposed")
                {
                    var preflight = RunPreflight(request.plan);
                    if (!preflight.valid)
                        return Reply(request.plan, FirstCode(preflight), SafeObserveActive(), preflight);
                    if (request.mode == "validate-proposed")
                        return Reply(request.plan, "", SafeObserveActive(), preflight);

                    H1ValidationResult postflight;
                    var observation = Materialize(request.plan, out postflight);
                    return Reply(request.plan, "", observation, postflight);
                }

                ProjectionObservation current;
                var currentValidation = RunCurrentValidation(request.plan, out current);
                return Reply(request.plan, currentValidation.valid ? "" : FirstCode(currentValidation), current, currentValidation);
            }
            catch (H1ValidationFailureException validationFailure)
            {
                return Reply(request.plan, FirstCode(validationFailure.Result), SafeObserveActive(), validationFailure.Result);
            }
            catch (Exception exception)
            {
                Debug.LogError("ARKUS_H1_PROJECTION_FAILURE:" + exception.GetType().Name + ":" + exception.Message);
                return Reply(request.plan,
                    exception is InvalidDataException && exception.Message.StartsWith("projection.", StringComparison.Ordinal)
                        ? exception.Message : "projection.editor-failure",
                    SafeObserveActive(), null);
            }
        }

        private static string Reply(ProjectionPlan plan, string errorCode, ProjectionObservation observation, H1ValidationResult validation)
        {
            return JsonUtility.ToJson(new ProjectionReply
            {
                schemaId = "arkus.h1-projection-worker-result@1",
                sceneLogicalId = SceneId,
                expectedInputDigest = plan.inputDigest,
                errorCode = errorCode ?? "",
                observation = observation ?? EmptyObservation(),
                validation = validation
            });
        }

        private static string FirstCode(H1ValidationResult validation)
        {
            return validation != null && validation.diagnostics != null && validation.diagnostics.Length != 0
                ? validation.diagnostics[0].code : "projection.validation-failed";
        }

        private static ProjectionObservation SafeObserveActive()
        {
            try { return ObserveActive(); }
            catch (Exception) { return EmptyObservation(); }
        }

'''
    text = text[:start] + execute + text[end:]

    text = text.replace(
        '        private static ProjectionObservation Materialize(ProjectionPlan plan)\n        {\n            var previous = ReadManifest();',
        '        private static ProjectionObservation Materialize(ProjectionPlan plan, out H1ValidationResult postflight)\n        {\n            postflight = null;\n            var previous = ReadManifest();')
    text = text.replace(
        '                    if (SameGraph(plan, existing) && SameRealization(plan, existing)) return existing;',
        '''                    if (SameGraph(plan, existing) && SameRealization(plan, existing))
                    {
                        ProjectionObservation verified;
                        postflight = RunPostflight(plan, previous.scenePath, previous.generationId, existing, true, out verified);
                        if (!postflight.valid) throw new H1ValidationFailureException(postflight);
                        return verified;
                    }''')
    text = text.replace(
        '            if (!EditorSceneManager.SaveScene(scene, scenePath, false)) throw new IOException("projection.scene-save-failed");',
        '''            var nonFiniteFault = Path.Combine(H1Bootstrap.ProjectRoot(), "Library", "Arkus", "H1Projection", "inject-non-finite-transform");
            if (File.Exists(nonFiniteFault) && created.Count != 0)
                created.OrderBy(value => value.Key, StringComparer.Ordinal).First().Value.transform.localPosition = new Vector3(float.NaN, 0f, 0f);

            if (!EditorSceneManager.SaveScene(scene, scenePath, false)) throw new IOException("projection.scene-save-failed");''')

    old_stage = '''            ValidateSourceBytes(plan);
            var staged = ObserveScene(scenePath, generationId, plan.inputDigest, plan.canonicalHash, plan.catalogueFingerprint);
            if (!SameGraph(plan, staged) || !SameRealization(plan, staged))
                throw new InvalidDataException("projection.stage-observation-mismatch");'''
    new_stage = '''            ProjectionObservation staged;
            postflight = RunPostflight(plan, scenePath, generationId, null, true, out staged);
            if (!postflight.valid) throw new H1ValidationFailureException(postflight);'''
    if old_stage not in text:
        raise SystemExit('H1-08 stage insertion anchor missing')
    text = text.replace(old_stage, new_stage, 1)

    validate_start = text.index('        private static void ValidatePlan(ProjectionPlan plan)')
    validate_end = text.index('        private static void ValidateComponents(ProjectionNode node)', validate_start)
    validators = r'''        private static H1ValidationResult RunPreflight(ProjectionPlan plan)
        {
            var shapeValid = true;
            var checks = new List<H1ValidationCheck>
            {
                H1ProjectionValidation.Check("unity.catalogue.snapshot", SceneId, "", "Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json",
                    "refresh accepted catalogue snapshot or repair source binding", ValidateCatalogueSnapshot),
                H1ProjectionValidation.Check("unity.component.adapter-inventory", SceneId, "", "Assets/Arkus/H1",
                    "repair the declared/effective H1 component adapter inventory", () => H1ComponentProjection.CaptureInventory()),
                H1ProjectionValidation.Check("unity.plan.node-shape", SceneId, "", Root,
                    "repair canonical node identity/shape", () =>
                    {
                        try { ValidatePlanShape(plan); }
                        catch (InvalidDataException) { shapeValid = false; throw; }
                    })
            };

            foreach (var node in plan.nodes.Where(value => value != null).OrderBy(value => value.objectId ?? "", StringComparer.Ordinal))
            {
                var captured = node;
                var resource = string.IsNullOrEmpty(captured.objectId) ? SceneId : captured.objectId;
                var logical = captured.sourceLogicalId ?? "";
                var managed = string.IsNullOrEmpty(captured.sourcePath) ? Root + "/" + resource : captured.sourcePath;
                checks.Add(H1ProjectionValidation.Check("unity.plan.source-binding", resource, logical, managed,
                    "repair logical source identity/path/type", () =>
                    {
                        if (!shapeValid) return;
                        ResolveSource(captured);
                    }));
                checks.Add(H1ProjectionValidation.Check("unity.plan.component", resource, logical, Root + "/" + resource,
                    "repair component schema/field/reference", () =>
                    {
                        if (!shapeValid) return;
                        ValidateComponents(captured);
                    }));
            }

            checks.Add(H1ProjectionValidation.Check("unity.plan.hierarchy", SceneId, "", Root,
                "repair parent or canonical component target", () =>
                {
                    if (!shapeValid) return;
                    ValidatePlanHierarchy(plan);
                }));
            return H1ProjectionValidation.Run(H1ProjectionValidation.Preflight, "proposed", plan.inputDigest, checks);
        }

        private static H1ValidationResult RunCurrentValidation(ProjectionPlan plan, out ProjectionObservation observation)
        {
            var manifest = ReadManifest();
            if (manifest == null)
            {
                var empty = EmptyObservation();
                var result = H1ProjectionValidation.Run(H1ProjectionValidation.PostMaterialization, "current", plan.inputDigest, new[]
                {
                    H1ProjectionValidation.Check("unity.scene.finite-transform", SceneId, "", Root,
                        "materialize a managed generation before validating current state", () => { throw new InvalidDataException("projection.active-scene-missing"); })
                });
                observation = empty;
                return result;
            }
            return RunPostflight(plan, manifest.scenePath, manifest.generationId, null, false, out observation);
        }

        private static H1ValidationResult RunPostflight(ProjectionPlan plan, string scenePath, string generationId,
            ProjectionObservation seed, bool comparePlan, out ProjectionObservation observation)
        {
            var current = seed;
            var finiteValid = true;
            var checks = new List<H1ValidationCheck>
            {
                H1ProjectionValidation.Check("unity.scene.finite-transform", SceneId, "", scenePath,
                    "repair non-finite local transform values before publication", () =>
                    {
                        try { H1ProjectionValidation.ValidateFiniteTransforms(scenePath); }
                        catch (InvalidDataException) { finiteValid = false; throw; }
                    }),
                ObservationClassCheck("unity.scene.managed-marker", plan, scenePath, generationId,
                    "repair managed root/object marker identity and hierarchy", () => finiteValid),
                ObservationClassCheck("unity.scene.prefab-link", plan, scenePath, generationId,
                    "repair prefab derivative/source lineage without mutating source", () => finiteValid),
                ObservationClassCheck("unity.scene.component", plan, scenePath, generationId,
                    "repair effective component field/reference realization", () => finiteValid),
                H1ProjectionValidation.Check("unity.scene.effective-observation", SceneId, "", scenePath,
                    "repair effective managed scene observation", () =>
                    {
                        if (!finiteValid) return;
                        try
                        {
                            current = ObserveScene(scenePath, generationId, plan.inputDigest, plan.canonicalHash, plan.catalogueFingerprint);
                        }
                        catch (InvalidDataException error) when (OwnedBySpecializedPostflight(error.Message)) { }
                    })
            };
            if (comparePlan)
            {
                checks.Add(H1ProjectionValidation.Check("unity.scene.plan-observation", SceneId, "", scenePath,
                    "repair effective scene so graph and realization match the proposed plan", () =>
                    {
                        if (current == null) return;
                        if (!SameGraph(plan, current) || !SameRealization(plan, current))
                            throw new InvalidDataException("projection.stage-observation-mismatch");
                    }));
            }
            var result = H1ProjectionValidation.Run(H1ProjectionValidation.PostMaterialization,
                comparePlan ? "proposed-effective" : "current", plan.inputDigest, checks);
            observation = current ?? EmptyObservation();
            return result;
        }

        private static H1ValidationCheck ObservationClassCheck(string invariantId, ProjectionPlan plan, string scenePath,
            string generationId, string context, Func<bool> enabled)
        {
            return H1ProjectionValidation.Check(invariantId, SceneId, "", scenePath, context, () =>
            {
                if (!enabled()) return;
                try
                {
                    ObserveScene(scenePath, generationId, plan.inputDigest, plan.canonicalHash, plan.catalogueFingerprint);
                }
                catch (InvalidDataException error) when (!OwnsPostflightCode(invariantId, error.Message)) { }
            });
        }

        private static bool OwnedBySpecializedPostflight(string code) =>
            OwnsPostflightCode("unity.scene.managed-marker", code) ||
            OwnsPostflightCode("unity.scene.prefab-link", code) ||
            OwnsPostflightCode("unity.scene.component", code);

        private static bool OwnsPostflightCode(string invariantId, string code)
        {
            if (string.IsNullOrEmpty(code)) return false;
            if (invariantId == "unity.scene.managed-marker")
                return code == "projection.root-count" || code.IndexOf("marker", StringComparison.Ordinal) >= 0 ||
                    code == "projection.unmanaged-parent" || code == "projection.unmanaged-scene-member";
            if (invariantId == "unity.scene.prefab-link")
                return code.IndexOf("prefab", StringComparison.Ordinal) >= 0 || code.IndexOf("lineage", StringComparison.Ordinal) >= 0;
            if (invariantId == "unity.scene.component")
                return code.IndexOf("component", StringComparison.Ordinal) >= 0;
            return false;
        }

        private static void ValidatePlan(ProjectionPlan plan)
        {
            ValidatePlanShape(plan);
            foreach (var node in plan.nodes)
            {
                ResolveSource(node);
                ValidateComponents(node);
            }
            ValidatePlanHierarchy(plan);
        }

        private static void ValidatePlanShape(ProjectionPlan plan)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var node in plan.nodes)
            {
                if (node == null || string.IsNullOrEmpty(node.objectId) || !seen.Add(node.objectId) ||
                    string.IsNullOrEmpty(node.sourceLogicalId) || !IsHash(node.sourceContentSha256) ||
                    node.positionMm == null || node.rotationMilliDegrees == null || node.scalePpm == null ||
                    node.components == null || node.components.Length == 0 || node.components.Length > MaximumComponents ||
                    (node.sourceKind != "prefab" && node.sourceKind != "asset"))
                    throw new InvalidDataException("projection.invalid-node");
            }
        }

        private static void ValidatePlanHierarchy(ProjectionPlan plan)
        {
            var seen = new HashSet<string>(plan.nodes.Select(node => node.objectId), StringComparer.Ordinal);
            foreach (var node in plan.nodes)
            {
                if (!string.IsNullOrEmpty(node.parentObjectId) && !seen.Contains(node.parentObjectId))
                    throw new InvalidDataException("projection.unbound-parent");
                foreach (var component in node.components)
                    if (component.schemaId == H1ComponentProjection.CanonicalLinkSchema && !seen.Contains(component.targetObjectId))
                        throw new InvalidDataException("projection.component-reference-unresolved");
            }
        }

'''
    text = text[:validate_start] + validators + text[validate_end:]

    old_asset = '''            if (component.kind != kind || component.referenceKind != referenceKind || string.IsNullOrEmpty(component.referenceLogicalId) ||
                string.IsNullOrEmpty(component.referencePath) || string.IsNullOrEmpty(component.referenceGuid) ||
                string.IsNullOrEmpty(component.referenceLocalFileId) || !IsHash(component.referenceContentSha256))
                throw new InvalidDataException("projection.component-reference-invalid");
        }'''
    new_asset = '''            if (component.kind != kind || component.referenceKind != referenceKind || string.IsNullOrEmpty(component.referenceLogicalId) ||
                string.IsNullOrEmpty(component.referencePath) || string.IsNullOrEmpty(component.referenceGuid) ||
                string.IsNullOrEmpty(component.referenceLocalFileId) || !IsHash(component.referenceContentSha256))
                throw new InvalidDataException("projection.component-reference-invalid");
            H1ComponentProjection.ValidateReference(component.schemaId, component.referencePath, component.referenceGuid,
                component.referenceLocalFileId, component.referenceContentSha256);
        }'''
    if old_asset not in text:
        raise SystemExit('H1-08 asset-component insertion anchor missing')
    text = text.replace(old_asset, new_asset, 1)

    text = text.replace(
        '[Serializable] private sealed class ProjectionReply { public string schemaId; public string sceneLogicalId; public string expectedInputDigest; public string errorCode; public ProjectionObservation observation; }',
        '[Serializable] private sealed class ProjectionReply { public string schemaId; public string sceneLogicalId; public string expectedInputDigest; public string errorCode; public ProjectionObservation observation; public H1ValidationResult validation; }')
    if 'public H1ValidationResult validation;' not in text:
        raise SystemExit('H1-08 reply schema insertion failed')
    scene_path.write_text(text)

component = component_path.read_text()
if 'internal static void ValidateReference(' not in component:
    marker = '        internal static void ApplyTransform(GameObject owner, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)'
    at = component.index(marker)
    addition = r'''        internal static void ValidateReference(string schemaId, string path, string guid, string localFileId, string contentSha256)
        {
            ValidateSchema(schemaId);
            if (schemaId == MeshRendererSchema)
            {
                ResolveAsset<Material>(path, guid, localFileId, contentSha256, "material");
                return;
            }
            if (schemaId == AnimatorSchema)
                ResolveAsset<AnimationClip>(path, guid, localFileId, contentSha256, "animation-clip");
        }

'''
    component = component[:at] + addition + component[at:]
    component_path.write_text(component)

print('H1_08_PATCH_COMPLETE')
