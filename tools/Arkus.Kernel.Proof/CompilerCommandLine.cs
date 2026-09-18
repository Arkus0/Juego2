using System;
using System.Collections.Generic;
using System.IO;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// The command line the C# compiler was actually invoked with.
    /// </summary>
    /// <remarks>
    /// Evaluated MSBuild properties describe what the build resolves before any
    /// target runs; a target can still change them afterwards. The compiler
    /// command line is the last word on language version, warnings-as-errors,
    /// suppressions, references and source files, so it is the oracle and the
    /// evaluated properties are defence in depth around it.
    /// </remarks>
    public sealed class CompilerCommandLine
    {
        private readonly List<KeyValuePair<string, string>> options;

        private CompilerCommandLine(
            List<KeyValuePair<string, string>> options,
            IReadOnlyList<string> sources)
        {
            this.options = options;
            Sources = sources;
        }

        /// <summary>Absolute paths of every source file passed to the compiler.</summary>
        public IReadOnlyList<string> Sources { get; }

        /// <summary>Parses raw compiler arguments.</summary>
        /// <param name="arguments">Arguments in command-line order.</param>
        /// <param name="workingDirectory">Directory that relative source paths resolve against.</param>
        /// <returns>The parsed command line.</returns>
        public static CompilerCommandLine Parse(IEnumerable<string> arguments, string workingDirectory)
        {
            if (arguments is null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            if (workingDirectory is null)
            {
                throw new ArgumentNullException(nameof(workingDirectory));
            }

            var parsedOptions = new List<KeyValuePair<string, string>>();
            var sources = new List<string>();

            foreach (var argument in arguments)
            {
                if (argument.Length == 0)
                {
                    continue;
                }

                if (argument[0] != '/' || File.Exists(Path.Combine(workingDirectory, argument)))
                {
                    sources.Add(Path.GetFullPath(Path.Combine(workingDirectory, argument)));
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
                    parsedOptions.Add(
                        new KeyValuePair<string, string>(
                            body.Substring(0, separator),
                            body.Substring(separator + 1)));
                }
            }

            sources.Sort(StringComparer.Ordinal);
            return new CompilerCommandLine(parsedOptions, sources);
        }

        /// <summary>Whether a switch such as <c>warnaserror+</c> is present.</summary>
        /// <param name="name">Switch name including any trailing sign.</param>
        /// <returns><c>true</c> when present.</returns>
        public bool HasSwitch(string name)
        {
            foreach (var option in options)
            {
                if (string.Equals(option.Key, name, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Reads the last value of a named option, such as <c>langversion</c>.</summary>
        /// <param name="name">Option name.</param>
        /// <returns>The value, or an empty string.</returns>
        public string Value(string name)
        {
            var result = string.Empty;
            foreach (var option in options)
            {
                if (string.Equals(option.Key, name, StringComparison.Ordinal))
                {
                    result = option.Value;
                }
            }

            return result;
        }

        /// <summary>Reads every value of a repeatable option, such as <c>reference</c>.</summary>
        /// <param name="name">Option name.</param>
        /// <returns>The values in command-line order.</returns>
        public IReadOnlyList<string> Values(string name)
        {
            var results = new List<string>();
            foreach (var option in options)
            {
                if (string.Equals(option.Key, name, StringComparison.Ordinal))
                {
                    results.Add(option.Value);
                }
            }

            return results;
        }

        /// <summary>Reads every warning identifier suppressed on the command line.</summary>
        /// <returns>Ordinal-sorted warning identifiers.</returns>
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
