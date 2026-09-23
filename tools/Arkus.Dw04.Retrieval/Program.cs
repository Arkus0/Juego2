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

if (args[1] == "city" && args.Length == 3)
{
    var source = Read(CityProductionQueryProvider.ProgrammeSourcePath);
    var dataset = new CityProductionQueryProvider().BuildAndValidate(source);
    var row = dataset.Queries.AllSubjects.Single(x => x.FactId == args[2]);
    var sourceRow = source.Split('\n').Single(x => x.StartsWith("| `" + row.FactId + "` |", StringComparison.Ordinal));
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
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        row.FactId, row.PaId, row.RecordKind, row.SourceKey,
        row.MaterialText, row.DispositionText,
        Provenance = Provenance(row.Provenance)
    }, jsonOptions));
    return;
}

throw new ArgumentException("Unsupported retrieval query");
