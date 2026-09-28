using UnityEngine;
using UnityEngine.InputSystem;

namespace Proto.Runtime
{
    /// Orbital third-person camera with sphere-cast collision (never ends up inside walls), wheel zoom.
    public class OrbitCamera : MonoBehaviour
    {
        public Transform target;
        public float distance = 3.8f, minDistance = 1.3f, maxDistance = 8f;
        public float pivotHeight = 1.6f, shoulder = 0.35f, sensitivity = 0.1f;
        public float pitch = 10f, yaw;
        public LayerMask collide = ~0;
        float current;

        void Start()
        {
            current = distance;
            SnapBehind();
            Lock(true);
        }

        public void SnapBehind() { if (target != null) yaw = target.eulerAngles.y; pitch = 10f; }

        static void Lock(bool on)
        {
            Cursor.lockState = on ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !on;
        }

        void LateUpdate()
        {
            if (target == null) return;
            var m = Mouse.current; var kb = Keyboard.current;
            if (kb != null && kb.escapeKey.wasPressedThisFrame) Lock(false);
            if (m != null && m.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked) Lock(true);
            if (m != null && Cursor.lockState == CursorLockMode.Locked)
            {
                var d = m.delta.ReadValue();
                yaw += d.x * sensitivity;
                pitch = Mathf.Clamp(pitch - d.y * sensitivity, -30f, 70f);
                float sc = m.scroll.ReadValue().y;
                if (Mathf.Abs(sc) > 0.01f) distance = Mathf.Clamp(distance - Mathf.Sign(sc) * 0.4f, minDistance, maxDistance);
            }
            var rot = Quaternion.Euler(pitch, yaw, 0);
            var pivot = target.position + Vector3.up * pivotHeight;
            var offset = rot * new Vector3(shoulder, 0, -distance);
            float len = offset.magnitude;
            var dir = offset / len;
            float allowed = len;
            if (Physics.SphereCast(pivot, 0.24f, dir, out var hit, len, collide, QueryTriggerInteraction.Ignore))
                allowed = Mathf.Max(0.35f, hit.distance - 0.05f);
            current = allowed < current ? allowed : Mathf.MoveTowards(current, allowed, Time.deltaTime * 3.5f);
            transform.position = pivot + dir * current;
            transform.rotation = Quaternion.LookRotation(pivot + rot * Vector3.right * shoulder * 0.5f - transform.position, Vector3.up);
        }
    }
}
