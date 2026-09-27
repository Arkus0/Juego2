using System;
using System.Linq;
using Juego2.ART;
using Unity.AI.Navigation;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering.Universal;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>Builds disposable play-mode scenes for S07 (motion/IK) and S06 (control/camera), then enters Play Mode.</summary>
    public static class H2F01PlayBuilder
    {
        const string Human = "Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx";
        const string Ual1 = "Assets/Arkus/ART/External/UAL/UAL1.fbx";
        const string Ual1Rm = "Assets/H2F01Inputs/Animation/UAL1_RM.fbx";
        const string Dir = "Assets/H2F01/Play";

        internal static void Folder()
        {
            if (!AssetDatabase.IsValidFolder(Dir)) AssetDatabase.CreateFolder("Assets/H2F01", "Play");
        }

        static AnimatorController SingleState(string name, AnimationClip clip)
        {
            var path = $"{Dir}/{name}.controller";
            AssetDatabase.DeleteAsset(path);
            var c = AnimatorController.CreateAnimatorControllerAtPath(path);
            c.layers[0].stateMachine.AddState(clip.name).motion = clip;
            return c;
        }

        internal static AnimatorController Locomotion()
        {
            var path = $"{Dir}/Locomotion.controller";
            AssetDatabase.DeleteAsset(path);
            var c = AnimatorController.CreateAnimatorControllerAtPath(path);
            c.AddParameter("Speed", AnimatorControllerParameterType.Float);
            c.CreateBlendTreeInController("Locomotion", out var tree, 0);
            tree.blendParameter = "Speed";
            tree.AddChild(H2F01RenderSpike.UalClip("Idle_Loop"), 0f);
            tree.AddChild(H2F01RenderSpike.UalClip("Walk_Loop"), 1.45f);
            return c;
        }

        internal static GameObject Citizen(string holderName, Vector3 at, RuntimeAnimatorController controller, bool rootMotion)
        {
            var holder = new GameObject(holderName);
            holder.transform.position = at;
            var human = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Human), holder.transform);
            H2F01HumanSpike.RemapConverted(human, null);
            var animator = human.GetComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = rootMotion;
            return holder;
        }

        internal static void Driver(string scenario, string outRel)
        {
            var go = new GameObject("H2F01_PLAY_DRIVER");
            var d = go.AddComponent<H2F01PlayDriver>();
            d.scenario = scenario;
            d.outDir = Out(outRel + "/.keep").Replace("/.keep", "").Replace("\\.keep", "");
        }

        internal static void Play(string sceneName)
        {
            var path = $"{Dir}/{sceneName}.unity";
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), path);
            AssetDatabase.SaveAssets();
            EditorSettings.enterPlayModeOptionsEnabled = false; // full domain reload: third-party statics (GC2) stay honest
            Log("H2F01_PLAY_ENTER " + path);
            EditorApplication.EnterPlaymode();
        }

        /// <summary>
        /// New projects here default to activeInputHandler=0 (legacy Input Manager only) even with the Input System
        /// package installed; the selected stack needs 2 (Both) or 1 (Input System). Takes effect on next editor start.
        /// </summary>
        public static void EnableInputSystem()
        {
            var so = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            var p = so.FindProperty("activeInputHandler");
            int before = p.intValue;
            p.intValue = 2;
            so.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();
            Log($"H2F01_INPUT_HANDLER before={before} after={p.intValue}");
        }

        // ------------------------------------------------------------------ S07

        public static void S07Motion()
        {
            Folder();
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var sun = UnityEngine.Object.FindFirstObjectByType<Light>();
            sun.shadows = LightShadows.Soft; sun.transform.rotation = Quaternion.Euler(40, -30, 0);
            H2F01RenderSpike.BaselineLook();
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.localScale = new Vector3(6, 1, 6);
            ground.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Arkus/ART/Materials/Cobble.mat");
            Citizen("rm_walker", new Vector3(0, 0, -6), SingleState("WalkRM", H2F01RenderSpike.UalClip("Walk_Loop", Ual1Rm)), true);
            var ip = Citizen("inplace_walker", new Vector3(3, 0, -6), SingleState("WalkInPlace", H2F01RenderSpike.UalClip("Walk_Loop")), false);
            var cc = ip.AddComponent<CharacterController>(); cc.center = new Vector3(0, 0.9f, 0); cc.height = 1.8f; cc.radius = 0.27f;
            var ik = Citizen("ik_actor", new Vector3(-3, 0, 0), SingleState("IdleIK", H2F01RenderSpike.UalClip("Idle_Loop")), false);
            // table in front (ART Source() placement rule) and a contact target on its top
            var tablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Arkus/ART/External/Props/Models/Table_Large.fbx");
            var tableHolder = new GameObject("table_holder").transform;
            tableHolder.position = new Vector3(-3, 0, 0.62f);
            var table = (GameObject)PrefabUtility.InstantiatePrefab(tablePrefab, tableHolder);
            table.transform.localRotation = tablePrefab.transform.localRotation; table.transform.localScale = tablePrefab.transform.localScale;
            H2F01HumanSpike.RemapConverted(table, "PropWood");
            var tb = table.GetComponentsInChildren<Renderer>().Select(r => r.bounds).Aggregate((a, b) => { a.Encapsulate(b); return a; });
            var target = new GameObject("ik_target").transform;
            var animator = ik.GetComponentInChildren<Animator>();
            // reachable contact: on the table top, ~0.45 m forward of the right shoulder (arm ~0.6 m)
            var shoulder = animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            // bar-counter contact height (~1.05 m) inside arm reach (~0.55 m): the Bar F01 case, not a table stretch
            target.position = new Vector3(shoulder.x, shoulder.y - 0.33f, shoulder.z + 0.34f);
            target.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            var rigGo = new GameObject("H2F01_Rig"); rigGo.transform.SetParent(animator.transform, false);
            var rig = rigGo.AddComponent<Rig>();
            var ikGo = new GameObject("RightHandTwoBoneIK"); ikGo.transform.SetParent(rigGo.transform, false);
            var constraint = ikGo.AddComponent<TwoBoneIKConstraint>();
            var data = constraint.data;
            data.root = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            data.mid = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            data.tip = animator.GetBoneTransform(HumanBodyBones.RightHand);
            data.target = target;
            data.targetPositionWeight = 1; data.targetRotationWeight = 0; data.hintWeight = 0;
            constraint.data = data;
            constraint.weight = 0;
            var builder = animator.gameObject.AddComponent<RigBuilder>();
            builder.layers.Add(new RigLayer(rig, true));
            var cam = new GameObject("s07_play_camera").AddComponent<Camera>();
            cam.transform.position = new Vector3(5.5f, 1.7f, 2.5f); cam.transform.LookAt(new Vector3(0, 0.9f, -2.5f));
            cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            Driver("s07_motion", "s07_play");
            Play("Play_S07_Motion");
        }

        // ------------------------------------------------------------------ S06 minimal composition

        public static void S06Minimal()
        {
            Folder();
            H2F01WorldSpike.ConfigureAgent();
            OpenArtScene();
            H2F01RenderSpike.BaselineLook();
            var inspector = UnityEngine.Object.FindFirstObjectByType<Art01WalkInspector>();
            if (inspector != null) UnityEngine.Object.DestroyImmediate(inspector.gameObject); // legacy-Input ART rig
            foreach (var cam0 in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)) UnityEngine.Object.DestroyImmediate(cam0.gameObject);
            // nav bake saved as an asset so it survives entering play mode
            var navHost = new GameObject("H2F01_NAV");
            var surface = navHost.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All; surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.overrideVoxelSize = true; surface.voxelSize = 0.07f;
            surface.BuildNavMesh();
            AssetDatabase.DeleteAsset($"{Dir}/S06_NavMesh.asset");
            AssetDatabase.CreateAsset(surface.navMeshData, $"{Dir}/S06_NavMesh.asset");
            // player
            float startZ = -30;
            var startY = Physics.Raycast(new Vector3(0, 40, startZ), Vector3.down, out var hit, 80) ? hit.point.y : 0;
            var player = Citizen("H2F01_PLAYER_MINIMAL", new Vector3(0, startY + 0.05f, startZ), Locomotion(), false);
            foreach (var t in player.GetComponentsInChildren<Transform>(true)) t.gameObject.layer = 2;
            var body = player.AddComponent<CharacterController>();
            body.center = new Vector3(0, 0.9f, 0); body.height = 1.8f; body.radius = 0.27f; body.stepOffset = 0.3f; body.slopeLimit = 40f;
            var cameraRoot = new GameObject("camera_root").transform;
            var main = new GameObject("MainCamera").AddComponent<Camera>();
            main.tag = "MainCamera"; main.nearClipPlane = 0.05f; main.fieldOfView = 55;
            main.gameObject.AddComponent<CinemachineBrain>();
            main.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            var vcamGo = new GameObject("CM_ThirdPerson");
            var vcam = vcamGo.AddComponent<CinemachineCamera>();
            vcam.Follow = cameraRoot;
            vcam.Lens.FieldOfView = 55; vcam.Lens.NearClipPlane = 0.05f;
            var follow = vcamGo.AddComponent<CinemachineThirdPersonFollow>();
            follow.CameraDistance = 3.0f; follow.ShoulderOffset = new Vector3(0.35f, 0.05f, 0); follow.VerticalArmLength = 0.2f; follow.CameraSide = 0.6f;
            var avoid = follow.AvoidObstacles;
            avoid.Enabled = true; avoid.CollisionFilter = ~(1 << 2); avoid.CameraRadius = 0.2f; avoid.DampingIntoCollision = 0.1f; avoid.DampingFromCollision = 0.5f;
            follow.AvoidObstacles = avoid;
            var ctl = player.AddComponent<H2F01MinimalController>();
            ctl.cameraRoot = cameraRoot; ctl.viewCamera = main.transform;
            Driver("s06_route", "s06_minimal");
            var walker = GameObject.Find("H2F01_PLAY_DRIVER").AddComponent<H2F01RouteWalker>();
            walker.player = player.transform; walker.view = main; walker.label = "minimal";
            walker.start = new Vector3(0, startY, startZ);
            var bar = Physics.Raycast(new Vector3(0, 40, 22), Vector3.down, out var h22, 80) ? h22.point.y : 0;
            walker.goal = new Vector3(-1f, bar + 0.4f, 29f);
            Play("Play_S06_Minimal");
        }
    }
}
