using System;
using System.IO;
using System.Linq;
using Juego2.Arkus;
using Juego2.Foundation.Editor;
using NUnit.Framework;
using UnityEngine;

namespace Juego2.Foundation.Tests
{
    public sealed class J2FoundationBaselineTests
    {
        [Test]
        public void AdoptedBaselineVerifiesGreen()
        {
            var findings = J2FoundationBaseline.Verify();
            Assert.That(findings, Is.Empty, string.Join("\n", findings));
        }

        const string Manifest = "{\n  \"dependencies\": {\n{0}\n  }\n}\n";

        static string ManifestWith(Func<string, string, string> edit) => Manifest.Replace("{0}", string.Join(",\n",
            J2FoundationBaseline.DirectPackages.Select(p => edit(p.Key, p.Value)).Where(l => l != null)));

        static string LockWith(Func<string, string, int, string> edit, params string[] extra)
        {
            var rows = J2FoundationBaseline.DirectPackages.Select(p => edit(p.Key, p.Value, 0)).Where(l => l != null).Concat(extra);
            return "{\n  \"dependencies\": {\n" + string.Join(",\n", rows) + "\n  }\n}\n";
        }

        static string Row(string name, string version) => $"    \"{name}\": \"{version}\"";
        static string LockRow(string name, string version, int depth) =>
            $"    \"{name}\": {{\n      \"version\": \"{version}\",\n      \"depth\": {depth},\n      \"source\": \"registry\"\n    }}";

        [Test]
        public void PackageBaseline_ExactSetIsGreen()
        {
            Assert.That(J2PackageBaseline.Verify(ManifestWith(Row), LockWith(LockRow)), Is.Empty);
        }

        [Test]
        public void PackageBaseline_FloatingOrChangedVersionFails()
        {
            var manifest = ManifestWith((n, v) => Row(n, n == "com.unity.splines" ? "2.9.x" : v));
            var findings = J2PackageBaseline.Verify(manifest, LockWith(LockRow));
            Assert.That(findings, Has.Some.StartsWith("J2_PACKAGE_VERSION:com.unity.splines"));
            Assert.That(findings, Has.Some.StartsWith("J2_PACKAGE_NOT_EXACT:com.unity.splines"));
        }

        [Test]
        public void PackageBaseline_UnreviewedDirectOrMissingDirectFails()
        {
            var manifest = ManifestWith((n, v) => n == "com.unity.inputsystem" ? null : Row(n, v)).Replace("\n  }", ",\n    \"com.example.unreviewed\": \"1.0.0\"\n  }");
            var findings = J2PackageBaseline.Verify(manifest, LockWith(LockRow));
            Assert.That(findings, Has.Member("J2_PACKAGE_UNREVIEWED_DIRECT:com.example.unreviewed"));
            Assert.That(findings, Has.Member("J2_PACKAGE_MISSING_DIRECT:com.unity.inputsystem"));
        }

        [Test]
        public void PackageBaseline_NotAdmittedOrTransitiveDirectFails()
        {
            var locked = LockWith((n, v, d) => LockRow(n, v, n == "com.unity.collections" ? 1 : d), LockRow("com.unity.cinemachine", "3.1.7", 1));
            var findings = J2PackageBaseline.Verify(ManifestWith(Row), locked);
            Assert.That(findings, Has.Member("J2_PACKAGE_NOT_ADMITTED:com.unity.cinemachine"));
            Assert.That(findings, Has.Member("J2_LOCK_MISMATCH:com.unity.collections=2.6.8/1"));
        }

