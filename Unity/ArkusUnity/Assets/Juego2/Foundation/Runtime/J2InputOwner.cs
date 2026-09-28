using UnityEngine;
using UnityEngine.InputSystem;

namespace Juego2.Foundation
{
    /// <summary>
    /// Juego2 owns input (H2F-01A authority rule 8): this component creates the enabled state of the Juego2 action
    /// asset. GC2 and other executors only read its actions; they never enable device actions of their own.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class J2InputOwner : MonoBehaviour
    {
        public InputActionAsset actions;
        public string actionMap = "Player";

        void OnEnable()
        {
            if (actions == null) return;
            var map = actions.FindActionMap(actionMap, true);
            map.Enable();
        }

        void OnDisable()
        {
            if (actions == null) return;
            actions.FindActionMap(actionMap, false)?.Disable();
        }
    }
}
