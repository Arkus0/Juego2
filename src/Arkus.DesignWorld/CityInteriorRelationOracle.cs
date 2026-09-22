using System;
using System.Collections.Generic;
using System.Linq;

namespace Arkus.DesignWorld
{
    public sealed class CityInteriorRelationOracle
    {
        private const string RelationType = "allocates-depth";
        private const string RelationRule =
            "each CITY-02 I1-I3 allocation must have exactly one allocates-depth relation to its own depth fact";

        public void Validate(
            DesignWorldProjection interiors,
            IReadOnlyCollection<string> expectedInteriorIds)
        {
            if (interiors == null)
            {
                throw new ArgumentNullException(nameof(interiors));
            }
            if (expectedInteriorIds == null)
            {
                throw new ArgumentNullException(nameof(expectedInteriorIds));
            }

            var facts = interiors.Facts.ToDictionary(fact => fact.FactId, StringComparer.Ordinal);
            foreach (var id in expectedInteriorIds.OrderBy(value => value, StringComparer.Ordinal))
            {
                var allocationId = "allocation." + id;
                var depthId = "depth." + id;

                if (!facts.TryGetValue(allocationId, out var allocation))
                {
                    throw new CityInvariantException(
                        "city.interior_allocation_projection_missing",
                        id,
                        "every CITY-02 I1-I3 subject must retain one projected allocation fact",
                        "Expected projected allocation fact '" + allocationId +
                        "' with relation '" + RelationType + "' -> '" + depthId + "'; observed allocation fact: <missing>.",
                        CityDesignWorldProvider.ProgrammeSourcePath,
                        CityDesignWorldProvider.InteriorsSourcePath);
                }

                if (!facts.TryGetValue(depthId, out var depth))
                {
                    throw new CityInvariantException(
                        "city.interior_depth_projection_missing",
                        id,
                        "every CITY-02 I1-I3 subject must retain one projected depth fact",
                        "Expected projected depth fact '" + depthId +
                        "' as target of '" + RelationType + "'; observed depth fact: <missing>.",
                        CityDesignWorldProvider.ProgrammeSourcePath,
                        allocation.Provenance.SourcePath,
                        CityDesignWorldProvider.InteriorsSourcePath);
                }

                var canonical = allocation.Relations
                    .Where(relation => StringComparer.Ordinal.Equals(relation.RelationType, RelationType))
                    .OrderBy(relation => relation.TargetFactId, StringComparer.Ordinal)
                    .ToList();

                if (canonical.Count == 0)
                {
                    throw new CityInvariantException(
                        "city.interior_allocation_relation_missing",
                        id,
                        RelationRule,
                        "Expected exactly one '" + RelationType + "' -> '" + depthId +
                        "' on projected allocation '" + allocationId + "'; observed relations: " +
                        DescribeRelations(allocation.Relations) + ".",
                        CityDesignWorldProvider.ProgrammeSourcePath,
                        allocation.Provenance.SourcePath,
                        depth.Provenance.SourcePath);
                }

                if (canonical.Count != 1)
                {
                    throw new CityInvariantException(
                        "city.interior_allocation_relation_cardinality",
                        id,
                        RelationRule,
                        "Expected exactly one '" + RelationType + "' -> '" + depthId +
                        "' on projected allocation '" + allocationId + "'; observed " + canonical.Count +
                        " canonical relations with targets " + DescribeTargets(canonical) +
                        "; all observed relations: " + DescribeRelations(allocation.Relations) + ".",
                        CityDesignWorldProvider.ProgrammeSourcePath,
                        allocation.Provenance.SourcePath,
                        depth.Provenance.SourcePath);
                }

                if (!StringComparer.Ordinal.Equals(canonical[0].TargetFactId, depthId))
                {
                    throw new CityInvariantException(
                        "city.interior_allocation_relation_target_invalid",
                        id,
                        RelationRule,
                        "Expected '" + RelationType + "' -> '" + depthId +
                        "' on projected allocation '" + allocationId + "'; observed '" + RelationType +
                        "' -> '" + canonical[0].TargetFactId + "'.",
                        CityDesignWorldProvider.ProgrammeSourcePath,
                        allocation.Provenance.SourcePath,
                        depth.Provenance.SourcePath);
                }
            }
        }

        private static string DescribeRelations(IEnumerable<DesignRelation> relations)
        {
            var observed = relations
                .OrderBy(relation => relation.RelationType, StringComparer.Ordinal)
                .ThenBy(relation => relation.TargetFactId, StringComparer.Ordinal)
                .Select(relation => "'" + relation.RelationType + "' -> '" + relation.TargetFactId + "'")
                .ToList();
            return observed.Count == 0 ? "<none>" : "[" + string.Join(", ", observed) + "]";
        }

        private static string DescribeTargets(IEnumerable<DesignRelation> relations)
        {
            var targets = relations
                .Select(relation => "'" + relation.TargetFactId + "'")
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToList();
            return targets.Count == 0 ? "<none>" : "[" + string.Join(", ", targets) + "]";
        }
    }
}
