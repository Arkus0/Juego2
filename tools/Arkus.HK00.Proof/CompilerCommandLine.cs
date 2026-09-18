using System;
using System.Collections.Generic;
using System.IO;

namespace Arkus.HK00.Proof
{
    internal sealed class CompilerCommandLine
    {
        private readonly List<KeyValuePair<string, string>> options;

        private CompilerCommandLine(List<KeyValuePair<string, string>> options, IReadOnlyList<string> sources)
        {
            this.options = options;
            Sources = sources;
        }

        public IReadOnlyList<string> Sources { get; }

        public static CompilerCommandLine Parse(IEnumerable<string> arguments, string workingDirectory)
        {
            var parsedOptions = new List<KeyValuePair<string, string>>();
            var sources = new List<string>();
            foreach (var argument in arguments)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    continue;
                }

                var resolvedCandidate = Path.IsPathRooted(argument)
                    ? argument
                    : Path.Combine(workingDirectory, argument);

                if (File.Exists(resolvedCandidate))
                {
                    sources.Add(Path.GetFullPath(resolvedCandidate));
                    continue;
                }

                if (argument[0] != '/' && argument[0] != '-')
                {
                    sources.Add(Path.GetFullPath(resolvedCandidate));
                    continue;
                }

                var body = argument.Substring(1);
                var separator = body.IndexOf(':', StringComparison.Ordinal);
                if (separator < 0)
                {
                    parsedOptions.Add(new KeyValuePair<string, string>(body, string.Empty));
                }
                else
                {
                    parsedOptions.Add(new KeyValuePair<string, string>(body.Substring(0, separator), body.Substring(separator + 1)));
                }
            }

            sources.Sort(StringComparer.Ordinal);
            return new CompilerCommandLine(parsedOptions, sources);
        }

        public bool HasSwitch(string name)
        {
            foreach (var option in options)
            {
                if (string.Equals(option.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public string Value(string name)
        {
            var result = string.Empty;
            foreach (var option in options)
            {
                if (string.Equals(option.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    result = option.Value;
                }
            }

            return result;
        }

        public IReadOnlyList<string> Values(string name)
        {
            var results = new List<string>();
            foreach (var option in options)
            {
                if (string.Equals(option.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(option.Value);
                }
            }

            return results;
        }

        public IReadOnlyList<string> SuppressedWarnings()
        {
            var results = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var value in Values("nowarn"))
            {
                foreach (var token in value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    results.Add(token.Trim());
                }
            }

            return new List<string>(results);
        }
    }
}
