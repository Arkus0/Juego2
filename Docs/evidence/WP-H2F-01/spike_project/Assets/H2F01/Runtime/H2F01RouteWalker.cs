using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Juego2.H2F01
{
    /// <summary>
    /// S06 scripted play: steers a virtual gamepad along NavMesh corners (bridge -> W12 -> F01 -> interior)
    /// so the real Input System -> controller -> Cinemachine path is exercised, and measures camera clipping,
    /// occlusion, jerks, stalls and arrival. Works for any controller that reads a Gamepad.
    /// </summary>
    public sealed class H2F01RouteWalker : MonoBehaviour
    {
        public Transform player;
        public Camera view;
        public Vector3 start, goal;
        public string label = "minimal";
        public float seconds = 70f;

        public IEnumerator Run(H2F01PlayDriver driver)
        {
            var pad = InputSystem.AddDevice<Gamepad>("H2F01VirtualPad");
            var path = new NavMeshPath();
            NavMesh.SamplePosition(start, out var s, 2f, NavMesh.AllAreas);
            NavMesh.SamplePosition(goal, out var g, 2f, NavMesh.AllAreas);
            NavMesh.CalculatePath(s.position, g.position, NavMesh.AllAreas, path);
            var corners = path.corners.ToList();
            driver.Note($"{label} path={path.status} corners={corners.Count}");
            int next = 1, frames = Mathf.RoundToInt(seconds * 30), clipFrames = 0, occludedFrames = 0, jerks = 0, stalls = 0;
            float maxJerk = 0, bestProgress = float.MaxValue, lastImprove = 0, minCamClear = float.MaxValue;
            Quaternion lastRot = view.transform.rotation;
            int captured = 0;
            bool arrived = false;
            var mask = ~(1 << 2); // player is on Ignore Raycast
            for (int f = 0; f < frames; f++)
            {
                if (next < corners.Count && Flat(corners[next] - player.position).magnitude < 0.45f) next++;
                Vector2 stick = Vector2.zero;
                if (next < corners.Count)
                {
                    var dir = Flat(corners[next] - player.position).normalized;
                    var camYaw = Quaternion.Euler(0, -view.transform.eulerAngles.y, 0);
                    var local = camYaw * dir;
                    stick = new Vector2(local.x, local.z);
                }
                else arrived = true;
                InputSystem.QueueStateEvent(pad, new GamepadState { leftStick = stick });
                yield return null;
                // camera measurements after the frame
                var camPos = view.transform.position;
                var head = player.position + Vector3.up * 1.6f;
                if (Physics.CheckSphere(camPos, 0.08f, mask, QueryTriggerInteraction.Ignore)) clipFrames++;
                var hits = Physics.OverlapSphere(camPos, 0.6f, mask, QueryTriggerInteraction.Ignore);
                foreach (var h in hits) minCamClear = Mathf.Min(minCamClear, Vector3.Distance(h.ClosestPoint(camPos), camPos));
                if (Physics.Linecast(camPos, head, out var hit, mask, QueryTriggerInteraction.Ignore)) occludedFrames++;
                float jerk = Quaternion.Angle(lastRot, view.transform.rotation);
                maxJerk = Mathf.Max(maxJerk, jerk);
                if (jerk > 12f) jerks++;
                lastRot = view.transform.rotation;
                float remaining = Flat(corners[corners.Count - 1] - player.position).magnitude;
                if (remaining < bestProgress - 0.05f) { bestProgress = remaining; lastImprove = f; }
                else if (f - lastImprove > 90 && !arrived) { stalls++; lastImprove = f; }
                if (f % 45 == 0 && captured < 40) { driver.Capture(view, $"{label}_frame_{captured:D2}", 640, 360); captured++; }
                if (arrived && f > 0) { driver.Note($"{label} arrived frame={f} t={f / 30f:F1}s"); break; }
            }
            driver.Note($"{label} arrived={arrived} final_distance={bestProgress:F2} camera_clip_frames={clipFrames} occluded_frames={occludedFrames} jerk_frames(>12deg)={jerks} max_jerk_deg={maxJerk:F1} stalls={stalls} min_camera_clearance={(minCamClear == float.MaxValue ? -1 : minCamClear):F3} frames_captured={captured}");
            InputSystem.RemoveDevice(pad);
        }

        static Vector3 Flat(Vector3 v) => new Vector3(v.x, 0, v.z);
    }
}
