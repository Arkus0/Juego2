using System;
using System.Collections.Generic;
using System.Linq;

namespace Arkus.DesignWorld
{
    public sealed class CityInteriorRelationOracle
    {
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
                        "Projected allocation fact '" + allocationId + "' is missing.",
                        CityDesignWorldProvider.ProgrammeSourcePath,
                        CityDesignWorldProvider.InteriorsSourcePath);
                }

                if (!facts.TryGetValue(depthId, out var depth))
                {
                    throw new CityInvariantException(
                        "city.interior_depth_projection_missing",
                        id,
                        "every CITY-02 I1-I3 subject must retain one projected depth fact",
                        "Projected depth fact '" + depthId + "' is missing.",
                        CityDesignWorldProvider.ProgrammeSourcePath,
                        CityDesignWorldProvider.InteriorsSourcePath);
                }

                var canonical = allocation.Relations
                    .Where(relation => StringComparer.Ordinal.Equals(relation.RelationType, "allocates-depth"))
                    .ToList();

                if (canonical.Count == 0)
                {
                    throw new CityInvariantException(
                        "city.interior_allocation_relation_missing",
                        id,
                        "each CITY-02 I1-I3 allocation must have exactly one allocates-depth relation to its own depth fact",
                        "Projected allocation '" + allocationId + "' has no allocates-depth relation to '" + depthId + "'.",
                        allocation.Provenance.SourcePath,
                        depth.Provenance.SourcePath);
                }

                if (canonical.Count != 1)
                {
                    throw new CityInvariantException(
                        "city.interior_allocation_relation_cardinality",
                        id,
                        "each CITY-02 I1-I3 allocation must have exactly one allocates-depth relation to its own depth fact",
                        "Projected allocation '" + allocationId + "' has " + canonical.Count + " allocates-depth relations; exactly one is required.",
                        allocation.Provenance.SourcePath,
                        depth.Provenance.SourcePath);
                }

                if (!StringComparer.Ordinal.Equals(canonical[0].TargetFactId, depthId))
                {
                    throw new CityInvariantException(
                        "city.interior_allocation_relation_target_invalid",
                        id,
                        "each CITY-02 I1-I3 allocation must have exactly one allocates-depth relation to its own depth fact",
                        "Projected allocation '" + allocationId + "' targets '" + canonical[0].TargetFactId + "' instead of required '" + depthId + "'.",
                        allocation.Provenance.SourcePath,
                        depth.Provenance.SourcePath);
                }
            }
        }
    }
}
