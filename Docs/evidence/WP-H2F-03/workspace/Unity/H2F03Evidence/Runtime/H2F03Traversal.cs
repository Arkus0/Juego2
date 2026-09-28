using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using Juego2.Arkus;
using Juego2.Gc2Adapter;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

namespace Juego2.H2F03.Evidence
{
    /// <summary>
    /// WP-H2F-03 scripted third-person traversal of the integrated fixture (evidence only). It drives the GC2 player only
    /// through the Juego2 action map (a virtual gamepad feeding <c>J2_Input</c>), sends the GC2 NPC along the NavMesh
    /// through GC2's public motion API, presses Interact at the door, records the diagnostic profiler series of
    /// PERFORMANCE_ORACLE.md, every error/exception, the contact-IK distance and, on the capture attempt, a continuous
    /// frame sequence. It is idle unless the editor launcher wrote <c>play_config.json</c>, so the owner can play the
    /// same scene by hand.
    /// </summary>
    public sealed class H2F03Traversal : MonoBehaviour
    {
        [Serializable]
        public struct Waypoint
        {
            public string name;
            public Vector3 position;
            public string action;      // "" | "interact" | "look"
            public float lookYaw;      // world yaw (degrees) the camera turns to for "look"
            public float hold;         // seconds held at the waypoint
        }

        [Serializable]
        public sealed class Config
        {
            public bool active;
            public string attempt;
            public bool capture;
            public string resultsDir;
            public string candidateSha;
            public string unityTree;
            public float routeWatchdogSeconds = 360f;
        }

        public Waypoint[] route = Array.Empty<Waypoint>();
        public Vector3[] npcLoop = Array.Empty<Vector3>();
        public string playerKey = "j2.char.player";
        public string walkerKey = "j2.npc.fixture_walker";
        public string contactKey = "j2.npc.fixture_contact";
        public string lampKey = "j2.lamp.fixture_house";
        public H2F03FixtureAuthority authority;
        public float captureInterval = 0.25f;

        public static bool Completed { get; private set; }
        public static string ResultPath { get; private set; }

        Config config;
        Gamepad pad;
        Character player, walker, contact;
        Camera view;
        readonly List<string> errors = new List<string>();
        readonly List<string> events = new List<string>();
        readonly Dictionary<string, Series> series = new Dictionary<string, Series>(StringComparer.Ordinal);
        readonly List<ProfilerRecorder> recorders = new List<ProfilerRecorder>();
        readonly List<(string, ProfilerRecorder)> named = new List<(string, ProfilerRecorder)>();
        float started, routeStarted;
        int frameIndex, captureIndex, waypointIndex = -1;
        RenderTexture captureTarget, captureSmall;
        string captureDir;
        readonly List<float> contactDistance = new List<float>();
        float contactDistanceRigOff = -1f;
        readonly List<string> walkerLog = new List<string>();
        int walkerArrivals, walkerFailures;
        float footMin = float.MaxValue, footMax = float.MinValue;
        Vector3 lastPlayerPosition;
        UnityEngine.InputSystem.InputSettings originalInputSettings;
        bool gpuAvailable;

        sealed class Series
        {
            public readonly List<double> values = new List<double>();
            public readonly List<string> where = new List<string>();
        }

        [Serializable]
        sealed class Summary
        {
            public string name;
            public int count;
            public double min, median, p95, max;
            public string[] worst;
        }

        [Serializable]
        sealed class Result
        {
            public string schema = "juego2.h2f03.play-attempt@1";
            public string attempt, candidateSha, unityTree, unityVersion, pipeline, qualityLevel, renderer, device, cpu, gpu;
            public int systemMemoryMb, graphicsMemoryMb, screenWidth, screenHeight, captureWidth, captureHeight, frames, captures;
            public bool capture, completed, watchdogFired, gpuTimingAvailable, urpActive, navMeshPresent, playerDriven, ikSolving,
                interactionRefusedFirst, interactionAcceptedSecond, lampOnAfterAccepted, playerHasRuntimeController, walkerHasNavMeshAgent,
                playerAnimated;
            public float durationSeconds, warmupSeconds, contactIkMeanDistance, contactIkRigOffDistance, footStrideRange;
            public int walkerArrivals, walkerFailures, reached;
            public string[] waypointsReached, errors, events, authorityLog, walkerLog, interactResults, features;
            public Summary[] metrics;
        }

