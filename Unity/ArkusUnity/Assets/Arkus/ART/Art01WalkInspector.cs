using UnityEngine;

namespace Juego2.ART
{
    /// <summary>
    /// Local ART benchmark inspection only. It moves a clothed scale instrument along
    /// the authored surface; no NPC, quest, world-authoring or keeper semantics.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class Art01WalkInspector : MonoBehaviour
    {
        public Camera view;
        public float speed = 2.6f;
        public float turnSpeed = 110f;
        CharacterController body;
        float falling;

        void Awake() => body = GetComponent<CharacterController>();

        void Start()
        {
            // The other posed people are still-photo rulers, not world actors.
            foreach (var tag in FindObjectsByType<Art01Piece>(FindObjectsSortMode.None))
                if (tag.assemblyRole == "SCALE_REFERENCE" && !tag.transform.IsChildOf(transform))
                    tag.gameObject.SetActive(false);
        }

        void Update()
        {
            float turn = Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("Horizontal") * 0.6f;
            transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
            var forward = Input.GetAxisRaw("Vertical");
            var side = Input.GetKey(KeyCode.Q) ? -1f : Input.GetKey(KeyCode.E) ? 1f : 0f;
            var planar = (transform.forward * forward + transform.right * side);
            if (planar.sqrMagnitude > 1f) planar.Normalize();
            falling = body.isGrounded ? -0.15f : falling - 9.81f * Time.deltaTime;
            body.Move((planar * speed + Vector3.up * falling) * Time.deltaTime);
        }

        void LateUpdate()
        {
            if (view == null) return;
            view.transform.position = transform.TransformPoint(new Vector3(0, 1.8f, -3.1f));
            view.transform.LookAt(transform.position + Vector3.up * 1.35f);
        }
    }
}
