using System;
using System.Collections.Generic;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// A directed dependency graph over project names.
    /// </summary>
    /// <remarks>
    /// Kept free of MSBuild and file-system concerns so the cycle and edge rules
    /// can be unit-tested directly instead of only through a whole repository.
    /// </remarks>
    public sealed class DependencyGraph
    {
        private readonly SortedDictionary<string, SortedSet<string>> edges =
            new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);

        /// <summary>Creates a graph with the given nodes and no edges.</summary>
        /// <param name="nodes">Node names.</param>
        public DependencyGraph(IEnumerable<string> nodes)
        {
            if (nodes is null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            foreach (var node in nodes)
            {
                if (!edges.ContainsKey(node))
                {
                    edges[node] = new SortedSet<string>(StringComparer.Ordinal);
                }
            }
        }

        /// <summary>All node names, ordinal-sorted.</summary>
        public IReadOnlyCollection<string> Nodes => edges.Keys;

        /// <summary>Adds a directed edge.</summary>
        /// <param name="from">Dependent project.</param>
        /// <param name="to">Dependency.</param>
        public void AddEdge(string from, string to)
        {
            if (!edges.TryGetValue(from, out var targets))
            {
                targets = new SortedSet<string>(StringComparer.Ordinal);
                edges[from] = targets;
            }

            targets.Add(to);

            if (!edges.ContainsKey(to))
            {
                edges[to] = new SortedSet<string>(StringComparer.Ordinal);
            }
        }

        /// <summary>Direct dependencies of a node.</summary>
        /// <param name="node">Node name.</param>
        /// <returns>Ordinal-sorted dependency names.</returns>
        public IReadOnlyCollection<string> DependenciesOf(string node)
        {
            return edges.TryGetValue(node, out var targets)
                ? targets
                : (IReadOnlyCollection<string>)Array.Empty<string>();
        }

        /// <summary>
        /// Finds every simple cycle reachable in the graph.
        /// </summary>
        /// <returns>
        /// One canonical path per cycle, each ending at the node it started from.
        /// </returns>
        public IReadOnlyList<IReadOnlyList<string>> FindCycles()
        {
            var results = new List<IReadOnlyList<string>>();
            var seenSignatures = new HashSet<string>(StringComparer.Ordinal);
            var state = new Dictionary<string, int>(StringComparer.Ordinal);
            var stack = new List<string>();

            foreach (var node in edges.Keys)
            {
                state[node] = 0;
            }

            foreach (var node in edges.Keys)
            {
                if (state[node] == 0)
                {
                    Visit(node, state, stack, results, seenSignatures);
                }
            }

            return results;
        }

        private void Visit(
            string node,
            Dictionary<string, int> state,
            List<string> stack,
            List<IReadOnlyList<string>> results,
            HashSet<string> seenSignatures)
        {
            state[node] = 1;
            stack.Add(node);

            foreach (var next in edges[node])
            {
                if (!state.TryGetValue(next, out var nextState))
                {
                    continue;
                }

                if (nextState == 1)
                {
                    var start = stack.LastIndexOf(next);
                    if (start >= 0)
                    {
                        var cycle = new List<string>();
                        for (var i = start; i < stack.Count; i++)
                        {
                            cycle.Add(stack[i]);
                        }

                        cycle.Add(next);

                        var signature = string.Join(" -> ", cycle);
                        if (seenSignatures.Add(signature))
                        {
                            results.Add(cycle);
                        }
                    }
                }
                else if (nextState == 0)
                {
                    Visit(next, state, stack, results, seenSignatures);
                }
            }

            stack.RemoveAt(stack.Count - 1);
            state[node] = 2;
        }
    }
}
