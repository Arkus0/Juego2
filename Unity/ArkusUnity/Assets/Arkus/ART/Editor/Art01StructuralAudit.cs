using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Juego2.ART.Editor
{
    /// <summary>
    /// Renderer-independent STRUCTURAL audit of the ART-01 benchmark (checkpoint 2).
    /// It measures effective geometry through temporary visual-mesh colliders on an audit-only
    /// layer and never saves the scene. Materials, shaders, lighting and atmosphere are
    /// deliberately outside this audit; they belong to the URP CANDIDATE checkpoint.
    /// </summary>
    public static class Art01StructuralAudit
    {
        const string ScenePath = "Assets/Arkus/ART/Art01Benchmark.unity";
        const int VisLayer = 31;
        const int VisMask = 1 << VisLayer;
        const int PlayMask = ~VisMask;
        const float JoinTol = 0.05f;    // visible closed-join production tolerance (dimensional profile)
        const float ContactTol = 0.02f; // continuous seam / contact tolerance (dimensional profile)
        const float LeakDepth = 0.62f;  // a probe travelling this far past a facade plane is inside the building

        [Serializable] public sealed class Check
        {
            public string group, id, subject, measured, expected;
            public bool pass;
        }
        [Serializable] public sealed class Report
        {
            public string schema = "juego2.art01.structural-audit@1";
            public string unityVersion;
            public string scene = ScenePath;
            public string rendererIndependence =
                "Geometry-only: temporary audit-layer MeshColliders on every MeshFilter; no material, shader, light, fog or pixel is read.";
            public string manifestSha256;
            public string structuralDigestSha256;
            public int pieces;
            public bool passed;
            public int checkCount, failureCount;
            public string[] failures;
            public Check[] checks;
        }
        [Serializable] sealed class MExc { public string measure; public float min; public float max; public string reason; }
        [Serializable] sealed class MProv { public string sourceLockDest; public string[] unityAssets; }
        [Serializable] sealed class MRouteSegment { public string route; public float z0; public float z1; }
        [Serializable] sealed class MPiece
        {
            public string id, kind, readiness, role, formPrimitive, routeWidthSource;
            public MRouteSegment[] routeSegments;
            public MProv provenance;
            public MExc[] dimensionalExceptions;
            public float roofPitchMinDeg, roofPitchMaxDeg;
        }
        [Serializable] sealed class Manifest { public MPiece[] pieces; public MPiece[] assemblies; }

        static readonly List<Check> Checks = new List<Check>();
        static Dictionary<string, MPiece> kit = new Dictionary<string, MPiece>();
        static Art01Piece[] all;

        static string F(float v) => v.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
        static string V(Vector3 v) => $"({F(v.x)},{F(v.y)},{F(v.z)})";

        static void Add(string group, string id, string subject, bool pass, string measured, string expected) =>
            Checks.Add(new Check { group = group, id = id, subject = subject, pass = pass, measured = measured, expected = expected });

        static void Band(string group, string id, string subject, float value, float min, float max, string kitId = null, string measure = null)
        {
            bool inBand = value >= min - 0.0005f && value <= max + 0.0005f;
            if (!inBand && kitId != null && measure != null && kit.TryGetValue(kitId, out var entry) && entry.dimensionalExceptions != null)
                foreach (var e in entry.dimensionalExceptions)
                    if (e.measure == measure && value >= e.min - 0.0005f && value <= e.max + 0.0005f && !string.IsNullOrEmpty(e.reason))
                    {
                        Add(group, id, subject, true, F(value), $"[{F(min)},{F(max)}] via declared exception [{F(e.min)},{F(e.max)}]: {e.reason}");
                        return;
                    }
            Add(group, id, subject, inBand, F(value), $"[{F(min)},{F(max)}]");
        }

        public static void Run() => Execute("UNITY_STRUCTURAL_AUDIT.json", "STRUCTURAL_SCENE_DIGEST.json", true);
        public static void Baseline() => Execute("UNITY_STRUCTURAL_AUDIT_BASELINE.json", null, false);

        static string EvidencePath(string name) => Path.GetFullPath(Path.Combine(Application.dataPath,
            "../../../Docs/evidence/WP-ART-01/" + name));

        static string Sha(byte[] bytes)
        {
            using (var sha = SHA256.Create())
                return string.Concat(sha.ComputeHash(bytes).Select(b => b.ToString("x2")));
        }

        static void Execute(string reportName, string digestName, bool strict)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Checks.Clear();
            var report = new Report { unityVersion = Application.unityVersion };
            var manifestPath = EvidencePath("KIT_COMPOSITION_MANIFEST.json");
            var manifestText = File.ReadAllText(manifestPath, Encoding.UTF8);
            report.manifestSha256 = Sha(File.ReadAllBytes(manifestPath));
            var manifest = JsonUtility.FromJson<Manifest>(manifestText);
            kit = new Dictionary<string, MPiece>();
            foreach (var p in (manifest.pieces ?? new MPiece[0]).Concat(manifest.assemblies ?? new MPiece[0]))
                kit[p.id] = p;
            all = UnityEngine.Object.FindObjectsByType<Art01Piece>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            report.pieces = all.Length;
            var digest = Digest();
            report.structuralDigestSha256 = digest.Item1;
            AddVisColliders();
            bool back = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = true;
            Physics.SyncTransforms();
            try
            {
                Identity();
                foreach (var house in all.Where(p => p.assemblyRole == "BUILDING_ASSEMBLY"))
                    Building(house);
                GroundContact();
                Attachment();
                Street();
                Surfaces();
                Humans();
            }
            finally { Physics.queriesHitBackfaces = back; }
            var failures = Checks.Where(c => !c.pass).Select(c => $"{c.group}/{c.id} {c.subject}: measured {c.measured}, expected {c.expected}").ToArray();
            report.checks = Checks.ToArray();
            report.checkCount = Checks.Count;
            report.failures = failures;
            report.failureCount = failures.Length;
            report.passed = failures.Length == 0;
            File.WriteAllText(EvidencePath(reportName), JsonUtility.ToJson(report, true) + "\n");
            if (digestName != null)
                File.WriteAllText(EvidencePath(digestName), digest.Item2);
            Debug.Log($"ART01_STRUCTURAL_AUDIT {(report.passed ? "GREEN" : "RED")} checks={report.checkCount} failures={report.failureCount} digest={report.structuralDigestSha256}");
            if (strict && !report.passed)
                throw new Exception("ART01_STRUCTURAL_AUDIT_RED " + string.Join("; ", failures.Take(25)));
        }

        // ------------------------------------------------------------------ helpers

        static Art01Piece Owner(Component c) => c == null ? null : c.GetComponentInParent<Art01Piece>(true);

        static void AddVisColliders()
        {
            foreach (var mf in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                var mr = mf.GetComponent<MeshRenderer>();
                if (mr == null || !mr.enabled || mf.sharedMesh == null || mf.GetComponent<TextMesh>() != null) continue;
                var go = new GameObject("__art01_vis") { layer = VisLayer, hideFlags = HideFlags.DontSave };
                go.transform.SetParent(mf.transform, false);
                go.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
            }
        }

        static IEnumerable<Collider> VisOf(Art01Piece piece) =>
            piece.GetComponentsInChildren<MeshCollider>(true).Where(c => c.gameObject.layer == VisLayer && Owner(c) == piece);

        static bool Hit(Vector3 origin, Vector3 dir, float length, int mask, Func<Art01Piece, bool> accept, out RaycastHit best)
        {
            best = default;
            float bestDistance = float.MaxValue;
            bool found = false;
            foreach (var h in Physics.RaycastAll(origin, dir, length, mask, QueryTriggerInteraction.Ignore))
            {
                var owner = Owner(h.collider);
                if (accept != null && !accept(owner)) continue;
                if (h.distance < bestDistance) { bestDistance = h.distance; best = h; found = true; }
            }
            return found;
        }

        static Bounds LocalBounds(Transform frame, IEnumerable<Collider> colliders)
        {
            bool any = false;
            var b = new Bounds();
            foreach (var c in colliders)
            {
                var wb = c.bounds;
                for (int i = 0; i < 8; i++)
                {
                    var corner = new Vector3((i & 1) == 0 ? wb.min.x : wb.max.x, (i & 2) == 0 ? wb.min.y : wb.max.y,
                        (i & 4) == 0 ? wb.min.z : wb.max.z);
                    var local = frame.InverseTransformPoint(corner);
                    if (!any) { b = new Bounds(local, Vector3.zero); any = true; } else b.Encapsulate(local);
                }
            }
            return b;
        }

        static Bounds WorldBounds(Art01Piece piece)
        {
            var list = VisOf(piece).ToList();
            if (list.Count == 0)
            {
                var r = piece.GetComponentsInChildren<Renderer>(true).Where(x => Owner(x) == piece).ToList();
                if (r.Count == 0) return new Bounds(piece.transform.position, Vector3.zero);
                var rb = r[0].bounds; foreach (var x in r) rb.Encapsulate(x.bounds); return rb;
            }
            var b = list[0].bounds; foreach (var c in list) b.Encapsulate(c.bounds); return b;
        }

        static bool IsBuiltinCube(Mesh m) => m != null && m.name == "Cube" && AssetDatabase.GetAssetPath(m).StartsWith("Library/");

        // ------------------------------------------------------------------ identity / provenance

        static readonly HashSet<string> Structural = new HashSet<string> {
            "FACADE_HOST", "ROOF_DERIVED", "BUILDING_ASSEMBLY", "GABLE" };

        static void Identity()
        {
            var seen = new HashSet<string>();
            foreach (var p in all)
            {
                if (!seen.Add(p.logicalId)) Add("identity", "unique_logical_id", p.logicalId, false, "duplicate", "unique");
                bool inspection = p.assemblyRole == "INSPECTION_ONLY";
                if (!inspection && p.presentationState != "KEEPER_READY")
                    Add("identity", "presentation_state", p.logicalId, false, p.presentationState, "KEEPER_READY");
                if (string.IsNullOrEmpty(p.kitId) || !kit.TryGetValue(p.kitId, out var entry))
                {
                    Add("identity", "kit_id_resolves", p.logicalId, false, p.kitId ?? "<none>", "manifest piece/assembly id");
                    continue;
                }
                string okReadiness = inspection ? "INSPECTION_ONLY" : "KEEPER_READY";
                Add("identity", "kit_id_resolves", p.logicalId, entry.readiness == okReadiness, p.kitId + " " + entry.readiness, okReadiness);
                foreach (var mf in p.GetComponentsInChildren<MeshFilter>(true).Where(m => Owner(m) == p))
                {
                    var mesh = mf.sharedMesh;
                    if (IsBuiltinCube(mesh))
                    {
                        var s = mf.transform.lossyScale;
                        float minDim = Mathf.Min(Mathf.Abs(s.x), Mathf.Min(Mathf.Abs(s.y), Mathf.Abs(s.z)));
                        bool ok = entry.formPrimitive == "BOX" && !Structural.Contains(p.assemblyRole) && minDim <= 0.6f + 1e-4f;
                        Add("identity", "box_form_declared_linear_or_slab", p.logicalId, ok,
                            $"formPrimitive={entry.formPrimitive} role={p.assemblyRole} minDim={F(minDim)}",
                            "declared BOX kit piece, not facade/roof/building, one dimension <= 0.60 m");
                        continue;
                    }
                    var path = AssetDatabase.GetAssetPath(mesh);
                    if (path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) continue; // checked via prefab source
                    bool listed = entry.provenance?.unityAssets != null && entry.provenance.unityAssets.Contains(path);
                    Add("identity", "generated_mesh_in_kit_entry", p.logicalId, listed, path, p.kitId + ".provenance.unityAssets");
                }
                foreach (var t in p.GetComponentsInChildren<Transform>(true))
                {
                    if (!PrefabUtility.IsOutermostPrefabInstanceRoot(t.gameObject) || Owner(t) != p) continue;
                    var src = PrefabUtility.GetCorrespondingObjectFromOriginalSource(t.gameObject);
                    var path = AssetDatabase.GetAssetPath(src);
                    bool allowed = entry.kind == "source_model"
                        ? "Assets/Arkus/ART/External/" + entry.provenance?.sourceLockDest == path
                        : entry.provenance?.unityAssets != null && entry.provenance.unityAssets.Contains(path);
                    Add("identity", "source_model_matches_kit", p.logicalId, allowed, path, p.kitId);
                    var srcEntry = kit.Values.FirstOrDefault(k => k.kind == "source_model" &&
                        "Assets/Arkus/ART/External/" + k.provenance?.sourceLockDest == path);
                    if (path.StartsWith("Assets/Arkus/ART/External/"))
                    {
                        bool legal = srcEntry != null && srcEntry.readiness != "REJECT_FOR_KEEPER" &&
                            (srcEntry.readiness == "KEEPER_READY" || entry.kind == "juego2_derivative");
                        Add("identity", "source_admission", p.logicalId, legal,
                            srcEntry == null ? "unlocked source" : srcEntry.id + " " + srcEntry.readiness,
                            "KEEPER_READY source, or SOURCE_ONLY consumed only through a named derivative");
                    }
                }
            }
            foreach (var r in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var o = Owner(r);
                bool ok = o != null && o.assemblyRole != "BENCHMARK" && o.assemblyRole != "BUILDING_ASSEMBLY";
                if (!ok) Add("identity", "renderer_owned_by_kit_piece", r.gameObject.name, false,
                    o == null ? "untagged" : o.assemblyRole, "nearest Art01Piece is a kit piece");
            }
            foreach (var c in UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (c.gameObject.layer == VisLayer) continue;
                var o = Owner(c);
                if (o == null || o.assemblyRole == "BENCHMARK")
                    Add("identity", "collider_owned_by_kit_piece", c.gameObject.name, false, "untagged", "nearest Art01Piece is a kit piece");
            }
            Add("identity", "all_renderers_and_colliders_owned", "scene", !Checks.Any(c => !c.pass &&
                (c.id == "renderer_owned_by_kit_piece" || c.id == "collider_owned_by_kit_piece")), "see individual checks", "no untagged geometry");
        }

        // ------------------------------------------------------------------ buildings

        sealed class Host
        {
            public Art01Piece piece;
            public int side, storey;
            public Bounds local;           // house-local
            public bool hasOpening;
            public Rect opening;           // host-local (x, y)
            public Rect frameClear;        // host-local, doors only
            public string kind;
        }

        static int SideOf(Transform house, Transform host)
        {
            float yaw = Mathf.Repeat((Quaternion.Inverse(house.rotation) * host.rotation).eulerAngles.y, 360f);
            if (Mathf.Abs(Mathf.DeltaAngle(yaw, 180)) < 1) return 0; // front -z
            if (Mathf.Abs(Mathf.DeltaAngle(yaw, 0)) < 1) return 1;   // back +z
            if (Mathf.Abs(Mathf.DeltaAngle(yaw, 270)) < 1) return 2; // left -x
            return 3;                                                // right +x
        }

        static readonly string[] SideName = { "front", "back", "left", "right" };
        static Vector3 SideNormal(int side) => side == 0 ? Vector3.back : side == 1 ? Vector3.forward : side == 2 ? Vector3.left : Vector3.right;
        static float U(int side, Vector3 local) => side < 2 ? local.x : local.z;
        static float N(int side, Vector3 local) => side < 2 ? local.z : local.x;

        static Rect OpenRect(Art01Piece host, float step, Func<Art01Piece, bool> blocker, float height, out int openCells)
        {
            var T = host.transform;
            float umin = float.MaxValue, umax = float.MinValue, vmin = float.MaxValue, vmax = float.MinValue;
            openCells = 0;
            for (float u = -0.99f; u <= 0.99f + 1e-4f; u += step)
                for (float v = 0.01f; v <= height - 0.01f + 1e-4f; v += step)
                {
                    var origin = T.TransformPoint(new Vector3(u, v, 1.0f));
                    bool blocked = Hit(origin, -T.forward, 2.0f, VisMask, blocker, out _);
                    if (blocked) continue;
                    openCells++;
                    umin = Mathf.Min(umin, u); umax = Mathf.Max(umax, u); vmin = Mathf.Min(vmin, v); vmax = Mathf.Max(vmax, v);
                }
            if (openCells == 0) return new Rect();
            return Rect.MinMaxRect(umin - step * 0.5f, vmin - step * 0.5f, umax + step * 0.5f, vmax + step * 0.5f);
        }

        static void Building(Art01Piece house)
        {
            var H = house.transform;
            string hid = house.logicalId;
            var children = all.Where(p => p.transform.parent == H).ToList();
            var hosts = new List<Host>();
            foreach (var p in children.Where(p => p.assemblyRole == "FACADE_HOST"))
            {
                var h = new Host { piece = p, side = SideOf(H, p.transform), local = LocalBounds(H, VisOf(p)) };
                var key = ((p.kitId ?? "") + "|" + string.Join("|", p.GetComponentsInChildren<MeshFilter>(true)
                    .Where(m => Owner(m) == p && m.sharedMesh != null).Select(m => m.sharedMesh.name))).ToLowerInvariant();
                h.kind = key.Contains("door") ? "door" : key.Contains("window") ? "window" : "straight";
                hosts.Add(h);
            }
            if (hosts.Count == 0) { Add("building", "hosts_present", hid, false, "0", ">0"); return; }
            float floor = hosts.Min(h => h.local.min.y);
            foreach (var h in hosts) h.storey = Mathf.RoundToInt((h.local.min.y - floor) / 3.1f);
            int storeys = hosts.Max(h => h.storey) + 1;
            bool bar = children.Any(p => p.assemblyRole == "THRESHOLD");
            float eave = hosts.Max(h => h.local.max.y);

            // Storey heights and wall depth.
            for (int s = 0; s < storeys; s++)
            {
                var row = hosts.Where(h => h.storey == s).ToList();
                float height = row.Max(h => h.local.size.y);
                bool social = bar && s == 0;
                Band("dimension", social ? "ground_social_floor_to_floor" : "floor_to_floor", $"{hid} storey {s}", height,
                    social ? 3.0f : 2.8f, social ? 3.6f : 3.4f);
            }
            foreach (var h in hosts)
                Band("dimension", "wall_depth_at_openings_returns", h.piece.logicalId,
                    h.side < 2 ? h.local.size.z : h.local.size.x, 0.25f, 0.50f);

            // Openings measured by through-rays against the host only, then host + frame.
            foreach (var h in hosts)
            {
                float height = h.local.size.y;
                var rect = OpenRect(h.piece, h.kind == "straight" ? 0.1f : 0.025f, o => o == h.piece, height, out int cells);
                h.hasOpening = cells > 0;
                h.opening = rect;
                if (h.kind == "straight")
                {
                    Add("opening", "plain_host_is_closed", h.piece.logicalId, !h.hasOpening, $"{cells} open cells", "0");
                    continue;
                }
                if (!h.hasOpening) { Add("opening", "hosted_opening_exists", h.piece.logicalId, false, "none", "real through-opening"); continue; }
                float fill = cells * (h.kind == "straight" ? 0.01f : 0.000625f) / (rect.width * rect.height);
                Add("opening", "single_rectangular_opening", h.piece.logicalId, fill > 0.85f, F(fill), "> 0.85 of bounding rect open");
                if (h.kind == "window")
                {
                    Band("dimension", "window_opening_width", h.piece.logicalId, rect.width, 0.65f, 1.40f, h.piece.kitId, "WINDOW_OPENING_WIDTH");
                    Band("dimension", "window_opening_height", h.piece.logicalId, rect.height, 0.80f, 1.55f, h.piece.kitId, "WINDOW_OPENING_HEIGHT");
                    Band("dimension", "window_sill_above_floor", h.piece.logicalId, rect.yMin, 0.70f, 1.10f, h.piece.kitId, "WINDOW_SILL");
                }
                else
                {
                    string frameId = h.piece.logicalId + ".frame";
                    var clear = OpenRect(h.piece, 0.02f, o => o == h.piece || (o != null && o.logicalId == frameId), height, out _);
                    h.frameClear = clear;
                    bool isPublic = all.Any(p => p.logicalId == h.piece.logicalId + ".open_leaf");
                    Band("dimension", isPublic ? "public_door_clear_width" : "private_door_clear_width", h.piece.logicalId,
                        clear.width, isPublic ? 0.85f : 0.75f, isPublic ? 1.10f : 1.00f, "art01.quaternius.medieval.doorframe_flat_brick",
                        isPublic ? "PUBLIC_DOOR_CLEAR_WIDTH" : "PRIVATE_DOOR_CLEAR_WIDTH");
                    Band("dimension", "door_clear_height", h.piece.logicalId, clear.yMax, 1.95f, 2.20f,
                        "art01.quaternius.medieval.doorframe_flat_brick", "DOOR_CLEAR_HEIGHT");
                    var leaf = all.FirstOrDefault(p => p.logicalId == h.piece.logicalId + ".leaf");
                    if (leaf != null)
                    {
                        var frame = all.FirstOrDefault(p => p.logicalId == frameId);
                        var T = h.piece.transform;
                        float outerFace = LocalBounds(T, VisOf(h.piece)).max.z;
                        if (frame != null) outerFace = Mathf.Max(outerFace, LocalBounds(T, VisOf(frame)).max.z);
                        float leafFace = LocalBounds(T, VisOf(leaf)).max.z;
                        Band("dimension", "door_reveal_depth", leaf.logicalId, outerFace - leafFace, 0.10f, 0.40f);
                    }
                }
            }
            // Adjacent openings on the same side/storey keep >= 0.25 m of host between them.
            foreach (var group in hosts.Where(h => h.hasOpening && h.kind != "straight").GroupBy(h => (h.side, h.storey)))
            {
                var spans = group.Select(h =>
                {
                    var a = H.InverseTransformPoint(h.piece.transform.TransformPoint(new Vector3(h.opening.xMin, 1, 0)));
                    var b = H.InverseTransformPoint(h.piece.transform.TransformPoint(new Vector3(h.opening.xMax, 1, 0)));
                    return (Mathf.Min(U(h.side, a), U(h.side, b)), Mathf.Max(U(h.side, a), U(h.side, b)), h.piece.logicalId);
                }).OrderBy(x => x.Item1).ToList();
                for (int i = 1; i < spans.Count; i++)
                    Band("dimension", "opening_separation", spans[i - 1].Item3 + "|" + spans[i].Item3,
                        spans[i].Item1 - spans[i - 1].Item2, 0.25f, 99f);
            }

            Inserts(H, hid, hosts);
            Envelope(H, hid, hosts, children, floor, eave, storeys);
            Roof(H, hid, hosts, children, eave);
            Base(H, hid, hosts, children, floor);
            Interior(hid, children);
            if (bar) Threshold(H, hid, hosts, children);
        }

        static void Inserts(Transform H, string hid, List<Host> hosts)
        {
            var inserts = all.Where(p => p.assemblyRole == "HOSTED_INSERT" && p.transform.parent == H).ToList();
            var byHost = hosts.ToDictionary(h => h.piece.logicalId);
            var placed = new List<(Host host, Bounds local, Art01Piece piece)>();
            foreach (var ins in inserts)
            {
                string hostId = ins.logicalId.Substring(0, ins.logicalId.LastIndexOf('.'));
                if (!byHost.TryGetValue(hostId, out var host))
                {
                    Add("insert", "insert_has_host", ins.logicalId, false, "no host " + hostId, "host facade piece");
                    continue;
                }
                var T = host.piece.transform;
                var lb = LocalBounds(T, VisOf(ins));
                bool swing = ins.logicalId.EndsWith(".open_leaf");
                if (!swing)
                {
                    bool centred = host.hasOpening && host.opening.Contains(new Vector2(lb.center.x, lb.center.y)) ||
                        host.hasOpening && Mathf.Abs(lb.center.x - host.opening.center.x) < 0.05f && lb.center.y > host.opening.yMin - 0.05f && lb.center.y < host.opening.yMax + 0.30f;
                    Add("insert", "insert_fills_real_opening", ins.logicalId, host.hasOpening && centred,
                        host.hasOpening ? $"centre ({F(lb.center.x)},{F(lb.center.y)}) in opening {F(host.opening.xMin)}..{F(host.opening.xMax)} x {F(host.opening.yMin)}..{F(host.opening.yMax)}" : "host has no opening",
                        "insert centred on a real host opening (no sticker)");
                }
                placed.Add((host, LocalBounds(H, VisOf(ins)), ins));
            }
            // Inserts may not project beyond the facade end or clash with another host's insert.
            foreach (var g in placed.GroupBy(x => (x.host.side, x.host.storey)))
            {
                var rowHosts = hosts.Where(h => h.side == g.Key.side && h.storey == g.Key.storey).ToList();
                float fu0 = rowHosts.Min(h => Mathf.Min(U(h.side, h.local.min), U(h.side, h.local.max)));
                float fu1 = rowHosts.Max(h => Mathf.Max(U(h.side, h.local.min), U(h.side, h.local.max)));
                var list = g.ToList();
                foreach (var x in list)
                {
                    float a = Mathf.Min(U(g.Key.side, x.local.min), U(g.Key.side, x.local.max));
                    float b = Mathf.Max(U(g.Key.side, x.local.min), U(g.Key.side, x.local.max));
                    float over = Mathf.Max(fu0 - a, b - fu1, 0);
                    Add("insert", "insert_within_facade_extent", x.piece.logicalId, over <= ContactTol, F(over), $"<= {F(ContactTol)} beyond {SideName[g.Key.side]} facade end");
                }
                for (int i = 0; i < list.Count; i++)
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        if (list[i].host == list[j].host) continue;
                        var bi = list[i].local; var bj = list[j].local;
                        float ov = Mathf.Min(Mathf.Min(bi.max.x, bj.max.x) - Mathf.Max(bi.min.x, bj.min.x),
                            Mathf.Min(Mathf.Min(bi.max.y, bj.max.y) - Mathf.Max(bi.min.y, bj.min.y),
                                Mathf.Min(bi.max.z, bj.max.z) - Mathf.Max(bi.min.z, bj.min.z)));
                        if (ov > ContactTol)
                            Add("insert", "no_insert_clash_between_hosts", list[i].piece.logicalId + "|" + list[j].piece.logicalId, false, F(ov), $"<= {F(ContactTol)} overlap");
                    }
                Add("insert", "no_insert_clash_between_hosts", $"{hid} {SideName[g.Key.side]} storey {g.Key.storey}",
                    !Checks.Any(c => !c.pass && c.id == "no_insert_clash_between_hosts" && c.subject.Contains(hid + ".")), "pairwise", "no clash");
            }
        }

        static void Envelope(Transform H, string hid, List<Host> hosts, List<Art01Piece> children, float floor, float eave, int storeys)
        {
            var roof = children.FirstOrDefault(p => p.assemblyRole == "ROOF_DERIVED");
            var roofLocal = roof != null ? LocalBounds(H, VisOf(roof)) : new Bounds();
            bool ridgeAlongZ = roofLocal.size.z >= roofLocal.size.x;
            float roofTop = roof != null ? roofLocal.max.y : eave;
            var openings = new List<(int side, Rect r)>();
            foreach (var h in hosts.Where(h => h.hasOpening))
            {
                var a = H.InverseTransformPoint(h.piece.transform.TransformPoint(new Vector3(h.opening.xMin, h.opening.yMin, 0)));
                var b = H.InverseTransformPoint(h.piece.transform.TransformPoint(new Vector3(h.opening.xMax, h.opening.yMax, 0)));
                openings.Add((h.side, Rect.MinMaxRect(Mathf.Min(U(h.side, a), U(h.side, b)) - 0.03f, Mathf.Min(a.y, b.y) - 0.03f,
                    Mathf.Max(U(h.side, a), U(h.side, b)) + 0.03f, Mathf.Max(a.y, b.y) + 0.03f)));
            }
            Func<Art01Piece, bool> mine = o => o != null && o.transform.IsChildOf(H) && o.assemblyRole != "NATURE" && o.assemblyRole != "DRESSING";
            Func<Art01Piece, bool> roofSkin = o => o != null && o.transform.IsChildOf(H) && o.assemblyRole == "ROOF_DERIVED";
            for (int side = 0; side < 4; side++)
            {
                var sideHosts = hosts.Where(h => h.side == side).ToList();
                if (sideHosts.Count == 0) continue;
                bool eaveSide = ridgeAlongZ ? side >= 2 : side < 2;
                float top = eaveSide ? eave + 0.10f : roofTop;
                var n = SideNormal(side);
                var inward = H.TransformDirection(-n);
                int leaks = 0, samples = 0;
                var examples = new List<string>();
                var modules = sideHosts.Select(h => U(side, h.local.center)).ToList();
                for (float y = floor - 0.12f; y <= top + 1e-4f; y += 0.05f)
                {
                    int s = Mathf.Clamp(Mathf.FloorToInt((y - floor) / 3.1227f), 0, storeys - 1);
                    var row = sideHosts.Where(h => h.storey == s).ToList();
                    if (row.Count == 0) row = sideHosts;
                    float plane = side == 0 || side == 2 ? row.Min(h => N(side, h.local.min)) : row.Max(h => N(side, h.local.max));
                    float u0 = row.Min(h => Mathf.Min(U(side, h.local.min), U(side, h.local.max)));
                    float u1 = row.Max(h => Mathf.Max(U(side, h.local.min), U(side, h.local.max)));
                    var us = new List<float>();
                    for (float u = u0 + 0.03f; u <= u1 - 0.03f + 1e-4f; u += 0.05f) us.Add(u);
                    foreach (var m in modules) { us.Add(m - 1f + 0.004f); us.Add(m + 1f - 0.004f); }
                    foreach (var u in us)
                    {
                        if (u <= u0 || u >= u1) continue;
                        var local = side < 2 ? new Vector3(u, y, plane) : new Vector3(plane, y, u);
                        var origin = H.TransformPoint(local + n * 0.5f);
                        samples++;
                        bool hit = Hit(origin, inward, 40f, VisMask, mine, out var rh);
                        float p = hit ? rh.distance - 0.5f : float.PositiveInfinity;
                        if (p <= LeakDepth) continue;
                        if (openings.Any(o => o.side == side && o.r.Contains(new Vector2(u, y)))) continue;
                        // Inside the envelope = below the roof-skin underside measured from inside the building.
                        var interior = local - n * 0.5f; interior.y = floor + 0.3f;
                        bool inside = Hit(H.TransformPoint(interior), Vector3.up, 30f, VisMask, roofSkin, out var skin) &&
                            H.TransformPoint(local).y < skin.point.y - 0.005f;
                        if (!inside) continue; // exterior roof detail or open sky beside/above the silhouette
                        leaks++;
                        if (examples.Count < 12) examples.Add($"u={F(u)} y={F(y)} depth={(hit ? F(p) : "through")}");
                    }
                }
                Add("envelope", "facade_envelope_closed", $"{hid} {SideName[side]}", leaks == 0,
                    $"{leaks}/{samples} leaking probes {string.Join("; ", examples)}",
                    $"no probe passes > {F(LeakDepth)} m past the facade plane outside a real opening (eave slit, jamb void, gap, notch)");
            }
            // Diagonal corner probes: a re-entrant notch lets the probe travel past the envelope corner.
            for (int s = 0; s < storeys; s++)
            {
                var row = hosts.Where(h => h.storey == s).ToList();
                float x0 = row.Where(h => h.side == 2).Min(h => h.local.min.x), x1 = row.Where(h => h.side == 3).Max(h => h.local.max.x);
                float z0 = row.Where(h => h.side == 0).Min(h => h.local.min.z), z1 = row.Where(h => h.side == 1).Max(h => h.local.max.z);
                float y0 = row.Min(h => h.local.min.y), y1 = row.Max(h => h.local.max.y);
                foreach (float y in new[] { y0 + 0.15f, (y0 + y1) * 0.5f, y1 - 0.06f })
                    foreach (var (cx, cz) in new[] { (x0, z0), (x1, z0), (x0, z1), (x1, z1) })
                    {
                        var dir = new Vector3(cx < 0 ? 1 : -1, 0, cz < 0 ? 1 : -1).normalized;
                        var origin = H.TransformPoint(new Vector3(cx, y, cz) - dir * 0.5f);
                        bool hit = Hit(origin, H.TransformDirection(dir), 5f, VisMask, mine, out var rh);
                        float p = hit ? rh.distance - 0.5f : 99f;
                        Add("envelope", "corner_closed", $"{hid} storey {s} corner ({F(cx)},{F(cz)}) y={F(y)}", p <= 0.07f,
                            F(p), "<= 0.070 m past envelope corner (no re-entrant notch)");
                    }
            }
        }

        static void Roof(Transform H, string hid, List<Host> hosts, List<Art01Piece> children, float eave)
        {
            var roof = children.FirstOrDefault(p => p.assemblyRole == "ROOF_DERIVED");
            if (roof == null) { Add("roof", "roof_present", hid, false, "none", "roof assembly"); return; }
            var rl = LocalBounds(H, VisOf(roof));
            bool ridgeAlongZ = rl.size.z >= rl.size.x;
            var top = hosts.Where(h => h.local.max.y > eave - 0.01f).ToList();
            Func<Art01Piece, bool> capping = o => o != null && o.transform.IsChildOf(H) &&
                (o.assemblyRole == "ROOF_DERIVED" || o.assemblyRole == "GABLE" || o.assemblyRole == "EAVE");
            foreach (var side in ridgeAlongZ ? new[] { 2, 3 } : new[] { 0, 1 })
            {
                var row = top.Where(h => h.side == side).ToList();
                float face = side == 2 || side == 0 ? row.Min(h => N(side, h.local.min)) : row.Max(h => N(side, h.local.max));
                float worst = 0;
                foreach (var h in row)
                    for (float u = -0.9f; u <= 0.9f; u += 0.3f)
                    {
                        float along = U(side, h.local.center) + u;
                        var p = side < 2 ? new Vector3(along, eave - 0.05f, face - SideNormal(side).z * 0.01f)
                                         : new Vector3(face - SideNormal(side).x * 0.01f, eave - 0.05f, along);
                        float gap = Hit(H.TransformPoint(p), H.up, 3f, VisMask, capping, out var rh) ? rh.point.y - eave : 3f;
                        worst = Mathf.Max(worst, gap);
                    }
                Add("roof", "roof_underside_meets_wall_top", $"{hid} {SideName[side]}", worst <= JoinTol, F(worst), $"<= {F(JoinTol)} m");
                float eaveEdge = side == 2 ? -rl.min.x : side == 3 ? rl.max.x : side == 0 ? -rl.min.z : rl.max.z;
                Band("dimension", "eave_projection_from_wall_face", $"{hid} {SideName[side]}", eaveEdge - Mathf.Abs(face), 0.25f, 0.65f);
            }
            // Pitch from two top-surface samples on each slope.
            float half = ridgeAlongZ ? rl.extents.x : rl.extents.z;
            var slopes = new List<float>();
            foreach (int s in new[] { -1, 1 })
            {
                float a = s * half * 0.25f, b = s * half * 0.65f;
                Vector3 Pt(float o) => ridgeAlongZ ? new Vector3(o, rl.max.y + 1, rl.center.z) : new Vector3(rl.center.x, rl.max.y + 1, o);
                if (Hit(H.TransformPoint(Pt(a)), Vector3.down, 20f, VisMask, o => o == roof, out var ha) &&
                    Hit(H.TransformPoint(Pt(b)), Vector3.down, 20f, VisMask, o => o == roof, out var hb))
                    slopes.Add(Mathf.Atan2(ha.point.y - hb.point.y, Mathf.Abs(b - a)) * Mathf.Rad2Deg);
            }
            float pitch = slopes.Count == 0 ? -1 : slopes.Average();
            MPiece re = null;
            if (roof.kitId != null) kit.TryGetValue(roof.kitId, out re);
            Add("dimension", "roof_pitch_declared_band", roof.logicalId,
                re != null && re.roofPitchMaxDeg > 0 && pitch >= re.roofPitchMinDeg && pitch <= re.roofPitchMaxDeg,
                F(pitch) + " deg", re == null ? "declared" : $"[{F(re.roofPitchMinDeg)},{F(re.roofPitchMaxDeg)}] deg");
            foreach (var g in children.Where(p => p.assemblyRole == "GABLE"))
            {
                var gl = LocalBounds(H, VisOf(g));
                int side = ridgeAlongZ ? (gl.center.z < 0 ? 0 : 1) : (gl.center.x < 0 ? 2 : 3);
                var row = top.Where(h => h.side == side).ToList();
                if (row.Count == 0) continue;
                float wallFace = side == 0 || side == 2 ? row.Min(h => N(side, h.local.min)) : row.Max(h => N(side, h.local.max));
                float gableFace = side == 0 || side == 2 ? N(side, gl.min) : N(side, gl.max);
                Add("roof", "gable_face_flush_with_wall", g.logicalId, Mathf.Abs(gableFace - wallFace) <= JoinTol,
                    F(Mathf.Abs(gableFace - wallFace)), $"<= {F(JoinTol)} m");
            }
        }

        static void Base(Transform H, string hid, List<Host> hosts, List<Art01Piece> children, float floor)
        {
            // Every host bottom rests on plinth/threshold/floor (ground storey) or on the storey below.
            foreach (var h in hosts.Where(h => h.storey > 0))
            {
                var below = hosts.FirstOrDefault(l => l.side == h.side && l.storey == h.storey - 1 &&
                    Mathf.Abs(U(h.side, l.local.center) - U(h.side, h.local.center)) < 0.01f);
                bool stacked = below != null && Mathf.Abs(below.local.max.y - h.local.min.y) <= ContactTol &&
                    Mathf.Abs(N(h.side, below.local.min) - N(h.side, h.local.min)) <= ContactTol &&
                    Mathf.Abs(N(h.side, below.local.max) - N(h.side, h.local.max)) <= ContactTol;
                Add("contact", "upper_storey_stacked_flush", h.piece.logicalId, stacked,
                    below == null ? "no host below" : $"top gap {F(below.local.max.y - h.local.min.y)}, face offsets {F(N(h.side, below.local.min) - N(h.side, h.local.min))}/{F(N(h.side, below.local.max) - N(h.side, h.local.max))}",
                    $"same-module host below; top and both faces within {F(ContactTol)} m");
            }
            foreach (var h in hosts.Where(h => h.storey == 0))
            {
                var T = h.piece.transform;
                var lb = LocalBounds(T, VisOf(h.piece));
                float zmid = lb.center.z;
                int unsupported = 0, n = 0;
                for (float u = -0.95f; u <= 0.951f; u += 0.1f)
                {
                    n++;
                    var origin = T.TransformPoint(new Vector3(u, 0.06f, zmid));
                    Func<Art01Piece, bool> support = o => o != null && o != h.piece &&
                        (o.assemblyRole == "PLINTH" || o.assemblyRole == "THRESHOLD" || o.assemblyRole == "INTERIOR_FLOOR");
                    if (!Hit(origin, -T.up, 0.06f + ContactTol + 0.01f, VisMask, support, out _)) unsupported++;
                }
                Add("contact", "facade_base_supported", h.piece.logicalId, unsupported == 0, $"{unsupported}/{n} unsupported samples",
                    "plinth, threshold or public floor within 0.02 m");
            }
            // Plinth visible zone above the adjacent street/scenic ground (not kerb).
            foreach (var p in children.Where(p => p.assemblyRole == "PLINTH"))
            {
                var wb = WorldBounds(p);
                var lb = LocalBounds(H, VisOf(p));
                float best = float.MaxValue;
                foreach (var dir in new[] { Vector3.left, Vector3.right, Vector3.forward, Vector3.back })
                {
                    var probe = lb.center + Vector3.Scale(dir, lb.extents + Vector3.one * 0.35f);
                    probe.y = lb.max.y + 0.5f;
                    var world = H.TransformPoint(probe);
                    if (Hit(world, Vector3.down, 2f, VisMask, o => o != null && (o.assemblyRole == "STREET" || o.assemblyRole == "SCENIC"), out var g) &&
                        !Hit(world, Vector3.down, g.distance - 0.01f, VisMask, o => o != null && o.transform.IsChildOf(H), out _))
                        best = Mathf.Min(best, wb.max.y - g.point.y);
                }
                if (best < float.MaxValue) Band("dimension", "plinth_visible_above_ground", p.logicalId, best, 0.15f, 0.60f);
            }
        }

        static void Interior(string hid, List<Art01Piece> children)
        {
            var floorPiece = children.FirstOrDefault(p => p.assemblyRole == "INTERIOR_FLOOR");
            var ceiling = children.FirstOrDefault(p => p.assemblyRole == "INTERIOR_CEILING");
            if (floorPiece == null || ceiling == null) return;
            Band("dimension", "clear_room_height", hid, WorldBounds(ceiling).min.y - WorldBounds(floorPiece).max.y, 2.4f, 3.0f);
        }

        static void Threshold(Transform H, string hid, List<Host> hosts, List<Art01Piece> children)
        {
            var door = hosts.FirstOrDefault(h => all.Any(p => p.logicalId == h.piece.logicalId + ".open_leaf"));
            if (door == null) { Add("threshold", "public_door_present", hid, false, "none", "public open door host"); return; }
            var T = door.piece.transform;
            var centre = T.TransformPoint(new Vector3(door.frameClear.center.x, 0, 0));
            float outer = LocalBounds(T, VisOf(door.piece)).max.z;
            var facePoint = T.TransformPoint(new Vector3(0, 0, outer));
            var outward = T.forward;
            // Profile along the public approach (gameplay colliders only).
            var profile = new List<(float d, float y, string owner)>();
            for (float d = 3.0f; d >= -1.0f; d -= 0.02f)
            {
                var p = new Vector3(centre.x, 4f, centre.z) + outward * (d + Vector3.Dot(facePoint - centre, outward));
                if (Hit(p, Vector3.down, 6f, PlayMask, o => o != null &&
                        (o.assemblyRole == "STREET" || o.assemblyRole == "THRESHOLD" || o.assemblyRole == "INTERIOR_FLOOR"), out var h))
                    profile.Add((d, h.point.y, Owner(h.collider).logicalId));
            }
            var risers = new List<(float d, float rise)>();
            for (int i = 1; i < profile.Count; i++)
            {
                float dy = profile[i].y - profile[i - 1].y;
                if (dy > 0.02f) risers.Add((profile[i].d, dy));
            }
            Add("threshold", "public_approach_risers", hid, risers.Count >= 1 && risers.All(r => r.rise >= 0.12f - 0.0005f && r.rise <= 0.18f + 0.0005f),
                string.Join(", ", risers.Select(r => $"{F(r.rise)}@{F(r.d)}")), "each riser [0.120,0.180] m");
            if (risers.Count > 0)
                Band("dimension", "threshold_landing_depth", hid, risers[risers.Count - 1].d, 0.90f, 1.50f);
            var inside = profile.Where(x => x.d < -0.05f).ToList();
            var landingTop = profile.Where(x => x.d > 0.05f && x.d < 0.5f).Select(x => x.y).DefaultIfEmpty(float.NaN).Max();
            bool level = inside.Count > 0 && inside.All(x => Mathf.Abs(x.y - landingTop) <= ContactTol);
            Add("threshold", "landing_continues_level_into_public_floor", hid, level,
                inside.Count == 0 ? "no interior support" : $"landing {F(landingTop)} interior {F(inside.Min(x => x.y))}..{F(inside.Max(x => x.y))}",
                $"|delta| <= {F(ContactTol)} m");
            // Body clearance through the doorway and along the approach (0.56 m wide body, 1.70 m tall).
            int blocked = 0; var blockers = new HashSet<string>();
            foreach (var x in profile.Where(x => x.d <= 1.5f && x.d >= -0.6f))
            {
                var foot = new Vector3(centre.x, x.y, centre.z) + outward * (x.d + Vector3.Dot(facePoint - centre, outward));
                foreach (var c in Physics.OverlapCapsule(foot + Vector3.up * 0.40f, foot + Vector3.up * 1.42f, 0.28f, PlayMask | VisMask, QueryTriggerInteraction.Ignore))
                {
                    var o = Owner(c);
                    if (o == null || o.assemblyRole == "THRESHOLD" || o.assemblyRole == "INTERIOR_FLOOR" || o.assemblyRole == "STREET" || o.assemblyRole == "INSPECTION_ONLY") continue;
                    blocked++; blockers.Add(o.logicalId);
                }
            }
            Add("threshold", "public_door_body_clearance", hid, blocked == 0, blocked == 0 ? "clear" : string.Join(",", blockers),
                "0.56 m x 1.70 m body passes step, landing and doorway");
            foreach (var post in children.Where(p => p.assemblyRole == "PORCH_POST"))
            {
                var pb = WorldBounds(post);
                bool top = Hit(new Vector3(pb.center.x, pb.max.y - 0.03f, pb.center.z), Vector3.up, 0.2f, VisMask,
                    o => o != null && o.assemblyRole == "PORCH_ROOF", out var ch);
                Add("threshold", "porch_post_supports_canopy", post.logicalId, top && ch.distance - 0.03f <= ContactTol,
                    top ? F(ch.distance - 0.03f) : "no canopy above", $"<= {F(ContactTol)} m");
            }
            var fascia = children.Where(p => p.assemblyRole == "EAVE" && p.logicalId.Contains("porch")).Select(WorldBounds).ToList();
            if (fascia.Count > 0)
            {
                var fb = fascia[0];
                float ground = Hit(new Vector3(fb.center.x, fb.min.y - 0.01f, fb.center.z), Vector3.down, 4f, PlayMask, null, out var gh) ? gh.point.y : fb.min.y - 5f;
                Band("dimension", "porch_clear_head_height", hid, fb.min.y - ground, 2.1f, 2.8f);
            }
        }

        // ------------------------------------------------------------------ contact / attachment

        static readonly HashSet<string> GroundRoles = new HashSet<string> {
            "STREET", "SCENIC", "SCENIC_WATER", "THRESHOLD", "INTERIOR_FLOOR", "INTERIOR_FURNITURE", "EDGE", "RETAINING", "BRIDGE_SPAN", "PLINTH", "PORCH_FOOT", "PARAPET" };

        static void GroundContact()
        {
            // Box-form grounded pieces: every bottom corner is at or below its support (embedded is fine).
            foreach (var p in all.Where(p => new[] { "PLINTH", "RETAINING", "THRESHOLD", "PORCH_FOOT", "PARAPET" }.Contains(p.assemblyRole)))
            {
                var mf = p.GetComponent<MeshFilter>();
                if (mf == null || !IsBuiltinCube(mf.sharedMesh)) continue;
                int floating = 0; float worst = 0;
                foreach (var (x, z) in new[] { (-0.5f, -0.5f), (0.5f, -0.5f), (-0.5f, 0.5f), (0.5f, 0.5f), (0f, 0f) })
                {
                    var bottom = p.transform.TransformPoint(new Vector3(x * 0.98f, -0.5f, z * 0.98f));
                    float gap = Support(p, bottom);
                    if (gap > ContactTol) { floating++; worst = Mathf.Max(worst, gap); }
                }
                Add("contact", "grounded_box_bottom_supported", p.logicalId, floating == 0,
                    floating == 0 ? "supported/embedded" : $"{floating} corners float, worst {F(worst)}", $"gap <= {F(ContactTol)} m");
            }
            // Mesh-form edges/kerbs: sample the bottom line through vertices.
            foreach (var p in all.Where(p => p.assemblyRole == "EDGE" || p.assemblyRole == "SCENIC_WATER"))
            {
                var mf = p.GetComponent<MeshFilter>();
                if (mf == null || IsBuiltinCube(mf.sharedMesh) || p.assemblyRole == "SCENIC_WATER") continue;
                var verts = mf.sharedMesh.vertices;
                float minY = verts.Min(v => v.y);
                int floating = 0; float worst = 0;
                foreach (var v in verts.Where(v => v.y <= minY + 1e-4f))
                {
                    float gap = Support(p, p.transform.TransformPoint(v));
                    if (gap > ContactTol) { floating++; worst = Mathf.Max(worst, gap); }
                }
                Add("contact", "edge_foot_embedded", p.logicalId, floating == 0, floating == 0 ? "embedded" : $"{floating} feet float, worst {F(worst)}", $"gap <= {F(ContactTol)} m");
            }
            // Source-model dressing/nature/scale references: pivot rests on a valid support.
            foreach (var p in all.Where(p => p.assemblyRole == "NATURE" || p.assemblyRole == "DRESSING" || p.assemblyRole == "SCALE_REFERENCE"))
            {
                if (p.connection != null && p.connection.StartsWith("ATTACHED_TO")) continue;
                if (p.transform.parent != null && p.transform.parent.GetComponent<Art01WalkInspector>() != null) continue;
                var pivot = p.transform.position;
                float gap = Support(p, pivot, true);
                Band("contact", "pivot_rests_on_support", p.logicalId, gap, -0.12f, ContactTol);
            }
        }

        /// <summary>Signed gap from a bottom point down to the nearest support; negative = embedded.</summary>
        static float Support(Art01Piece self, Vector3 bottom, bool dressing = false)
        {
            var origin = bottom + Vector3.up * 0.5f;
            float best = float.PositiveInfinity;
            foreach (var h in Physics.RaycastAll(origin, Vector3.down, 4f, VisMask, QueryTriggerInteraction.Ignore))
            {
                var o = Owner(h.collider);
                if (o == null || o == self || !GroundRoles.Contains(o.assemblyRole)) continue;
                if (dressing && o.assemblyRole == "PLINTH") continue;
                float gap = bottom.y - h.point.y;
                if (gap < -0.5f + 1e-3f) { best = Mathf.Min(best, -0.5f); continue; } // covered: embedded under a support surface
                if (Mathf.Abs(gap) < Mathf.Abs(best)) best = gap;
            }
            return best;
        }

        static readonly HashSet<string> Floaters = new HashSet<string> {
            "EAVE", "CAP", "SIGN", "SIGN_INSERT", "PORCH_ROOF", "PORCH_POST", "CORNER", "CORNER_CLOSURE", "GABLE",
            "INTERIOR_CEILING", "INTERIOR_FURNITURE", "HOSTED_INSERT", "PARAPET", "BRIDGE_SPAN" };

        static void Attachment()
        {
            foreach (var p in all.Where(p => Floaters.Contains(p.assemblyRole) ||
                         p.connection != null && p.connection.StartsWith("ATTACHED_TO")))
            {
                Bounds b;
                if (p.assemblyRole == "SIGN_INSERT")
                {
                    var r = p.GetComponent<Renderer>();
                    b = r != null ? r.bounds : new Bounds(p.transform.position, Vector3.one * 0.01f);
                }
                else b = WorldBounds(p);
                var touching = Physics.OverlapBox(b.center, b.extents + Vector3.one * ContactTol, Quaternion.identity, VisMask, QueryTriggerInteraction.Ignore)
                    .Select(Owner).Where(o => o != null && o != p && o.assemblyRole != "NATURE" && o.assemblyRole != "SCALE_REFERENCE" &&
                        !(o.assemblyRole == "DRESSING" && p.assemblyRole != "DRESSING"))
                    .Select(o => o.logicalId).Distinct().ToList();
                Add("contact", "attached_piece_touches_host", p.logicalId, touching.Count > 0,
                    touching.Count > 0 ? string.Join(",", touching.Take(3)) : "floating", $"touches another kit piece within {F(ContactTol)} m");
            }
            // One readable face toward the public side for every sign lettering.
            foreach (var p in all.Where(p => p.assemblyRole == "SIGN_INSERT"))
            {
                var board = all.FirstOrDefault(b => b.assemblyRole == "SIGN" && b.transform.parent == p.transform.parent);
                if (board == null) { Add("contact", "sign_lettering_readable", p.logicalId, false, "no board", "board"); continue; }
                var toViewer = p.transform.position - WorldBounds(board).center;
                bool readable = Vector3.Dot(-p.transform.forward, toViewer) > 0;
                Add("contact", "sign_lettering_readable", p.logicalId, readable, readable ? "faces away from board" : "mirrored/behind board",
                    "text readable from the side it is mounted on");
            }
            // Dressing/nature may not penetrate unrelated structure (15% shrunk bounds).
            foreach (var p in all.Where(p => p.assemblyRole == "NATURE" || p.assemblyRole == "DRESSING"))
            {
                if (p.connection != null && p.connection.StartsWith("ATTACHED_TO")) continue;
                var b = WorldBounds(p);
                var shrunk = new Vector3(b.extents.x * 0.85f, b.extents.y * 0.85f, b.extents.z * 0.85f);
                var centre = b.center + Vector3.up * (b.extents.y * 0.15f + 0.03f);
                var hits = Physics.OverlapBox(centre, shrunk, Quaternion.identity, VisMask, QueryTriggerInteraction.Ignore)
                    .Select(Owner).Where(o => o != null && o != p && !GroundSupportOnly(o) && o.assemblyRole != "NATURE" && o.assemblyRole != "DRESSING" && o.assemblyRole != "SCALE_REFERENCE")
                    .Select(o => o.logicalId).Distinct().ToList();
                Add("contact", "dressing_clear_of_structure", p.logicalId, hits.Count == 0, hits.Count == 0 ? "clear" : string.Join(",", hits.Take(4)),
                    "no structural piece inside the dressing/nature volume");
            }
        }

        static bool GroundSupportOnly(Art01Piece o) =>
            o.assemblyRole == "SCENIC" || o.assemblyRole == "STREET" || o.assemblyRole == "INTERIOR_FLOOR" || o.assemblyRole == "INTERIOR_FURNITURE" || o.assemblyRole == "THRESHOLD";

        // ------------------------------------------------------------------ street

        static void Street()
        {
            var road = all.FirstOrDefault(p => p.assemblyRole == "STREET");
            if (road == null) { Add("street", "street_present", "benchmark", false, "none", "one street piece"); return; }
            int colliders = road.GetComponents<MeshCollider>().Length;
            Add("street", "single_traversable_collider", road.logicalId, colliders == 1, colliders.ToString(), "1");
            var mesh = road.GetComponent<MeshFilter>().sharedMesh;
            var stations = mesh.vertices.Select(v => road.transform.TransformPoint(v)).GroupBy(v => Mathf.Round(v.z * 1000f) / 1000f)
                .Select(g => (z: g.Key, hw: g.Max(v => Mathf.Abs(v.x)))).OrderBy(s => s.z).ToList();
            float HW(float z)
            {
                for (int i = 0; i < stations.Count - 1; i++)
                    if (z <= stations[i + 1].z) return Mathf.Lerp(stations[i].hw, stations[i + 1].hw, (z - stations[i].z) / (stations[i + 1].z - stations[i].z));
                return stations[stations.Count - 1].hw;
            }
            float z0 = stations[0].z, z1 = stations[stations.Count - 1].z;
            var porch = all.Where(p => p.assemblyRole == "PORCH_FOOT" || p.assemblyRole == "THRESHOLD").Select(WorldBounds).ToList();
            float porchStart = porch.Count > 0 ? porch.Min(b => b.min.z) - 0.1f : z1;
            int stacked = 0, missing = 0, obstructions = 0, n = 0;
            var obstructionIds = new HashSet<string>();
            for (float z = z0 + 0.25f; z < z1 - 0.05f; z += 0.25f)
            {
                float hw = HW(z);
                foreach (float f in new[] { -0.9f, -0.5f, 0f, 0.5f, 0.9f })
                {
                    var o = new Vector3(f * hw, 3f, z);
                    var hits = Physics.RaycastAll(o, Vector3.down, 6f, PlayMask, QueryTriggerInteraction.Ignore)
                        .Where(h => Owner(h.collider)?.assemblyRole != "INSPECTION_ONLY").OrderBy(h => h.distance).ToList();
                    n++;
                    if (hits.Count == 0) { missing++; continue; }
                    var topOwner = Owner(hits[0].collider);
                    if (z < porchStart && topOwner != road) missing++;
                    if (hits.Count(h => Mathf.Abs(h.point.y - hits[0].point.y) <= 0.05f) > 1) stacked++;
                }
                if (z < porchStart)
                {
                    var centre = new Vector3(0, 1.15f, z);
                    foreach (var c in Physics.OverlapBox(centre, new Vector3(Mathf.Max(0.05f, hw - 0.12f), 0.95f, 0.12f), Quaternion.identity,
                                 PlayMask | VisMask, QueryTriggerInteraction.Ignore))
                    {
                        var o = Owner(c);
                        if (o == null || o == road || o.assemblyRole == "EDGE" || o.assemblyRole == "SCALE_REFERENCE" || o.assemblyRole == "INSPECTION_ONLY") continue;
                        obstructions++; obstructionIds.Add(o.logicalId);
                    }
                }
            }
            Add("street", "route_support_is_street", road.logicalId, missing == 0, $"{missing}/{n} samples lack street support before threshold", "0");
            Add("street", "no_stacked_traversable_surfaces", road.logicalId, stacked == 0, $"{stacked}/{n}", "0 samples with two colliders within 0.05 m");
            AcceptedRouteWidths(road);
            Add("street", "route_clear_width_unobstructed", road.logicalId, obstructions == 0,
                obstructions == 0 ? "clear" : string.Join(",", obstructionIds.Take(8)), "street width minus 0.12 m each side clear from 0.20 to 2.10 m");
            // Kerb dimensions at every segment midpoint.
            foreach (var kerb in all.Where(p => p.assemblyRole == "EDGE" && p.GetComponent<MeshFilter>() != null && !IsBuiltinCube(p.GetComponent<MeshFilter>().sharedMesh)))
            {
                int side = WorldBounds(kerb).center.x < 0 ? -1 : 1;
                for (int i = 0; i < stations.Count - 1; i++)
                {
                    float z = (stations[i].z + stations[i + 1].z) * 0.5f, hw = HW(z);
                    bool edgeAtStation = Hit(new Vector3(side * (hw + 0.1f), 3f, z), Vector3.down, 6f, VisMask, o => o == kerb, out var top);
                    if (!edgeAtStation) continue;
                    bool rd = Hit(new Vector3(side * (hw - 0.02f), 3f, z), Vector3.down, 6f, VisMask, o => o == road, out var edge);
                    if (rd) Band("dimension", "kerb_rise_above_road_edge", $"{kerb.logicalId} z={F(z)}", top.point.y - edge.point.y, 0.08f, 0.16f);
                    float w = 0;
                    for (float x = 0; x < 0.6f; x += 0.005f)
                        if (Hit(new Vector3(side * (hw + x), 3f, z), Vector3.down, 6f, VisMask, o => o == kerb, out var k) && k.point.y > top.point.y - 0.01f) w += 0.005f;
                    Band("dimension", "kerb_visible_width", $"{kerb.logicalId} z={F(z)}", w, 0.12f, 0.30f);
                }
            }
            // Retaining/garden walls: visible height above adjacent ground.
            foreach (var wall in all.Where(p => p.assemblyRole == "RETAINING" && p.logicalId.Contains("garden") || p.logicalId.Contains("w12_casco.right.retaining")))
            {
                var b = WorldBounds(wall);
                var cap = all.Where(p => p.assemblyRole == "CAP" && p.transform.parent == wall.transform.parent).Select(WorldBounds)
                    .Where(cb => cb.Intersects(new Bounds(b.center, b.size + Vector3.one * 0.05f))).ToList();
                float topY = cap.Count > 0 ? cap.Max(cb => cb.max.y) : b.max.y;
                float ground = Hit(new Vector3(b.max.x + 0.3f, 3f, b.center.z), Vector3.down, 6f, VisMask, o => o != null && o.assemblyRole == "SCENIC", out var g)
                    ? g.point.y : b.min.y;
                Band("dimension", "low_retaining_wall_visible_height", wall.logicalId, topY - ground, 0.45f, 1.20f);
            }
        }

        /// <summary>
        /// Measured traversable width vs the predecessor-accepted CITY width class. The expected width is
        /// read from the accepted CITY-04 layout, never from the road mesh or the builder, so an accidental
        /// narrowing that stays "unobstructed" still fails.
        /// </summary>
        static void AcceptedRouteWidths(Art01Piece road)
        {
            var bench = kit.Values.FirstOrDefault(k => k.role == "BENCHMARK");
            if (bench == null || bench.routeSegments == null || bench.routeSegments.Length == 0 || string.IsNullOrEmpty(bench.routeWidthSource))
            {
                Add("street", "accepted_route_width_class", "benchmark", false, "no routeSegments/routeWidthSource", "declared benchmark route segments");
                return;
            }
            var sourcePath = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../", bench.routeWidthSource));
            var accepted = new Dictionary<string, float>();
            if (File.Exists(sourcePath))
                foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(
                             File.ReadAllText(sourcePath, Encoding.UTF8), "\\{\\s*\"id\"\\s*:\\s*\"([^\"]+)\"\\s*,\\s*\"width\"\\s*:\\s*([0-9.]+)"))
                    accepted[m.Groups[1].Value] = float.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
            foreach (var seg in bench.routeSegments)
            {
                if (!accepted.TryGetValue(seg.route, out var expected))
                {
                    Add("street", "accepted_route_width_class", seg.route, false, "route absent from " + bench.routeWidthSource, "accepted CITY route id");
                    continue;
                }
                float min = float.MaxValue, max = float.MinValue; int samples = 0;
                for (float z = seg.z0; z <= seg.z1 + 1e-4f; z += 0.5f)
                {
                    float left = float.NaN, right = float.NaN;
                    for (float x = -5f; x <= 5f + 1e-4f; x += 0.005f)
                        if (Hit(new Vector3(x, 3f, z), Vector3.down, 6f, PlayMask, o => o == road, out _))
                        {
                            if (float.IsNaN(left)) left = x;
                            right = x;
                        }
                    if (float.IsNaN(left)) continue;
                    float w = right - left + 0.005f;
                    min = Mathf.Min(min, w); max = Mathf.Max(max, w); samples++;
                }
                bool ok = samples > 0 && min >= expected - 0.01f && max <= expected + 0.02f;
                Add("street", "accepted_route_width_class", $"{seg.route} z={F(seg.z0)}..{F(seg.z1)}", ok,
                    samples == 0 ? "no road samples" : $"{F(min)}..{F(max)} m over {samples} stations",
                    $"{F(expected)} m from {bench.routeWidthSource} (-0.01/+0.02)");
            }
        }

        // ------------------------------------------------------------------ surfaces

        static void Surfaces()
        {
            int zfight = 0, samples = 0;
            var examples = new List<string>();
            for (float x = -13.875f; x <= 14f; x += 0.25f)
                for (float z = -36.875f; z <= 40f; z += 0.25f)
                {
                    samples++;
                    var hits = Physics.RaycastAll(new Vector3(x, 25f, z), Vector3.down, 40f, VisMask, QueryTriggerInteraction.Ignore)
                        .OrderBy(h => h.distance).ToList();
                    if (hits.Count < 2) continue;
                    var a = hits[0]; var b = hits[1];
                    var oa = Owner(a.collider); var ob = Owner(b.collider);
                    if (oa == null || ob == null || oa == ob) continue;
                    if (Mathf.Abs(a.point.y - b.point.y) < 0.004f && Mathf.Abs(a.normal.y) > 0.9f && Mathf.Abs(b.normal.y) > 0.9f)
                    {
                        zfight++;
                        if (examples.Count < 12) examples.Add($"({F(x)},{F(z)}) {oa.logicalId}|{ob.logicalId}");
                    }
                }
            Add("surface", "no_coplanar_overlapping_top_surfaces", "benchmark", zfight == 0, $"{zfight}/{samples} {string.Join("; ", examples)}",
                "no two different visible top surfaces within 0.004 m (z-fighting/duplicate layer)");
            // Claimed visual elements must actually be visible from above somewhere.
            foreach (var p in all.Where(p => p.assemblyRole == "SCENIC_WATER" || p.assemblyRole == "DRAIN"))
            {
                var b = WorldBounds(p);
                int seen = 0, total = 0;
                for (float x = b.min.x + 0.5f; x < b.max.x; x += 1f)
                    for (float z = b.min.z + 0.5f; z < b.max.z; z += 1f)
                    {
                        total++;
                        if (Hit(new Vector3(x, 25f, z), Vector3.down, 40f, VisMask, null, out var h) && Owner(h.collider) == p) seen++;
                    }
                Add("surface", "claimed_element_visible", p.logicalId, total > 0 && seen >= Mathf.Max(1, total / 10), $"{seen}/{total} top samples",
                    ">= 10% of samples see the element first (not buried/occluded dead geometry)");
            }
            var street = all.FirstOrDefault(p => p.assemblyRole == "STREET");
            if (street != null)
            {
                var sm = street.GetComponent<MeshFilter>().sharedMesh;
                bool channel = sm.subMeshCount >= 2 && sm.GetTriangles(1).Length > 0;
                float depth = 0;
                if (channel)
                {
                    var verts = sm.vertices;
                    var idx = sm.GetTriangles(1);
                    float minY = idx.Min(i => verts[i].y);
                    float lip = idx.Max(i => verts[i].y);
                    depth = lip - minY;
                }
                Add("surface", "drainage_channel_modelled_in_street", street.logicalId, channel && depth >= 0.02f,
                    channel ? $"channel submesh, depth {F(depth)}" : "no channel geometry", "visible modelled channel >= 0.02 m deep");
            }
        }

        static void Humans()
        {
            foreach (var p in all.Where(p => p.assemblyRole == "SCALE_REFERENCE"))
            {
                var smr = p.GetComponentInChildren<SkinnedMeshRenderer>(true);
                if (smr == null) { Add("dimension", "human_reference_height", p.logicalId, false, "no skinned mesh", "1.70-1.85"); continue; }
                float top = float.MinValue;
                foreach (var r in p.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    var baked = new Mesh();
                    r.BakeMesh(baked);
                    foreach (var v in baked.vertices) top = Mathf.Max(top, r.transform.TransformPoint(v).y);
                    UnityEngine.Object.DestroyImmediate(baked);
                }
                Band("dimension", "human_reference_height", p.logicalId, top - p.transform.position.y, 1.70f, 1.85f);
            }
        }

        // ------------------------------------------------------------------ digest

        static (string, string) Digest()
        {
            var rows = new List<string>();
            foreach (var p in all)
            {
                var t = p.transform;
                var q = t.rotation;
                if (q.w < 0) q = new Quaternion(-q.x, -q.y, -q.z, -q.w);
                var meshes = p.GetComponentsInChildren<MeshFilter>(true).Where(m => Owner(m) == p)
                    .Select(m => m.sharedMesh == null ? "null" : m.sharedMesh.name + "#" + m.sharedMesh.vertexCount).OrderBy(x => x, StringComparer.Ordinal);
                var text = p.GetComponent<TextMesh>();
                rows.Add(string.Join("|", p.logicalId, p.kitId ?? "", p.assemblyRole, p.presentationState, p.collisionRole,
                    V(t.position), ("(" + string.Join(",", new[] { q.x, q.y, q.z, q.w }.Select(c => c.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture))) + ")").Replace("-0.0000", "0.0000"),
                    V(t.lossyScale), string.Join("+", meshes), text != null ? "text:" + text.text : ""));
            }
            rows.Sort(StringComparer.Ordinal);
            var joined = string.Join("\n", rows);
            var sha = Sha(Encoding.UTF8.GetBytes(joined));
            var sb = new StringBuilder();
            sb.Append("{\n  \"schema\": \"juego2.art01.structural-scene-digest@1\",\n");
            sb.Append($"  \"unityVersion\": \"{Application.unityVersion}\",\n  \"scene\": \"{ScenePath}\",\n");
            sb.Append("  \"note\": \"Canonical renderer-independent record of every tagged piece: id|kit|role|state|collision|world position|rotation|scale|meshes|text. Scene YAML fileIDs are not deterministic; this digest is.\",\n");
            sb.Append($"  \"rowCount\": {rows.Count},\n  \"sha256\": \"{sha}\",\n  \"rows\": [\n");
            for (int i = 0; i < rows.Count; i++)
                sb.Append("    \"" + rows[i].Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"" + (i < rows.Count - 1 ? ",\n" : "\n"));
            sb.Append("  ]\n}\n");
            return (sha, sb.ToString());
        }
    }
}