        static string TempProject(Action<string> populate)
        {
            var root = Path.Combine(Path.GetTempPath(), "j2-boundary-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "Assets"));
            populate(root);
            return root;
        }

        static void Write(string root, string rel, string text)
        {
            var path = Path.Combine(root, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, text);
        }

        const string GatedAdapter = "{ \"name\": \"Juego2.Gc2Adapter\", \"references\": [\"GameCreator.Runtime.Core\", \"Juego2.Arkus\"], \"defineConstraints\": [\"JUEGO2_GC2_CORE\"] }";

        [Test]
        public void AssemblyBoundary_OnlyGatedAdapterMayReferenceGameCreator()
        {
            var clean = TempProject(r => Write(r, "Assets/Juego2/Gc2Adapter/Runtime/A.asmdef", GatedAdapter));
            Assert.That(J2AssemblyBoundary.Verify(clean), Is.Empty);

            var leak = TempProject(r => Write(r, "Assets/Arkus/Story/Story.asmdef", "{ \"name\": \"Arkus.Story\", \"references\": [\"GameCreator.Runtime.Core\"] }"));
            Assert.That(J2AssemblyBoundary.Verify(leak), Has.Member("J2_GC2_L1_REFERENCE_OUTSIDE_ADAPTER:Assets/Arkus/Story/Story.asmdef"));

            var ungated = TempProject(r => Write(r, "Assets/Juego2/Gc2Adapter/Runtime/A.asmdef", GatedAdapter.Replace("JUEGO2_GC2_CORE", "OTHER")));
            Assert.That(J2AssemblyBoundary.Verify(ungated), Has.Member("J2_GC2_ADAPTER_NOT_GATED:Assets/Juego2/Gc2Adapter/Runtime/A.asmdef"));

            var guid = TempProject(r => Write(r, "Assets/Arkus/X/X.asmdef", "{ \"name\": \"X\", \"references\": [\"GUID:0123456789abcdef0123456789abcdef\"] }"));
            Assert.That(J2AssemblyBoundary.Verify(guid), Has.Member("J2_ASMDEF_GUID_REFERENCE:Assets/Arkus/X/X.asmdef"));
        }

        [Test]
        public void AssemblyBoundary_ArkusSourceMayNotNameGameCreator()
        {
            var leak = TempProject(r => Write(r, "Assets/Arkus/CITY/Door.cs", "using GameCreator.Runtime.Common;\nclass Door {}"));
            Assert.That(J2AssemblyBoundary.Verify(leak), Has.Member("J2_GC2_L1_SOURCE_NAMES_GAMECREATOR:Assets/Arkus/CITY/Door.cs"));
            Assert.That(J2AssemblyBoundary.CodeNamesGameCreator("var root = \"Assets/Plugins/GameCreator\"; // GameCreator"), Is.False);
            Assert.That(J2AssemblyBoundary.CodeNamesGameCreator("GameCreator.Runtime.Characters.Character c;"), Is.True);
        }

        sealed class TableAuthority : IArkusFactAuthority
        {
            public string state = "closed";
            public event Action<string, string> FactChanged;
            public string GetFact(string key) => key == "j2.door.test:state" ? state : null;

            public ArkusTransitionResult Request(ArkusTransitionRequest request)
            {
                if (request.Transition != "door.open" || request.Entity != "j2.door.test")
                    return ArkusTransitionResult.Refused("UNKNOWN_TRANSITION");
                if (state != "closed") return new ArkusTransitionResult(ArkusTransitionStatus.Conflict, "PRECONDITION:" + state, "j2.door.test:state", state);
                state = "open";
                FactChanged?.Invoke("j2.door.test:state", state);
                return new ArkusTransitionResult(ArkusTransitionStatus.Accepted, "ACCEPTED", "j2.door.test:state", state);
            }
        }

        [Test]
        public void ArkusFacts_FailClosedWithoutAuthority()
        {
            ArkusFacts.Bind(null);
            Assert.That(ArkusFacts.Get("j2.door.test:state"), Is.Null);
            var result = ArkusFacts.Request("j2.char.player", "j2.door.test", "door.open");
            Assert.That(result.Accepted, Is.False);
            Assert.That(result.Reason, Is.EqualTo(ArkusFacts.NoAuthority));
        }

        [Test]
        public void ArkusFacts_RelaysAuthorityDecisions()
        {
            var authority = new TableAuthority();
            string changed = null;
            Action<string, string> handler = (k, v) => changed = k + "=" + v;
            ArkusFacts.Bind(authority);
            ArkusFacts.FactChanged += handler;
            try
            {
                Assert.That(ArkusFacts.Request(null, "j2.door.test", "door.open").Reason, Is.EqualTo(ArkusFacts.UnboundRequester));
                Assert.That(ArkusFacts.Request("j2.char.player", "Door (Clone)", "door.open").Reason, Is.EqualTo(ArkusFacts.UnboundEntity));
                Assert.That(authority.state, Is.EqualTo("closed"), "a refused request must not change state");
                var accepted = ArkusFacts.Request("j2.char.player", "j2.door.test", "door.open");
                Assert.That(accepted.Status, Is.EqualTo(ArkusTransitionStatus.Accepted));
                Assert.That(changed, Is.EqualTo("j2.door.test:state=open"));
                var conflict = ArkusFacts.Request("j2.char.player", "j2.door.test", "door.open");
                Assert.That(conflict.Status, Is.EqualTo(ArkusTransitionStatus.Conflict));
                Assert.That(ArkusFacts.Has("j2.door.test:state", "open"), Is.True);
            }
            finally
            {
                ArkusFacts.FactChanged -= handler;
                ArkusFacts.Bind(null);
            }
        }

        [Test]
        public void EntityKeysAreJuego2SemanticKeysNotEngineIdentities()
        {
            Assert.That(ArkusEntityBinding.IsValidKey("j2.char.player"), Is.True);
            Assert.That(ArkusEntityBinding.IsValidKey("j2.npc.probe_a"), Is.True);
            foreach (var bad in new[] { null, "", "Player", "j2", "j2.", "j2.Char", "d0e5f7c1a2b34c5d9e8f7a6b5c4d3e2f", "j2.door:state" })
                Assert.That(ArkusEntityBinding.IsValidKey(bad), Is.False, bad);
            var go = new GameObject("unbound");
            try { Assert.That(ArkusEntityBinding.KeyOf(go), Is.Null); }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }
    }
}
