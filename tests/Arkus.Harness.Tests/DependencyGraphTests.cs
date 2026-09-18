using System.Collections.Generic;
using Arkus.Kernel.Proof;
using Xunit;

namespace Arkus.Harness.Tests
{
    /// <summary>
    /// The cycle guard is itself guarded: a check that cannot fail proves nothing.
    /// </summary>
    public sealed class DependencyGraphTests
    {
        [Fact]
        public void AcyclicGraphHasNoCycles()
        {
            var graph = new DependencyGraph(new[] { "A", "B", "C" });
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("A", "C");

            Assert.Empty(graph.FindCycles());
        }

        [Fact]
        public void BackEdgeIsReportedAsCycle()
        {
            var graph = new DependencyGraph(new[] { "A", "B" });
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "A");

            var cycles = graph.FindCycles();

            Assert.Single(cycles);
            Assert.Contains("A", cycles[0]);
            Assert.Contains("B", cycles[0]);
        }

        [Fact]
        public void SelfReferenceIsReportedAsCycle()
        {
            var graph = new DependencyGraph(new[] { "A" });
            graph.AddEdge("A", "A");

            Assert.Single(graph.FindCycles());
        }

        [Fact]
        public void LongCycleIsReported()
        {
            var graph = new DependencyGraph(new[] { "A", "B", "C", "D" });
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("C", "D");
            graph.AddEdge("D", "B");

            var cycles = graph.FindCycles();

            Assert.Single(cycles);
            Assert.Equal(new List<string> { "B", "C", "D", "B" }, cycles[0]);
        }

        [Fact]
        public void DependenciesAreOrdinalSorted()
        {
            var graph = new DependencyGraph(new[] { "A" });
            graph.AddEdge("A", "Zeta");
            graph.AddEdge("A", "Alpha");

            Assert.Equal(new[] { "Alpha", "Zeta" }, graph.DependenciesOf("A"));
        }
    }
}
