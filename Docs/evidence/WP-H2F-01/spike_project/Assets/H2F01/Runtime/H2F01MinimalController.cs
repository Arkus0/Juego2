using UnityEngine;
using UnityEngine.InputSystem;

namespace Juego2.H2F01
{
    /// <summary>
    /// S06 "minimal project-owned composition": CharacterController + Input System action + camera-relative
    /// move + Animator speed. Presentation/locomotion only; no world authority. Non-keeper spike code.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class H2F01MinimalController : MonoBehaviour
    {
        public Transform cameraRoot;
        public Transform viewCamera;
        public float walkSpeed = 1.45f;
        public float turnDegPerSec = 540f;
        public float cameraYawFollow = 4f;
        InputAction move;
        CharacterController body;
        Animator animator;
        float fall, camYaw;
        public Vector3 lastVelocity;

        void Awake()
        {
            body = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            move = new InputAction("Move", InputActionType.Value);
            move.AddBinding("<Gamepad>/leftStick");
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            move.Enable();
            camYaw = transform.eulerAngles.y;
        }

        void OnDestroy() => move?.Dispose();

        void Update()
        {
            var input = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f);
            var yaw = Quaternion.Euler(0, viewCamera != null ? viewCamera.eulerAngles.y : camYaw, 0);
            var wish = yaw * new Vector3(input.x, 0, input.y);
            if (wish.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(wish), turnDegPerSec * Time.deltaTime);
            fall = body.isGrounded ? -1f : fall - 9.81f * Time.deltaTime;
            var velocity = wish * walkSpeed + Vector3.up * fall;
            body.Move(velocity * Time.deltaTime);
            lastVelocity = body.velocity;
            if (animator != null) animator.SetFloat("Speed", new Vector3(lastVelocity.x, 0, lastVelocity.z).magnitude);
            // camera root: follow body, yaw eases behind the heading (bounded smoothing, no free-look in the spike)
            camYaw = Mathf.LerpAngle(camYaw, transform.eulerAngles.y, 1f - Mathf.Exp(-cameraYawFollow * Time.deltaTime));
            if (cameraRoot != null)
            {
                cameraRoot.position = transform.position + Vector3.up * 1.5f;
                cameraRoot.rotation = Quaternion.Euler(10f, camYaw, 0);
            }
        }
    }
}
