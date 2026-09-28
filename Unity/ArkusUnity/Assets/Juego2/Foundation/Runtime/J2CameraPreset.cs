using UnityEngine;

namespace Juego2.Foundation
{
    /// <summary>Juego2 player camera preset realized by a GC2 MainCamera + Third Person ShotCamera (S06 amendment).</summary>
    [CreateAssetMenu(menuName = "Juego2/Foundation/Camera Preset", fileName = "J2_CameraPreset")]
    public sealed class J2CameraPreset : ScriptableObject
    {
        public float radius = 3f;
        public bool autoAlign = true;
        public float alignDelay = 0.5f;
        public float alignSmoothTime = 1f;
        public float nearClip = 0.05f;
        public float fieldOfView = 55f;
        public UnityEngine.Object inputActions;
        public string actionMap = "Player";
        public string lookAction = "Look";
        public string zoomAction = "Zoom";
    }
}
