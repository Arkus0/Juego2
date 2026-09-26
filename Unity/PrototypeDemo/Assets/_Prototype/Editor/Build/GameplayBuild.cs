using System.Collections.Generic;
using System.Linq;
using Proto.Runtime;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Proto.Build.Geo;

namespace Proto.Build
{
    /// Player, camera, HUD, secrets, NPCs, auto-capture shots.
    public class GameplayBuild
    {
        readonly TerrainBuild tb; readonly Layout lay; readonly Transform root;
        const string UAL = "Assets/ThirdParty/Quaternius/UAL/Models/";
        public static readonly Vector2 Start = P(39.2f, -70.6f);
        public GameplayBuild(TerrainBuild t, Layout l, Transform r) { tb = t; lay = l; root = r; }

        public static float WalkHeight(TerrainBuild tb, Vector2 p)
        {
            if (Bridge.InFootprint(p, 0f, out float s) && s > 0.1f) return Bridge.Deck(s);
            foreach (var pl in Seed.Platforms) if (InPoly(p, pl.Poly)) return pl.H;
            return tb.Ground(p);
        }

        float Hgt(Vector2 p) => WalkHeight(tb, p);

        static AnimationClip Clip(string name)
        {
            foreach (var path in new[] { UAL + "UAL1.fbx", UAL + "UAL2.fbx" })
                foreach (var o in AssetDatabase.LoadAllAssetsAtPath(path))
                    if (o is AnimationClip c && !c.name.StartsWith("__preview") && c.name.EndsWith("|" + name)) return c;
            Debug.LogWarning("[Proto] missing clip " + name);
            return null;
        }

        static AnimatorController Controller()
        {
            string path = MatLib.GenDir + "/Townsfolk.controller";
            AssetDatabase.DeleteAsset(path);
            var ac = AnimatorController.CreateAnimatorControllerAtPath(path);
            ac.AddParameter("Speed", AnimatorControllerParameterType.Float);
            var sm = ac.layers[0].stateMachine;
            var move = ac.CreateBlendTreeInController("Move", out var tree, 0);
            tree.blendParameter = "Speed";
            tree.useAutomaticThresholds = false;
            tree.AddChild(Clip("Idle_Loop"), 0f);
            tree.AddChild(Clip("Walk_Loop"), 1.25f);
            tree.AddChild(Clip("Jog_Fwd_Loop"), 3.4f);
            sm.defaultState = move;
            foreach (var (state, clip) in new[] { ("Talk", "Idle_Talking_Loop"), ("Sit", "Sitting_Idle_Loop"), ("SitTalk", "Sitting_Talking_Loop"),
                ("Counter", "Counter_Idle_Loop"), ("LookAround", "Idle_LookAround_Loop"), ("FoldArms", "Idle_FoldArms_Loop"), ("Carry", "Walk_Carry_Loop") })
            {
                var st = sm.AddState(state);
                st.motion = Clip(clip);
            }
            return ac;
        }

        static readonly Color[] Coats =
        {
            new Color(0.20f, 0.24f, 0.33f), new Color(0.27f, 0.32f, 0.24f), new Color(0.36f, 0.27f, 0.21f), new Color(0.42f, 0.2f, 0.19f),
            new Color(0.34f, 0.34f, 0.33f), new Color(0.48f, 0.42f, 0.28f), new Color(0.17f, 0.17f, 0.18f), new Color(0.3f, 0.36f, 0.38f),
        };
        static readonly Dictionary<Color, Material> coatMats = new Dictionary<Color, Material>();

        static Material Coat(Color c)
        {
            if (coatMats.TryGetValue(c, out var m)) return m;
            m = new Material(MatLib.Lit) { name = "Coat_" + ColorUtility.ToHtmlStringRGB(c) };
            m.SetColor("_BaseColor", c); m.SetFloat("_Smoothness", 0.25f); m.enableInstancing = true;
            AssetDatabase.CreateAsset(m, $"{MatLib.GenDir}/{m.name}.mat");
            coatMats[c] = m;
            return m;
        }

