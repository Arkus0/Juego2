using UnityEngine;

namespace Juego2.Foundation
{
    /// <summary>
    /// Juego2 character presentation preset. Data only: the GC2 materializer (Juego2.Gc2Adapter.Editor) builds the
    /// GC2 Character from it (S06 amendment, PUBLIC_AUTHORING_SURFACE.md). The model and controller are content
    /// inputs supplied by ART/fixture owners; the preset never becomes canonical identity.
    /// </summary>
    [CreateAssetMenu(menuName = "Juego2/Foundation/Character Preset", fileName = "J2_CharacterPreset")]
    public sealed class J2CharacterPreset : ScriptableObject
    {
        public bool isPlayer = true;
        public float height = 1.8f;
        public float radius = 0.28f;
        public float linearSpeed = 1.45f;
        [Tooltip("Juego2 action map path read by the GC2 player unit (asset/map/action). Ignored for NPCs, whose GC2 input is None.")]
        public UnityEngine.Object inputActions;
        public string moveActionMap = "Player";
        public string moveAction = "Move";
        public bool navMeshDriver;
        public FootstepDetector footstepDetector = FootstepDetector.Fulcrum;
        public UnityEngine.Object footstepSounds;
        [Tooltip("Content input: the ART Humanoid model prefab (explicit avatar mapping).")]
        public GameObject model;
        [Tooltip("Content input: the UAL locomotion controller (1D blend on 'Speed', layer 0 IK Pass on).")]
        public RuntimeAnimatorController locomotionController;

        public enum FootstepDetector
        {
            None,
            Fulcrum,
            AnimationCurves,
        }
    }
}