        void Awake()
        {
            Completed = false;
            ResultPath = null;
            var dir = Arg("-h2f03-results");
            if (dir == null) return;
            var path = Path.Combine(dir, "play_config.json");
            if (!File.Exists(path)) return;
            config = JsonUtility.FromJson<Config>(File.ReadAllText(path));
            if (config == null || !config.active) config = null;
        }

        IEnumerator Start()
        {
            if (config == null) yield break; // owner play: nothing scripted
            Application.logMessageReceivedThreaded += OnLog;
            started = Time.realtimeSinceStartup;
            player = Find(playerKey);
            walker = Find(walkerKey);
            contact = Find(contactKey);
            view = Camera.main;
            // The scripted run must not depend on which window has focus: keep the player loop and the virtual pad live.
            Application.runInBackground = true;
            originalInputSettings = InputSystem.settings;
            var settings = Instantiate(originalInputSettings);
            settings.backgroundBehavior = UnityEngine.InputSystem.InputSettings.BackgroundBehavior.IgnoreFocus;
            settings.editorInputBehaviorInPlayMode = UnityEngine.InputSystem.InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings = settings;
            pad = InputSystem.AddDevice<Gamepad>("H2F03VirtualPad");
            StartRecorders();
            if (config.capture) SetupCapture();
            ProbeContact(true);
            yield return new WaitForSecondsRealtime(3f); // declared warm-up
            routeStarted = Time.realtimeSinceStartup;
            StartCoroutine(Walker());
            yield return Route();
            ProbeContact(false);
            Finish();
        }

        static Character Find(string key)
        {
            var go = ArkusEntityBinding.Resolve(key);
            return go != null ? go.GetComponent<Character>() : null;
        }

        // ------------------------------------------------------------------ route

        readonly List<string> reached = new List<string>();
        readonly List<string> interactResults = new List<string>();
        bool watchdog;

        IEnumerator Route()
        {
            if (player == null) { errors.Add("H2F03_PLAYER_MISSING"); yield break; }
            var interact = player.GetComponent<J2Gc2InteractInput>();
            if (interact != null) interact.Interacted += ok => interactResults.Add(ok ? "target" : "no-target");
            for (waypointIndex = 0; waypointIndex < route.Length; waypointIndex++)
            {
                var w = route[waypointIndex];
                float segmentStart = Time.realtimeSinceStartup, lastProgress = segmentStart;
                float best = float.MaxValue;
                while (true)
                {
                    if (Time.realtimeSinceStartup - routeStarted > config.routeWatchdogSeconds) { watchdog = true; errors.Add("H2F03_ROUTE_WATCHDOG at " + w.name); Stick(Vector2.zero, Vector2.zero, false); yield break; }
                    var delta = w.position - player.transform.position;
                    delta.y = 0;
                    if (delta.magnitude < 0.45f) break;
                    if (delta.magnitude < best - 0.05f) { best = delta.magnitude; lastProgress = Time.realtimeSinceStartup; }
                    if (Time.realtimeSinceStartup - lastProgress > 8f) { errors.Add($"H2F03_STUCK before {w.name} at {V(player.transform.position)}"); break; }
                    Stick(Steer(delta.normalized, delta.magnitude), Vector2.zero, false);
                    yield return null;
                }
                Stick(Vector2.zero, Vector2.zero, false);
                reached.Add(w.name);
                events.Add($"{Time.realtimeSinceStartup - routeStarted:0.0}s reached {w.name} at {V(player.transform.position)}");
                if (w.action == "interact") yield return Interact();
                if (w.action == "look") yield return Look(w.lookYaw);
                if (w.hold > 0) yield return Hold(w.hold);
                if (config.capture) CaptureKey(w.name);
            }
            waypointIndex = route.Length;
        }

