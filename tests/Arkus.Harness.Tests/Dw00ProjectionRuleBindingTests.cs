using System;
using System.Collections.Generic;
using System.Linq;
using Arkus.DesignWorld;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Dw00ProjectionRuleBindingTests
    {
        private static readonly DesignProjectionVersion VersionOne = new DesignProjectionVersion(1, "dw00-v1");

        [Fact]
        public void CurrentRuleRebuildRejectsCachedProjectionWhenFieldRuleChangesWithoutVersionBump()
        {
            var source = NeutralSource();
            var universe = NeutralUniverse();
            var originalReader = Reader(source, fieldValue: 10, relationType: "contains");
            var cached = new DesignWorldProjector().Build(universe, originalReader, VersionOne);
            var changedRuleReader = Reader(source, fieldValue: 11, relationType: "contains");

            var validation = new DesignWorldProjectionValidator().Validate(cached, universe, changedRuleReader, VersionOne);

            Assert.False(validation.IsValid);
            Assert.Contains(validation.Issues, issue => issue.MachineCode == "dw.projection_rules_stale");
            Assert.DoesNotContain(validation.Issues, issue => issue.MachineCode == "dw.projection_version_stale");
            Assert.DoesNotContain(validation.Issues, issue => issue.MachineCode.StartsWith("dw.provenance_", StringComparison.Ordinal));
        }

        [Fact]
        public void CurrentRuleRebuildRejectsCachedProjectionWhenRelationRuleChangesWithoutVersionBump()
        {
            var source = NeutralSource();
            var universe = NeutralUniverse();
            var originalReader = Reader(source, fieldValue: 10, relationType: "contains");
            var cached = new DesignWorldProjector().Build(universe, originalReader, VersionOne);
            var changedRuleReader = Reader(source, fieldValue: 10, relationType: "includes");

            var validation = new DesignWorldProjectionValidator().Validate(cached, universe, changedRuleReader, VersionOne);

            Assert.False(validation.IsValid);
            Assert.Contains(validation.Issues, issue => issue.MachineCode == "dw.projection_rules_stale");
            Assert.DoesNotContain(validation.Issues, issue => issue.MachineCode == "dw.projection_version_stale");
            Assert.DoesNotContain(validation.Issues, issue => issue.MachineCode.StartsWith("dw.provenance_", StringComparison.Ordinal));
        }

        private static StaticDesignAuthorityUniverse NeutralUniverse()
        {
            return new StaticDesignAuthorityUniverse(new[] { "root", "item-a" });
        }

        private static AnchoredTextAuthorityReader Reader(string source, int fieldValue, string relationType)
        {
            return new AnchoredTextAuthorityReader(
                "neutral-authority",
                "fixture/rule-binding.txt",
                source,
                new[]
                {
                    new AnchoredFactDefinition(
                        "root",
                        "neutral-root",
                        "root catalogue",
                        new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                        {
                            ["complete"] = DesignValue.Boolean(true)
                        },
                        new[] { new DesignRelation(relationType, "item-a") }),
                    new AnchoredFactDefinition(
                        "item-a",
                        "neutral-item",
                        "item-a alpha=10",
                        new Dictionary<string, DesignValue>(StringComparer.Ordinal)
                        {
                            ["value"] = DesignValue.Integer(fieldValue)
                        })
                });
        }

        private static string NeutralSource()
        {
            return "root catalogue\nitem-a alpha=10\n";
        }
    }
}
