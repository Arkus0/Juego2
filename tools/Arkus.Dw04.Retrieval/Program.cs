using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using Arkus.DesignWorld;

// Read-only adapter over accepted DW-02/03 production query providers. No oracle
// or expected answer is imported into this route; it returns material + provenance.
if (args.Length < 3)
    throw new ArgumentException("Usage: <repository-root> city <id> | pa-fixture <paId> <key> | pa-finding <paId> <key>");

var root = Path.GetFullPath(args[0]);
var jsonOptions = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
string Read(string path) => File.ReadAllText(Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar)));
object Provenance(DesignAuthorityAnchor p) => new
{
    p.AuthorityId, p.SourcePath, p.Anchor, p.SourceDigest, p.AnchorDigest
};
object CompactPaProvenance(DesignAuthorityAnchor p) => new
{
    p.SourcePath, p.Anchor
};

if (args[1] == "city" && args.Length == 3)
{
    var source = Read(CityProductionQueryProvider.ProgrammeSourcePath);
    var dataset = new CityProductionQueryProvider().BuildAndValidate(source);
    var row = dataset.Queries.AllSubjects.Single(x => x.FactId == args[2]);
    var ledgerStart = source.IndexOf("## 4. District × location programme", StringComparison.Ordinal);
    var ledgerEnd = source.IndexOf("### 4.1 Required-domain coverage", ledgerStart, StringComparison.Ordinal);
    if (ledgerStart < 0 || ledgerEnd <= ledgerStart)
        throw new InvalidDataException("Accepted CITY-02 §4 ledger boundaries missing");
    var sourceRow = source.Substring(ledgerStart, ledgerEnd - ledgerStart).Split('\n')
        .Single(x => x.StartsWith("| `" + row.FactId + "` |", StringComparison.Ordinal));
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        row.FactId, row.District, row.Label, row.Importance,
        row.SpatialDepth, row.InteriorDepth, row.ProgrammeKind,
        SourceRow = sourceRow,
        Provenance = Provenance(row.Provenance)
    }, jsonOptions));
    return;
}

if ((args[1] == "pa-fixture" || args[1] == "pa-finding") && args.Length == 4)
{
    var sources = new PaAcceptedCorpusSources(
        Read(PaProjectionManifest.Pa01Path), Read(PaProjectionManifest.Pa02Path),
        Read(PaProjectionManifest.Pa03Path), Read(PaProjectionManifest.Pa04Path),
        Read(PaProjectionManifest.Pa05Path), Read(PaProjectionManifest.Pa05FixturesPath));
    var dataset = new PaDesignWorldProvider().BuildAndValidate(sources);
    PaCorpusQueryRecord row = args[1] == "pa-fixture"
        ? dataset.Queries.Fixture(args[2], args[3])
        : dataset.Queries.ByPa(args[2], "finding").Single(x => x.SourceKey == args[3]);

    // DW-04 uses PA queries as a compact navigation/provenance index and opens
    // exact accepted source bytes on demand. Deliberately do not inject the full
    // material/disposition body here; source truth remains in accepted PA files.
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        row.FactId, row.PaId, row.RecordKind, row.SourceKey,
        Provenance = CompactPaProvenance(row.Provenance)
    }, jsonOptions));
    return;
}

throw new ArgumentException("Unsupported retrieval query");
