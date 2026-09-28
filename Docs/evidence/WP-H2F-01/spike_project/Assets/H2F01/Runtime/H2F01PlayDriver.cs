using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;

namespace Juego2.H2F01
{
    /// <summary>
    /// Disposable play-mode measurement driver for the H2F-01 spikes (non-keeper). The Editor builder
    /// creates the scene, sets <see cref="scenario"/> and enters play mode; this driver measures, captures
    /// frames and exits the batch editor.
    /// </summary>
    public sealed class H2F01PlayDriver : MonoBehaviour
    {
        public string scenario;
        public string outDir;
        readonly List<string> log = new List<string>();

        IEnumerator Start()
        {
            Time.captureFramerate = 30;
            Directory.CreateDirectory(outDir);
            IEnumerator run = scenario switch
            {
                "s07_motion" => S07Motion(),
                "s06_route" => GetComponent<H2F01RouteWalker>().Run(this),
                _ => Fail("unknown scenario " + scenario),
            };
            yield return run;
            Finish(0);
        }

        IEnumerator Fail(string message) { Note("FAIL " + message); yield break; }

        public void Note(string line) { log.Add(line); Debug.Log("H2F01_PLAY " + line); }

        public void Capture(Camera camera, string name, int width = 960, int height = 540)
        {
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

        void Finish(int code)
        {
            File.WriteAllLines(Path.Combine(outDir, scenario + "_log.txt"), log);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(code);
#endif
        }

        // ------------------------------------------------------------------ S07 motion

        IEnumerator S07Motion()
        {
            var rm = GameObject.Find("rm_walker").GetComponentInChildren<Animator>();
            var inplace = GameObject.Find("inplace_walker");
            var ipAnimator = inplace.GetComponentInChildren<Animator>();
            var ik = GameObject.Find("ik_actor").GetComponentInChildren<Animator>();
            var ikConstraint = FindObjectsByType<TwoBoneIKConstraint>(FindObjectsSortMode.None).First();
            var ikTarget = GameObject.Find("ik_target").transform;
            var cam = GameObject.Find("s07_play_camera").GetComponent<Camera>();
            var rmStart = rm.transform.position;
            var rmForward = rm.transform.forward;
            float ipSpeed = 0;
            var ipBody = inplace.GetComponent<CharacterController>();
            var feetSpeeds = new List<float>();
            Vector3 lastL = Vector3.zero, lastR = Vector3.zero;
            ikConstraint.weight = 0;
            for (int f = 0; f < 150; f++)
            {
                if (f == 30)
                {
                    // measured root-motion speed after 1 s drives the in-place walker from now on
                    ipSpeed = Vector3.Distance(Flat(rm.transform.position), Flat(rmStart)) / 1f;
                    Note($"rm_speed_after_1s={ipSpeed:F3}");
                }
                if (f >= 30) ipBody.Move(inplace.transform.forward * ipSpeed / 30f + Vector3.down * 0.01f);
                var l = ipAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                var r = ipAnimator.GetBoneTransform(HumanBodyBones.RightFoot).position;
                if (f > 31)
                {
                    // stance foot = lower foot; horizontal world speed of the stance foot approximates foot slide
                    var stance = l.y < r.y ? Flat(l - lastL) : Flat(r - lastR);
                    feetSpeeds.Add(stance.magnitude * 30f);
                }
                lastL = l; lastR = r;
                if (f == 60) Capture(cam, "s07_motion_t2s");
                if (f == 70)
                {
                    // contact target from the *animated* shoulder (bind-pose shoulder differs under Idle_Loop)
                    var sh = ik.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
                    ikTarget.position = sh + ik.transform.forward * 0.30f + Vector3.down * 0.28f + ik.transform.right * 0.05f;
                    Note($"ik_target_from_animated_shoulder distance={Vector3.Distance(sh, ikTarget.position):F3}");
                }
                if (f == 75)
                {
                    float d0 = Vector3.Distance(ik.GetBoneTransform(HumanBodyBones.RightHand).position, ikTarget.position);
                    Note($"ik_hand_to_target_weight0={d0:F3}");
                    ikConstraint.weight = 1;
                }
                if (f == 90)
                {
                    float d1 = Vector3.Distance(ik.GetBoneTransform(HumanBodyBones.RightHand).position, ikTarget.position);
                    Note($"ik_hand_to_target_weight1={d1:F3}");
                    Capture(cam, "s07_motion_ik_contact");
                }
                yield return null;
            }
            var disp = rm.transform.position - rmStart;
            float secs = 150 / 30f;
            Note($"rm_displacement={disp} speed={Flat(disp).magnitude / secs:F3} heading_deg={Vector3.SignedAngle(rmForward, Flat(disp), Vector3.up):F1} vertical_drift={disp.y:F3}");
            feetSpeeds.Sort();
            Note($"inplace_stance_foot_speed_p50={feetSpeeds[feetSpeeds.Count / 2]:F3} p10={feetSpeeds[feetSpeeds.Count / 10]:F3} (m/s; 0 = no slide) walk_speed={ipSpeed:F3}");
            Capture(cam, "s07_motion_t5s");
        }

        static Vector3 Flat(Vector3 v) => new Vector3(v.x, 0, v.z);
    }
}
