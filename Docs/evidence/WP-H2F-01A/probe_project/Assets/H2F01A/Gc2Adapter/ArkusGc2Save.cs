using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.SaveSystem;
using Juego2.H2F01A.Arkus;
using UnityEngine;

namespace Juego2.H2F01A.Gc2Adapter
{
    /// <summary>
    /// Hosts the Juego2-owned payload inside GC2's SaveLoadManager through the public IGameSave contract. GC2 only
    /// transports an opaque, versioned Arkus payload; Arkus validates and applies it (or refuses it) on load.
    /// </summary>
    public sealed class ArkusSaveHost : IGameSave
    {
        public const string ID = "juego2.arkus.probe-payload";

        public string SaveID => ID;
        public bool IsShared => false;
        public Type SaveType => typeof(Payload);
        public LoadMode LoadMode => LoadMode.Greedy;

        public event Action<bool, string> Loaded;

        public object GetSaveData(bool includeNonSavable) => ArkusProbeAuthority.Snapshot();

        public Task OnLoad(object value)
        {
            bool ok = ArkusProbeAuthority.Restore(value as Payload, out string reason);
            Loaded?.Invoke(ok, reason);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Juego2-owned GC2 storage backend (public TDataStorage extension point). Writes one JSON file in the disposable
    /// workspace instead of GC2's default PlayerPrefs, so the probe leaves no registry residue.
    /// </summary>
    [Title("Juego2 Workspace JSON")]
    [Category("Juego2/Workspace JSON")]
    [Serializable]
    public class Juego2WorkspaceStorage : TDataStorage
    {
        [SerializeField] private string m_RelativeFile = "../../../out/c06/gc2_storage.json";

        [Serializable] class Entry { public string key; public string json; }
        [Serializable] class File { public List<Entry> entries = new List<Entry>(); }

        static Dictionary<string, string> Cache;

        string FullPath => Path.GetFullPath(Path.Combine(Application.dataPath, m_RelativeFile));

        Dictionary<string, string> Data
        {
            get
            {
                if (Cache != null) return Cache;
                Cache = new Dictionary<string, string>();
                if (System.IO.File.Exists(FullPath))
                    foreach (var e in JsonUtility.FromJson<File>(System.IO.File.ReadAllText(FullPath)).entries) Cache[e.key] = e.json;
                return Cache;
            }
        }

        public static void ResetCache() => Cache = null;

        /// <summary>Probe-only access for the negative control (replace a stored payload with a foreign one).</summary>
        public static IEnumerable<string> Keys => Cache?.Keys.ToArray() ?? Array.Empty<string>();
        public static void RawSet(string key, string json) { if (Cache != null) Cache[key] = json; }
        public static string RawGet(string key) => Cache != null && Cache.TryGetValue(key, out var v) ? v : null;

        public override Task Commit()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FullPath));
            var file = new File { entries = Data.OrderBy(kv => kv.Key, StringComparer.Ordinal).Select(kv => new Entry { key = kv.Key, json = kv.Value }).ToList() };
            System.IO.File.WriteAllText(FullPath, JsonUtility.ToJson(file, true));
            return Task.CompletedTask;
        }

        public override Task DeleteAll() { Data.Clear(); return Task.CompletedTask; }
        public override Task DeleteKey(string key) { Data.Remove(key); return Task.CompletedTask; }
        public override Task<bool> HasKey(string key) => Task.FromResult(Data.ContainsKey(key));

        public override Task<object> Get(string key, Type type) =>
            Task.FromResult(Data.TryGetValue(key, out var json) && !string.IsNullOrEmpty(json) ? JsonUtility.FromJson(json, type) : null);

        public override Task Set(string key, object value)
        {
            Data[key] = JsonUtility.ToJson(value, false);
            return Task.CompletedTask;
        }
    }
}
