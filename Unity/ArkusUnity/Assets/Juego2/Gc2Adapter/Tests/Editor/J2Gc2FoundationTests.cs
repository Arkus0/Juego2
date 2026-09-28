using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using GameCreator.Runtime.VisualScripting;
using Juego2.Arkus;
using Juego2.Foundation;
using Juego2.Foundation.Editor;
using Juego2.Gc2Adapter.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Juego2.Gc2Adapter.Tests
{
    /// <summary>GC2 Core 2.19.61 foundation seam: S06 presets, the 01A lints and the Arkus adapter, on the real project.</summary>
    public sealed class J2Gc2FoundationTests
    {
        InputActionAsset input;

        [SetUp]
        public void SetUp()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            input = AssetDatabase.LoadAssetAtPath<InputActionAsset>(J2FoundationBaseline.InputAsset);
            ArkusFacts.Bind(null);
        }

        [TearDown]
        public void TearDown() => ArkusFacts.Bind(null);

        static SerializedProperty P(UnityEngine.Object o, string path) =>
            new SerializedObject(o).FindProperty(path) ?? throw new AssertionException("missing serialized path " + path);

        static string RefType(UnityEngine.Object o, string path) => P(o, path).managedReferenceFullTypename.Split(' ').Last();

        [Test]
        public void S06Presets_MaterializeGc2PlayerNpcAndCamera_ReadingOnlyJuego2Input()
        {
            var player = J2Gc2Presets.MaterializeCharacter(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(J2FoundationBaseline.PlayerPreset), "j2.char.player", Vector3.zero);
            var npc = J2Gc2Presets.MaterializeCharacter(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(J2FoundationBaseline.NpcPreset), "j2.npc.test_a", new Vector3(3, 0, 0));
            var camera = J2Gc2Presets.MaterializePlayerCamera(AssetDatabase.LoadAssetAtPath<J2CameraPreset>(J2FoundationBaseline.CameraPreset));

            Assert.That(P(player, "m_IsPlayer").boolValue, Is.True);
            Assert.That(player.Motion.Height, Is.EqualTo(1.8f).Within(1e-4));
            Assert.That(player.Motion.Radius, Is.EqualTo(0.28f).Within(1e-4));
            Assert.That(player.Motion.LinearSpeed, Is.EqualTo(1.45f).Within(1e-4));
            Assert.That(RefType(player, "m_Kernel.m_Player.m_InputMove.m_Input"), Is.EqualTo(typeof(InputValueVector2InputAction).FullName));
            Assert.That(P(player, "m_Kernel.m_Player.m_InputMove.m_Input.m_Input.m_InputAsset").objectReferenceValue, Is.SameAs(input));
            Assert.That(P(player, "m_Kernel.m_Player.m_InputMove.m_Input.m_Input.m_Action").stringValue, Is.EqualTo("Move"));
            Assert.That(player.GetComponent<J2InputOwner>().actions, Is.SameAs(input));
            Assert.That(ArkusEntityBinding.KeyOf(player.gameObject), Is.EqualTo("j2.char.player"));

            Assert.That(P(npc, "m_IsPlayer").boolValue, Is.False);
            Assert.That(RefType(npc, "m_Kernel.m_Player.m_InputMove.m_Input"), Is.EqualTo(typeof(InputValueVector2None).FullName));
            Assert.That(RefType(npc, "m_Kernel.m_Driver"), Is.EqualTo(typeof(UnitDriverNavmesh).FullName));
            Assert.That(npc.GetComponent<J2InputOwner>(), Is.Null);

            Assert.That(camera.CompareTag("MainCamera"), Is.True);
            Assert.That(camera.GetComponent<MainCamera>(), Is.Not.Null);
            var shot = UnityEngine.Object.FindFirstObjectByType<ShotCamera>();
            Assert.That(RefType(shot, "m_ShotType"), Is.EqualTo(typeof(ShotTypeThirdPerson).FullName));
            Assert.That(P(shot, "m_ShotType.m_ThirdPerson.m_InputRotate.m_Input.m_Input.m_Action").stringValue, Is.EqualTo("Look"));
            Assert.That(P(shot, "m_ShotType.m_Zoom.m_InputZoom.m_Input.m_Input.m_Action").stringValue, Is.EqualTo("Zoom"));
            Assert.That(P(shot, "m_ShotType.m_ThirdPerson.m_InputRotate.m_Input.m_Input.m_InputAsset").objectReferenceValue, Is.SameAs(input));

            var findings = J2Gc2Lint.CheckScene(EditorSceneManager.GetActiveScene());
            Assert.That(findings, Is.Empty, string.Join("\n", findings));
        }

        /// <summary>The 01A identity audit on the materialized presets: no GC2 component carries a serialized identity.</summary>
        [Test]
        public void MaterializedPresets_CarryNoGc2Identity_OnlyJuego2Keys()
        {
            J2Gc2Presets.MaterializeCharacter(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(J2FoundationBaseline.PlayerPreset), "j2.char.player", Vector3.zero);
            J2Gc2Presets.MaterializeCharacter(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(J2FoundationBaseline.NpcPreset), "j2.npc.test_a", Vector3.right * 3);
            J2Gc2Presets.MaterializePlayerCamera(AssetDatabase.LoadAssetAtPath<J2CameraPreset>(J2FoundationBaseline.CameraPreset));
            var identity = new Regex(@"(guid|uniqueid|saveid|^m_id$|^m_saveuniqueid$)", RegexOptions.IgnoreCase);
            var hits = new List<string>();
            foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
                foreach (var c in root.GetComponentsInChildren<Component>(true))
                {
                    if (c == null || !c.GetType().FullName.StartsWith("GameCreator.", StringComparison.Ordinal)) continue;
                    var it = new SerializedObject(c).GetIterator();
                    while (it.Next(true))
                        if (identity.IsMatch(it.name) && (it.propertyType != SerializedPropertyType.String || !string.IsNullOrEmpty(it.stringValue)))
                            hits.Add(c.GetType().Name + "." + it.propertyPath);
                }
            Assert.That(hits, Is.Empty, string.Join("\n", hits));
            var keys = UnityEngine.Object.FindObjectsByType<ArkusEntityBinding>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select(b => b.entityKey).ToList();
            Assert.That(keys, Is.EquivalentTo(new[] { "j2.char.player", "j2.npc.test_a" }));
        }

        static GameObject Bound(string key)
        {
            var go = new GameObject(key);
            var b = go.AddComponent<ArkusEntityBinding>();
            b.entityKey = key;
            b.kind = "prop";
            return go;
        }

        static IEnumerable<string> Codes(params GameObject[] roots) =>
            J2Gc2Lint.Check(roots, AssetDatabase.LoadAssetAtPath<InputActionAsset>(J2FoundationBaseline.InputAsset)).Select(f => f.Code);

        static Trigger TriggerWith(GameObject go, params Instruction[] instructions)
        {
            var trigger = go.AddComponent<Trigger>();
            Trigger.Reconfigure(trigger, new EventCharacterOnInteract(), new InstructionList(instructions));
            return trigger;
        }

        static InstructionArkusRequestTransition Request(bool stop) =>
            new InstructionArkusRequestTransition(GetGameObjectSelf.Create(), GetGameObjectTarget.Create(), "door.open", stop);

        [Test]
        public void Lint_L2_RememberAndSceneLoadingAreRejected()
        {
            var a = Bound("j2.door.a");
            a.AddComponent<Remember>();
            var b = Bound("j2.door.b");
            TriggerWith(b, Request(true), new InstructionCommonSceneLoad());
            Assert.That(Codes(a), Has.Member("J2_GC2_L2_REMEMBER"));
            Assert.That(Codes(b), Has.Member("J2_GC2_L2_SCENE_INSTRUCTION"));
        }

        /// <summary>A Local Name Variables component authored through its serialized list (the Inspector's surface).</summary>
        static GameObject Variables(string name)
        {
            var go = new GameObject("vars_" + name);
            var component = go.AddComponent<LocalNameVariables>();
            var so = new SerializedObject(component);
            var list = so.FindProperty("m_Runtime.m_List.m_Source");
            list.arraySize = 1;
            list.GetArrayElementAtIndex(0).managedReferenceValue = new NameVariable(name, new ValueString("x"));
            so.ApplyModifiedPropertiesWithoutUndo();
            return go;
        }

        [Test]
        public void Lint_L3_VariableNamedAsJuego2FactIsRejected()
        {
            Assert.That(Codes(Variables("j2.door.a:state")), Has.Some.StartsWith("J2_GC2_L3_VARIABLE_NAMED_AS_FACT"));
            Assert.That(Codes(Variables("j2_char_player")), Has.Some.StartsWith("J2_GC2_L3_VARIABLE_NAMED_AS_FACT"));
            Assert.That(Codes(Variables("hover-count")), Is.Empty);
            Assert.That(Codes(Variables("j2x")), Is.Empty);
        }

        [Test]
        public void Lint_L4_AdapterEntryOnUnboundObjectIsRejected()
        {
            var unbound = new GameObject("barrel");
            TriggerWith(unbound, Request(true));
            Assert.That(Codes(unbound), Has.Member("J2_GC2_L4_ADAPTER_ON_UNBOUND_OBJECT"));
            var bound = Bound("j2.door.c");
            TriggerWith(bound, Request(true));
            Assert.That(Codes(bound), Is.Empty);
        }

        [Test]
        public void Lint_L5_StockMutationOfBoundObjectNeedsAStoppingArkusRequestFirst()
        {
            var ungated = Bound("j2.door.d");
            TriggerWith(ungated, new InstructionTransformChangePosition());
            var weak = Bound("j2.door.e");
            TriggerWith(weak, Request(false), new InstructionTransformChangePosition());
            var gated = Bound("j2.door.f");
            TriggerWith(gated, Request(true), new InstructionTransformChangePosition());
            Assert.That(Codes(ungated), Has.Some.StartsWith("J2_GC2_L5_UNGATED_MUTATION"));
            Assert.That(Codes(weak), Has.Some.StartsWith("J2_GC2_L5_UNGATED_MUTATION"));
            Assert.That(Codes(gated), Is.Empty);
        }

        [Test]
        public void Lint_L6_DeviceInputOrNonJuego2ActionsAreRejected()
        {
            var player = J2Gc2Presets.MaterializeCharacter(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(J2FoundationBaseline.PlayerPreset), "j2.char.player", Vector3.zero);
            var so = new SerializedObject(player);
            so.FindProperty("m_Kernel.m_Player.m_InputMove.m_Input").managedReferenceValue = new InputValueVector2KeyboardWASD();
            so.ApplyModifiedPropertiesWithoutUndo();
            Assert.That(Codes(player.gameObject), Has.Member("J2_GC2_L6_DEVICE_INPUT:InputValueVector2KeyboardWASD"));

            var npc = J2Gc2Presets.MaterializeCharacter(AssetDatabase.LoadAssetAtPath<J2CharacterPreset>(J2FoundationBaseline.NpcPreset), "j2.npc.test_b", Vector3.right * 3);
            var npcSo = new SerializedObject(npc);
            npcSo.FindProperty("m_Kernel.m_Player.m_InputMove.m_Input").managedReferenceValue = new InputValueVector2InputAction();
            npcSo.ApplyModifiedPropertiesWithoutUndo();
            Assert.That(Codes(npc.gameObject), Has.Member("J2_GC2_L6_NPC_INPUT_ENABLED"));
            Assert.That(Codes(npc.gameObject), Has.Member("J2_GC2_L6_INPUT_ACTION_NOT_JUEGO2"));
        }

        sealed class ProbeSave : IGameSave
        {
            public string SaveID => "probe";
            public bool IsShared => false;
            public Type SaveType => typeof(string);
            public object GetSaveData(bool includeNonSavables) => "";
            public LoadMode LoadMode => LoadMode.Greedy;
            public System.Threading.Tasks.Task OnLoad(object value) => System.Threading.Tasks.Task.CompletedTask;
        }

        [Test]
        public void Lint_L7_Juego2PersistenceThroughGc2IsRejected()
        {
            Assert.That(J2Gc2Lint.CheckSaveImplementations(), Is.Empty, "production Juego2 assemblies must not persist through GC2");
            var probe = J2Gc2Lint.CheckSaveImplementations(name => name == typeof(ProbeSave).Assembly.GetName().Name);
            Assert.That(probe.Select(f => f.Where), Has.Member(typeof(ProbeSave).FullName));
        }

        sealed class TableAuthority : IArkusFactAuthority
        {
            public readonly Dictionary<string, string> Facts = new Dictionary<string, string> { ["j2.door.g:state"] = "closed" };
            public event Action<string, string> FactChanged;
            public string GetFact(string key) => Facts.TryGetValue(key, out var v) ? v : null;
            public ArkusTransitionResult Request(ArkusTransitionRequest r)
            {
                if (r.Transition != "door.open" || Facts[r.Entity + ":state"] != "closed") return ArkusTransitionResult.Refused("PRECONDITION");
                Facts[r.Entity + ":state"] = "open";
                FactChanged?.Invoke(r.Entity + ":state", "open");
                return new ArkusTransitionResult(ArkusTransitionStatus.Accepted, "ACCEPTED", r.Entity + ":state", "open");
            }
        }

        [Test]
        public void AdapterSeam_ConditionAndPropertiesReadArkus_FailClosedWithoutAuthority()
        {
            var door = Bound("j2.door.g");
            var args = new Args(door);
            var condition = new ConditionArkusFact("j2.door.g:state", "closed");
            var fact = GetStringArkusFact.Create("j2.door.g:state");
            Assert.That(condition.Check(args), Is.False, "no authority bound: the fact reads as undeclared");
            Assert.That(fact.Get(args), Is.EqualTo(""));

            ArkusFacts.Bind(new TableAuthority());
            door.SetActive(false); door.SetActive(true); // registers the binding
            Assert.That(condition.Check(args), Is.True);
            Assert.That(fact.Get(args), Is.EqualTo("closed"));
            Assert.That(GetGameObjectArkusEntity.Create("j2.door.g").Get(args), Is.SameAs(door));
            Assert.That(GetGameObjectArkusEntity.Create("j2.door.unknown").Get(args), Is.Null);
        }
    }
}