        Vector2 Steer(Vector3 direction, float distance)
        {
            var forward = view != null ? Vector3.ProjectOnPlane(view.transform.forward, Vector3.up).normalized : Vector3.forward;
            var right = Vector3.Cross(Vector3.up, forward);
            var stick = new Vector2(Vector3.Dot(direction, right), Vector3.Dot(direction, forward));
            return stick.normalized * Mathf.Clamp01(0.35f + distance);
        }

        IEnumerator Interact()
        {
            float lampBefore = LampIntensity();
            yield return Press();
            yield return Hold(1.0f);
            events.Add($"after first interact: door={authority?.DoorState} lamp={LampIntensity():0.00} (before {lampBefore:0.00})");
            if (config.capture) CaptureKey("interact_refused");
            authority?.Unlock();
            events.Add("fixture authority unlocked (Arkus decision)");
            yield return Press();
            yield return Hold(1.5f);
            events.Add($"after second interact: door={authority?.DoorState} lamp={LampIntensity():0.00}");
            if (config.capture) CaptureKey("interact_accepted");
        }

        IEnumerator Press()
        {
            Stick(Vector2.zero, Vector2.zero, true);
            yield return null;
            yield return null;
            Stick(Vector2.zero, Vector2.zero, false);
            yield return null;
        }

        IEnumerator Look(float yaw)
        {
            float until = Time.realtimeSinceStartup + 6f;
            while (Time.realtimeSinceStartup < until && view != null)
            {
                float current = view.transform.eulerAngles.y;
                float error = Mathf.DeltaAngle(current, yaw);
                if (Mathf.Abs(error) < 6f) break;
                Stick(Vector2.zero, new Vector2(Mathf.Sign(error) * Mathf.Clamp01(Mathf.Abs(error) / 45f + 0.25f), 0), false);
                yield return null;
            }
            Stick(Vector2.zero, Vector2.zero, false);
            events.Add($"look yaw target {yaw:0} reached {(view != null ? view.transform.eulerAngles.y : -1):0}");
        }

        IEnumerator Hold(float seconds)
        {
            float until = Time.realtimeSinceStartup + seconds;
            while (Time.realtimeSinceStartup < until) { Stick(Vector2.zero, Vector2.zero, false); yield return null; }
        }

        void Stick(Vector2 move, Vector2 look, bool south)
        {
            if (pad == null) return;
            var state = new GamepadState { leftStick = move, rightStick = look };
            if (south) state = state.WithButton(GamepadButton.South);
            InputSystem.QueueStateEvent(pad, state);
        }

        float LampIntensity()
        {
            var go = ArkusEntityBinding.Resolve(lampKey);
            var light = go != null ? go.GetComponent<Light>() : null;
            return light != null ? light.intensity : -1f;
        }

        // ------------------------------------------------------------------ NPC on the NavMesh (GC2 public motion API)

