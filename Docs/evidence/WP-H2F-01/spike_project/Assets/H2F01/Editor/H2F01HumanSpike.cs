using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Juego2.H2F01.Editor.H2F01Common;

namespace Juego2.H2F01.Editor
{
    /// <summary>
    /// S07 humanoid spike (static part). The ART clothed citizen (Universal Base Characters derivative, Humanoid
    /// avatar from its own skeleton) is posed with same-emitter UAL1 and gap-filler UAL2 civilian clips, and each
    /// pose is measured (feet vs ground, sit hips vs seat) and captured. Disposable scene; no keeper content.
    /// </summary>
    public static class H2F01HumanSpike
    {
        const string Human = "Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx";
        const string Ual1 = "Assets/Arkus/ART/External/UAL/UAL1.fbx";
        const string Ual2 = "Assets/H2F01Inputs/Animation/UAL2.fbx";

        static readonly (string clip, string src, string use)[] Clips =
        {
            ("Idle_Loop", Ual1, "stand"), ("Idle_Talking_Loop", Ual1, "stand"), ("Idle_LookAround_Loop", Ual1, "stand"),
            ("Walk_Loop", Ual1, "walk"), ("Walk_Formal_Loop", Ual1, "walk"), ("Drink", Ual1, "stand"),
            ("Counter_Give", Ual1, "stand"), ("Interact", Ual1, "stand"),
            ("Sitting_Idle_Loop", Ual1, "sit"), ("Sitting_Talking_Loop", Ual1, "sit"),
            ("Idle_FoldArms_Loop", Ual2, "stand"), ("Idle_Rail_Loop", Ual2, "stand"), ("Walk_Carry_Loop", Ual2, "walk"),
            ("Yes", Ual2, "stand"), ("StepUp", Ual2, "step"),
        };

        [Serializable] class PoseMetric { public string clip, source, use; public float length, footMin, footMax, hipsY, seatGap; public bool humanMotion; }
        [Serializable] class Report { public string avatar; public bool human, valid; public float heightM; public List<PoseMetric> poses = new List<PoseMetric>(); public List<string> notes = new List<string>(); }

        /// <summary>
        /// ART-01's Art01Materials.Get re-binds Shader.Find("Standard") on every call (recorded S07 negative evidence),
        /// so the spike maps Source material names to the already URP-converted ART palette assets directly.
        /// </summary>
        internal static void RemapConverted(GameObject go, string overrideId)
        {
            string Id(string n) =>
                overrideId ?? (n.Contains("Regular_Male") || n.Contains("Skin_Regular_Male_Light") ? "Skin"
                : n.Contains("Eye") ? "Eye" : n.Contains("Hair") ? "Hair" : n.Contains("Cloth_D8D2C4") ? "Shirt"
                : n.Contains("Cloth_2E4A4E") ? "Coat" : n.Contains("Cloth_3A3530") ? "Trousers" : n.Contains("Cloth_4A3222") ? "Shoes"
                : n.Contains("Trim_Furniture") ? "PropWood" : null);
            // Unknown (e.g. third-party runtime) materials are left untouched and remain visible to the shader audit.
            foreach (var r in go.GetComponentsInChildren<Renderer>(true))
                r.sharedMaterials = r.sharedMaterials.Select(m => m == null || Id(m.name) == null ? m
                    : AssetDatabase.LoadAssetAtPath<Material>($"Assets/Arkus/ART/Materials/{Id(m.name)}.mat") ?? m).ToArray();
        }

        public static void Poses()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var sun = UnityEngine.Object.FindFirstObjectByType<Light>();
            sun.color = new Color(0.83f, 0.89f, 0.93f); sun.intensity = 1.0f; sun.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(40, -30, 0);
            H2F01RenderSpike.BaselineLook();
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.localScale = new Vector3(8, 1, 8);
            ground.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Arkus/ART/Materials/Cobble.mat");

