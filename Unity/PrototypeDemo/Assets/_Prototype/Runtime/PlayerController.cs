using UnityEngine;
using UnityEngine.InputSystem;

namespace Proto.Runtime
{
    /// Third-person walker: WASD/arrows relative to the camera, Shift to run, R to go back to the start.
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public float walkSpeed = 1.4f, runSpeed = 3.5f, turnSpeed = 10f, gravity = -20f;
        public Transform cam;
        public Animator anim;
        CharacterController cc;
        Vector3 start; Quaternion startRot;
        float speed, vy;

        public static PlayerController Instance { get; private set; }
        /// Walk-test autopilot: when set, the player walks towards this point instead of reading the keyboard.
        public Vector3? autopilot;
        public float Speed => speed;

        void Awake() => Instance = this;

        void Start()
        {
            cc = GetComponent<CharacterController>();
            if (anim == null) anim = GetComponentInChildren<Animator>();
            if (cam == null && Camera.main != null) cam = Camera.main.transform;
            start = transform.position; startRot = transform.rotation;
        }

        void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;
            float x = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1 : 0) - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1 : 0);
            float y = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1 : 0) - (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1 : 0);
            bool running = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
            if (DemoHUD.Talking) { x = 0; y = 0; }
            if (autopilot.HasValue)
            {
                var to = autopilot.Value - transform.position; to.y = 0;
                var f = cam != null ? Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized : Vector3.forward;
                var rgt = Vector3.Cross(Vector3.up, f);
                var dn = to.normalized;
                x = Vector3.Dot(dn, rgt); y = Vector3.Dot(dn, f);
                running = false;
            }

            Vector3 fwd = cam != null ? Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized : transform.forward;
            Vector3 right = Vector3.Cross(Vector3.up, fwd);
            Vector3 dir = fwd * y + right * x;
            if (dir.sqrMagnitude > 1) dir.Normalize();
            float target = dir.magnitude * (running ? runSpeed : walkSpeed);
            speed = Mathf.MoveTowards(speed, target, (target > speed ? 6f : 9f) * Time.deltaTime);
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
            Vector3 move = (dir.sqrMagnitude > 0.001f ? dir.normalized : transform.forward) * speed;
            if (cc.isGrounded && vy < 0) vy = -2f;
            vy += gravity * Time.deltaTime;
            cc.Move((move + Vector3.up * vy) * Time.deltaTime);
            if (anim != null)
            {
                anim.SetFloat("Speed", speed);
                anim.speed = speed > 2.2f ? Mathf.Clamp(speed / runSpeed, 0.8f, 1.2f) : 1f;
            }
            if (kb.rKey.wasPressedThisFrame) Respawn("De vuelta en la Orilla sur.");
            if (transform.position.y < -4f) Respawn("Cuidado con el río.");
        }

        public void Respawn(string msg)
        {
            cc.enabled = false;
            transform.SetPositionAndRotation(start, startRot);
            cc.enabled = true;
            vy = 0; speed = 0;
            DemoHUD.Say(msg, 2.5f);
            var orbit = cam != null ? cam.GetComponent<OrbitCamera>() : null;
            if (orbit != null) orbit.SnapBehind();
        }
    }
}
