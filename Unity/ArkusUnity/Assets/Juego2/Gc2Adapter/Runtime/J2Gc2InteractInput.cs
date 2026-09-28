using System;
using GameCreator.Runtime.Characters;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Juego2.Gc2Adapter
{
    /// <summary>
    /// Juego2 owns input (H2F-01A authority rule 8): the Juego2 <c>Interact</c> action asks the GC2 player's public
    /// <c>Interaction.Interact()</c> to use its current target. GC2 player units read only movement, so without this relay
    /// the J2 action map could not reach an interactable. It decides nothing: the target's Trigger reaches Arkus through
    /// the adapter seam (<see cref="InstructionArkusRequestTransition"/>). Found by WP-H2F-03's integrated fixture.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Character))]
    public sealed class J2Gc2InteractInput : MonoBehaviour
    {
        public const string DefaultAction = "Interact";

        public InputActionAsset actions;
        public string actionMap = "Player";
        public string interactAction = DefaultAction;

        InputAction action;
        Character character;

        /// <summary>Observation hook: raised after each press with whether GC2 had a target to interact with.</summary>
        public event Action<bool> Interacted;

        void OnEnable()
        {
            character = GetComponent<Character>();
            if (actions == null) return;
            action = actions.FindActionMap(actionMap, true).FindAction(interactAction, true);
            action.performed += OnPerformed;
        }

        void OnDisable()
        {
            if (action != null) action.performed -= OnPerformed;
            action = null;
        }

        void OnPerformed(InputAction.CallbackContext context)
        {
            bool interacted = character != null && character.Interaction != null && character.Interaction.Interact();
            Interacted?.Invoke(interacted);
        }
    }
}
