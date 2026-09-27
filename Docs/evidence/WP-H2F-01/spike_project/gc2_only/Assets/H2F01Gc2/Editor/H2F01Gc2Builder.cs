using System.Reflection;
using GameCreator.Editor.Cameras;
using GameCreator.Editor.Characters;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Characters;
using Juego2.ART;
using Juego2.H2F01.Editor;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Juego2.H2F01.Gc2
{
    /// <summary>
    /// Owner-supplied candidate (Game Creator 2 Core) on the same S06 route/metrics as the minimal composition.
    /// Only GC2's Character (player locomotion) + Main Camera/Third Person shot are used; GC2 Variables, SaveLoad,
    /// Remember and Triggers are deliberately absent. Lives only in the isolated GC2 workspace.
    /// </summary>
    public static class H2F01Gc2Builder
    {
        public static void S06Gc2()
        {
            try { Build(); }
            catch (System.Exception e) { Debug.LogError("H2F01_GC2_BUILD_FAILED " + e); EditorApplication.Exit(1); }
        }

        static void Build()
        {
            H2F01PlayBuilder.Folder();
            H2F01WorldSpike.ConfigureAgent();
            H2F01Common.OpenArtScene();
            H2F01RenderSpike.BaselineLook();
            var inspector = Object.FindFirstObjectByType<Art01WalkInspector>();
            if (inspector != null) Object.DestroyImmediate(inspector.gameObject);
            foreach (var cam0 in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)) Object.DestroyImmediate(cam0.gameObject);
            var navHost = new GameObject("H2F01_NAV");
            var surface = navHost.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All; surface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
            surface.overrideVoxelSize = true; surface.voxelSize = 0.07f;
            surface.BuildNavMesh();
            AssetDatabase.DeleteAsset("Assets/H2F01/Play/S06_Gc2_NavMesh.asset");
            AssetDatabase.CreateAsset(surface.navMeshData, "Assets/H2F01/Play/S06_Gc2_NavMesh.asset");

            // GC2 player exactly as its own menu creates it, then the ART citizen swapped in through GC2's API
            CharacterEditor.CreatePlayer(null);
            var player = GameObject.Find("Player");
            var character = player.GetComponent<Character>();
            float startY = Physics.Raycast(new Vector3(0, 40, -30), Vector3.down, out var hit, 80) ? hit.point.y : 0;
            player.transform.position = new Vector3(0, startY + character.Motion.Height * 0.5f + 0.05f, -30);
            var citizen = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx");
            var rtc = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Plugins/GameCreator/Packages/Core/Runtime/Characters/Assets/Controllers/CompleteLocomotion.controller");
            // GC2's ChangeModel calls Destroy() (runtime-only); in edit mode the old mannequin would survive and hide
            // the new model, so remove it explicitly first.
            if (character.Animim.Animator != null) Object.DestroyImmediate(character.Animim.Animator.gameObject);
            character.ChangeModel(citizen, new Character.ChangeOptions { controller = rtc, offset = Vector3.zero });
            character.Motion.LinearSpeed = 1.45f; // civilian walk, same as the minimal composition
            H2F01HumanSpike.RemapConverted(character.Animim.Mannequin.gameObject, null);
            foreach (var t in player.GetComponentsInChildren<Transform>(true)) t.gameObject.layer = 2;

            // Main Camera (GC2 component) + Camera Shot set to Third Person with heading auto-align
            var main = new GameObject("Main Camera");
            main.tag = "MainCamera";
            var camera = main.AddComponent<Camera>();
            camera.nearClipPlane = 0.05f; camera.fieldOfView = 55;
            main.AddComponent<MainCamera>();
            camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            ShotCameraEditor.CreateElement(null);
            var shot = Object.FindFirstObjectByType<ShotCamera>();
            var third = new ShotTypeThirdPerson();
            var system = typeof(ShotTypeThirdPerson).GetField("m_ThirdPerson", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(third);
            var align = system.GetType().GetField("m_Align", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(system);
            align.GetType().GetField("m_AutoAlign", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(align, true);
            align.GetType().GetField("m_Delay", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(align, 0.5f);
            align.GetType().GetField("m_SmoothTime", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(align, 1.0f);
            var radius = typeof(GameCreator.Runtime.Common.GetDecimalDecimal).GetMethod("Create", new[] { typeof(float) }).Invoke(null, new object[] { 3.0f });
            system.GetType().GetField("m_Radius", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(system, radius);
            var so = new SerializedObject(shot);
            so.FindProperty("m_ShotType").managedReferenceValue = third;
            so.ApplyModifiedPropertiesWithoutUndo();

            H2F01PlayBuilder.Driver("s06_route", "s06_gc2");
            var walker = GameObject.Find("H2F01_PLAY_DRIVER").AddComponent<H2F01RouteWalker>();
            walker.player = player.transform; walker.view = camera; walker.label = "gc2";
            walker.start = new Vector3(0, startY, -30);
            var bar = Physics.Raycast(new Vector3(0, 40, 22), Vector3.down, out var h22, 80) ? h22.point.y : 0;
            walker.goal = new Vector3(-1f, bar + 0.4f, 29f);
            H2F01PlayBuilder.Play("Play_S06_Gc2");
        }
    }
}
