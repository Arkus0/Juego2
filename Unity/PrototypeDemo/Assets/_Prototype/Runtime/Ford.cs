using UnityEngine;

namespace Proto.Runtime
{
    /// X5 vado: low water (stones dry, crossing open) / high water (Arroyo raised, crossing blocked). Key F toggles.
    public class Ford : MonoBehaviour
    {
        public Transform flood;
        public BoxCollider blocker;
        public bool high;
        public const float HighLevel = 1.15f;
        float level;

        public static Ford Instance { get; private set; }
        void Awake() => Instance = this;

        public void Toggle()
        {
            high = !high;
            DemoHUD.Say(high ? "Aguas altas: el Arroyo cubre las pasaderas. El vado no se puede cruzar." : "Aguas bajas: las pasaderas asoman. Se puede cruzar el vado.", 4f);
        }

        void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame) Toggle();
            float target = high ? HighLevel : -0.05f;
            level = Mathf.MoveTowards(level, target, Time.deltaTime * 0.6f);
            if (flood != null)
            {
                flood.gameObject.SetActive(level > 0.0f);
                flood.localScale = new Vector3(1, Mathf.Max(0.001f, level), 1);
            }
            if (blocker != null) blocker.gameObject.SetActive(high);
        }
    }
}
