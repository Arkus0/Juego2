using System;
using System.Collections.Generic;
using System.Linq;

namespace Arkus.DesignWorld
{
    public sealed class H1ProjectionLifecycleIssue
    {
        public H1ProjectionLifecycleIssue(string machineCode, string subjectId, string detail)
        {
            MachineCode = machineCode ?? throw new ArgumentNullException(nameof(machineCode));
            SubjectId = subjectId ?? string.Empty;
            Detail = detail ?? string.Empty;
        }

        public string MachineCode { get; }
        public string SubjectId { get; }
        public string Detail { get; }
    }

    public sealed class H1ProjectionLifecycleReport
    {
        internal H1ProjectionLifecycleReport(IEnumerable<H1ProjectionLifecycleIssue> issues)
        {
            Issues = new List<H1ProjectionLifecycleIssue>(
                    issues ?? throw new ArgumentNullException(nameof(issues)))
                .OrderBy(issue => issue.SubjectId, StringComparer.Ordinal)
                .ThenBy(issue => issue.MachineCode, StringComparer.Ordinal)
                .ThenBy(issue => issue.Detail, StringComparer.Ordinal)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<H1ProjectionLifecycleIssue> Issues { get; }
        public bool IsCurrent => Issues.Count == 0;
    }

    /// <summary>
    /// Computes whether a previously built H1 projection remains eligible for CTX USE.
    /// This is derived context currentness only; it never grants H1 product authority.
    /// </summary>
    public sealed class H1ProjectionLifecycleGuard
    {
        public H1ProjectionLifecycleReport ValidateCurrent(
            H1CatalogueDataset dataset,
            H1AcceptedAuthoritySources sources,
            string currentAcceptedH104CandidateSha,
            DesignProjectionVersion currentProjectionVersion)
        {
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));
            return ValidateCurrent(
                dataset.Projection,
                sources,
                currentAcceptedH104CandidateSha,
                currentProjectionVersion);
        }

        public H1ProjectionLifecycleReport ValidateCurrent(
            DesignWorldProjection projection,
            H1AcceptedAuthoritySources sources,
            string currentAcceptedH104CandidateSha,
            DesignProjectionVersion currentProjectionVersion)
        {
            if (projection == null) throw new ArgumentNullException(nameof(projection));
            if (sources == null) throw new ArgumentNullException(nameof(sources));
            if (string.IsNullOrWhiteSpace(currentAcceptedH104CandidateSha))
                throw new ArgumentException("Current accepted H1-04 identity cannot be empty.", nameof(currentAcceptedH104CandidateSha));
            if (currentProjectionVersion == null) throw new ArgumentNullException(nameof(currentProjectionVersion));

            var issues = new List<H1ProjectionLifecycleIssue>();
            if (!StringComparer.Ordinal.Equals(
                    currentAcceptedH104CandidateSha,
                    H1ProjectionManifest.AcceptedH104CandidateSha))
            {
                issues.Add(new H1ProjectionLifecycleIssue(
                    "h1.lifecycle_accepted_authority_stale",
                    "WP-H1-04",
                    "Current accepted H1-04 candidate differs from the candidate frozen into this derived projection adapter."));
            }

            try
            {
                H1AcceptedAuthorityParser.ValidateAcceptedSourceBlobs(sources);
            }
            catch (H1CatalogueProjectionException error)
            {
                issues.Add(new H1ProjectionLifecycleIssue(
                    "h1.lifecycle_source_stale",
                    error.SubjectId,
                    error.MachineCode + ": " + error.Detail));
                return new H1ProjectionLifecycleReport(issues);
            }

            List<H1RecordDefinition> definitions;
            H1MultiSourceAuthorityReader reader;
            StaticDesignAuthorityUniverse universe;
            try
            {
                definitions = new H1AcceptedAuthorityParser().Parse(sources);
                reader = new H1MultiSourceAuthorityReader(definitions, sources);
                universe = new StaticDesignAuthorityUniverse(definitions.Select(item => item.FactId));
            }
            catch (H1CatalogueProjectionException error)
            {
                issues.Add(new H1ProjectionLifecycleIssue(
                    "h1.lifecycle_authority_shape_stale",
                    error.SubjectId,
                    error.MachineCode + ": " + error.Detail));
                return new H1ProjectionLifecycleReport(issues);
            }

            var generic = new DesignWorldProjectionValidator().Validate(
                projection,
                universe,
                reader,
                currentProjectionVersion);
            foreach (var issue in generic.Issues)
            {
                issues.Add(new H1ProjectionLifecycleIssue(
                    issue.MachineCode == "dw.projection_version_stale"
                        ? "h1.lifecycle_projection_schema_stale"
                        : "h1.lifecycle_generic_projection_invalid",
                    issue.FactId,
                    issue.MachineCode + ": " + issue.Detail));
            }

            var semantic = new H1SourceAuthorityOracle().Validate(projection, sources);
            foreach (var issue in semantic.Issues)
            {
                issues.Add(new H1ProjectionLifecycleIssue(
                    "h1.lifecycle_semantic_corruption",
                    issue.FactId,
                    issue.MachineCode + ": " + issue.Detail));
            }

            return new H1ProjectionLifecycleReport(issues);
        }
    }
}
