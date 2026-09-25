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
            // gameplay view first (third person, as the player sees it at the start)
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(Path.Combine(dir, "00_juego_inicio.png"));
            yield return new WaitForSeconds(0.5f);
            if (orbit) orbit.enabled = false;
            foreach (var s in shots)
            {
                if (Ford.Instance != null && Ford.Instance.high != s.highWater) { Ford.Instance.Toggle(); yield return new WaitForSeconds(3f); }
                cam.transform.position = s.pos;
                cam.transform.rotation = Quaternion.LookRotation(s.look - s.pos, Vector3.up);
                yield return new WaitForSeconds(1.2f);
                yield return new WaitForEndOfFrame();
                ScreenCapture.CaptureScreenshot(Path.Combine(dir, s.name + ".png"));
                yield return new WaitForSeconds(0.4f);
            }
            yield return new WaitForSeconds(1f);
            Application.Quit();
        }
    }
}
