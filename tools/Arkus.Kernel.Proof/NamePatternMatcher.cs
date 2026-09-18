using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// Matches assembly or package names against simple <c>*</c> glob patterns.
    /// </summary>
    public sealed class NamePatternMatcher
    {
        private readonly List<Regex> patterns = new List<Regex>();

        /// <summary>Creates a matcher.</summary>
        /// <param name="globPatterns">Patterns where <c>*</c> matches any run of characters.</param>
        public NamePatternMatcher(IEnumerable<string> globPatterns)
        {
            if (globPatterns is null)
            {
                throw new ArgumentNullException(nameof(globPatterns));
            }

            foreach (var pattern in globPatterns)
            {
                patterns.Add(new Regex(ToRegex(pattern), RegexOptions.CultureInvariant | RegexOptions.IgnoreCase));
            }
        }

        /// <summary>Whether a name matches any pattern.</summary>
        /// <param name="name">Candidate name.</param>
        /// <returns><c>true</c> on a match.</returns>
        public bool IsMatch(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            foreach (var pattern in patterns)
            {
                if (pattern.IsMatch(name))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ToRegex(string globPattern)
        {
            if (string.IsNullOrEmpty(globPattern))
            {
                throw new ProofToolException("Empty forbidden-name pattern.");
            }

            var builder = new StringBuilder("^");
            foreach (var c in globPattern)
            {
                if (c == '*')
                {
                    builder.Append(".*");
                }
                else
                {
                    builder.Append(Regex.Escape(c.ToString()));
                }
            }

            return builder.Append('$').ToString();
        }
    }
}
