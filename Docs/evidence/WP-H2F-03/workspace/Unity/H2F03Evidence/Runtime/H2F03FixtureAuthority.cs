using System;
using System.Collections.Generic;
using Juego2.Arkus;
using UnityEngine;

namespace Juego2.H2F03.Evidence
{
    /// <summary>
    /// WP-H2F-03 fixture-local Arkus authority on the accepted <see cref="IArkusFactAuthority"/> seam. It stands in for
    /// the production host that WP-GC2-00 binds; it is evidence code, never product. It owns one door fact and starts
    /// locked, so the first interaction is refused (the GC2 list must stop) and a later one is accepted.
    /// </summary>
    public sealed class H2F03FixtureAuthority : MonoBehaviour, IArkusFactAuthority
    {
        public string doorKey = "j2.door.fixture_house";
        public bool locked = true;

        readonly Dictionary<string, string> facts = new Dictionary<string, string>(StringComparer.Ordinal);
        public readonly List<string> Log = new List<string>();

        public event Action<string, string> FactChanged;

        public string DoorState => GetFact(doorKey + ":state");

        void OnEnable()
        {
            facts[doorKey + ":state"] = "closed";
            ArkusFacts.Bind(this);
        }

        void OnDisable()
        {
            if (ReferenceEquals(ArkusFacts.Current, this)) ArkusFacts.Bind(null);
        }

        public void Unlock() => locked = false;

        public string GetFact(string factKey) => factKey != null && facts.TryGetValue(factKey, out var v) ? v : null;

        public ArkusTransitionResult Request(ArkusTransitionRequest request)
        {
            ArkusTransitionResult result;
            var key = doorKey + ":state";
            if (request.Entity != doorKey || request.Transition != "door.open") result = ArkusTransitionResult.Refused("UNKNOWN_TRANSITION");
            else if (locked) result = ArkusTransitionResult.Refused("LOCKED");
            else if (facts[key] != "closed") result = ArkusTransitionResult.Refused("ALREADY_OPEN");
            else
            {
                facts[key] = "open";
                result = new ArkusTransitionResult(ArkusTransitionStatus.Accepted, "ACCEPTED", key, "open");
                FactChanged?.Invoke(key, "open");
            }
            Log.Add($"{request.Requester}|{request.Entity}|{request.Transition}|{result.Status}|{result.Reason}");
            return result;
        }
    }
}
