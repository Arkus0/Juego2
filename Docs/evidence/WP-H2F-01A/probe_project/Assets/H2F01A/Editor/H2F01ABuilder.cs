using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameCreator.Editor.Cameras;
using GameCreator.Editor.Characters;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using Juego2.ART;
using Juego2.H2F01.Editor;
using Juego2.H2F01A.Arkus;
using Juego2.H2F01A.Gc2Adapter;
using Unity.AI.Navigation;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01A.Editor
{
    /// <summary>
    /// Builds the disposable C02–C07 probe scenes. GC2 objects are created through GC2's own public editor menu
    /// entry points and configured through Unity serialization (the same surface the Inspector writes), never through
    /// private GC2 members. Every serialized path written is recorded as authoring-surface evidence.
    /// </summary>
    public static class H2F01ABuilder
    {
        const string Dir = "Assets/H2F01A/Play";
        const string Citizen = "Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx";
        const string Ual1 = "Assets/Arkus/ART/External/UAL/UAL1.fbx";
        const string Ual2 = "Assets/H2F01Inputs/Animation/UAL2.fbx";
        const string Gc2Characters = "Assets/Plugins/GameCreator/Packages/Core/Runtime/Characters/Assets";
        const string Props = "Assets/Arkus/ART/External/Props/Models";
        const string Medieval = "Assets/Arkus/ART/External/Medieval/Models";

        static readonly List<string> AuthoringPaths = new List<string>();

        // ------------------------------------------------------------------ entry points (one per play run)

        public static void C02() => Guard("c02", () => BuildC02(gc2Camera: false));
        public static void C02Gc2Camera() => Guard("c02g", () => BuildC02(gc2Camera: true));
        public static void C03() => Guard("c03", BuildC03);
        public static void C04() => Guard("c04", BuildC04);
        public static void C05() => Guard("c05", BuildC05);
        public static void C06() => Guard("c06", BuildC06);
        public static void C07() => Guard("c07", BuildC07);

        static void Guard(string scenario, Action build)
        {
            try
            {
                AuthoringPaths.Clear();
                Folder();
                build();
            }
            catch (Exception e)
            {
                Debug.LogError("H2F01A_BUILD_FAILED " + scenario + " " + e);
                EditorApplication.Exit(1);
            }
        }

        // ------------------------------------------------------------------ scenarios

        static void BuildC02(bool gc2Camera)
        {
            string scenario = gc2Camera ? "c02g" : "c02";
            var (start, goal) = ArtRoute();
            var player = MakeCharacter(true, "J2_Player", start, UalLocomotion(), 1.45f);
            Bind(player.gameObject, "j2.char.player", "character");
            var npcStart = NavPoint(start + new Vector3(1.6f, 0, 2.5f));
            var npc = MakeCharacter(false, "J2_NPC_A", npcStart, UalLocomotion(), 1.2f);
            Bind(npc.gameObject, "j2.npc.probe_a", "character");
            SetRef(npc, "m_Kernel.m_Driver", new UnitDriverNavmesh());
            // Both variants: GC2 body reading the Juego2 action map; only the camera differs.
            // c02 = H2F-01 camera (Cinemachine 3), c02g = GC2 Main Camera + Third Person shot (the S06 amendment).
            PlayerInputAction(player);
            Camera view = gc2Camera ? Gc2ThirdPersonCamera() : CinemachineRig(player.transform, 1.5f - 0.9f);
            var d = Driver(scenario, view, J2Input());
            d.routeStart = start; d.routeGoal = goal;
            d.npcGoal = NavPoint(npcStart + new Vector3(0, 0, 9f));
            Play(scenario);
        }

        static void BuildC03()
        {
            FlatScene("c03");
            var player = MakeCharacter(true, "J2_Player", new Vector3(0, 0, -3), UalLocomotion(), 1.45f);
            Bind(player.gameObject, "j2.char.player", "character");
            PlayerInputAction(player);
            var npc = MakeCharacter(false, "J2_NPC_A", new Vector3(4, 0, -3), UalLocomotion(), 1.2f);
            Bind(npc.gameObject, "j2.npc.probe_a", "character");
            var door = Interactable("j2.door.probe", "door", Medieval + "/Door_3_Flat.fbx", new Vector3(0, 0, 1.2f), "door.open");
            Hotspot(door, 2.5f);
            Interactable("j2.chair.probe", "chair", Props + "/Chair_1.fbx", new Vector3(4, 0, 1.5f), "seat.take");
            var crate = Prop(Props + "/Crate_Wooden.fbx", new Vector3(-3, 0, 1.2f), "PropWood");
            float crateTop = Bounds(crate).max.y;
            Interactable("j2.mug.probe", "mug", Props + "/Mug.fbx", new Vector3(-3, crateTop, 1.2f), "item.take");
            // negative: same GC2 Trigger + adapter instruction, but no Juego2 binding
            Interactable(null, "unbound", Props + "/Barrel.fbx", new Vector3(-1.5f, 0, -5.5f), "item.take");
            var view = StaticView(new Vector3(0, 6.5f, -10), new Vector3(0, 0.5f, 0));
            Driver("c03", view, J2Input());
            Play("c03");
        }

        static void BuildC04()
        {
            FlatScene("c04");
            var sounds = FootstepSounds();
            var a = MakeCharacter(false, "J2_NPC_UAL", new Vector3(-1, 0, -5), UalLocomotion(), 1.2f);
            Bind(a.gameObject, "j2.npc.presenter_ual", "character");
            ConfigurePresentation(a, sounds, new FootstepDetectorFulcrum(), ragdoll: true);
            var b = MakeCharacter(false, "J2_NPC_GC2LOCO", new Vector3(1, 0, -5),
                AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Gc2Characters + "/Controllers/CompleteLocomotion.controller"), 1.2f);
            Bind(b.gameObject, "j2.npc.presenter_gc2loco", "character");
            ConfigurePresentation(b, sounds, new FootstepDetectorAnimationCurves(), ragdoll: false);
            var look = new GameObject("J2_LookTarget");
            look.transform.position = new Vector3(-3.5f, 1.6f, -2.5f); // ~45 deg off the NPC heading (GC2 RigLookTo max angle is 90)
            var view = StaticView(new Vector3(5.5f, 3.2f, -8.5f), new Vector3(0, 0.9f, -2));
            var d = Driver("c04", view, null);
            d.gestureClip = H2F01RenderSpike.UalClip("Interact");
            d.stateClip = H2F01RenderSpike.UalClip("Sitting_Idle_Loop");
            Play("c04");
        }

        static void BuildC05()
        {
            FlatScene("c05");
            var door = Prop(Medieval + "/Door_3_Flat.fbx", new Vector3(-1.5f, 0, 2), "WoodDark");
            Bind(door, "j2.door.c05", "door");
            var lampGo = new GameObject("J2_Lamp");
            lampGo.transform.position = new Vector3(1.5f, 2.2f, 1.5f);
            var lamp = lampGo.AddComponent<Light>();
            lamp.type = LightType.Point; lamp.intensity = 0f; lamp.range = 6f; lamp.color = new Color(1f, 0.8f, 0.55f);
            Bind(lampGo, "j2.lamp.c05", "lamp");
            var rule = new GameObject("J2_Rule_DoorOpensLamp");
            Bind(rule, "j2.rule.c05_door_lamp", "rule");
            var trigger = rule.AddComponent<Trigger>();
            // Event + Instruction list through GC2's public Trigger.Reconfigure; stock items configured through serialization.
            Trigger.Reconfigure(trigger, new EventOnArkusFactChanged("j2.door.c05:state"), new InstructionList(
                new InstructionLogicCheckConditions(),
                new InstructionArkusRequestTransition(GetGameObjectArkusEntity.Create("j2.lamp.c05"), GetGameObjectSelf.Create(), "lamp.on"),
                new InstructionLightChangeIntensity(),
                new InstructionCommonDebugText("placeholder")));
            Record("Trigger.Reconfigure(EventOnArkusFactChanged, InstructionList[CheckConditions, ArkusRequest, LightIntensity, DebugText])");
            var so = new SerializedObject(trigger);
            SetRef(so, "m_Instructions.m_Instructions.Array.data[0].m_Conditions.m_Conditions", null, arraySize: 1);
            SetRef(so, "m_Instructions.m_Instructions.Array.data[0].m_Conditions.m_Conditions.Array.data[0]", new ConditionArkusFact("j2.door.c05:state", "open"));
            SetRef(so, "m_Instructions.m_Instructions.Array.data[2].m_Light.m_Property", new GetGameObjectArkusEntity("j2.lamp.c05"));
            SetRef(so, "m_Instructions.m_Instructions.Array.data[3].m_Message.m_Property", new GetStringArkusFact("j2.lamp.c05:light"));
            EditorUtility.SetDirty(trigger);
            var view = StaticView(new Vector3(0, 3.5f, -6), new Vector3(0, 1.2f, 1.5f));
            Driver("c05", view, null);
            Play("c05");
        }

        static void BuildC06()
        {
            // GC2's save system is configured to the Juego2-owned storage backend (public TDataStorage extension)
            var settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Plugins/GameCreator/Data/Resources/Settings/core.general.asset");
            if (settings == null) throw new Exception("GC2 core.general settings asset not generated yet (run C01 windowed first)");
            var so = new SerializedObject(settings);
            SetRef(so, "m_Repository.m_Save.m_Storage", new Juego2WorkspaceStorage());
            AssetDatabase.SaveAssets();
            FlatScene("c06");
            var marker = new GameObject("J2_SceneInstanceMarker");
            Bind(marker, "j2.system.c06_marker", "marker");
            var view = StaticView(new Vector3(0, 4, -8), Vector3.zero);
            Driver("c06", view, null);
            var path = $"{Dir}/H2F01A_c06.unity";
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), path);
            // GC2 Load reloads the saved scene by name, so the probe scene must be in the build list
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(path, true) };
            Record("EditorBuildSettings.scenes += H2F01A_c06 (GC2 load reloads the saved scene by name)");
            Play("c06", alreadySaved: true);
        }

        static void BuildC07()
        {
            var (start, goal) = ArtRoute();
            var player = MakeCharacter(true, "J2_Player", start, UalLocomotion(), 1.45f);
            Bind(player.gameObject, "j2.char.player", "character");
            PlayerInputAction(player);
            var sounds = FootstepSounds();
            ConfigurePresentation(player, sounds, new FootstepDetectorFulcrum(), ragdoll: false);
            // NPC waits beside the bridge approach, off the player's corridor, then walks to its bench seat
            var npcStart = NavPoint(start + new Vector3(2.2f, 0, 4f));
            var npc = MakeCharacter(false, "J2_NPC_A", npcStart, UalLocomotion(), 1.2f);
            Bind(npc.gameObject, "j2.npc.probe_a", "character");
            SetRef(npc, "m_Kernel.m_Driver", new UnitDriverNavmesh());
            ConfigurePresentation(npc, sounds, new FootstepDetectorFulcrum(), ragdoll: true);
            var seat = NavPoint(npcStart + new Vector3(0.4f, 0, 3.5f));
            Interactable("j2.chair.c07", "chair", Props + "/Chair_1.fbx", seat + new Vector3(0.6f, 0, 0.3f), "seat.take");
            // mug beside the route end (inside the bar), the player's interaction
            Interactable("j2.mug.c07", "mug", Props + "/Mug.fbx", NavPoint(goal) + new Vector3(0.9f, 0.9f, 0.3f), "item.take");
            // C05 rule on the bar door state: Arkus fact -> GC2 condition -> Arkus request -> stock GC2 light effect
            var lampGo = new GameObject("J2_Lamp");
            lampGo.transform.position = NavPoint(goal) + new Vector3(0, 2.4f, 0);
            var lamp = lampGo.AddComponent<Light>();
            lamp.type = LightType.Point; lamp.intensity = 0f; lamp.range = 7f; lamp.color = new Color(1f, 0.8f, 0.55f);
            Bind(lampGo, "j2.lamp.c07", "lamp");
            var doorFacts = new GameObject("J2_BarDoor_Semantic");
            doorFacts.transform.position = NavPoint(goal) + new Vector3(0, 0, -4f);
            Bind(doorFacts, "j2.door.c07", "door");
            var rule = new GameObject("J2_Rule_DoorOpensLamp");
            Bind(rule, "j2.rule.c07_door_lamp", "rule");
            var trigger = rule.AddComponent<Trigger>();
            Trigger.Reconfigure(trigger, new EventOnArkusFactChanged("j2.door.c07:state"), new InstructionList(
                new InstructionLogicCheckConditions(),
                new InstructionArkusRequestTransition(GetGameObjectArkusEntity.Create("j2.lamp.c07"), GetGameObjectSelf.Create(), "lamp.on"),
                new InstructionLightChangeIntensity()));
            var tso = new SerializedObject(trigger);
            SetRef(tso, "m_Instructions.m_Instructions.Array.data[0].m_Conditions.m_Conditions", null, arraySize: 1);
            SetRef(tso, "m_Instructions.m_Instructions.Array.data[0].m_Conditions.m_Conditions.Array.data[0]", new ConditionArkusFact("j2.door.c07:state", "open"));
            SetRef(tso, "m_Instructions.m_Instructions.Array.data[2].m_Light.m_Property", new GetGameObjectArkusEntity("j2.lamp.c07"));
            var settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/Plugins/GameCreator/Data/Resources/Settings/core.general.asset");
            SetRef(new SerializedObject(settings), "m_Repository.m_Save.m_Storage", new Juego2WorkspaceStorage());
            var view = Gc2ThirdPersonCamera(); // amended S06 realization: GC2 body + GC2 camera + Juego2 input
            var d = Driver("c07", view, J2Input());
            d.routeStart = start; d.routeGoal = goal; d.npcGoal = seat;
            d.gestureClip = H2F01RenderSpike.UalClip("Interact");
            Play("c07");
        }

        // ------------------------------------------------------------------ characters

        /// <summary>
        /// GC2's own editor entry points (GameObject/Game Creator/Characters/Player|Character), then the ART citizen swapped
        /// in through the public Character.ChangeModel. ChangeModel's runtime-only Destroy() leaves the default mannequin in
        /// edit mode (H2F-01 finding), so the previous animator object is removed first.
        /// </summary>
        static Character MakeCharacter(bool player, string name, Vector3 feet, RuntimeAnimatorController rtc, float speed)
        {
            if (player) CharacterEditor.CreatePlayer(null); else CharacterEditor.CreateCharacter(null);
            var go = Selection.activeGameObject;
            if (go == null || go.GetComponent<Character>() == null) throw new Exception("GC2 CreatePlayer/CreateCharacter produced no Character");
            go.name = name;
            var c = go.GetComponent<Character>();
            c.Motion.Height = 1.8f;  // project agent (H2F-01): height 1.8, radius 0.28
            c.Motion.Radius = 0.28f;
            c.Motion.LinearSpeed = speed;
            go.transform.position = feet + Vector3.up * (c.Motion.Height * 0.5f + 0.02f);
            if (c.Animim.Animator != null) UnityEngine.Object.DestroyImmediate(c.Animim.Animator.gameObject);
            c.ChangeModel(CitizenPrefab(), new Character.ChangeOptions { controller = rtc, offset = Vector3.zero });
            Record($"CharacterEditor.Create{(player ? "Player" : "Character")} + Character.ChangeModel(J2_Citizen) [{name}]");
            // NPC preset rule: the dormant player unit reads no device, so no loose GC2 input action stays enabled
            if (!player) SetRef(new SerializedObject(c), "m_Kernel.m_Player.m_InputMove.m_Input", new InputValueVector2None());
            foreach (var t in go.GetComponentsInChildren<Transform>(true)) t.gameObject.layer = 2; // camera metrics ignore the body
            EditorUtility.SetDirty(c);
            return c;
        }

        /// <summary>The GC2 player's move input reads the Juego2 Input System action map (GC2 public InputAction value).</summary>
        static void PlayerInputAction(Character player) => BindAction(new SerializedObject(player), "m_Kernel.m_Player.m_InputMove", "Move");

        static void ConfigurePresentation(Character c, MaterialSoundsAsset sounds, FootstepDetectorBase detector, bool ragdoll)
        {
            var so = new SerializedObject(c);
            SetObj(so, "m_Footsteps.m_FootstepSounds.m_SoundsAsset", sounds);
            SetRef(so, "m_Footsteps.m_FootstepDetector", detector);
            if (!ragdoll) return;
            SetRef(so, "m_Ragdoll.m_Ragdoll", new RagdollDefault());
            SetObj(so, "m_Ragdoll.m_Ragdoll.m_BoneRack.m_Skeleton", AssetDatabase.LoadAssetAtPath<Skeleton>(Gc2Characters + "/3D/Skeleton.asset"));
            SetObj(so, "m_Ragdoll.m_Ragdoll.m_RecoverFaceDown", AssetDatabase.LoadAssetAtPath<AnimationClip>(Gc2Characters + "/3D/Animations/Actions/Human@Action_StandFaceDown.anim"));
            SetObj(so, "m_Ragdoll.m_Ragdoll.m_RecoverFaceUp", AssetDatabase.LoadAssetAtPath<AnimationClip>(Gc2Characters + "/3D/Animations/Actions/Human@Action_StandFaceUp.anim"));
        }

        static void SetRef(Character c, string path, object value)
        {
            var so = new SerializedObject(c);
            SetRef(so, path, value);
        }

        static void Bind(GameObject go, string key, string kind)
        {
            var b = go.AddComponent<ArkusEntityBinding>();
            b.entityKey = key; b.kind = kind;
        }

        /// <summary>A prop with a GC2 Trigger (On Interact) that submits one Arkus transition, then logs the Arkus fact.</summary>
        static GameObject Interactable(string key, string kind, string model, Vector3 at, string transition)
        {
            var holder = Prop(model, at, kind == "door" ? "WoodDark" : "PropWood");
            holder.name = "J2_" + (key ?? "unbound_" + kind);
            if (key != null) Bind(holder, key, kind);
            var box = holder.AddComponent<BoxCollider>();
            var b = Bounds(holder);
            box.center = holder.transform.InverseTransformPoint(b.center); box.size = b.size;
            var trigger = holder.AddComponent<Trigger>();
            string fact = transition.StartsWith("door") ? "state" : transition.StartsWith("seat") ? "occupant" : "holder";
            Trigger.Reconfigure(trigger, new EventCharacterOnInteract(), new InstructionList(
                new InstructionArkusRequestTransition(GetGameObjectSelf.Create(), GetGameObjectTarget.Create(), transition),
                new InstructionCommonDebugText("placeholder")));
            var so = new SerializedObject(trigger);
            SetRef(so, "m_Instructions.m_Instructions.Array.data[1].m_Message.m_Property",
                new GetStringArkusFact(key == null ? "<unbound>" : ArkusProbeAuthority.Key(key, fact)));
            Record($"Trigger.Reconfigure(EventCharacterOnInteract, [ArkusRequest '{transition}', DebugText(Arkus fact)]) [{holder.name}]");
            EditorUtility.SetDirty(trigger);
            return holder;
        }

        static void Hotspot(GameObject target, float radius)
        {
            var hotspot = target.AddComponent<Hotspot>();
            var so = new SerializedObject(hotspot);
            SetRef(so, "m_Radius.m_Property", new GetDecimalDecimal(radius));
            Record($"Hotspot(InRadius {radius} m, target=Player) [{target.name}]");
        }

        static GameObject Prop(string model, Vector3 at, string materialOverride)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(model) ?? throw new Exception("Missing model " + model);
            var holder = new GameObject(Path.GetFileNameWithoutExtension(model));
            holder.transform.position = at;
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(asset, holder.transform);
            inst.transform.localRotation = asset.transform.localRotation; inst.transform.localScale = asset.transform.localScale;
            H2F01HumanSpike.RemapConverted(inst, materialOverride);
            return holder;
        }

        static Bounds Bounds(GameObject go)
        {
            var rs = go.GetComponentsInChildren<Renderer>();
            if (rs.Length == 0) return new Bounds(go.transform.position, Vector3.one * 0.3f);
            var b = rs[0].bounds;
            foreach (var r in rs) b.Encapsulate(r.bounds);
            return b;
        }

        // ------------------------------------------------------------------ assets

        static void Folder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/H2F01A")) AssetDatabase.CreateFolder("Assets", "H2F01A");
            if (!AssetDatabase.IsValidFolder(Dir)) AssetDatabase.CreateFolder("Assets/H2F01A", "Play");
        }

        /// <summary>The ART clothed citizen with URP ART materials, saved once as a model prefab GC2 can instantiate.</summary>
        static GameObject CitizenPrefab()
        {
            var path = $"{Dir}/J2_Citizen.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Citizen));
            H2F01HumanSpike.RemapConverted(inst, null);
            var prefab = PrefabUtility.SaveAsPrefabAsset(inst, path);
            UnityEngine.Object.DestroyImmediate(inst);
            return prefab;
        }

        /// <summary>Juego2's selected UAL1 idle/walk, driven by the normalized 'Speed' parameter GC2's Kinematic Animim writes.</summary>
        static AnimatorController UalLocomotion()
        {
            var path = $"{Dir}/J2_UAL_Locomotion.controller";
            var existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (existing != null) return existing;
            var c = AnimatorController.CreateAnimatorControllerAtPath(path);
            c.AddParameter("Speed", AnimatorControllerParameterType.Float);
            c.CreateBlendTreeInController("Locomotion", out var tree, 0);
            tree.blendParameter = "Speed";
            tree.AddChild(H2F01RenderSpike.UalClip("Idle_Loop"), 0f);
            tree.AddChild(H2F01RenderSpike.UalClip("Walk_Loop"), 1f);
            var layers = c.layers;
            layers[0].iKPass = true; // GC2 look-at/feet rigs run from OnAnimatorIK
            c.layers = layers;
            return c;
        }

        const string J2InputJson = @"{
    ""name"": ""J2_Input"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""actions"": [
                { ""name"": ""Move"", ""type"": ""Value"", ""expectedControlType"": ""Vector2"" },
                { ""name"": ""Look"", ""type"": ""Value"", ""expectedControlType"": ""Vector2"" },
                { ""name"": ""Zoom"", ""type"": ""Value"", ""expectedControlType"": ""Vector2"" },
                { ""name"": ""Interact"", ""type"": ""Button"" }
            ],
            ""bindings"": [
                { ""path"": ""<Gamepad>/leftStick"", ""action"": ""Move"" },
                { ""name"": ""WASD"", ""path"": ""2DVector"", ""action"": ""Move"", ""isComposite"": true },
                { ""name"": ""up"", ""path"": ""<Keyboard>/w"", ""action"": ""Move"", ""isPartOfComposite"": true },
                { ""name"": ""down"", ""path"": ""<Keyboard>/s"", ""action"": ""Move"", ""isPartOfComposite"": true },
                { ""name"": ""left"", ""path"": ""<Keyboard>/a"", ""action"": ""Move"", ""isPartOfComposite"": true },
                { ""name"": ""right"", ""path"": ""<Keyboard>/d"", ""action"": ""Move"", ""isPartOfComposite"": true },
                { ""path"": ""<Gamepad>/rightStick"", ""action"": ""Look"" },
                { ""path"": ""<Mouse>/delta"", ""action"": ""Look"" },
                { ""path"": ""<Mouse>/scroll"", ""action"": ""Zoom"" },
                { ""path"": ""<Gamepad>/buttonSouth"", ""action"": ""Interact"" },
                { ""path"": ""<Keyboard>/e"", ""action"": ""Interact"" }
            ]
        }
    ],
    ""controlSchemes"": []
}
";

        /// <summary>The Juego2-owned action map: the only input source for the GC2 body and the GC2 camera.</summary>
        static InputActionAsset J2Input()
        {
            var path = $"{Dir}/J2_Input.inputactions";
            if (!File.Exists(path) || File.ReadAllText(path) != J2InputJson)
            {
                File.WriteAllText(path, J2InputJson);
                AssetDatabase.ImportAsset(path);
            }
            return AssetDatabase.LoadAssetAtPath<InputActionAsset>(path) ?? throw new Exception("J2_Input import failed");
        }

        static void BindAction(SerializedObject so, string inputProperty, string action)
        {
            SetRef(so, inputProperty + ".m_Input", new InputValueVector2InputAction());
            SetObj(so, inputProperty + ".m_Input.m_Input.m_InputAsset", J2Input());
            SetStr(so, inputProperty + ".m_Input.m_Input.m_ActionMap", "Player");
            SetStr(so, inputProperty + ".m_Input.m_Input.m_Action", action);
        }

        /// <summary>Footstep sounds keyed on the ART URP materials' _BaseMap: cobble and dark wood get distinct clips.</summary>
        static MaterialSoundsAsset FootstepSounds()
        {
            var path = $"{Dir}/J2_Footsteps.asset";
            var existing = AssetDatabase.LoadAssetAtPath<MaterialSoundsAsset>(path);
            if (existing != null) return existing;
            var cobble = Tone("j2_step_cobble", 180f);
            var wood = Tone("j2_step_wood", 420f);
            var asset = ScriptableObject.CreateInstance<MaterialSoundsAsset>();
            AssetDatabase.CreateAsset(asset, path);
            var so = new SerializedObject(asset);
            SetStr(so, "m_TextureName", "_BaseMap");
            SetRef(so, "m_MaterialSounds.m_MaterialSounds", null, arraySize: 2);
            int i = 0;
            foreach (var (mat, clip) in new[] { ("Cobble", cobble), ("WoodDark", wood) })
            {
                var tex = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Arkus/ART/Materials/{mat}.mat").GetTexture("_BaseMap");
                if (tex == null) throw new Exception($"ART material {mat} has no _BaseMap texture to key footsteps on");
                string e = $"m_MaterialSounds.m_MaterialSounds.Array.data[{i}]";
                SetRef(so, e, MaterialSoundTexture.Create());
                SetStr(so, e + ".m_Name", mat);
                SetObj(so, e + ".m_Texture", tex);
                SetFloat(so, e + ".m_Volume", 0.8f);
                var v = P(so, e + ".m_Variations"); v.arraySize = 1; so.ApplyModifiedPropertiesWithoutUndo(); so.Update();
                SetObj(so, e + ".m_Variations.Array.data[0]", clip);
                i++;
            }
            AssetDatabase.SaveAssets();
            return asset;
        }

        /// <summary>A short synthetic click written as a WAV in the disposable workspace (no external audio content).</summary>
        static AudioClip Tone(string name, float hz)
        {
            var path = $"{Dir}/{name}.wav";
            if (!File.Exists(path))
            {
                const int rate = 22050; int n = rate / 10;
                using var w = new BinaryWriter(File.Create(path));
                w.Write("RIFF".ToCharArray()); w.Write(36 + n * 2); w.Write("WAVEfmt ".ToCharArray());
                w.Write(16); w.Write((short)1); w.Write((short)1); w.Write(rate); w.Write(rate * 2); w.Write((short)2); w.Write((short)16);
                w.Write("data".ToCharArray()); w.Write(n * 2);
                for (int k = 0; k < n; k++) w.Write((short)(Mathf.Sin(2 * Mathf.PI * hz * k / rate) * Mathf.Exp(-k / (float)rate * 40f) * 12000));
            }
            AssetDatabase.ImportAsset(path);
            return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }

        // ------------------------------------------------------------------ scenes and cameras

        static (Vector3 start, Vector3 goal) ArtRoute()
        {
            H2F01WorldSpike.ConfigureAgent();
            OpenArtScene();
            H2F01RenderSpike.BaselineLook();
            var inspector = UnityEngine.Object.FindFirstObjectByType<Art01WalkInspector>();
            if (inspector != null) UnityEngine.Object.DestroyImmediate(inspector.gameObject);
            foreach (var cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)) UnityEngine.Object.DestroyImmediate(cam.gameObject);
            var navHost = new GameObject("H2F01A_NAV");
            var surface = navHost.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All; surface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
            surface.overrideVoxelSize = true; surface.voxelSize = 0.07f;
            surface.BuildNavMesh();
            var navPath = $"{Dir}/ArtRoute_NavMesh.asset";
            AssetDatabase.DeleteAsset(navPath);
            AssetDatabase.CreateAsset(surface.navMeshData, navPath);
            float startY = Physics.Raycast(new Vector3(0, 40, -30), Vector3.down, out var hit, 80) ? hit.point.y : 0;
            float barY = Physics.Raycast(new Vector3(0, 40, 22), Vector3.down, out var h22, 80) ? h22.point.y : 0;
            // identical start/goal to H2F-01 S06 (bridge z=-30 -> Bar F01 interior)
            return (new Vector3(0, startY, -30), new Vector3(-1f, barY + 0.4f, 29f));
        }

        static Vector3 NavPoint(Vector3 near)
        {
            return UnityEngine.AI.NavMesh.SamplePosition(near, out var hit, 3f, UnityEngine.AI.NavMesh.AllAreas) ? hit.position : near;
        }

        static void FlatScene(string scenario)
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var sun = UnityEngine.Object.FindFirstObjectByType<Light>();
            sun.shadows = LightShadows.Soft; sun.transform.rotation = Quaternion.Euler(40, -30, 0);
            H2F01RenderSpike.BaselineLook();
            foreach (var cam in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)) UnityEngine.Object.DestroyImmediate(cam.gameObject);
            Patch("Floor_Cobble", new Vector3(0, -0.1f, -3.5f), new Vector3(12, 0.2f, 7), "Cobble");
            Patch("Floor_WoodDark", new Vector3(0, -0.1f, 3.5f), new Vector3(12, 0.2f, 7), "WoodDark");
        }

        static void Patch(string name, Vector3 center, Vector3 size, string material)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = name; g.transform.position = center; g.transform.localScale = size;
            g.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Assets/Arkus/ART/Materials/{material}.mat");
        }

        static Camera StaticView(Vector3 at, Vector3 lookAt)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f; cam.fieldOfView = 55;
            go.transform.position = at; go.transform.LookAt(lookAt);
            cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            go.AddComponent<AudioListener>();
            return cam;
        }

        /// <summary>The accepted H2F-01 camera (Cinemachine 3 ThirdPersonFollow, same parameters as S06) over any body.</summary>
        static Camera CinemachineRig(Transform body, float headAboveOrigin)
        {
            var rootGo = new GameObject("J2_CameraRoot");
            var rig = rootGo.AddComponent<J2CameraRig>();
            rig.body = body; rig.headAboveBodyOrigin = headAboveOrigin;
            rootGo.transform.position = body.position + Vector3.up * headAboveOrigin;
            rootGo.transform.rotation = Quaternion.Euler(10f, body.eulerAngles.y, 0);
            var main = new GameObject("Main Camera").AddComponent<Camera>();
            main.tag = "MainCamera"; main.nearClipPlane = 0.05f; main.fieldOfView = 55;
            main.gameObject.AddComponent<CinemachineBrain>();
            main.gameObject.AddComponent<AudioListener>();
            main.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            var vcamGo = new GameObject("CM_ThirdPerson");
            var vcam = vcamGo.AddComponent<CinemachineCamera>();
            vcam.Follow = rootGo.transform;
            vcam.Lens.FieldOfView = 55; vcam.Lens.NearClipPlane = 0.05f;
            var follow = vcamGo.AddComponent<CinemachineThirdPersonFollow>();
            follow.CameraDistance = 3.0f; follow.ShoulderOffset = new Vector3(0.35f, 0.05f, 0); follow.VerticalArmLength = 0.2f; follow.CameraSide = 0.6f;
            var avoid = follow.AvoidObstacles;
            avoid.Enabled = true; avoid.CollisionFilter = ~(1 << 2); avoid.CameraRadius = 0.2f; avoid.DampingIntoCollision = 0.1f; avoid.DampingFromCollision = 0.5f;
            follow.AvoidObstacles = avoid;
            Record("Cinemachine 3 CinemachineCamera + ThirdPersonFollow(S06 params) following J2CameraRig over the GC2 body");
            return main;
        }

        /// <summary>GC2's own Main Camera + Third Person shot with the H2F-01 S06 parameters (the amended S06 camera).</summary>
        static Camera Gc2ThirdPersonCamera()
        {
            var main = new GameObject("Main Camera");
            main.tag = "MainCamera";
            var camera = main.AddComponent<Camera>();
            camera.nearClipPlane = 0.05f; camera.fieldOfView = 55;
            main.AddComponent<MainCamera>();
            main.AddComponent<AudioListener>();
            camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            ShotCameraEditor.CreateElement(null);
            var shot = UnityEngine.Object.FindFirstObjectByType<ShotCamera>();
            var so = new SerializedObject(shot);
            SetRef(so, "m_ShotType", new ShotTypeThirdPerson());
            SetBool(so, "m_ShotType.m_ThirdPerson.m_Align.m_AutoAlign", true);
            SetFloat(so, "m_ShotType.m_ThirdPerson.m_Align.m_Delay", 0.5f);
            SetFloat(so, "m_ShotType.m_ThirdPerson.m_Align.m_SmoothTime", 1.0f);
            SetRef(so, "m_ShotType.m_ThirdPerson.m_Radius.m_Property", new GetDecimalDecimal(3.0f));
            // camera orbit and zoom read the Juego2 action map too (GC2 defaults read devices directly)
            BindAction(so, "m_ShotType.m_ThirdPerson.m_InputRotate", "Look");
            BindAction(so, "m_ShotType.m_Zoom.m_InputZoom", "Zoom");
            Record("GC2 MainCamera + ShotCamera(Third Person: radius 3, auto-align delay 0.5, smooth 1.0) targeting the GC2 Player");
            return camera;
        }

        static H2F01AProbeDriver Driver(string scenario, Camera view, InputActionAsset input)
        {
            var go = new GameObject("H2F01A_PROBE_DRIVER");
            var d = go.AddComponent<H2F01AProbeDriver>();
            d.scenario = scenario;
            d.outDir = Path.GetDirectoryName(Out(scenario + "/.keep"));
            d.view = view;
            d.input = input;
            return d;
        }

        static void Play(string scenario, bool alreadySaved = false)
        {
            var path = $"{Dir}/H2F01A_{scenario}.unity";
            if (!alreadySaved) EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), path);
            AssetDatabase.SaveAssets();
            File.WriteAllLines(Out($"{scenario}/authoring_paths.txt"), AuthoringPaths);
            var audit = AuditShaders(); // no silent unsupported/magenta material in any probe scene
            WriteJson($"{scenario}/shader_audit.json", JsonUtility.ToJson(audit, true));
            Debug.Log($"H2F01A_SHADER_AUDIT {scenario} green={audit.green} materials={audit.materials} failures={string.Join(";", audit.failures)}");
            EditorSettings.enterPlayModeOptionsEnabled = false; // full domain reload: GC2 statics stay honest
            Debug.Log("H2F01A_PLAY_ENTER " + path);
            EditorApplication.EnterPlaymode();
        }

        // ------------------------------------------------------------------ serialization helpers (the Inspector's surface)

        static void Record(string line) => AuthoringPaths.Add(line);

        static SerializedProperty P(SerializedObject so, string path) =>
            so.FindProperty(path) ?? throw new Exception($"H2F01A_AUTHORING_PATH_MISSING {so.targetObject.GetType().Name}:{path}");

        static void Apply(SerializedObject so, string path, string what)
        {
            so.ApplyModifiedPropertiesWithoutUndo(); so.Update();
            Record($"{so.targetObject.GetType().Name}.{path} = {what}");
        }

        static void SetRef(SerializedObject so, string path, object value, int arraySize = -1)
        {
            var p = P(so, path);
            if (arraySize >= 0) { p.arraySize = arraySize; Apply(so, path, $"arraySize {arraySize}"); return; }
            p.managedReferenceValue = value;
            Apply(so, path, value?.GetType().Name ?? "null");
        }

        static void SetObj(SerializedObject so, string path, UnityEngine.Object value)
        {
            P(so, path).objectReferenceValue = value;
            Apply(so, path, value != null ? value.name : "null");
        }

        static void SetStr(SerializedObject so, string path, string value) { P(so, path).stringValue = value; Apply(so, path, "'" + value + "'"); }
        static void SetFloat(SerializedObject so, string path, float value) { P(so, path).floatValue = value; Apply(so, path, value.ToString("0.###")); }
        static void SetBool(SerializedObject so, string path, bool value) { P(so, path).boolValue = value; Apply(so, path, value.ToString()); }
    }
}