            var report = new Report();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Human);
            var chairPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Arkus/ART/External/Props/Models/Chair_1.fbx");
            AnimationMode.StartAnimationMode();
            int col = 0;
            foreach (var (clipName, src, use) in Clips)
            {
                var clip = H2F01RenderSpike.UalClip(clipName, src);
                var cell = new Vector3((col % 5) * 3f, 0, (col / 5) * -4f);
                col++;
                // Humanoid sampling writes the Animator root, so each citizen sits under a placement container.
                var holder = new GameObject("cell_" + clipName).transform;
                holder.position = cell;
                var human = (GameObject)PrefabUtility.InstantiatePrefab(prefab, holder);
                human.name = "citizen_" + clipName;
                RemapConverted(human, null);
                var animator = human.GetComponentInChildren<Animator>();
                if (report.avatar == null)
                {
                    report.avatar = animator.avatar.name; report.human = animator.avatar.isHuman; report.valid = animator.avatar.isValid;
                    var head = animator.GetBoneTransform(HumanBodyBones.Head);
                    report.heightM = head.position.y + 0.12f;
                    var hb = animator.GetBoneTransform(HumanBodyBones.Hips);
                    report.notes.Add($"Hips bone '{hb.name}' parent '{hb.parent?.name}' bind world y {hb.position.y:F3}; LeftFoot '{animator.GetBoneTransform(HumanBodyBones.LeftFoot).name}'");
                }
                var m = new PoseMetric { clip = clipName, source = System.IO.Path.GetFileNameWithoutExtension(src), use = use, length = clip.length, humanMotion = clip.humanMotion, footMin = 99, footMax = -99 };
                // feet over the cycle
                for (int k = 0; k < 8; k++)
                {
                    AnimationMode.SampleAnimationClip(animator.gameObject, clip, clip.length * k / 8f);
                    foreach (var bone in new[] { HumanBodyBones.LeftToes, HumanBodyBones.RightToes, HumanBodyBones.LeftFoot, HumanBodyBones.RightFoot })
                    {
                        var b = animator.GetBoneTransform(bone);
                        if (b == null) continue;
                        m.footMin = Mathf.Min(m.footMin, b.position.y); m.footMax = Mathf.Max(m.footMax, b.position.y);
                    }
                }
                float showTime = use == "walk" ? clip.length * 0.25f : clip.length * 0.4f;
                AnimationMode.SampleAnimationClip(animator.gameObject, clip, showTime);
                m.hipsY = animator.GetBoneTransform(HumanBodyBones.Hips).position.y;
                if (use == "sit" && chairPrefab != null)
                {
                    // ART's Source() rule: keep the Props root (-90 X, x100) under a wrapper; the wrapper carries pose.
                    var chairHolder = new GameObject("chair_" + clipName).transform;
                    chairHolder.position = cell + new Vector3(0, 0, -0.30f);
                    var chair = (GameObject)PrefabUtility.InstantiatePrefab(chairPrefab, chairHolder);
                    chair.transform.localPosition = Vector3.zero;
                    chair.transform.localRotation = chairPrefab.transform.localRotation;
                    chair.transform.localScale = chairPrefab.transform.localScale;
                    RemapConverted(chair, "PropWood");
                    foreach (var r in chair.GetComponentsInChildren<MeshFilter>()) r.gameObject.AddComponent<MeshCollider>();
                    Physics.SyncTransforms();
                    var hips = animator.GetBoneTransform(HumanBodyBones.Hips).position;
                    chairHolder.position += new Vector3(hips.x - chairHolder.position.x, 0, 0);
                    Physics.SyncTransforms();
                    var seatB = chair.GetComponentsInChildren<Renderer>().Select(r => r.bounds).Aggregate((a, b) => { a.Encapsulate(b); return a; });
                    float seat = Physics.Raycast(new Vector3(seatB.center.x, 3, seatB.center.z), Vector3.down, out var hit, 5) ? hit.point.y : 0;
                    report.notes.Add($"Chair_1 bounds {seatB.size.x:F2}x{seatB.size.y:F2}x{seatB.size.z:F2} seat raycast {seat:F3} hit={(hit.collider ? hit.collider.name : "none")}");
                    chairHolder.position += new Vector3(hips.x - seatB.center.x, 0, hips.z - seatB.center.z);
                    // pelvis sits ~0.10 m above the seat surface for this body; report the residual the authoring anchor must absorb
                    m.seatGap = hips.y - seat - 0.10f;
                    report.notes.Add($"{clipName}: hips {hips.y:F3} seat {seat:F3} (chair Chair_1 as imported); required vertical anchor offset {-m.seatGap:F3} m");
                    holder.position += new Vector3(0, -m.seatGap, 0);
                }
                report.poses.Add(m);
            }
            // per-clip 3/4 front capture (others hidden), walks also from the side
            var holders = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t => t.parent == null && (t.name.StartsWith("cell_") || t.name.StartsWith("chair_"))).ToList();
            foreach (var (clipName, _, use) in Clips)
            {
                foreach (var h in holders) h.gameObject.SetActive(h.name == "cell_" + clipName || h.name == "chair_" + clipName);
                var focus = holders.First(h => h.name == "cell_" + clipName).GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Hips).position;
                var cam = CameraAt("s07_cam", focus + new Vector3(1.3f, 0.55f, 2.6f), focus + new Vector3(0, 0.1f, 0), 42);
                cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
                Image(cam, $"s07/pose_{clipName}.png", 640, 720);
                UnityEngine.Object.DestroyImmediate(cam.gameObject);
                if (use == "walk" || use == "step" || use == "sit")
                {
                    var side = CameraAt("s07_side", focus + new Vector3(3.0f, 0.1f, 0), focus + new Vector3(0, -0.1f, 0), 42);
                    side.GetUniversalAdditionalCameraData().renderPostProcessing = true;
                    Image(side, $"s07/pose_{clipName}_side.png", 640, 720);
                    UnityEngine.Object.DestroyImmediate(side.gameObject);
                }
            }
            AnimationMode.StopAnimationMode();
            WriteJson("s07/pose_report.json", JsonUtility.ToJson(report, true));
            Log($"H2F01_S07_POSES_DONE avatar={report.avatar} human={report.human} height={report.heightM:F2} poses={report.poses.Count}");
        }
    }
}
