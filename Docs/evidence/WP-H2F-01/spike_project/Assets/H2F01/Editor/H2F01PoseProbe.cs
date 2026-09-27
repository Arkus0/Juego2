using System.Linq;
using Juego2.ART;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>Diagnostic: which edit-mode humanoid posing path works on the ART clothed citizen.</summary>
    public static class H2F01PoseProbe
    {
        public static void Run()
        {
            OpenArtScene();
            var inspector = Object.FindFirstObjectByType<Art01WalkInspector>();
            var animator = inspector.GetComponentInChildren<Animator>(true);
            var clip = H2F01RenderSpike.UalClip("Idle_Loop");
            Log($"H2F01_POSE_PROBE animator={animator.name} avatar={(animator.avatar ? animator.avatar.name : "null")} human={(animator.avatar && animator.avatar.isHuman)} valid={(animator.avatar && animator.avatar.isValid)} clip={clip.name} humanMotion={clip.humanMotion} length={clip.length} controller={(animator.runtimeAnimatorController ? animator.runtimeAnimatorController.name : "null")}");
            var hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            Vector3 Hand() => hand != null ? hand.position : Vector3.zero;
            var bind = Hand();

            AnimationMode.StartAnimationMode();
            AnimationMode.SampleAnimationClip(animator.gameObject, clip, 0.25f);
            var modeA = Hand();
            AnimationMode.StopAnimationMode();

            AnimationMode.StartAnimationMode();
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(animator.gameObject, clip, 0.25f);
            AnimationMode.EndSampling();
            var modeB = Hand();
            AnimationMode.StopAnimationMode();

            var graph = PlayableGraph.Create("probe");
            graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            var output = AnimationPlayableOutput.Create(graph, "pose", animator);
            var playable = AnimationClipPlayable.Create(graph, clip);
            playable.SetTime(0.25);
            output.SetSourcePlayable(playable);
            graph.Evaluate(0);
            var modeC = Hand();
            graph.Destroy();

            Log($"H2F01_POSE_PROBE_HAND bind={bind} animationMode={modeA} beginSampling={modeB} playable={modeC}");
        }
    }
}