        IEnumerator Walker()
        {
            if (walker == null || npcLoop.Length == 0) { errors.Add("H2F03_WALKER_MISSING"); yield break; }
            int i = 0;
            while (waypointIndex < route.Length)
            {
                var target = npcLoop[i % npcLoop.Length];
                bool done = false, success = false;
                walker.Motion.MoveToLocation(new Location(target), 0.3f, (c, ok) => { done = true; success = ok; }, 1);
                float until = Time.realtimeSinceStartup + 60f;
                while (!done && Time.realtimeSinceStartup < until && waypointIndex < route.Length) yield return null;
                if (success) walkerArrivals++; else if (done) walkerFailures++;
                bool onMesh = NavMesh.SamplePosition(walker.transform.position, out _, 1.2f, NavMesh.AllAreas);
                walkerLog.Add($"{Time.realtimeSinceStartup - routeStarted:0.0}s target {V(target)} done={done} success={success} at {V(walker.transform.position)} onNavMesh={onMesh}");
                i++;
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        // ------------------------------------------------------------------ profiler (PERFORMANCE_ORACLE.md)

        static readonly (ProfilerCategory, string)[] Counters =
        {
            (ProfilerCategory.Internal, "Main Thread"),
            (ProfilerCategory.Render, "Draw Calls Count"),
            (ProfilerCategory.Render, "Batches Count"),
            (ProfilerCategory.Render, "SetPass Calls Count"),
            (ProfilerCategory.Render, "Triangles Count"),
            (ProfilerCategory.Render, "Vertices Count"),
            (ProfilerCategory.Memory, "Total Used Memory"),
            (ProfilerCategory.Memory, "Total Reserved Memory"),
            (ProfilerCategory.Memory, "GC Reserved Memory"),
            (ProfilerCategory.Memory, "System Used Memory"),
            (ProfilerCategory.Memory, "GC Allocated In Frame"),
        };

        void StartRecorders()
        {
            foreach (var (category, name) in Counters)
            {
                var r = ProfilerRecorder.StartNew(category, name, 1);
                recorders.Add(r);
                named.Add((name, r));
            }
            gpuAvailable = SystemInfo.supportsGpuRecorder;
        }

        readonly FrameTiming[] timings = new FrameTiming[1];

        void Update()
        {
            if (config == null || routeStarted <= 0 || Completed) return;
            frameIndex++;
            var where = waypointIndex >= 0 && waypointIndex < route.Length ? route[waypointIndex].name : "end";
            Add("unscaledDeltaTimeMs", Time.unscaledDeltaTime * 1000.0, where);
            foreach (var (name, r) in named)
                if (r.Valid && r.Count > 0)
                {
                    double v = r.LastValue;
                    if (name == "Main Thread") v /= 1e6; // ns -> ms
                    Add(name == "Main Thread" ? "mainThreadMs" : name, v, where);
                }
            FrameTimingManager.CaptureFrameTimings();
            if (FrameTimingManager.GetLatestTimings(1, timings) > 0 && timings[0].gpuFrameTime > 0)
            {
                gpuAvailable = true;
                Add("gpuFrameMs", timings[0].gpuFrameTime, where);
            }
            if (config.capture && Time.realtimeSinceStartup - lastCapture >= captureInterval) CaptureFrame();
        }

        void LateUpdate()
        {
            if (config == null || routeStarted <= 0 || Completed) return;
            var anim = player != null ? player.GetComponentInChildren<Animator>() : null;
            var foot = anim != null && anim.isHuman ? anim.GetBoneTransform(HumanBodyBones.LeftFoot) : null;
            var position = player != null ? player.transform.position : Vector3.zero;
            bool moving = player != null && Time.deltaTime > 0 && Vector3.ProjectOnPlane(position - lastPlayerPosition, Vector3.up).magnitude / Time.deltaTime > 0.8f;
            lastPlayerPosition = position;
            if (foot != null && moving)
            {
                var local = player.transform.InverseTransformPoint(foot.position).z;
                footMin = Mathf.Min(footMin, local);
                footMax = Mathf.Max(footMax, local);
            }
            var d = ContactDistance();
            if (d >= 0 && contactDistanceRigOff < 0) contactDistance.Add(d);
        }

        void Add(string name, double value, string where)
        {
            if (!series.TryGetValue(name, out var s)) series[name] = s = new Series();
            s.values.Add(value);
            s.where.Add(where);
        }

        static Summary Summarize(string name, Series s, int skip)
        {
            var values = s.values.Skip(skip).ToList();
            var wheres = s.where.Skip(skip).ToList();
            if (values.Count == 0) return new Summary { name = name };
            var sorted = values.OrderBy(v => v).ToList();
            double Pct(double p) => sorted[Mathf.Clamp((int)Math.Ceiling(p * sorted.Count) - 1, 0, sorted.Count - 1)];
            var worst = values.Select((v, i) => (v, i)).OrderByDescending(t => t.v).Take(10).Select(t => $"{t.v:0.###}@{wheres[t.i]}#{t.i + skip}").ToArray();
            return new Summary { name = name, count = values.Count, min = sorted[0], median = Pct(0.5), p95 = Pct(0.95), max = sorted[sorted.Count - 1], worst = worst };
        }

        // ------------------------------------------------------------------ contact IK (Animation Rigging on a GC2 body)

        TwoBoneIKConstraint Constraint() => contact != null ? contact.GetComponentInChildren<TwoBoneIKConstraint>(true) : null;

        float ContactDistance()
        {
            var c = Constraint();
            if (c == null || c.data.tip == null || c.data.target == null) return -1f;
            return Vector3.Distance(c.data.tip.position, c.data.target.position);
        }

        IEnumerator RigOffProbe()
        {
            var rig = contact != null ? contact.GetComponentInChildren<Rig>(true) : null;
            if (rig == null) yield break;
            rig.weight = 0f;
            yield return new WaitForSecondsRealtime(0.6f);
            contactDistanceRigOff = ContactDistance();
            rig.weight = 1f;
        }

        void ProbeContact(bool atStart)
        {
            if (!atStart) return;
            var c = Constraint();
            events.Add(c == null ? "contact IK constraint missing" : $"contact IK tip={c.data.tip?.name} target={c.data.target?.name}");
        }

        // ------------------------------------------------------------------ capture (attempt A only)

        float lastCapture;

        void SetupCapture()
        {
            captureDir = Path.Combine(config.resultsDir, "captures", "attempt_" + config.attempt);
            Directory.CreateDirectory(Path.Combine(captureDir, "frames"));
            captureTarget = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32) { name = "H2F03Capture", antiAliasing = 1 };
            captureSmall = new RenderTexture(640, 360, 0, RenderTextureFormat.ARGB32) { name = "H2F03CaptureSmall" };
            if (view != null) view.targetTexture = captureTarget;
        }

        void CaptureFrame()
        {
            lastCapture = Time.realtimeSinceStartup;
            if (captureTarget == null) return;
            Graphics.Blit(captureTarget, captureSmall);
            int index = captureIndex++;
            AsyncGPUReadback.Request(captureSmall, 0, TextureFormat.RGBA32, request =>
            {
                if (request.hasError) return;
                var bytes = ImageConversion.EncodeNativeArrayToPNG(request.GetData<byte>(), UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm, 640u, 360u);
                File.WriteAllBytes(Path.Combine(captureDir, "frames", $"f{index:0000}.png"), bytes.ToArray());
            });
        }

        void CaptureKey(string name)
        {
            if (captureTarget == null) return;
            var previous = RenderTexture.active;
            RenderTexture.active = captureTarget;
            var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            tex.Apply();
            RenderTexture.active = previous;
            File.WriteAllBytes(Path.Combine(captureDir, $"key_{name}.png"), tex.EncodeToPNG());
            Destroy(tex);
        }

        // ------------------------------------------------------------------ result

        void OnLog(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
            var first = (stackTrace ?? "").Split('\n').FirstOrDefault(l => l.Trim().Length > 0)?.Trim() ?? "";
            lock (errors) errors.Add($"{type}: {condition.Split('\n')[0]} @ {first}");
        }

        void Finish()
        {
            StartCoroutine(FinishRoutine());
        }

        IEnumerator FinishRoutine()
        {
            yield return RigOffProbe();
            AsyncGPUReadback.WaitAllRequests();
            Application.logMessageReceivedThreaded -= OnLog;
            var pipeline = GraphicsSettings.currentRenderPipeline;
            var lamp = LampIntensity();
            var log = authority != null ? authority.Log.ToArray() : new string[0];
            int skip = Mathf.Max(0, (int)(1f / Mathf.Max(0.001f, Time.unscaledDeltaTime)));
            var result = new Result
            {
                attempt = config.attempt,
                candidateSha = config.candidateSha,
                unityTree = config.unityTree,
                unityVersion = Application.unityVersion,
                pipeline = pipeline != null ? pipeline.name : "builtin",
                qualityLevel = QualitySettings.names[QualitySettings.GetQualityLevel()],
                renderer = pipeline is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset urp && urp.rendererDataList.Length > 0 && urp.rendererDataList[0] != null ? urp.rendererDataList[0].name : "",
                features = RendererFeatures(),
                device = SystemInfo.deviceModel,
                cpu = SystemInfo.processorType,
                gpu = SystemInfo.graphicsDeviceName + " / " + SystemInfo.graphicsDeviceVersion,
                systemMemoryMb = SystemInfo.systemMemorySize,
                graphicsMemoryMb = SystemInfo.graphicsMemorySize,
                screenWidth = Screen.width,
                screenHeight = Screen.height,
                captureWidth = captureTarget != null ? 1280 : 0,
                captureHeight = captureTarget != null ? 720 : 0,
                capture = config.capture,
                frames = frameIndex,
                captures = captureIndex,
                completed = waypointIndex >= route.Length && !watchdog,
                watchdogFired = watchdog,
                gpuTimingAvailable = gpuAvailable,
                urpActive = pipeline is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset,
                navMeshPresent = NavMesh.CalculateTriangulation().vertices.Length > 0,
                playerDriven = reached.Count >= Math.Min(2, route.Length) && player != null,
                playerHasRuntimeController = player != null && player.GetComponent<CharacterController>() != null,
                walkerHasNavMeshAgent = walker != null && walker.GetComponent<NavMeshAgent>() != null,
                playerAnimated = footMax - footMin > 0.25f,
                footStrideRange = footMax > footMin ? footMax - footMin : 0f,
                ikSolving = contactDistance.Count > 0 && contactDistance.Average() < 0.05f && contactDistanceRigOff > 0.08f,
                contactIkMeanDistance = contactDistance.Count > 0 ? contactDistance.Average() : -1f,
                contactIkRigOffDistance = contactDistanceRigOff,
                interactionRefusedFirst = log.Length > 0 && log[0].EndsWith("|Refused|LOCKED"),
                interactionAcceptedSecond = log.Length > 1 && log[1].EndsWith("|Accepted|ACCEPTED"),
                lampOnAfterAccepted = lamp > 0.1f,
                durationSeconds = Time.realtimeSinceStartup - routeStarted,
                warmupSeconds = 3f,
                walkerArrivals = walkerArrivals,
                walkerFailures = walkerFailures,
                reached = reached.Count,
                waypointsReached = reached.ToArray(),
                errors = errors.ToArray(),
                events = events.ToArray(),
                authorityLog = log,
                walkerLog = walkerLog.ToArray(),
                interactResults = interactResults.ToArray(),
                metrics = series.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => Summarize(p.Key, p.Value, skip)).ToArray(),
            };
            var raw = new StringBuilder();
            foreach (var p in series.OrderBy(p => p.Key, StringComparer.Ordinal))
                raw.Append(p.Key).Append(',').Append(string.Join(",", p.Value.values.Select(v => v.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)))).Append('\n');
            Directory.CreateDirectory(config.resultsDir);
            ResultPath = Path.Combine(config.resultsDir, $"play_{config.attempt}.json");
            File.WriteAllText(ResultPath, JsonUtility.ToJson(result, true) + "\n", new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(config.resultsDir, $"play_{config.attempt}_series.csv"), raw.ToString(), new UTF8Encoding(false));
            foreach (var r in recorders) r.Dispose();
            if (view != null) view.targetTexture = null;
            if (pad != null) InputSystem.RemoveDevice(pad);
            if (originalInputSettings != null) InputSystem.settings = originalInputSettings;
            Debug.Log($"H2F03_PLAY_ATTEMPT attempt={config.attempt} completed={result.completed} reached={result.reached}/{route.Length} errors={result.errors.Length} frames={result.frames} captures={result.captures} ik={result.ikSolving} interact={result.interactionRefusedFirst}/{result.interactionAcceptedSecond} walker={walkerArrivals}/{walkerFailures}");
            Completed = true;
        }

        static string[] RendererFeatures()
        {
            if (!(GraphicsSettings.currentRenderPipeline is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset urp)) return new string[0];
            var data = urp.rendererDataList.Length > 0 ? urp.rendererDataList[0] as UnityEngine.Rendering.Universal.ScriptableRendererData : null;
            return data != null ? data.rendererFeatures.Where(f => f != null).Select(f => f.GetType().Name + (f.isActive ? "" : "(inactive)")).ToArray() : new string[0];
        }

        static string V(Vector3 v) => $"({v.x:0.00},{v.y:0.00},{v.z:0.00})";

        static string Arg(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++) if (args[i] == name) return args[i + 1];
            return null;
        }
    }
}
