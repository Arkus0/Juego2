using System;

namespace Juego2.Arkus
{
    public enum ArkusTransitionStatus
    {
        Accepted,
        Refused,
        Conflict,
    }

    /// <summary>Juego2 intent submitted by local gameplay. Keys are Juego2 semantic keys, never engine/plugin IDs.</summary>
    public readonly struct ArkusTransitionRequest
    {
        public readonly string Requester, Entity, Transition;

        public ArkusTransitionRequest(string requester, string entity, string transition)
        {
            Requester = requester;
            Entity = entity;
            Transition = transition;
        }
    }

    /// <summary>Arkus decides. Presentation may show a durable consequence only after <see cref="ArkusTransitionStatus.Accepted"/>.</summary>
    public readonly struct ArkusTransitionResult
    {
        public readonly ArkusTransitionStatus Status;
        public readonly string Reason, FactKey, Value;

        public ArkusTransitionResult(ArkusTransitionStatus status, string reason, string factKey, string value)
        {
            Status = status;
            Reason = reason;
            FactKey = factKey;
            Value = value;
        }

        public bool Accepted => Status == ArkusTransitionStatus.Accepted;

        public static ArkusTransitionResult Refused(string reason) =>
            new ArkusTransitionResult(ArkusTransitionStatus.Refused, reason, null, null);
    }

    /// <summary>
    /// The runtime seam between local gameplay (GC2 Core via the Juego2 adapter) and Arkus canonical authority
    /// (GC2_ARKUS_RUNTIME_SPLIT.md). Implementations validate and commit through the accepted Arkus boundary; the
    /// production host is bound by WP-GC2-00. Nothing here references Game Creator or Unity plugin types.
    /// </summary>
    public interface IArkusFactAuthority
    {
        /// <summary>Current value of a declared fact (<c>entityKey:fact</c>), or null when undeclared.</summary>
        string GetFact(string factKey);

        ArkusTransitionResult Request(ArkusTransitionRequest request);

        /// <summary>Raised after an accepted transition changes a fact: (fact key, new value).</summary>
        event Action<string, string> FactChanged;
    }

    /// <summary>
    /// Process-wide binding point for the active authority. Fails closed: with no authority bound, facts read as
    /// undeclared and every request is refused with <c>NO_AUTHORITY</c>; no local fallback state exists.
    /// </summary>
    public static class ArkusFacts
    {
        public const string NoAuthority = "NO_AUTHORITY";
        public const string UnboundRequester = "UNBOUND_REQUESTER";
        public const string UnboundEntity = "UNBOUND_ENTITY";

        static IArkusFactAuthority current;

        public static IArkusFactAuthority Current => current;

        /// <summary>Raised after the bound authority accepts a transition: (fact key, new value).</summary>
        public static event Action<string, string> FactChanged;

        public static void Bind(IArkusFactAuthority authority)
        {
            if (current != null) current.FactChanged -= Relay;
            current = authority;
            if (current != null) current.FactChanged += Relay;
        }

        public static string Get(string factKey) => current?.GetFact(factKey);

        public static bool Has(string factKey, string value) => Get(factKey) is string v && v == (value ?? "");

        public static ArkusTransitionResult Request(string requester, string entity, string transition)
        {
            if (current == null) return ArkusTransitionResult.Refused(NoAuthority);
            if (!ArkusEntityBinding.IsValidKey(requester)) return ArkusTransitionResult.Refused(UnboundRequester);
            if (!ArkusEntityBinding.IsValidKey(entity)) return ArkusTransitionResult.Refused(UnboundEntity);
            return current.Request(new ArkusTransitionRequest(requester, entity, transition));
        }

        static void Relay(string key, string value) => FactChanged?.Invoke(key, value);
    }
}
