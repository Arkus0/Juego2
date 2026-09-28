using System;
using System.Collections.Generic;
using GameCreator.Editor.Cameras;
using GameCreator.Editor.Characters;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using Juego2.Arkus;
using Juego2.Foundation;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Juego2.Gc2Adapter.Editor
{
    /// <summary>
    /// Materializes Juego2 presets into GC2 Core 2.19.61 presentation objects (S06 amendment). GC2 objects are
    /// created through GC2's public editor entry points and configured through Unity serialization, never through
    /// private GC2 members (PUBLIC_AUTHORING_SURFACE.md). Every serialized path written is recorded so a Core
    /// version change that moves a field fails closed with <c>J2_GC2_AUTHORING_PATH_MISSING</c>.
    /// </summary>
    public static class J2Gc2Presets
    {
        public const string PlayerPreset = "Assets/Juego2/Foundation/Presets/J2_Player.asset";
        public const string NpcPreset = "Assets/Juego2/Foundation/Presets/J2_Npc_Civilian.asset";
        public const string CameraPreset = "Assets/Juego2/Foundation/Presets/J2_PlayerCamera.asset";

        public static readonly List<string> AuthoringLog = new List<string>();

        /// <summary>Builds a GC2 Character for <paramref name="entityKey"/> at <paramref name="feet"/>.</summary>
        public static Character MaterializeCharacter(J2CharacterPreset preset, string entityKey, Vector3 feet)
        {
            if (preset == null) throw new ArgumentNullException(nameof(preset));
            if (!ArkusEntityBinding.IsValidKey(entityKey)) throw new ArgumentException("J2_ENTITY_KEY_INVALID " + entityKey);
            if (preset.isPlayer) CharacterEditor.CreatePlayer(null); else CharacterEditor.CreateCharacter(null);
            var go = Selection.activeGameObject;
            var character = go != null ? go.GetComponent<Character>() : null;
            if (character == null) throw new InvalidOperationException("J2_GC2_CHARACTER_NOT_CREATED");
            Record($"CharacterEditor.Create{(preset.isPlayer ? "Player" : "Character")} [{entityKey}]");
            go.name = entityKey;
            character.Motion.Height = preset.height;
            character.Motion.Radius = preset.radius;
            character.Motion.LinearSpeed = preset.linearSpeed;
            go.transform.position = feet + Vector3.up * (preset.height * 0.5f + 0.02f);
            if (preset.model != null)
            {
                // Edit-mode ChangeModel only calls runtime Destroy(): remove the previous animator object first (H2F-01 finding).
                if (character.Animim.Animator != null) UnityEngine.Object.DestroyImmediate(character.Animim.Animator.gameObject);
                character.ChangeModel(preset.model, new Character.ChangeOptions { controller = preset.locomotionController, offset = Vector3.zero });
                Record($"Character.ChangeModel({preset.model.name}, {(preset.locomotionController != null ? preset.locomotionController.name : "null")})");
            }

            var so = new SerializedObject(character);
            if (preset.isPlayer) BindVector2(so, "m_Kernel.m_Player.m_InputMove", Actions(preset.inputActions), preset.moveActionMap, preset.moveAction);
            else SetRef(so, "m_Kernel.m_Player.m_InputMove.m_Input", new InputValueVector2None()); // NPC preset rule
            if (preset.navMeshDriver) SetRef(so, "m_Kernel.m_Driver", new UnitDriverNavmesh());
            switch (preset.footstepDetector)
            {
                case J2CharacterPreset.FootstepDetector.Fulcrum:
                    SetRef(so, "m_Footsteps.m_FootstepDetector", new FootstepDetectorFulcrum());
                    break;
                case J2CharacterPreset.FootstepDetector.AnimationCurves:
                    SetRef(so, "m_Footsteps.m_FootstepDetector", new FootstepDetectorAnimationCurves());
                    break;
            }
            if (preset.footstepSounds is MaterialSoundsAsset sounds) SetObj(so, "m_Footsteps.m_FootstepSounds.m_SoundsAsset", sounds);

            var binding = go.AddComponent<ArkusEntityBinding>();
            binding.entityKey = entityKey;
            binding.kind = preset.isPlayer ? "player" : "character";
            var realization = go.AddComponent<J2PresetRealization>();
            realization.preset = preset;
            realization.presetKind = "character";
            if (preset.isPlayer && preset.inputActions is InputActionAsset actions)
            {
                var owner = go.AddComponent<J2InputOwner>();
                owner.actions = actions;
                owner.actionMap = preset.moveActionMap;
            }
            EditorUtility.SetDirty(character);
            return character;
        }

        /// <summary>URP camera + GC2 MainCamera + Third Person ShotCamera; orbit/zoom read the Juego2 action map.</summary>
        public static Camera MaterializePlayerCamera(J2CameraPreset preset)
        {
            if (preset == null) throw new ArgumentNullException(nameof(preset));
            var main = new GameObject("J2_PlayerCamera") { tag = "MainCamera" };
            var camera = main.AddComponent<Camera>();
            camera.nearClipPlane = preset.nearClip;
            camera.fieldOfView = preset.fieldOfView;
            main.AddComponent<MainCamera>();
            main.AddComponent<AudioListener>();
            camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            var realization = main.AddComponent<J2PresetRealization>();
            realization.preset = preset;
            realization.presetKind = "camera";
            ShotCameraEditor.CreateElement(null);
            var shotGo = Selection.activeGameObject;
            var shot = shotGo != null ? shotGo.GetComponent<ShotCamera>() : null;
            if (shot == null) throw new InvalidOperationException("J2_GC2_SHOT_NOT_CREATED");
            Record("GC2 MainCamera + ShotCameraEditor.CreateElement");
            var so = new SerializedObject(shot);
            SetRef(so, "m_ShotType", new ShotTypeThirdPerson());
            SetBool(so, "m_ShotType.m_ThirdPerson.m_Align.m_AutoAlign", preset.autoAlign);
            SetFloat(so, "m_ShotType.m_ThirdPerson.m_Align.m_Delay", preset.alignDelay);
            SetFloat(so, "m_ShotType.m_ThirdPerson.m_Align.m_SmoothTime", preset.alignSmoothTime);
            SetRef(so, "m_ShotType.m_ThirdPerson.m_Radius.m_Property", new GetDecimalDecimal(preset.radius));
            var actions = Actions(preset.inputActions);
            BindVector2(so, "m_ShotType.m_ThirdPerson.m_InputRotate", actions, preset.actionMap, preset.lookAction);
            BindVector2(so, "m_ShotType.m_Zoom.m_InputZoom", actions, preset.actionMap, preset.zoomAction);
            EditorUtility.SetDirty(shot);
            return camera;
        }

        static InputActionAsset Actions(UnityEngine.Object value) =>
            value as InputActionAsset ?? throw new InvalidOperationException("J2_PRESET_INPUT_ACTIONS_MISSING");

        static void BindVector2(SerializedObject so, string property, InputActionAsset asset, string map, string action)
        {
            if (asset.FindActionMap(map, false)?.FindAction(action, false) == null)
                throw new InvalidOperationException($"J2_INPUT_ACTION_MISSING {asset.name}/{map}/{action}");
            SetRef(so, property + ".m_Input", new InputValueVector2InputAction());
            SetObj(so, property + ".m_Input.m_Input.m_InputAsset", asset);
            SetStr(so, property + ".m_Input.m_Input.m_ActionMap", map);
            SetStr(so, property + ".m_Input.m_Input.m_Action", action);
        }

        // ------------------------------------------------------------------ the Inspector's serialization surface

        static void Record(string line) => AuthoringLog.Add(line);

        static SerializedProperty P(SerializedObject so, string path) =>
            so.FindProperty(path) ?? throw new InvalidOperationException($"J2_GC2_AUTHORING_PATH_MISSING {so.targetObject.GetType().Name}:{path}");

        static void Apply(SerializedObject so, string path, string what)
        {
            so.ApplyModifiedPropertiesWithoutUndo();
            so.Update();
            Record($"{so.targetObject.GetType().Name}.{path} = {what}");
        }

        static void SetRef(SerializedObject so, string path, object value)
        {
            P(so, path).managedReferenceValue = value;
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
