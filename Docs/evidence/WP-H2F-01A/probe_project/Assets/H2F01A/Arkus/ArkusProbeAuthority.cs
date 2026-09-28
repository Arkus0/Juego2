using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Juego2.H2F01A.Arkus
{
    /// <summary>
    /// Probe-scale stand-in for Arkus canonical authority (H2F-01A only; not the Arkus kernel). It owns the Juego2
    /// entity keys, the facts about them and the causal transition log. Its assembly references nothing, so Game
    /// Creator types cannot appear here; GC2 reaches it only through the Juego2-owned adapter assembly.
    /// </summary>
    public static class ArkusProbeAuthority
    {
        public const string PayloadSchema = "juego2.h2f01a.probe-payload@1";
        public const int PayloadVersion = 1;
        const string Requester = "{requester}";

        // Juego2-owned transition table: transition -> (fact, required current value, new value).
        static readonly Dictionary<string, (string fact, string from, string to)> Rules =
            new Dictionary<string, (string, string, string)>
            {
                ["door.open"] = ("state", "closed", "open"),
                ["door.close"] = ("state", "open", "closed"),
                ["seat.take"] = ("occupant", "", Requester),
                ["seat.leave"] = ("occupant", Requester, ""),
                ["item.take"] = ("holder", "", Requester),
                ["lamp.on"] = ("light", "off", "on"),
            };

        static readonly SortedDictionary<string, string> Facts = new SortedDictionary<string, string>(StringComparer.Ordinal);
        static readonly List<string> Transitions = new List<string>();

        /// <summary>Raised after an accepted transition: (fact key, new value).</summary>
        public static event Action<string, string> FactChanged;

        public static IReadOnlyList<string> Log => Transitions;
        public static int FactCount => Facts.Count;

        public static void Reset()
        {
            Facts.Clear();
            Transitions.Clear();
        }

        public static void Declare(string entityKey, string fact, string value) => Facts[Key(entityKey, fact)] = value ?? "";

        public static string Key(string entityKey, string fact) => entityKey + ":" + fact;

        public static string Get(string key) => key != null && Facts.TryGetValue(key, out var v) ? v : null;

        public static bool Has(string key, string value) => Get(key) is string v && v == (value ?? "");

        public static TransitionResult Request(string requester, string entityKey, string transition)
        {
            TransitionResult Reject(string reason)
            {
                Transitions.Add($"{Transitions.Count:D3}|{requester}|{entityKey}|{transition}|REJECTED:{reason}");
                return new TransitionResult(false, reason, null, null);
            }
            if (string.IsNullOrEmpty(requester) || requester.StartsWith("<")) return Reject("UNBOUND_REQUESTER");
            if (string.IsNullOrEmpty(entityKey) || entityKey.StartsWith("<")) return Reject("UNBOUND_ENTITY");
            if (!Rules.TryGetValue(transition ?? "", out var rule)) return Reject("UNKNOWN_TRANSITION");
            var key = Key(entityKey, rule.fact);
            if (!Facts.TryGetValue(key, out var current)) return Reject("UNDECLARED_FACT");
            var required = rule.from == Requester ? requester : rule.from;
            if (current != required) return Reject("PRECONDITION:" + (current == "" ? "<empty>" : current));
            var next = rule.to == Requester ? requester : rule.to;
            Facts[key] = next;
            Transitions.Add($"{Transitions.Count:D3}|{requester}|{entityKey}|{transition}|ACCEPTED:{key}={next}");
            FactChanged?.Invoke(key, next);
            return new TransitionResult(true, "ACCEPTED", key, next);
        }

        public static Payload Snapshot() => new Payload
        {
            schema = PayloadSchema,
            version = PayloadVersion,
            keys = Facts.Keys.ToArray(),
            values = Facts.Values.ToArray(),
            log = Transitions.ToArray(),
        };

        /// <summary>Fails closed: an unknown schema/version or a malformed payload leaves the current state untouched.</summary>
        public static bool Restore(Payload payload, out string reason)
        {
            if (payload == null) { reason = "NULL_PAYLOAD"; return false; }
            if (payload.schema != PayloadSchema) { reason = "SCHEMA:" + payload.schema; return false; }
            if (payload.version != PayloadVersion) { reason = "VERSION:" + payload.version; return false; }
            if (payload.keys == null || payload.values == null || payload.keys.Length != payload.values.Length) { reason = "MALFORMED"; return false; }
            Facts.Clear();
            for (int i = 0; i < payload.keys.Length; i++) Facts[payload.keys[i]] = payload.values[i];
            Transitions.Clear();
            Transitions.AddRange(payload.log ?? Array.Empty<string>());
            reason = "RESTORED";
            return true;
        }

        public static string Digest()
        {
            var text = new StringBuilder(PayloadSchema).Append('\n');
            foreach (var kv in Facts) text.Append(kv.Key).Append('=').Append(kv.Value).Append('\n');
            foreach (var line in Transitions) text.Append(line).Append('\n');
            using (var sha = SHA256.Create())
                return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(text.ToString())).Select(b => b.ToString("x2")));
        }
    }

    public readonly struct TransitionResult
    {
        public readonly bool Accepted;
        public readonly string Reason, Key, Value;
        public TransitionResult(bool accepted, string reason, string key, string value)
        {
            Accepted = accepted; Reason = reason; Key = key; Value = value;
        }
    }

    /// <summary>Versioned, Juego2-owned payload. Field layout is JsonUtility-compatible on purpose.</summary>
    [Serializable]
    public class Payload
    {
        public string schema;
        public int version;
        public string[] keys;
        public string[] values;
        public string[] log;
    }
}