        GameObject Body(string name, string variant, bool female, Color coat, Transform parent, AnimatorController ac, int tint = 0)
        {
            if (variant != null && CharacterKit.Available(variant))
                return CharacterKit.Spawn(variant, name, parent, ac, tint);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(UAL + (female ? "Mannequin_F.fbx" : "UAL1.fbx"));
            var go = (GameObject)PrefabUtility.InstantiatePrefab(model, parent);
            go.name = name;
            var anim = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();
            anim.runtimeAnimatorController = ac;
            anim.applyRootMotion = false;
            anim.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            if (female) anim.avatar = AssetDatabase.LoadAllAssetsAtPath(UAL + "Mannequin_F.fbx").OfType<Avatar>().FirstOrDefault() ?? anim.avatar;
            var joints = Coat(coat * 0.55f + new Color(0.05f, 0.05f, 0.05f));
            foreach (var r in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++) mats[i] = i == 0 ? Coat(coat) : joints;
                r.sharedMaterials = mats;
            }
            return go;
        }

        public void Build()
        {
            coatMats.Clear();
            var ac = Controller();
            int playerLayer = LayerMask.NameToLayer("Player"), npcLayer = LayerMask.NameToLayer("NPC");

            // ---- player
            var player = new GameObject("Player");
            player.layer = playerLayer;
            player.transform.SetParent(root, false);
            player.transform.position = W(Start, Hgt(Start) + 0.05f);
            player.transform.rotation = Quaternion.LookRotation(W(P(46, -50) - Start, 0));
            var cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f; cc.radius = 0.3f; cc.center = new Vector3(0, 0.92f, 0); cc.stepOffset = 0.38f; cc.slopeLimit = 52f; cc.skinWidth = 0.04f;
            var body = Body("Cuerpo", "Forastero", false, new Color(0.18f, 0.3f, 0.32f), player.transform, ac);
            foreach (var t in body.GetComponentsInChildren<Transform>()) t.gameObject.layer = playerLayer;
            var pc = player.AddComponent<PlayerController>();
            pc.anim = body.GetComponent<Animator>();

            // ---- camera
            var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
            camGo.transform.SetParent(root, false);
            var cam = camGo.AddComponent<Camera>();
            cam.fieldOfView = 60; cam.nearClipPlane = 0.08f; cam.farClipPlane = 3000f;
            var cd = camGo.AddComponent<UniversalAdditionalCameraData>();
            cd.renderPostProcessing = true; cd.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing; cd.antialiasingQuality = AntialiasingQuality.High;
            camGo.AddComponent<AudioListener>();
            var orbit = camGo.AddComponent<OrbitCamera>();
            orbit.target = player.transform;
            orbit.collide = ~((1 << playerLayer) | (1 << npcLayer) | (1 << LayerMask.NameToLayer("Interact")) | (1 << 2));
            pc.cam = camGo.transform;
            camGo.transform.position = player.transform.position + new Vector3(0, 2, -4);

            // ---- HUD + secrets
            var hud = new GameObject("HUD").AddComponent<DemoHUD>();
            hud.transform.SetParent(root, false);
            foreach (var s in Seed.Secrets)
            {
                var go = new GameObject("Secreto_" + s.Id);
                go.transform.SetParent(root, false);
                go.transform.position = W(s.At, Hgt(s.At));
                var sp = go.AddComponent<SecretSpot>();
                sp.id = s.Id; sp.title = s.Name; sp.line = s.Text;
                sp.radius = s.Id == "disc.bar.secondary_layer" ? 2.2f : 3.2f;
            }

            // ---- townsfolk
            var npcRoot = new GameObject("Vecinos").transform; npcRoot.SetParent(root, false);
            int n = 0;
            var rng = new Rng(321);
            var used = new Dictionary<string, int>();
            GameObject Npc(string role, string hello, string variant, Vector2 at, Vector2 face, string state, Vector2[] path = null, float speed = 1.15f, float yOff = 0)
            {
                bool female = variant == "Vecina" || variant == "Moza" || variant == "Abuela" || variant == "Tendera" || variant == "Paisana" || variant == "Chavala";
                used.TryGetValue(variant, out int k); used[variant] = k + 1;     // repeated variants get re-tinted clothes
                var go = Body($"NPC_{n:00}_{role}", variant, female, Coats[n % Coats.Length], npcRoot, ac, k);
                n++;
                foreach (var t in go.GetComponentsInChildren<Transform>()) t.gameObject.layer = npcLayer;
                go.transform.position = W(at, Hgt(at) + yOff);
                go.transform.rotation = Quaternion.LookRotation(W(face, 0).sqrMagnitude > 0 ? W(face, 0) : Vector3.forward);
                go.transform.localScale = Vector3.one * rng.Range(0.94f, 1.04f);
                var npc = go.AddComponent<NPC>();
                npc.role = role; npc.hello = hello; npc.idleState = state; npc.speed = speed; npc.pause = rng.Range(1.5f, 4f);
                npc.ground = ~((1 << playerLayer) | (1 << npcLayer) | (1 << LayerMask.NameToLayer("Interact")) | (1 << 2));
                if (path != null) npc.path = path.Select(q => W(q, Hgt(q))).ToArray();
                return go;
            }
            Vector2[] Along(string routeId, float side, float s0 = 0, float s1 = 9999)
            {
                var r = Seed.Routes.First(x => x.Id == routeId);
                var pts = Resample(r.Pts, 2f);
                var off = Offset(pts, side);
                var cum = Cum(pts);
                return off.Where((q, i) => cum[i] >= s0 && cum[i] <= s1).ToArray();
            }
            Vector2[] BridgePath() => Enumerable.Range(0, 21).Select(i => Bridge.At(-4 + i * 2.1f, 0.7f)).ToArray();

            Npc("Vecina", "¡Hola! Buenos días.", "Vecina", P(41.5f, -67.2f), P(-1, -0.2f), "LookAround");
            Npc("Pescador", "¡Hola! Hoy baja tranquilo el río.", "Paisano", P(36.2f, -63.6f), P(0.3f, 1), "FoldArms");
            Npc("Vecino", "¡Hola!", "Vecino", Bridge.At(-3, 0.7f), Bridge.Dir, "Move", BridgePath(), 1.1f);
            Npc("Vecina", "¡Hola, buenas!", "Moza", Along("W12", 0.7f)[0], P(1, 1), "Move", Along("W12", 0.7f), 1.2f);
            Npc("Vecino", "Hola, ¿qué tal?", "Abuelo", Along("W12", -0.7f)[4], P(-1, -1), "Move", Along("W12", -0.7f).Reverse().ToArray(), 1.05f);
            Npc("Vecina", "¡Hola!", "Abuela", Along("W06", 0.6f)[2], P(-1, 0), "Move", Along("W06", 0.6f), 1.0f);
            Npc("Vecino", "¡Hola!", "Joven", Along("W05", 0.6f)[1], P(1, 0.3f), "Move", Along("W05", 0.6f), 1.2f);
            Npc("Vecina", "¡Hola! Buenas tardes.", "Paisana", Along("W04", 1.2f)[0], P(1, 0.3f), "Move", Along("W04", 1.2f), 1.1f);
            Npc("Vecino", "¡Hola!", "Paisano", Along("P8a", 0.5f)[3], P(1, 0), "Move", Along("P8a", 0.5f), 1.15f);
            Npc("Vecina", "Hola.", "Chavala", Along("W13", 0.5f)[2], P(0, 1), "Move", Along("W13", 0.5f), 0.95f);
            Npc("Vecino", "¡Hola! Bonita tarde.", "Abuelo", Along("P8c", 0.5f)[5], P(-1, 0), "Move", Along("P8c", 0.5f), 1.1f);
            Npc("Vecina", "¡Hola!", "Vecina", Along("microB", 0.4f)[0], P(1, 1), "Move", Along("microB", 0.4f).Concat(Along("microA", -0.4f).Reverse()).ToArray(), 1.0f);
            // stationary
            Npc("Camarero", "¡Hola! ¿Qué te pongo?", "Camarero", P(94.9f, 49.4f), P(-1, 0), "Counter");
            Npc("Parroquiano", "¡Hola!", "Paisano", P(92.7f, 48.9f), P(1, 0), "LookAround");
            Npc("Vecina", "¡Hola! Aquí, de charla.", "Abuela", P(78.6f, 33.2f), P(1, 0.4f), "Talk");
            Npc("Vecino", "¡Hola!", "Abuelo", P(80.0f, 33.8f), P(-1, -0.4f), "Talk");
            Npc("Tendera", "¡Hola! Tengo manzanas buenas.", "Tendera", P(139.2f, 43.4f), P(0, 1), "FoldArms");
            Npc("Clienta", "¡Hola!", "Moza", P(139.6f, 45.2f), P(0, -1), "LookAround");
            Npc("Vecino", "Hola. Estoy leyendo los bandos.", "Vecino", P(157.0f, 64.4f), P(0, 1), "LookAround");
            Npc("Barquero", "¡Hola! La barca ya no sale.", "Paisano", P(-1.2f, -3.0f), P(-0.5f, -1), "FoldArms");
            Npc("Vecina", "¡Hola! Mira el agua.", "Paisana", P(114.0f, 117.0f), P(-1, 0), "LookAround");
            Npc("Chaval", "¡Hola!", "Chaval", P(150.5f, 111.6f), P(0, 1), "LookAround");
            Npc("Vecina", "¡Hola! Buenos días.", "Vecina", Along("P8e", 0.6f)[0], P(-1, 0), "Move", Along("P8e", 0.6f), 1.0f);
            // inside: patrons at the bar tables and a keeper behind every walk-in counter
            Npc("Parroquiano", "¡Hola! Siéntate, que aquí se está bien.", "Abuelo", P(88.95f, 48.0f), P(1, 0), "SitTalk");
            Npc("Parroquiana", "¡Hola! ¿Eres de fuera?", "Moza", P(90.85f, 51.2f), P(-1, 0), "Sit");
            var keepers = new Dictionary<string, (string role, string hello, string variant)>
            {
                { "F03", ("Tendera", "¡Hola! ¿Qué va a ser hoy?", "Moza") },
                { "F09", ("Tabernero", "¡Hola! Aquí el orujo es de casa.", "Vecino") },
                { "F12", ("Panadera", "¡Hola! El pan acaba de salir del horno.", "Vecina") },
                { "F13", ("Quesero", "¡Hola! Prueba el picón, que pica.", "Paisano") },
            };
            foreach (var (id, _) in Interiors.Shops)
            {
                var h = lay.Houses.FirstOrDefault(x => x.Id == id);
                if (h == null || !keepers.TryGetValue(id, out var k)) continue;
                var (at, face) = Interiors.Keeper(h);
                var go = Npc(k.role, k.hello, k.variant, at, face, "Counter");
                go.transform.position = W(at, h.FL);
            }

            // ---- auto capture shots (used by `PuenteBar.exe -autocapture <dir>`)
            var capGo = new GameObject("AutoCapture");
            capGo.transform.SetParent(root, false);
            var capture = capGo.AddComponent<AutoCapture>();
            var shots = new List<AutoCapture.Shot>();
            foreach (var c in DemoBuild.Cams)
                shots.Add(new AutoCapture.Shot { name = c.Name, pos = W(c.From, Hgt(c.From) + c.Eye), look = c.To });
            shots.Add(new AutoCapture.Shot { name = "7_interior_bar", pos = W(P(89.2f, 46.6f), 8.62f + 1.65f), look = new Vector3(94.5f, 9.4f, 50.5f) });
            shots.Add(new AutoCapture.Shot { name = "8_rendija_patio", pos = W(P(85.6f, 57.65f), Hgt(P(85.6f, 57.65f)) + 1.65f), look = new Vector3(93, 9.6f, 56.9f) });
            shots.Add(new AutoCapture.Shot { name = "9_vado_aguas_bajas", pos = new Vector3(96.5f, 7.2f, 106f), look = new Vector3(90.3f, 0.4f, 117.5f) });
            shots.Add(new AutoCapture.Shot { name = "10_vado_aguas_altas", pos = new Vector3(96.5f, 7.2f, 106f), look = new Vector3(90.3f, 0.4f, 117.5f), highWater = true });
            shots.Add(new AutoCapture.Shot { name = "11_desembarcadero", pos = W(P(5, 2), 1f + 1.7f), look = new Vector3(-6, 0.4f, -8) });
            shots.Add(new AutoCapture.Shot { name = "12_calle_comercial", pos = W(P(176, 64), Hgt(P(176, 64)) + 1.7f), look = new Vector3(151, 16, 68) });
            shots.Add(new AutoCapture.Shot { name = "13_aerea", pos = new Vector3(-20, 70, -110), look = new Vector3(100, 5, 30) });
            var bar = lay.Houses.First(x => x.Id == "F01");
            shots.Add(new AutoCapture.Shot { name = "7b_bar_hogar", pos = new Vector3(93.2f, bar.FL + 1.6f, 47.0f), look = new Vector3(90.1f, bar.FL + 0.9f, 52.6f) });
            foreach (var (id, kind) in Interiors.Shops)
            {
                var h = lay.Houses.FirstOrDefault(x => x.Id == id);
                if (h == null) continue;
                shots.Add(new AutoCapture.Shot { name = $"14_interior_{kind}", pos = W(h.A + h.Dir * (h.W * 0.5f) + h.In * 0.8f, h.FL + 1.6f), look = W(h.A + h.Dir * (h.W * 0.45f) + h.In * (h.D - 0.6f), h.FL + 1.0f) });
            }
            shots.Add(new AutoCapture.Shot { name = "15_orilla_sur_granjas", pos = new Vector3(52f, 13f, -58f), look = new Vector3(26f, 4.5f, -77f) });
            shots.Add(new AutoCapture.Shot { name = "16_traseras_huertas", pos = new Vector3(60, 34, 95), look = new Vector3(112, 8, 38) });
            capture.shots = shots.ToArray();
            // walk test through the chain: Orilla sur -> Puente Viejo -> S02 -> W12 -> Casco -> traza B -> Bar F01
            var wp = new List<(string, Vector2)> { ("orilla_sur", P(40.6f, -67.8f)) };
            foreach (var s in new[] { 36f, 28f, 20f, 12f, 4f }) wp.Add((s == 20f ? "*lomo_puente" : "puente_" + s, Bridge.At(s, 0)));
            wp.AddRange(new[] { ("*cabeza_S02", P(50.5f, -29f)), ("w12_a", P(56.3f, -17.5f)), ("w12_b", P(62.2f, -5.5f)), ("*w12_c", P(68.2f, 8.5f)),
                ("w12_d", P(74.2f, 22.5f)), ("*plazuela_casco", P(80.4f, 35.4f)), ("traza_b", P(84.0f, 41.4f)), ("traza_b2", P(89.0f, 43.1f)),
                ("puerta_bar", P(91.0f, 44.4f)), ("umbral", P(91.0f, 46.3f)), ("*dentro_bar", P(91.6f, 48.6f)) });
            capture.walk = wp.Select(w => new AutoCapture.Waypoint { name = w.Item1, pos = W(w.Item2, Hgt(w.Item2)) }).ToArray();
        }
    }
}
