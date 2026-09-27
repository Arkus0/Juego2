using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Characters.IK;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using Juego2.H2F01A.Arkus;
using Juego2.H2F01A.Gc2Adapter;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Juego2.H2F01A
{
    public sealed partial class H2F01AProbeDriver
    {
        public AnimationClip gestureClip, stateClip;

        readonly List<string> requests = new List<string>();

        void HookRequests()
        {
            InstructionArkusRequestTransition.EventResult += (r, e, t, ok, why) => requests.Add($"{r}|{e}|{t}|{(ok ? "ACCEPTED" : "REJECTED")}|{why}");
        }

        static IEnumerator Frames(int n) { for (int i = 0; i < n; i++) yield return null; }

        IEnumerator MoveTo(Character c, Vector3 point, float stop = 0.25f, float timeout = 20f)
        {
            bool done = false;
            c.Motion.MoveToLocation(new Location(point), stop, (_, __) => done = true);
            int f = 0;
            while (!done && f < timeout * 30) { f++; yield return null; }
            yield return Frames(8);
        }

        IEnumerator Press(Gamepad pad)
        {
            InputSystem.QueueStateEvent(pad, new GamepadState().WithButton(GamepadButton.South, true));
            yield return Frames(3);
            InputSystem.QueueStateEvent(pad, new GamepadState());
            yield return Frames(12);
        }

        static string Target(Character c) => c.Interaction.Target?.Instance != null ? c.Interaction.Target.Instance.name : "none";

        int ConsoleCount(string exact) => consoleLines.Count(l => l.Trim() == exact);

        // ------------------------------------------------------------------ C02 character composition + S06 amendment facts

        IEnumerator C02()
        {
            string label = scenario == "c02g" ? "c02_gc2camera" : "c02_cinemachine";
            var player = CharacterNamed("J2_Player");
            var npc = CharacterNamed("J2_NPC_A");
            if (input != null) input.Enable();
            yield return Frames(20);
            var pa = player.Animim.Animator;
            Put("camera_composition", scenario == "c02g" ? "GC2 MainCamera + ShotCamera ThirdPerson (S06 amendment)" : "Cinemachine 3 ThirdPersonFollow over J2CameraRig (H2F-01 camera)");
            Put("player_is_gc2_player", player.IsPlayer);
            Put("npc_is_gc2_player", npc.IsPlayer);
            Put("player_entity", ArkusEntityBinding.KeyOf(player.gameObject));
            Put("npc_entity", ArkusEntityBinding.KeyOf(npc.gameObject));
            Put("player_model", pa != null ? pa.gameObject.name : "none");
            Put("player_avatar_human_valid", pa != null && pa.isHuman && pa.avatar != null && pa.avatar.isValid);
            Put("player_hips_above_feet_m", HipsAboveFeet(player));
            Put("npc_hips_above_feet_m", HipsAboveFeet(npc));
            Put("player_foot_bone_above_ground_m", FeetToGround(player));
            Put("player_capsule_skin_width_m", player.GetComponent<CharacterController>()?.skinWidth ?? -1);
            Put("player_animator_controller", pa != null && pa.runtimeAnimatorController != null ? pa.runtimeAnimatorController.name : "none");
            Put("player_input_unit", player.Kernel.Player.GetType().Name);
            Put("player_move_input", "J2_Input/Player/Move (Input System action map owned by Juego2)");
            Put("player_driver", player.Driver.GetType().Name);
            Put("npc_driver", npc.Driver.GetType().Name);
            OwnershipAudit("");
            bool done = false, ok = false;
            int frames = 0;
            npc.Motion.MoveToLocation(new Location(npcGoal), 0.3f, (c, success) => { done = true; ok = success; });
            while (!done && frames < 30 * 40) { frames++; yield return null; }
            Put("npc_nav_callback", done);
            Put("npc_nav_success", ok);
            Put("npc_nav_seconds", frames / 30f);
            Put("npc_nav_distance_m", Vector3.Distance(Flat(npcGoal), Flat(npc.Feet)));
            Capture(view, label + "_npc_arrived");
            npc.gameObject.SetActive(false); // keep the S06 corridor identical to H2F-01
            yield return Route(player.transform, label);
            Capture(view, label + "_route_end");
        }

        // ------------------------------------------------------------------ C03 interaction / affordance execution

        IEnumerator C03()
        {
            ArkusProbeAuthority.Reset();
            ArkusProbeAuthority.Declare("j2.door.probe", "state", "closed");
            ArkusProbeAuthority.Declare("j2.chair.probe", "occupant", "");
            ArkusProbeAuthority.Declare("j2.mug.probe", "holder", "");
            HookRequests();
            var player = CharacterNamed("J2_Player");
            var npc = CharacterNamed("J2_NPC_A");
            var focus = new List<string>();
            player.Interaction.EventFocus += (c, i) => focus.Add("player>" + (i?.Instance != null ? i.Instance.name : "none"));
            npc.Interaction.EventFocus += (c, i) => focus.Add("npc>" + (i?.Instance != null ? i.Instance.name : "none"));
            var hotspot = FindFirstObjectByType<Hotspot>();
            int hotspotOn = 0, hotspotOff = 0;
            hotspot.EventOnActivate += () => hotspotOn++;
            hotspot.EventOnDeactivate += () => hotspotOff++;
            // Juego2 input glue: the J2 'Interact' action invokes the GC2 player's public Interaction.Interact()
            var interact = input.FindAction("Player/Interact", true);
            int presses = 0;
            interact.performed += _ => { presses++; player.Interaction.Interact(); };
            input.Enable();
            var pad = InputSystem.AddDevice<Gamepad>("H2F01AVirtualPad");
            yield return Frames(20);
            Put("hotspot_active_at_start", hotspot.IsActive);

            yield return MoveTo(player, new Vector3(0, 0, 0.1f));
            Put("player_focus_at_door", Target(player));
            Put("hotspot_active_near_door", hotspot.IsActive);
            yield return Press(pad);
            Put("door_after_first_press", ArkusProbeAuthority.Get("j2.door.probe:state"));
            Put("door_presentation_logged_after_first", ConsoleCount("open"));
            Capture(view, "c03_player_door");
            yield return Press(pad);
            Put("door_after_second_press", ArkusProbeAuthority.Get("j2.door.probe:state"));
            Put("door_presentation_logged_after_second", ConsoleCount("open"));

            yield return MoveTo(player, new Vector3(-3, 0, 0.1f));
            Put("player_focus_at_mug", Target(player));
            yield return Press(pad);
            Put("mug_after_player", ArkusProbeAuthority.Get("j2.mug.probe:holder"));
            yield return MoveTo(player, new Vector3(-1.2f, 0, -2.2f));

            yield return MoveTo(npc, new Vector3(4, 0, 0.4f));
            Put("npc_focus_at_chair", Target(npc));
            Put("npc_interact_returned", npc.Interaction.Interact());
            yield return Frames(10);
            Put("chair_after_npc", ArkusProbeAuthority.Get("j2.chair.probe:occupant"));
            Capture(view, "c03_npc_chair");

            yield return MoveTo(npc, new Vector3(-3, 0, 0.1f));
            Put("npc_focus_at_mug", Target(npc));
            npc.Interaction.Interact();
            yield return Frames(10);
            Put("mug_after_npc_attempt", ArkusProbeAuthority.Get("j2.mug.probe:holder"));

            yield return MoveTo(player, new Vector3(-1.5f, 0, -4.4f));
            Put("player_focus_at_unbound", Target(player));
            yield return Press(pad);

            InputSystem.RemoveDevice(pad);
            Put("player_interact_presses", presses);
            Put("hotspot_activations", hotspotOn);
            Put("focus_sequence", string.Join(",", focus));
            Put("adapter_requests", string.Join(" ; ", requests));
            Put("arkus_log", string.Join(" ; ", ArkusProbeAuthority.Log));
            Put("presentation_lines_open", ConsoleCount("open"));
            Put("presentation_lines_player", ConsoleCount("j2.char.player"));
            Put("presentation_lines_npc", ConsoleCount("j2.npc.probe_a"));
        }

        // ------------------------------------------------------------------ C04 character presentation helpers

        IEnumerator C04()
        {
            var a = CharacterNamed("J2_NPC_UAL");
            var b = CharacterNamed("J2_NPC_GC2LOCO");
            yield return Frames(30);
            var animator = a.Animim.Animator;

            // Gesture (UAL clip through GC2 Gestures): largest bone excursion from the idle pose, in the body frame
            var probeBones = new[] { HumanBodyBones.RightHand, HumanBodyBones.LeftHand, HumanBodyBones.RightLowerArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.Head }
                .Select(animator.GetBoneTransform).Where(t => t != null).ToArray();
            var rest = probeBones.Select(t => a.transform.InverseTransformPoint(t.position)).ToArray();
            float handMax = 0;
            _ = a.Gestures.CrossFade(gestureClip, null, BlendMode.Blend, new ConfigGesture(0f, gestureClip.length, 1f, false, 0.15f, 0.2f), true);
            bool playingMid = false;
            for (int f = 0; f < Mathf.CeilToInt(gestureClip.length * 30) + 20; f++)
            {
                yield return null;
                for (int k = 0; k < probeBones.Length; k++)
                    handMax = Mathf.Max(handMax, Vector3.Distance(rest[k], a.transform.InverseTransformPoint(probeBones[k].position)));
                if (f == Mathf.CeilToInt(gestureClip.length * 15)) { playingMid = a.Gestures.IsPlaying; Capture(view, "c04_gesture"); }
            }
            Put("gesture_clip", gestureClip.name);
            Put("gesture_playing_mid", playingMid);
            Put("gesture_max_bone_excursion_m", handMax);
            Put("gesture_playing_after", a.Gestures.IsPlaying);

            // Looping State (UAL sitting idle on layer 1)
            float hipsStand = HipsAboveFeet(a);
            _ = a.States.SetState(stateClip, null, 1, BlendMode.Blend, new ConfigState(0f, 1f, 1f, 0.25f, 0.25f));
            yield return Frames(45);
            float hipsState1 = HipsAboveFeet(a);
            Capture(view, "c04_state");
            yield return Frames(Mathf.CeilToInt(stateClip.length * 30) + 30);
            float hipsState2 = HipsAboveFeet(a);
            a.States.Stop(1, 0f, 0.25f);
            yield return Frames(40);
            Put("state_clip", stateClip.name);
            Put("state_hips_stand_m", hipsStand);
            Put("state_hips_in_state_m", hipsState1);
            Put("state_hips_after_one_loop_m", hipsState2);
            Put("state_hips_after_stop_m", HipsAboveFeet(a));

            // Look-at (GC2 RigLookTo on the Quaternius skeleton)
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var target = GameObject.Find("J2_LookTarget").transform;
            Vector3 localFace = Quaternion.Inverse(head.rotation) * a.transform.forward;
            float AngleToTarget() => Vector3.Angle(head.rotation * localFace, target.position - head.position);
            float lookBefore = AngleToTarget();
            var rig = a.IK.RequireRig<RigLookTo>();
            var look = new LookToTransform(0, target, Vector3.zero);
            rig.SetTarget(look);
            yield return Frames(60);
            float lookAfter = AngleToTarget();
            Capture(view, "c04_lookat");
            rig.RemoveTarget(look);
            yield return Frames(30);
            Put("lookat_angle_before_deg", lookBefore);
            Put("lookat_angle_with_target_deg", lookAfter);

            // Footsteps over two ART surfaces (cobble z<0, dark wood z>0)
            var steps = new Dictionary<string, int>();
            void Count(Character c, string tag, Transform bone)
            {
                string surface = Physics.Raycast(bone.position + Vector3.up * 0.2f, Vector3.down, out var hit, 1.5f, ~(1 << 2), QueryTriggerInteraction.Ignore) ? hit.collider.name : "none";
                string k = tag + ":" + surface;
                steps[k] = steps.TryGetValue(k, out var n) ? n + 1 : 1;
            }
            a.Footsteps.EventStep += bone => Count(a, "ual_fulcrum", bone);
            b.Footsteps.EventStep += bone => Count(b, "gc2loco_curves", bone);
            var clipPlays = new Dictionary<string, int>();
            var lastTime = new Dictionary<AudioSource, float>();
            bool doneA = false, doneB = false;
            a.Motion.MoveToLocation(new Location(new Vector3(-1, 0, 5)), 0.2f, (_, __) => doneA = true);
            b.Motion.MoveToLocation(new Location(new Vector3(1, 0, 5)), 0.2f, (_, __) => doneB = true);
            for (int f = 0; f < 30 * 15 && !(doneA && doneB); f++)
            {
                yield return null;
                foreach (var src in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
                {
                    if (src.clip == null || !src.isPlaying) { lastTime.Remove(src); continue; }
                    bool started = !lastTime.TryGetValue(src, out var prev) || src.time + 1e-4f < prev;
                    lastTime[src] = src.time;
                    if (started) clipPlays[src.clip.name] = clipPlays.TryGetValue(src.clip.name, out var n) ? n + 1 : 1;
                }
                if (f == 90) Capture(view, "c04_footsteps");
            }
            Put("footsteps_by_detector_surface", string.Join(",", steps.OrderBy(k => k.Key).Select(k => $"{k.Key}={k.Value}")));
            Put("footstep_clip_plays", string.Join(",", clipPlays.OrderBy(k => k.Key).Select(k => $"{k.Key}={k.Value}")));

            // Ragdoll -> recovery (RagdollDefault, GC2 Core skeleton + Core recover clips)
            float hipsBefore = HipsAboveFeet(a);
            var rag = a.Ragdoll.StartRagdoll();
            yield return Await(rag, 5f);
            yield return Frames(60);
            Put("ragdoll_is_ragdoll", a.Ragdoll.IsRagdoll);
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            Put("ragdoll_hips_height_m", hips.position.y);
            Capture(view, "c04_ragdoll");
            var recover = a.Ragdoll.StartRecover();
            yield return Await(recover, 10f);
            yield return Frames(30);
            Put("recover_task_completed", recover.IsCompleted);
            Put("recover_is_ragdoll", a.Ragdoll.IsRagdoll);
            Put("recover_hips_above_feet_m", HipsAboveFeet(a));
            Put("recover_hips_before_ragdoll_m", hipsBefore);
            Capture(view, "c04_recovered");
            var from = a.Feet;
            yield return MoveTo(a, a.Feet + new Vector3(1.5f, 0, 0), 0.2f, 8f);
            Put("recover_can_move_m", Vector3.Distance(Flat(from), Flat(a.Feet)));
        }

        // ------------------------------------------------------------------ C05 Arkus <-> GC2 public scripting seam

        IEnumerator C05()
        {
            ArkusProbeAuthority.Reset();
            ArkusProbeAuthority.Declare("j2.door.c05", "state", "closed");
            ArkusProbeAuthority.Declare("j2.lamp.c05", "light", "off");
            HookRequests();
            yield return Frames(15);
            var lamp = GameObject.Find("J2_Lamp").GetComponent<Light>();
            var rule = GameObject.Find("J2_Rule_DoorOpensLamp").GetComponent<Trigger>();
            int runs = 0;
            rule.EventAfterExecute += () => runs++;
            var args = new Args(gameObject);
            Put("condition_door_open_initially", new ConditionArkusFact("j2.door.c05:state", "open").Check(args));
            Put("property_fact_door_initially", GetStringArkusFact.Create("j2.door.c05:state").Get(args));
            Put("property_entity_lamp", GetGameObjectArkusEntity.Create("j2.lamp.c05").Get(args)?.name ?? "null");
            Put("property_entity_unknown", GetGameObjectArkusEntity.Create("j2.lamp.missing").Get(args)?.name ?? "null");

            var r0 = ArkusProbeAuthority.Request("j2.char.probe_author", "j2.door.c05", "door.close");
            yield return Frames(15);
            Put("rejected_transition_reason", r0.Reason);
            Put("trigger_runs_after_rejected", runs);

            ArkusProbeAuthority.Request("j2.char.probe_author", "j2.door.c05", "door.open");
            yield return Frames(20);
            Put("trigger_runs_after_open", runs);
            Put("condition_door_open_now", new ConditionArkusFact("j2.door.c05:state", "open").Check(args));
            Put("lamp_fact_after_open", ArkusProbeAuthority.Get("j2.lamp.c05:light"));
            Put("lamp_intensity_after_open", lamp.intensity);
            Put("presentation_lines_on", ConsoleCount("on"));
            Capture(view, "c05_lamp_on");

            ArkusProbeAuthority.Request("j2.char.probe_author", "j2.door.c05", "door.close");
            yield return Frames(20);
            Put("trigger_runs_after_close", runs);
            Put("lamp_requests_total", requests.Count(r => r.Contains("|lamp.on|")));
            Put("adapter_requests", string.Join(" ; ", requests));
            Put("arkus_log", string.Join(" ; ", ArkusProbeAuthority.Log));
        }

        // ------------------------------------------------------------------ C06 save-host feasibility

        IEnumerator C06()
        {
            var mgr = SaveLoadManager.Instance;
            Put("gc2_storage_backend", mgr.DataStorage.GetType().FullName);
            ArkusProbeAuthority.Reset();
            ArkusProbeAuthority.Declare("j2.door.c06", "state", "closed");
            ArkusProbeAuthority.Declare("j2.chair.c06", "occupant", "");
            ArkusProbeAuthority.Declare("j2.mug.c06", "holder", "");
            string s0 = ArkusProbeAuthority.Digest();
            var host = new ArkusSaveHost();
            string lastLoad = "none";
            bool lastOk = false;
            int loads = 0;
            host.Loaded += (okLoad, why) => { lastOk = okLoad; lastLoad = why; loads++; };
            yield return Await(SaveLoadManager.Subscribe(host, 50));
            ArkusProbeAuthority.Request("j2.char.player", "j2.door.c06", "door.open");
            ArkusProbeAuthority.Request("j2.npc.probe_a", "j2.chair.c06", "seat.take");
            string d1 = ArkusProbeAuthority.Digest();
            Put("payload_schema", ArkusProbeAuthority.PayloadSchema);
            Put("payload_version", ArkusProbeAuthority.PayloadVersion);
            Put("digest_saved", d1);
            int marker = GameObject.Find("J2_SceneInstanceMarker").GetInstanceID();
            yield return Await(mgr.Save(1));
            Put("has_save_slot1", mgr.HasSaveAt(1));
            string key = Juego2WorkspaceStorage.Keys.FirstOrDefault(k => k.EndsWith(ArkusSaveHost.ID));
            Put("stored_key", key ?? "none");
            string raw = key != null ? Juego2WorkspaceStorage.RawGet(key) : "";
            Put("stored_payload_is_juego2_schema", raw.Contains(ArkusProbeAuthority.PayloadSchema));
            var storageFile = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../out/c06/gc2_storage.json"));
            Put("storage_file_written", File.Exists(storageFile));
            Put("storage_keys", string.Join(",", Juego2WorkspaceStorage.Keys.OrderBy(k => k)));

            ArkusProbeAuthority.Request("j2.char.player", "j2.mug.c06", "item.take");
            Put("digest_changed_after_save", ArkusProbeAuthority.Digest() != d1);
            yield return Await(mgr.Load(1), 30f);
            yield return Frames(15);
            Put("load_host_accepted", lastOk);
            Put("load_host_reason", lastLoad);
            Put("load_host_calls", loads);
            Put("digest_after_load", ArkusProbeAuthority.Digest());
            Put("roundtrip_digest_match", ArkusProbeAuthority.Digest() == d1);
            Put("roundtrip_not_reset_baseline", ArkusProbeAuthority.Digest() != s0);
            Put("mug_holder_after_load", ArkusProbeAuthority.Get("j2.mug.c06:holder"));
            var markerNow = GameObject.Find("J2_SceneInstanceMarker");
            Put("gc2_load_reloaded_scene", markerNow == null || markerNow.GetInstanceID() != marker);

            // negative control: a foreign payload version in GC2 storage must be refused by Arkus
            raw = Juego2WorkspaceStorage.RawGet(key) ?? "";
            Put("tamper_source_has_version_1", raw.Contains("\"version\":1"));
            Juego2WorkspaceStorage.RawSet(key, raw.Replace("\"version\":1", "\"version\":99"));
            ArkusProbeAuthority.Request("j2.char.player", "j2.mug.c06", "item.take");
            string beforeTamperLoad = ArkusProbeAuthority.Digest();
            loads = 0;
            yield return Await(mgr.Load(1), 30f);
            yield return Frames(15);
            Put("tampered_load_host_accepted", lastOk);
            Put("tampered_load_host_reason", lastLoad);
            Put("tampered_load_host_calls", loads);
            Put("tampered_digest_equals_saved", ArkusProbeAuthority.Digest() == d1);
            Put("tampered_digest_equals_reset_baseline", ArkusProbeAuthority.Digest() == s0);
            Put("tampered_digest_equals_pre_load", ArkusProbeAuthority.Digest() == beforeTamperLoad);
            Put("playerprefs_has_gc2_key", key != null && PlayerPrefs.HasKey(key));
            yield return Await(mgr.Delete(1));
            Put("has_save_slot1_after_delete", mgr.HasSaveAt(1));
        }

        // ------------------------------------------------------------------ C07 selected-stack coexistence

        IEnumerator C07()
        {
            ArkusProbeAuthority.Reset();
            ArkusProbeAuthority.Declare("j2.chair.c07", "occupant", "");
            ArkusProbeAuthority.Declare("j2.mug.c07", "holder", "");
            ArkusProbeAuthority.Declare("j2.door.c07", "state", "closed");
            ArkusProbeAuthority.Declare("j2.lamp.c07", "light", "off");
            HookRequests();
            var player = CharacterNamed("J2_Player");
            var npc = CharacterNamed("J2_NPC_A");
            var lamp = GameObject.Find("J2_Lamp").GetComponent<Light>();
            var interact = input.FindAction("Player/Interact", true);
            interact.performed += _ => player.Interaction.Interact();
            input.Enable();
            var host = new ArkusSaveHost();
            yield return Await(SaveLoadManager.Subscribe(host, 50));
            yield return Frames(20);
            OwnershipAudit("pre_");
            int playerSteps = 0, npcSteps = 0;
            player.Footsteps.EventStep += _ => playerSteps++;
            npc.Footsteps.EventStep += _ => npcSteps++;

            bool done = false, ok = false;
            int frames = 0;
            npc.Motion.MoveToLocation(new Location(npcGoal), 0.3f, (c, success) => { done = true; ok = success; });
            while (!done && frames < 30 * 30) { frames++; yield return null; }
            Put("npc_nav_success", ok);
            Put("npc_focus_at_seat", Target(npc));
            npc.Interaction.Interact();
            yield return Frames(10);
            Put("chair_after_npc", ArkusProbeAuthority.Get("j2.chair.c07:occupant"));
            _ = npc.Gestures.CrossFade(gestureClip, null, BlendMode.Blend, new ConfigGesture(0f, gestureClip.length, 1f, false, 0.15f, 0.2f), true);
            npc.IK.RequireRig<RigLookTo>().SetTarget(new LookToTransform(0, player.transform, Vector3.up * 0.7f));

            yield return Route(player.transform, "c07_route");

            ArkusProbeAuthority.Request("j2.char.player", "j2.door.c07", "door.open");
            yield return Frames(15);
            Put("lamp_fact_after_door", ArkusProbeAuthority.Get("j2.lamp.c07:light"));
            Put("lamp_intensity_after_door", lamp.intensity);
            Put("player_focus_at_route_end", Target(player));
            var pad = InputSystem.AddDevice<Gamepad>("H2F01AVirtualPad2");
            yield return Frames(3);
            yield return Press(pad);
            InputSystem.RemoveDevice(pad);
            Put("mug_after_player", ArkusProbeAuthority.Get("j2.mug.c07:holder"));
            Capture(view, "c07_route_end");

            var rag = npc.Ragdoll.StartRagdoll();
            yield return Await(rag, 5f);
            yield return Frames(45);
            Put("npc_ragdoll", npc.Ragdoll.IsRagdoll);
            yield return Await(npc.Ragdoll.StartRecover(), 10f);
            yield return Frames(20);
            Put("npc_recovered", !npc.Ragdoll.IsRagdoll);

            var mgr = SaveLoadManager.Instance;
            yield return Await(mgr.Save(1));
            string key = Juego2WorkspaceStorage.Keys.FirstOrDefault(k => k.EndsWith(ArkusSaveHost.ID));
            var stored = key != null ? JsonUtility.FromJson<Payload>(Juego2WorkspaceStorage.RawGet(key)) : null;
            var now = ArkusProbeAuthority.Snapshot();
            Put("save_payload_matches_arkus", stored != null && stored.keys.SequenceEqual(now.keys) && stored.values.SequenceEqual(now.values) && stored.log.SequenceEqual(now.log));
            yield return Await(mgr.Delete(1));

            Put("player_footstep_events", playerSteps);
            Put("npc_footstep_events", npcSteps);
            OwnershipAudit("post_");
            Put("adapter_requests", string.Join(" ; ", requests));
            Put("arkus_log", string.Join(" ; ", ArkusProbeAuthority.Log));
            Put("arkus_digest", ArkusProbeAuthority.Digest());
        }
    }
}
