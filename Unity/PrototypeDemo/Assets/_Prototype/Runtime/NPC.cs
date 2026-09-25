using UnityEngine;
using UnityEngine.InputSystem;

namespace Proto.Runtime
{
    /// Townsperson: walks a street path (ground-snapped) or holds a pose; says hello when the player presses E nearby.
    public class NPC : MonoBehaviour
    {
        public string role = "Vecina";
        public string hello = "¡Hola!";
        public Vector3[] path;                 // world points; empty = stationary
        public bool pingPong = true;
        public float speed = 1.2f, pause = 2f;
        public string idleState = "Move";      // Move | Talk | Sit | SitTalk | Counter | LookAround | FoldArms
        public LayerMask ground = ~0;
        Animator anim;
        int idx, dirSign = 1;
        float waitUntil, talkUntil, curSpeed;
        Quaternion idleRot;

        void Start()
        {
            anim = GetComponentInChildren<Animator>();
            idleRot = transform.rotation;
            if (anim != null)
            {
                anim.Play(idleState, 0, Random.value);
                anim.speed = Random.Range(0.92f, 1.08f);
            }
        }

        void Update()
        {
            var player = PlayerController.Instance;
            float dist = player != null ? Vector3.Distance(player.transform.position, transform.position) : 99f;
            bool near = dist < 2.4f;
            if (near && Time.time > talkUntil) DemoHUD.Prompt($"E — saludar ({role})");
            if (near && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                talkUntil = Time.time + 3.2f;
                DemoHUD.Line(role, hello, 3.2f);
                if (anim != null && idleState != "Sit" && idleState != "SitTalk" && idleState != "Counter") anim.CrossFade("Talk", 0.25f);
            }
            if (Time.time < talkUntil)
            {
                if (player != null && idleState != "Sit" && idleState != "SitTalk")
                {
                    var to = player.transform.position - transform.position; to.y = 0;
                    if (to.sqrMagnitude > 0.01f) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(to), 6f * Time.deltaTime);
                }
                curSpeed = 0; SetSpeed();
                return;
            }
            if (anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("Talk") && idleState != "Talk") anim.CrossFade(idleState, 0.3f);

            if (path == null || path.Length < 2)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, idleRot, 2f * Time.deltaTime);
                return;
            }
            if (Time.time < waitUntil) { curSpeed = 0; SetSpeed(); return; }
            var target = path[idx];
            var delta = target - transform.position; delta.y = 0;
            // courtesy: wait if the player stands right in front
            if (player != null)
            {
                var toP = player.transform.position - transform.position; toP.y = 0;
                if (toP.magnitude < 1.1f && Vector3.Dot(toP.normalized, transform.forward) > 0.6f) { curSpeed = Mathf.MoveTowards(curSpeed, 0, 4 * Time.deltaTime); SetSpeed(); return; }
            }
            if (delta.magnitude < 0.3f)
            {
                idx += dirSign;
                if (idx >= path.Length || idx < 0)
                {
                    if (pingPong) { dirSign = -dirSign; idx += 2 * dirSign; }
                    else idx = 0;
                    waitUntil = Time.time + pause;
                }
                return;
            }
            curSpeed = Mathf.MoveTowards(curSpeed, speed, 2f * Time.deltaTime);
            var step = delta.normalized * curSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(delta), 5f * Time.deltaTime);
            var pos = transform.position + step;
            if (Physics.Raycast(pos + Vector3.up * 1.6f, Vector3.down, out var hit, 4f, ground, QueryTriggerInteraction.Ignore)) pos.y = hit.point.y;
            transform.position = pos;
            SetSpeed();
        }

        void SetSpeed() { if (anim != null) anim.SetFloat("Speed", curSpeed); }
    }
}
