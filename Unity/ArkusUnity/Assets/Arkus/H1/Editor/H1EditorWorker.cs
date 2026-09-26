using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Arkus.H1.Editor
{
    public static class H1EditorWorker
    {
        private const string InvocationSchema = "arkus.h1-unity-invocation@1";
        private const string ResultSchema = "arkus.h1-unity-result@1";
        private const string ProfileId = "arkus.h1-unity-launch-profile@1";
        private const string ProjectIdentity = "arkus.unity-project@1:ArkusUnity";
        private const string EditorVersion = "6000.3.24f1";
        private const string EditorRevision = "4e7b9b5b6244";
        private const string EntryPoint = "Arkus.H1.Editor.H1EditorWorker.Run";
        private const string ProjectProfileExecutor = "arkus.h1.worker.project-profile.inspect@1";
        private const string HierarchyExecutor = "arkus.h1.worker.hierarchy-probe.inspect@1";
        private const string CatalogueQueryExecutor = "arkus.h1.worker.catalogue.query@1";
        private const string CatalogueGetExecutor = "arkus.h1.worker.catalogue.get@1";
        private const string CatalogueResolveExecutor = "arkus.h1.worker.catalogue.resolve@1";
        private const string ProjectionMaterializeExecutor = "arkus.h1.worker.projection.materialize@1";
        private const string ProjectionObserveExecutor = "arkus.h1.worker.projection.observe@1";
        private const string ProjectionReconcileExecutor = "arkus.h1.worker.projection.reconcile@1";
        private const string ProjectionCleanRebuildExecutor = "arkus.h1.worker.projection.clean-rebuild@1";
        private const string HierarchyPayload = "hierarchy-probe:v1|root=diagnostic-root|child=diagnostic-child|component=Transform|active=true";

        public static void Run()
        {
            try
            {
                var invocationPath = RequireArgument("-arkusInvocation");
                var resultPath = RequireArgument("-arkusResult");
                var fields = Parse(File.ReadAllLines(invocationPath));
                ValidateEnvelope(fields);

                // executeMethod is invoked by the Editor on its main thread. Calling an Editor API
                // here makes that requirement observable instead of merely trusting a managed thread id.
                AssetDatabase.GetAllAssetPaths();

                var executor = Decode(fields, "executor");
                var payload = Decode(fields, "payload");
                string resultPayload;
                if (string.Equals(executor, ProjectProfileExecutor, StringComparison.Ordinal))
                {
                    if (!string.Equals(payload, "inspect", StringComparison.Ordinal))
                        throw new InvalidDataException("Project/profile worker payload mismatch.");
                    resultPayload = "inspect";
                }
                else if (string.Equals(executor, HierarchyExecutor, StringComparison.Ordinal))
                {
                    if (!string.Equals(payload, HierarchyPayload, StringComparison.Ordinal))
                        throw new InvalidDataException("Hierarchy worker payload mismatch.");
                    resultPayload = ExerciseHierarchyProbe();
                }
                else if (string.Equals(executor, CatalogueQueryExecutor, StringComparison.Ordinal) ||
                         string.Equals(executor, CatalogueGetExecutor, StringComparison.Ordinal) ||
                         string.Equals(executor, CatalogueResolveExecutor, StringComparison.Ordinal))
                {
                    // The external Arkus host owns public request semantics and mapping. The
                    // Editor worker only observes the independently bounded effective universe.
                    resultPayload = JsonUtility.ToJson(new CatalogueWorkerResponse
                    {
                        schemaId = "arkus.h1-catalogue-worker-observation@1",
                        requestJson = payload,
                        inventory = H1CatalogueInventory.Capture()
                    });
                }
                else if (string.Equals(executor, ProjectionMaterializeExecutor, StringComparison.Ordinal) ||
                         string.Equals(executor, ProjectionObserveExecutor, StringComparison.Ordinal))
                {
                    resultPayload = H1SceneProjection.Execute(payload);
                }
                else if (string.Equals(executor, ProjectionReconcileExecutor, StringComparison.Ordinal))
                {
                    resultPayload = H1SceneProjection.ExecuteReconciliation(payload);
                }
                else if (string.Equals(executor, ProjectionCleanRebuildExecutor, StringComparison.Ordinal))
                {
                    resultPayload = H1SceneProjection.ExecuteCleanRebuild(payload);
                }
                else
                {
                    throw new InvalidDataException("Unknown H1 Unity worker executor.");
                }

                WriteResult(resultPath, fields, resultPayload);
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogError("ARKUS_H1_EDITOR_WORKER_FAILURE:" + exception.GetType().Name);
                EditorApplication.Exit(73);
            }
        }

        private static string ExerciseHierarchyProbe()
        {
            GameObject root = null;
            try
            {
                root = new GameObject("diagnostic-root");
                var child = new GameObject("diagnostic-child");
                child.transform.SetParent(root.transform, false);
                if (!string.Equals(root.name, "diagnostic-root", StringComparison.Ordinal) ||
                    root.transform.childCount != 1 ||
                    !string.Equals(root.transform.GetChild(0).name, "diagnostic-child", StringComparison.Ordinal) ||
                    child.GetComponent<Transform>() == null ||
                    !child.activeSelf)
                {
                    throw new InvalidOperationException("Hierarchy-shaped diagnostic probe did not preserve its expected structure.");
                }
                return HierarchyPayload;
            }
            finally
            {
                if (root != null) UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void ValidateEnvelope(IReadOnlyDictionary<string, string> fields)
        {
            RequireLiteral(fields, "schema", InvocationSchema, false);
            RequireLiteral(fields, "profile", ProfileId, true);
            RequireLiteral(fields, "project", ProjectIdentity, true);
            RequireLiteral(fields, "editorVersion", EditorVersion, true);
            RequireLiteral(fields, "editorRevision", EditorRevision, true);
            RequireLiteral(fields, "entryPoint", EntryPoint, true);

            var invocation = Decode(fields, "invocation");
            if (!IsSafeInvocationId(invocation)) throw new InvalidDataException("Invalid invocation identity.");
            if (!string.Equals(Application.unityVersion, EditorVersion, StringComparison.Ordinal))
                throw new InvalidDataException("Effective Unity version disagrees with the launch profile.");

            Decode(fields, "capability");
            Decode(fields, "executor");
            Decode(fields, "payload");
        }

        private static void WriteResult(string resultPath, IReadOnlyDictionary<string, string> invocation, string payload)
        {
            var lines = new[]
            {
                "schema=" + ResultSchema,
                "invocation=" + invocation["invocation"],
                "capability=" + invocation["capability"],
                "executor=" + invocation["executor"],
                "profile=" + invocation["profile"],
                "project=" + invocation["project"],
                "editorVersion=" + invocation["editorVersion"],
                "editorRevision=" + invocation["editorRevision"],
                "mainThread=true",
                "payload=" + Encode(payload)
            };
            var directory = Path.GetDirectoryName(resultPath);
            if (string.IsNullOrEmpty(directory)) throw new InvalidDataException("Result path has no parent directory.");
            Directory.CreateDirectory(directory);
            var temp = resultPath + ".tmp-" + Guid.NewGuid().ToString("N");
            File.WriteAllLines(temp, lines, new UTF8Encoding(false));
            if (File.Exists(resultPath)) File.Delete(resultPath);
            File.Move(temp, resultPath);
        }

        private static Dictionary<string, string> Parse(IEnumerable<string> lines)
        {
            var fields = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var line in lines)
            {
                var separator = line.IndexOf('=');
                if (separator <= 0) throw new InvalidDataException("Malformed invocation field.");
                if (!fields.TryAdd(line.Substring(0, separator), line.Substring(separator + 1)))
                    throw new InvalidDataException("Duplicate invocation field.");
            }
            if (fields.Count != 10) throw new InvalidDataException("Invocation envelope has missing or unexpected fields.");
            return fields;
        }

        private static string RequireArgument(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length - 1; index++)
            {
                if (string.Equals(args[index], name, StringComparison.Ordinal))
                {
                    var value = args[index + 1];
                    if (string.IsNullOrWhiteSpace(value)) break;
                    return Path.GetFullPath(value);
                }
            }
            throw new InvalidDataException("Missing fixed H1 worker argument: " + name);
        }

        private static void RequireLiteral(IReadOnlyDictionary<string, string> fields, string key, string expected, bool encoded)
        {
            if (!fields.TryGetValue(key, out var raw)) throw new InvalidDataException("Missing invocation field: " + key);
            var actual = encoded ? DecodeValue(raw) : raw;
            if (!string.Equals(actual, expected, StringComparison.Ordinal)) throw new InvalidDataException("Invocation profile mismatch: " + key);
        }

        private static string Decode(IReadOnlyDictionary<string, string> fields, string key)
        {
            if (!fields.TryGetValue(key, out var value)) throw new InvalidDataException("Missing invocation field: " + key);
            return DecodeValue(value);
        }

        private static string Encode(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        private static string DecodeValue(string value) => Encoding.UTF8.GetString(Convert.FromBase64String(value));

        private static bool IsSafeInvocationId(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 96) return false;
            foreach (var c in value)
            {
                if (!(char.IsLetterOrDigit(c) || c == '-')) return false;
            }
            return true;
        }

        [Serializable]
        private sealed class CatalogueWorkerResponse
        {
            public string schemaId;
            public string requestJson;
            public EffectiveInventory inventory;
        }
    }
}
