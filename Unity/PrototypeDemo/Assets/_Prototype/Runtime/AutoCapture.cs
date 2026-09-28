using System.Collections;
using System.IO;
using UnityEngine;

namespace Proto.Runtime
{
    /// In a player build: `PuenteBar.exe -autocapture <dir>` renders the fixed review shots with live NPCs, then quits.
    public class AutoCapture : MonoBehaviour
    {
        [System.Serializable] public struct Shot { public string name; public Vector3 pos; public Vector3 look; public bool highWater; }
        public Shot[] shots;
        [System.Serializable] public struct Waypoint { public string name; public Vector3 pos; }
        public Waypoint[] walk;

        /// Drives the player through the chain with the real CharacterController; logs arrival, time and stalls.
        IEnumerator Walk(string dir, OrbitCamera orbit)
        {
            var p = PlayerController.Instance;
            var log = new System.Text.StringBuilder();
            p.Respawn("Recorrido automático");
            if (orbit) { orbit.enabled = true; orbit.SnapBehind(); }
            yield return new WaitForSeconds(1f);
            float total = 0, dist = 0; Vector3 last = p.transform.position;
            int shot = 0;
            foreach (var w in walk)
            {
                p.autopilot = w.pos;
                float t = 0, stall = 0; Vector3 prev = p.transform.position;
                bool ok = false;
                while (t < 45f)
                {
                    var d = w.pos - p.transform.position; d.y = 0;
                    if (d.magnitude < 0.7f) { ok = true; break; }
                    if (orbit) orbit.yaw = Mathf.LerpAngle(orbit.yaw, Quaternion.LookRotation(d).eulerAngles.y, Time.deltaTime * 2f);
                    t += Time.deltaTime; total += Time.deltaTime;
                    dist += (p.transform.position - last).magnitude; last = p.transform.position;
                    stall = (p.transform.position - prev).magnitude < 0.004f ? stall + Time.deltaTime : 0;
                    prev = p.transform.position;
                    if (stall > 3f) break;
                    yield return null;
                }
                string why = "";
                if (!ok)
                {
                    var dd = w.pos - p.transform.position; dd.y = 0;
                    var o = p.transform.position + Vector3.up * 0.5f;
                    foreach (var h in Physics.CapsuleCastAll(o, o + Vector3.up * 0.9f, 0.3f, dd.normalized, 1.2f))
                        if (h.collider.gameObject != p.gameObject) why += $" [{h.collider.name} @{h.point.x:F1},{h.point.y:F1},{h.point.z:F1}]";
                    why += $" pos=({p.transform.position.x:F1},{p.transform.position.z:F1})";
                }
                log.AppendLine($"{w.name}\t{(ok ? "OK" : "ATASCO")}\t{t:F1}s\ty={p.transform.position.y:F2}{why}");
                if (w.name.StartsWith("*"))
                {
                    yield return new WaitForEndOfFrame();
                    ScreenCapture.CaptureScreenshot(Path.Combine(dir, $"20_recorrido_{shot++}_{w.name.Trim('*')}.png"));
                }
                if (!ok) break;
            }
            p.autopilot = null;
            log.AppendLine($"TOTAL\t{total:F1}s\t{dist:F0} m\t(andar 1,4 m/s)");
            File.WriteAllText(Path.Combine(dir, "recorrido.txt"), log.ToString());
        }

        /// Characters are checked, not eyeballed: toes (ball) must be ahead of the ankle along transform.forward.
        /// Off-screen animators do not pose their bones, so every humanoid is forced to animate while measuring.
        static IEnumerator FacingCheck(string dir)
        {
            var anims = new System.Collections.Generic.List<Animator>();
            foreach (var a in FindObjectsByType<Animator>(FindObjectsSortMode.None)) if (a.isHuman) anims.Add(a);
            var modes = anims.ConvertAll(a => a.cullingMode);
            foreach (var a in anims) a.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            yield return null; yield return null;
            int ok = 0; var bad = new System.Collections.Generic.List<string>();
            foreach (var a in anims)
            {
                var foot = a.GetBoneTransform(HumanBodyBones.LeftFoot); var toes = a.GetBoneTransform(HumanBodyBones.LeftToes);
                if (foot == null || toes == null) continue;
                var d = toes.position - foot.position; d.y = 0;
                if (Vector3.Dot(d.normalized, a.transform.forward) > 0.3f) ok++; else bad.Add(a.name);
            }
            for (int i = 0; i < anims.Count; i++) anims[i].cullingMode = modes[i];
            File.WriteAllText(Path.Combine(dir, "orientacion.txt"), $"humanoides mirando hacia delante: {ok}" + System.Environment.NewLine + $"al revés: {bad.Count} {string.Join(", ", bad)}" + System.Environment.NewLine);
        }

        IEnumerator Start()
        {
            var args = System.Environment.GetCommandLineArgs();
            int i = System.Array.IndexOf(args, "-autocapture");
            if (i < 0 || i + 1 >= args.Length) yield break;
            string dir = args[i + 1];
            Directory.CreateDirectory(dir);
            var cam = Camera.main;
            var orbit = cam.GetComponent<OrbitCamera>();
            yield return new WaitForSeconds(4f);
            yield return FacingCheck(dir);
            // gameplay view first (third person, as the player sees it at the start)
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(Path.Combine(dir, "00_juego_inicio.png"));
            yield return new WaitForSeconds(0.5f);
            // greet the nearest townsperson, as if the player pressed E
            NPC nearest = null; float best = 99;
            foreach (var n in FindObjectsByType<NPC>(FindObjectsSortMode.None))
            {
                float d = Vector3.Distance(n.transform.position, PlayerController.Instance.transform.position);
                if (d < best) { best = d; nearest = n; }
            }
            if (nearest != null)
            {
                nearest.Greet();
                yield return new WaitForSeconds(1.2f);
                yield return new WaitForEndOfFrame();
                ScreenCapture.CaptureScreenshot(Path.Combine(dir, "01_saludo_hola.png"));
            }
            var fps = new System.Text.StringBuilder();
            if (orbit) orbit.enabled = false;
            foreach (var s in shots)
            {
                if (Ford.Instance != null && Ford.Instance.high != s.highWater) { Ford.Instance.Toggle(); yield return new WaitForSeconds(3f); }
                cam.transform.position = s.pos;
                cam.transform.rotation = Quaternion.LookRotation(s.look - s.pos, Vector3.up);
                yield return new WaitForSeconds(0.4f);
                float t0 = Time.realtimeSinceStartup; int frames = 0;
                while (Time.realtimeSinceStartup - t0 < 1.5f) { frames++; yield return null; }
                fps.AppendLine($"{s.name}\t{frames / (Time.realtimeSinceStartup - t0):F1} fps");
                yield return new WaitForEndOfFrame();
                ScreenCapture.CaptureScreenshot(Path.Combine(dir, s.name + ".png"));
                yield return new WaitForSeconds(0.4f);
            }
            fps.AppendLine($"GPU: {SystemInfo.graphicsDeviceName} · {Screen.width}x{Screen.height}");
            File.WriteAllText(Path.Combine(dir, "fps.txt"), fps.ToString());
            if (Ford.Instance != null && Ford.Instance.high) { Ford.Instance.Toggle(); yield return new WaitForSeconds(2f); }
            if (walk != null && walk.Length > 0) yield return Walk(dir, orbit);
            yield return new WaitForSeconds(1f);
            Application.Quit();
        }
    }
}
