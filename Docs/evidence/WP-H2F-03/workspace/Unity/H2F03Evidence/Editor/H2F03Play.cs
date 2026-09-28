using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Juego2.H2F03.Evidence
{
    /// <summary>
    /// Windowed Play Mode launcher for WP-H2F-03 (this workstation's licence has no headless play entitlement). It opens
    /// the sidecar with the current H1-managed scene additive, then runs attempts A (capture), B and C (clean), each from a
    /// fresh Play Mode entry, and exits the editor. State survives domain reloads through SessionState. Also provides the
    /// owner's menu entry to open the same composition for hand play.
    /// </summary>
    [InitializeOnLoad]
    public static class H2F03Play
    {
        const string Active = "H2F03_PLAY_ACTIVE", Index = "H2F03_PLAY_INDEX", ResultsKey = "H2F03_PLAY_RESULTS", Deadline = "H2F03_PLAY_DEADLINE";
        static readonly string[] Attempts = { "A", "B", "C" };

        static H2F03Play()
        {
            if (!SessionState.GetBool(Active, false)) return;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged -= OnPlayMode;
            EditorApplication.playModeStateChanged += OnPlayMode;
        }

        /// <summary><c>-executeMethod Juego2.H2F03.Evidence.H2F03Play.Run -h2f03-results DIR [-h2f03-sha SHA -h2f03-tree TREE]</c></summary>
        public static void Run()
        {
            SessionState.SetBool(Active, true);
            SessionState.SetInt(Index, 0);
            SessionState.SetString(ResultsKey, H2F03Io.Results);
            EditorApplication.delayCall += () =>
            {
                OpenComposition();
                StartAttempt();
            };
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged -= OnPlayMode;
            EditorApplication.playModeStateChanged += OnPlayMode;
        }

        [MenuItem("Juego2/H2F-03/Open integrated fixture (NON_KEEPER)")]
        public static void OpenComposition()
        {
            var sidecar = EditorSceneManager.OpenScene(H2F03Fixture.ScenePath, OpenSceneMode.Single);
            H2F03Fixture.OpenManaged(out _, out _);
            SceneManager.SetActiveScene(sidecar);
        }

        static void StartAttempt()
        {
            int i = SessionState.GetInt(Index, 0);
            var results = SessionState.GetString(ResultsKey, H2F03Io.Results);
            var config = new H2F03Traversal.Config
            {
                active = true,
                attempt = Attempts[i],
                capture = i == 0,
                resultsDir = results,
                candidateSha = H2F03Io.Arg("-h2f03-sha") ?? "",
                unityTree = H2F03Io.Arg("-h2f03-tree") ?? "",
            };
            File.WriteAllText(Path.Combine(results, "play_config.json"), JsonUtility.ToJson(config, true));
            SessionState.SetFloat(Deadline, (float)EditorApplication.timeSinceStartup + 900f);
            Debug.Log($"H2F03_PLAY_START attempt={config.attempt} capture={config.capture}");
            EditorApplication.EnterPlaymode();
        }

        static void Tick()
        {
            if (!SessionState.GetBool(Active, false)) return;
            if (EditorApplication.isPlaying && H2F03Traversal.Completed) EditorApplication.ExitPlaymode();
            if (EditorApplication.timeSinceStartup > SessionState.GetFloat(Deadline, float.MaxValue))
            {
                Debug.LogError("H2F03_PLAY_WATCHDOG editor-level timeout");
                File.WriteAllText(Path.Combine(SessionState.GetString(ResultsKey, H2F03Io.Results), $"play_{Attempts[SessionState.GetInt(Index, 0)]}_watchdog.txt"), "editor watchdog\n");
                SessionState.SetFloat(Deadline, float.MaxValue);
                if (EditorApplication.isPlaying) EditorApplication.ExitPlaymode();
            }
        }

        static void OnPlayMode(PlayModeStateChange change)
        {
            if (change != PlayModeStateChange.EnteredEditMode || !SessionState.GetBool(Active, false)) return;
            int next = SessionState.GetInt(Index, 0) + 1;
            if (next < Attempts.Length)
            {
                SessionState.SetInt(Index, next);
                EditorApplication.delayCall += StartAttempt;
                return;
            }
            SessionState.SetBool(Active, false);
            var results = SessionState.GetString(ResultsKey, H2F03Io.Results);
            File.Delete(Path.Combine(results, "play_config.json")); // owner play stays unscripted
            Debug.Log("H2F03_PLAY_DONE attempts=" + string.Join(",", Attempts.Where(a => File.Exists(Path.Combine(results, $"play_{a}.json")))));
            EditorApplication.delayCall += () => EditorApplication.Exit(0);
        }
    }
}
