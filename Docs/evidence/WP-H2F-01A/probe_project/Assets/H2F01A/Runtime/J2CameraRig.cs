using UnityEngine;

namespace Juego2.H2F01A
{
    /// <summary>
    /// The camera-root half of the accepted H2F-01 S06 realization (same rule as H2F01MinimalController): follow the
    /// body at head height, ease yaw behind its heading. Cinemachine 3 ThirdPersonFollow tracks this transform.
    /// Holds no dependency on the body's controller, so it works over a GC2 Character or the minimal controller.
    /// </summary>
    [DefaultExecutionOrder(-100)] // before CinemachineBrain's LateUpdate
    public sealed class J2CameraRig : MonoBehaviour
    {
        public Transform body;
        public float headAboveBodyOrigin = 1.5f;
        public float yawFollow = 4f;
        float yaw;

        void Start() => yaw = body != null ? body.eulerAngles.y : 0f;

        void LateUpdate()
        {
            if (body == null) return;
            yaw = Mathf.LerpAngle(yaw, body.eulerAngles.y, 1f - Mathf.Exp(-yawFollow * Time.deltaTime));
            transform.position = body.position + Vector3.up * headAboveBodyOrigin;
            transform.rotation = Quaternion.Euler(10f, yaw, 0f);
        }
    }
}
