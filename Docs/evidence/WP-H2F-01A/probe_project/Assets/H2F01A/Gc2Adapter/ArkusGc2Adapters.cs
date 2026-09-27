using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using Juego2.H2F01A.Arkus;
using UnityEngine;
using Event = GameCreator.Runtime.VisualScripting.Event;

namespace Juego2.H2F01A.Gc2Adapter
{
    // Juego2-owned GC2 adapters. Every type below derives from a public, documented GC2 Core extension base
    // (Condition, Instruction, Event, PropertyTypeGet*), so it appears in the GC2 pickers next to the stock items.
    // None of them stores canonical state: they read Arkus facts or submit transition requests to Arkus.

    [Title("Arkus Fact Equals")]
    [Category("Juego2/Arkus Fact Equals")]
    [Description("True when the Arkus-owned fact has the given value. Read-only; never writes GC2 or Arkus state.")]
    [Serializable]
    public class ConditionArkusFact : Condition
    {
        [SerializeField] private PropertyGetString m_Key = new PropertyGetString("");
        [SerializeField] private string m_Value = "";

        public ConditionArkusFact() { }

        public ConditionArkusFact(string key, string value)
        {
            m_Key = GetStringArkusLiteral.Create(key);
            m_Value = value;
        }

        protected override string Summary => $"Arkus {m_Key} is '{m_Value}'";

        protected override bool Run(Args args) => ArkusProbeAuthority.Has(m_Key.Get(args), m_Value);
    }

    [Title("Request Arkus Transition")]
    [Category("Juego2/Request Arkus Transition")]
    [Description("Submits a bounded semantic transition to Arkus for the entity bound to 'Entity'. Arkus decides; "
                 + "when it rejects, the remaining instructions of this list are not executed.")]
    [Serializable]
    public class InstructionArkusRequestTransition : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Entity = GetGameObjectSelf.Create();
        [SerializeField] private PropertyGetGameObject m_Requester = GetGameObjectTarget.Create();
        [SerializeField] private string m_Transition = "door.open";
        [SerializeField] private bool m_StopIfRejected = true;

        /// <summary>Observation hook for probes: (requester, entity, transition, accepted, reason).</summary>
        public static event Action<string, string, string, bool, string> EventResult;

        public InstructionArkusRequestTransition() { }

        public InstructionArkusRequestTransition(PropertyGetGameObject entity, PropertyGetGameObject requester,
            string transition, bool stopIfRejected = true)
        {
            m_Entity = entity;
            m_Requester = requester;
            m_Transition = transition;
            m_StopIfRejected = stopIfRejected;
        }

        public override string Title => $"Arkus request '{m_Transition}' on {m_Entity}";

        protected override Task Run(Args args)
        {
            string entity = ArkusEntityBinding.KeyOf(m_Entity.Get(args)) ?? "<unbound>";
            string requester = ArkusEntityBinding.KeyOf(m_Requester.Get(args)) ?? "<unbound>";
            TransitionResult result = ArkusProbeAuthority.Request(requester, entity, m_Transition);
            EventResult?.Invoke(requester, entity, m_Transition, result.Accepted, result.Reason);
            if (!result.Accepted && m_StopIfRejected) NextInstruction = int.MaxValue;
            return DefaultResult;
        }
    }

    [Title("On Arkus Fact Changed")]
    [Category("Juego2/On Arkus Fact Changed")]
    [Description("Runs the Trigger after Arkus accepts a transition that changes the given fact.")]
    [Serializable]
    public class EventOnArkusFactChanged : Event
    {
        [SerializeField] private string m_FactKey = "";

        public EventOnArkusFactChanged() { }

        public EventOnArkusFactChanged(string factKey) => m_FactKey = factKey;

        protected override void OnEnable(Trigger trigger)
        {
            base.OnEnable(trigger);
            ArkusProbeAuthority.FactChanged -= OnFact;
            ArkusProbeAuthority.FactChanged += OnFact;
        }

        protected override void OnDisable(Trigger trigger)
        {
            base.OnDisable(trigger);
            ArkusProbeAuthority.FactChanged -= OnFact;
        }

        void OnFact(string key, string value)
        {
            if (!IsActive || key != m_FactKey) return;
            _ = m_Trigger.Execute(Self);
        }
    }

    [Title("Arkus Fact")]
    [Category("Juego2/Arkus Fact")]
    [Description("The current value of an Arkus-owned fact, e.g. 'j2.door.bar_side:state'.")]
    [Serializable]
    public class GetStringArkusFact : PropertyTypeGetString
    {
        [SerializeField] private string m_Key = "";

        public GetStringArkusFact() { }
        public GetStringArkusFact(string key) => m_Key = key;

        public override string Get(Args args) => ArkusProbeAuthority.Get(m_Key) ?? "";
        public override string Get(GameObject gameObject) => ArkusProbeAuthority.Get(m_Key) ?? "";
        public override string String => $"Arkus[{m_Key}]";

        public static PropertyGetString Create(string key) => new PropertyGetString(new GetStringArkusFact(key));
    }

    /// <summary>A Juego2 fact key as a literal (keys are Juego2 strings, never GC2 variable names).</summary>
    [Title("Arkus Key")]
    [Category("Juego2/Arkus Key")]
    [Serializable]
    public class GetStringArkusLiteral : PropertyTypeGetString
    {
        [SerializeField] private string m_Key = "";

        public GetStringArkusLiteral() { }
        public GetStringArkusLiteral(string key) => m_Key = key;

        public override string Get(Args args) => m_Key;
        public override string Get(GameObject gameObject) => m_Key;
        public override string String => m_Key;

        public static PropertyGetString Create(string key) => new PropertyGetString(new GetStringArkusLiteral(key));
    }

    [Title("Arkus Entity")]
    [Category("Juego2/Arkus Entity")]
    [Description("The scene object currently bound to a Juego2 entity key.")]
    [Serializable]
    public class GetGameObjectArkusEntity : PropertyTypeGetGameObject
    {
        [SerializeField] private string m_EntityKey = "";

        public GetGameObjectArkusEntity() { }
        public GetGameObjectArkusEntity(string entityKey) => m_EntityKey = entityKey;

        public override GameObject Get(Args args) => ArkusEntityBinding.Resolve(m_EntityKey);
        public override GameObject Get(GameObject gameObject) => ArkusEntityBinding.Resolve(m_EntityKey);
        public override string String => $"Arkus entity {m_EntityKey}";

        public static PropertyGetGameObject Create(string entityKey) =>
            new PropertyGetGameObject(new GetGameObjectArkusEntity(entityKey));
    }
}
