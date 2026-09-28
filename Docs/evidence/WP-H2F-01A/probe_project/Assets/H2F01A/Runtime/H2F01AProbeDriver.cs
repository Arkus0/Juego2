using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Characters.IK;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using Juego2.H2F01A.Arkus;
using Juego2.H2F01A.Gc2Adapter;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Juego2.H2F01A
{
    /// <summary>
    /// Disposable play-mode measurement driver for the WP-H2F-01A probes (non-keeper). The Editor builder creates the
    /// scene, configures GC2 through its public/serialized authoring surface and enters play mode; this driver
    /// measures, captures and exits the editor. Juego2 semantics live in ArkusProbeAuthority, never in GC2.
    /// </summary>
    public sealed partial class H2F01AProbeDriver : MonoBehaviour
    {
        public string scenario;
        public string outDir;
        public InputActionAsset input;
        public Camera view;
        public Vector3 routeStart, routeGoal, npcGoal;
        public float routeSeconds = 70f;

        readonly List<string> log = new List<string>();
        readonly List<(string key, string value)> result = new List<(string, string)>();
        readonly List<string> consoleErrors = new List<string>();
        readonly List<string> consoleLines = new List<string>();

        void Awake()
        {
            Application.logMessageReceived += OnConsole;
        }

        void OnDestroy() => Application.logMessageReceived -= OnConsole;

        void OnConsole(string message, string stack, LogType type)
        {
            if (message.StartsWith("H2F01A_")) return;
            consoleLines.Add(message);
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                consoleErrors.Add(type + ": " + message.Split('\n')[0]);
        }

        static H2F01AProbeDriver active;

        IEnumerator Start()
        {
            // C06: GC2 load reloads the saved scene, which re-creates this scene object. The first driver persists and
            // owns the run; a re-created copy retires immediately.
            if (active != null && active != this) { Destroy(gameObject); yield break; }
            active = this;
            DontDestroyOnLoad(gameObject);
            Time.captureFramerate = 30;
            Directory.CreateDirectory(outDir);
            Note($"scenario={scenario} unity={Application.unityVersion} scene={SceneManager.GetActiveScene().name}");
            IEnumerator run = scenario switch
            {
                "c02" => C02(),
                "c02g" => C02(),
                "c03" => C03(),
                "c04" => C04(),
                "c05" => C05(),
                "c06" => C06(),
                "c07" => C07(),
                _ => Fail("unknown scenario " + scenario),
            };
            yield return run;
            Put("console_errors", consoleErrors.Count);
            foreach (var e in consoleErrors.Distinct().Take(12)) Note("console " + e);
            Finish();
        }

        IEnumerator Fail(string message) { Put("fatal", message); yield break; }

        // ------------------------------------------------------------------ shared helpers

        public void Note(string line) { log.Add(line); Debug.Log("H2F01A_PLAY " + line); }

        void Put(string key, object value)
        {
            string v = value switch
            {
                null => "null",
                bool b => b ? "true" : "false",
                float f => f.ToString("0.###", CultureInfo.InvariantCulture),
                double d => d.ToString("0.###", CultureInfo.InvariantCulture),
                int i => i.ToString(CultureInfo.InvariantCulture),
                _ => "\"" + value.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"",
            };
            result.RemoveAll(r => r.key == key);
            result.Add((key, v));
            Note($"{key}={v}");
        }

        void Finish()
        {
            File.WriteAllLines(Path.Combine(outDir, scenario + "_log.txt"), log);
            var json = new StringBuilder("{\n");
            json.Append($"  \"schema\": \"juego2.h2f01a.probe-result@1\",\n  \"scenario\": \"{scenario}\",\n");
            json.Append(string.Join(",\n", result.Select(r => $"  \"{r.key}\": {r.value}")));
            json.Append("\n}\n");
            File.WriteAllText(Path.Combine(outDir, scenario + "_result.json"), json.ToString(), new UTF8Encoding(false));
            Debug.Log("H2F01A_PLAY_DONE " + scenario);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(0);
#endif
        }

        public void Capture(Camera camera, string name, int width = 960, int height = 540)
        {
            if (camera == null) return;
            var rt = new RenderTexture(width, height, 24) { antiAliasing = 4 };
            var request = new RenderPipeline.StandardRequest { destination = rt };
            RenderPipeline.SubmitRenderRequest(camera, request);
            var old = RenderTexture.active; RenderTexture.active = rt;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0); tex.Apply();
            RenderTexture.active = old;
            File.WriteAllBytes(Path.Combine(outDir, name + ".png"), tex.EncodeToPNG());
            Destroy(tex); rt.Release(); Destroy(rt);
        }

        static Vector3 Flat(Vector3 v) => new Vector3(v.x, 0, v.z);

        static Character CharacterNamed(string name) => GameObject.Find(name)?.GetComponent<Character>();

        static IEnumerator Await(Task task, float timeout = 20f)
        {
            float t = 0;
            while (!task.IsCompleted && t < timeout) { t += Time.unscaledDeltaTime; yield return null; }
        }

        /// <summary>Hips height above the character's feet; the explicit Humanoid mapping puts it at ~0.9 m standing.</summary>
        static float HipsAboveFeet(Character c)
        {
            var a = c.Animim.Animator;
            var hips = a != null && a.isHuman ? a.GetBoneTransform(HumanBodyBones.Hips) : null;
            return hips == null ? -1 : hips.position.y - c.Feet.y;
        }

        /// <summary>Lowest foot bone above the physical ground under the body (H2F-01 S07 accepted -0.02..0.03 m).</summary>
        static float FeetToGround(Character c)
        {
            var a = c.Animim.Animator;
            if (a == null || !a.isHuman) return -9;
            float foot = Mathf.Min(a.GetBoneTransform(HumanBodyBones.LeftFoot).position.y, a.GetBoneTransform(HumanBodyBones.RightFoot).position.y);
            var from = c.transform.position + Vector3.up * 0.5f;
            return Physics.Raycast(from, Vector3.down, out var hit, 4f, ~(1 << 2), QueryTriggerInteraction.Ignore) ? foot - hit.point.y : -9;
        }

        /// <summary>
        /// Plugin identity audit for C02/C07: any serialized field on a GC2 component whose name denotes an identifier.
        /// Discovery-only reflection; the probe never writes such fields.
        /// </summary>
        static List<string> Gc2IdentityFields(GameObject root)
        {
            var found = new List<string>();
            foreach (var c in root.GetComponentsInChildren<Component>(true))
            {
                if (c == null || !(c.GetType().Namespace ?? "").StartsWith("GameCreator")) continue;
                foreach (var f in c.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var n = f.Name.ToLowerInvariant();
                    if ((n.Contains("uniqueid") || n.Contains("saveid") || n == "m_id" || n.EndsWith("guid"))
                        && (f.IsPublic || f.GetCustomAttribute<SerializeField>() != null || f.GetCustomAttribute<SerializeReference>() != null))
                        found.Add($"{c.gameObject.name}/{c.GetType().Name}.{f.Name}");
                }
            }
            return found;
        }

        /// <summary>Same measurements as H2F-01 S06 (H2F01RouteWalker) so the numbers are directly comparable.</summary>
        IEnumerator Route(Transform player, string label)
        {
            var pad = InputSystem.AddDevice<Gamepad>("H2F01AVirtualPad");
            var path = new NavMeshPath();
            NavMesh.SamplePosition(routeStart, out var s, 2f, NavMesh.AllAreas);
            NavMesh.SamplePosition(routeGoal, out var g, 2f, NavMesh.AllAreas);
            NavMesh.CalculatePath(s.position, g.position, NavMesh.AllAreas, path);
            var corners = path.corners.ToList();
            Note($"{label} path={path.status} corners={corners.Count}");
            int next = 1, frames = Mathf.RoundToInt(routeSeconds * 30), clipFrames = 0, occludedFrames = 0, jerks = 0, stalls = 0, captured = 0;
            float maxJerk = 0, bestProgress = float.MaxValue, lastImprove = 0, minCamClear = float.MaxValue, arrivedAt = -1;
            Quaternion lastRot = view.transform.rotation;
            bool arrived = false;
            var body = player.GetComponentsInChildren<Renderer>(true);
            int visSamples = 0, lostSamples = 0;
            float visMin = 1f, visSum = 0f, visLast = 0f;
            var mask = ~(1 << 2);
            for (int f = 0; f < frames && corners.Count > 1; f++)
            {
                if (next < corners.Count && Flat(corners[next] - player.position).magnitude < 0.45f) next++;
                Vector2 stick = Vector2.zero;
                if (next < corners.Count)
                {
                    var dir = Flat(corners[next] - player.position).normalized;
                    var local = Quaternion.Euler(0, -view.transform.eulerAngles.y, 0) * dir;
                    stick = new Vector2(local.x, local.z);
                }
                else arrived = true;
                InputSystem.QueueStateEvent(pad, new GamepadState { leftStick = stick });
                yield return null;
                var camPos = view.transform.position;
                var feet = player.position;
                var cc = player.GetComponent<CharacterController>();
                if (cc != null) feet = player.position + cc.center - Vector3.up * cc.height * 0.5f;
                var head = feet + Vector3.up * 1.6f;
                if (Physics.CheckSphere(camPos, 0.08f, mask, QueryTriggerInteraction.Ignore)) clipFrames++;
                foreach (var h in Physics.OverlapSphere(camPos, 0.6f, mask, QueryTriggerInteraction.Ignore))
                    minCamClear = Mathf.Min(minCamClear, Vector3.Distance(h.ClosestPoint(camPos), camPos));
                if (Physics.Linecast(camPos, head, out _, mask, QueryTriggerInteraction.Ignore)) occludedFrames++;
                float jerk = Quaternion.Angle(lastRot, view.transform.rotation);
                maxJerk = Mathf.Max(maxJerk, jerk);
                if (jerk > 12f) jerks++;
                lastRot = view.transform.rotation;
                float remaining = Flat(corners[corners.Count - 1] - player.position).magnitude;
                if (remaining < bestProgress - 0.05f) { bestProgress = remaining; lastImprove = f; }
                else if (f - lastImprove > 90 && !arrived) { stalls++; lastImprove = f; }
                if (f % 90 == 0 && captured < 20) { Capture(view, $"{label}_frame_{captured:D2}", 640, 360); captured++; }
                if (f >= 30 && f % 15 == 0)
                {
                    // rendered body visibility: share of screen pixels that change when the body is hidden
                    float vis = BodyVisibility(view, body);
                    visSamples++; visSum += vis; visMin = Mathf.Min(visMin, vis); visLast = vis;
                    if (vis < 0.002f) lostSamples++;
                }
                if (arrived) { arrivedAt = f / 30f; break; }
            }
            InputSystem.QueueStateEvent(pad, new GamepadState());
            yield return null;
            InputSystem.RemoveDevice(pad);
            Note($"{label} arrived={arrived} t={arrivedAt:F1}s final_distance={bestProgress:F2} camera_clip_frames={clipFrames} occluded_frames={occludedFrames} jerk_frames(>12deg)={jerks} max_jerk_deg={maxJerk:F1} stalls={stalls} min_camera_clearance={(minCamClear == float.MaxValue ? -1 : minCamClear):F3}");
            Put(label + "_arrived", arrived);
            Put(label + "_seconds", arrivedAt);
            Put(label + "_final_distance", bestProgress);
            Put(label + "_camera_clip_frames", clipFrames);
            Put(label + "_occluded_frames", occludedFrames);
            Put(label + "_jerk_frames", jerks);
            Put(label + "_max_jerk_deg", maxJerk);
            Put(label + "_stalls", stalls);
            Put(label + "_body_visibility_samples", visSamples);
            Put(label + "_body_lost_samples", lostSamples);
            Put(label + "_body_visible_share_mean", visSamples > 0 ? visSum / visSamples : 0f);
            Put(label + "_body_visible_share_min", visMin);
            Put(label + "_body_visible_share_at_arrival", visLast);
        }

        Texture2D visRead;

        float[] RenderLuma(Camera camera, int w, int h)
        {
            var rt = RenderTexture.GetTemporary(w, h, 24);
            RenderPipeline.SubmitRenderRequest(camera, new RenderPipeline.StandardRequest { destination = rt });
            var old = RenderTexture.active; RenderTexture.active = rt;
            if (visRead == null) visRead = new Texture2D(w, h, TextureFormat.RGB24, false);
            visRead.ReadPixels(new Rect(0, 0, w, h), 0, 0); visRead.Apply();
            RenderTexture.active = old; RenderTexture.ReleaseTemporary(rt);
            var px = visRead.GetPixels32();
            var luma = new float[px.Length];
            for (int i = 0; i < px.Length; i++) luma[i] = (px[i].r * 0.299f + px[i].g * 0.587f + px[i].b * 0.114f) / 255f;
            return luma;
        }

        float BodyVisibility(Camera camera, Renderer[] body)
        {
            const int w = 192, h = 108;
            var with = RenderLuma(camera, w, h);
            var was = body.Select(r => r.enabled).ToArray();
            foreach (var r in body) r.enabled = false;
            var without = RenderLuma(camera, w, h);
            for (int i = 0; i < body.Length; i++) body[i].enabled = was[i];
            int changed = 0;
            for (int i = 0; i < with.Length; i++) if (Mathf.Abs(with[i] - without[i]) > 0.04f) changed++;
            return changed / (float)with.Length;
        }

        /// <summary>Ownership audit shared by C02/C07: exactly one player body, one render camera and one input owner.</summary>
        void OwnershipAudit(string prefix)
        {
            var chars = FindObjectsByType<Character>(FindObjectsSortMode.None);
            Put(prefix + "gc2_characters", chars.Length);
            Put(prefix + "gc2_players", chars.Count(c => c.IsPlayer));
            Put(prefix + "main_camera_tagged", GameObject.FindGameObjectsWithTag("MainCamera").Length);
            Put(prefix + "enabled_cameras", FindObjectsByType<Camera>(FindObjectsSortMode.None).Count(c => c.enabled));
            Put(prefix + "gc2_camera_components", FindObjectsByType<GameCreator.Runtime.Cameras.TCamera>(FindObjectsSortMode.None).Length);
            Put(prefix + "gc2_shot_cameras", FindObjectsByType<GameCreator.Runtime.Cameras.ShotCamera>(FindObjectsSortMode.None).Length);
            Put(prefix + "cinemachine_brains", FindObjectsByType<Unity.Cinemachine.CinemachineBrain>(FindObjectsSortMode.None).Length);
            Put(prefix + "unity_player_input_components", FindObjectsByType<PlayerInput>(FindObjectsSortMode.None).Length);
            Put(prefix + "character_controllers", FindObjectsByType<CharacterController>(FindObjectsSortMode.None).Length);
            var enabledMaps = InputSystem.ListEnabledActions().Select(a => (a.actionMap?.asset != null ? a.actionMap.asset.name : "<loose>") + "/" + (a.actionMap?.name ?? "") + "/" + a.name).Distinct().OrderBy(x => x).ToArray();
            Put(prefix + "enabled_input_actions", string.Join(",", enabledMaps));
            var bindings = FindObjectsByType<ArkusEntityBinding>(FindObjectsSortMode.None);
            Put(prefix + "arkus_bindings", bindings.Length);
            Put(prefix + "arkus_binding_keys_unique", bindings.Select(b => b.entityKey).Distinct().Count() == bindings.Length);
            var ids = new List<string>();
            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects()) ids.AddRange(Gc2IdentityFields(root));
            Put(prefix + "gc2_identity_fields_in_scene", string.Join(",", ids));
        }
    }
}
