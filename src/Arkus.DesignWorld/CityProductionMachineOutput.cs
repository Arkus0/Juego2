using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkus.DesignWorld
{
    public sealed class CityContentShapeSubjectProvenance
    {
        internal CityContentShapeSubjectProvenance(DesignFact fact)
        {
            if (fact == null) throw new ArgumentNullException(nameof(fact));
            FactId = fact.FactId;
            SourcePath = fact.Provenance.SourcePath;
            Anchor = fact.Provenance.Anchor;
            SourceDigest = fact.Provenance.SourceDigest;
            AnchorDigest = fact.Provenance.AnchorDigest;
        }

        public string FactId { get; }
        public string SourcePath { get; }
        public string Anchor { get; }
        public string SourceDigest { get; }
        public string AnchorDigest { get; }
    }

    public sealed class CityContentShapeArtifact
    {
        internal CityContentShapeArtifact(
            CityContentShapeReport report,
            IEnumerable<CityContentShapeSubjectProvenance> subjects)
        {
            Report = report ?? throw new ArgumentNullException(nameof(report));
            if (subjects == null) throw new ArgumentNullException(nameof(subjects));
            Subjects = subjects
                .OrderBy(subject => subject.FactId, StringComparer.Ordinal)
                .ToList()
                .AsReadOnly();

            if (Subjects.Count != Report.TotalSubjects)
            {
                throw new CityProductionQueryException(
                    "city.report_provenance_universe_mismatch",
                    string.Empty,
                    "content-shape report provenance must cover every subject counted by the report",
                    "Report counts " + Report.TotalSubjects + " subjects but provenance covers " + Subjects.Count + ".",
                    CityProductionQueryProvider.ProgrammeSourcePath);
            }
        }

        public string SchemaId => "dw02-city-content-shape-artifact-v1";
        public string ManifestId => CityProductionProjectionManifest.ManifestId;
        public CityContentShapeReport Report { get; }
        public IReadOnlyList<CityContentShapeSubjectProvenance> Subjects { get; }

        public string ToNormalizedText()
        {
            var builder = new StringBuilder();
            builder.Append("artifact-schema=").Append(SchemaId).Append('\n');
            builder.Append("manifest=").Append(ManifestId).Append('\n');
            foreach (var subject in Subjects)
            {
                builder.Append("subject\t")
                    .Append(Escape(subject.FactId)).Append('\t')
                    .Append(Escape(subject.SourcePath)).Append('\t')
                    .Append(subject.SourceDigest).Append('\t')
                    .Append(subject.AnchorDigest).Append('\t')
                    .Append(Escape(subject.Anchor)).Append('\n');
            }
            builder.Append(Report.ToNormalizedText());
            return builder.ToString();
        }

        private static string Escape(string value) =>
            value.Replace("\\", "\\\\")
                .Replace("\t", "\\t")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
    }

    public static class CityProductionMachineOutput
    {
        public static CityContentShapeArtifact BuildContentShapeArtifact(CityProductionQueryDataset dataset)
        {
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));
            var report = dataset.Queries.BuildContentShapeReport();
            var subjects = dataset.Projection.Facts
                .Select(fact => new CityContentShapeSubjectProvenance(fact));
            return new CityContentShapeArtifact(report, subjects);
        }

        public static string BuildNormalizedSnapshot(CityProductionQueryDataset dataset)
        {
            if (dataset == null) throw new ArgumentNullException(nameof(dataset));
            var builder = new StringBuilder();
            builder.Append("machine-schema=dw02-city-production-output-v1\n");
            builder.Append(BuildContentShapeArtifact(dataset).ToNormalizedText());
            builder.Append(dataset.Queries.BuildNormalizedSnapshot());
            return builder.ToString();
        }
    }
}
