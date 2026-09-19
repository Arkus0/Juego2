using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Arkus.Harness.Protocol
{
    public sealed class JsonSchemaDocument
    {
        public const string Draft202012 = "https://json-schema.org/draft/2020-12/schema";

        public JsonSchemaDocument(SchemaNode root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
        }

        public string Dialect => Draft202012;
        public SchemaNode Root { get; }

        public IReadOnlyList<SchemaValidationIssue> ValidateDefinition()
        {
            var issues = new List<SchemaValidationIssue>();
            Root.ValidateDefinition("$", issues);
            return issues.AsReadOnly();
        }

        public IReadOnlyList<SchemaValidationIssue> ValidateValue(object? value)
        {
            var issues = new List<SchemaValidationIssue>();
            Root.ValidateValue(value, "$", issues);
            return issues.AsReadOnly();
        }

        public IReadOnlyDictionary<string, object?> ToData()
        {
            var data = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["$schema"] = Dialect
            };

            foreach (var pair in Root.ToData())
            {
                data[pair.Key] = pair.Value;
            }

            return new ReadOnlyDictionary<string, object?>(data);
        }

        public string SemanticFingerprint()
        {
            var builder = new StringBuilder();
            builder.Append(Dialect);
            builder.Append('|');
            Root.AppendFingerprint(builder);
            return builder.ToString();
        }
    }
}
