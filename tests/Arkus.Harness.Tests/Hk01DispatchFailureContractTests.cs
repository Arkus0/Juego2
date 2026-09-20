using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arkus.Harness.Protocol;
using Arkus.Harness.Runtime;
using Xunit;

namespace Arkus.Harness.Tests
{
    public sealed class Hk01DispatchFailureContractTests
    {
        [Fact]
        public void HandlerExceptionBeforePublicationBecomesDefinedNonRetryableCanonicalFailure()
        {
            var contract = ContractWith(new ThrowBeforePublicationHandler());

            var result = contract.Dispatch(
                "system.describe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                Empty(),
                InvocationResourceBudget.Start());

            AssertFailure(
                result,
                "contract.handler_failure",
                retryable: false,
                publicationCommitted: false,
                "HK01_BEFORE_PUBLICATION_SENTINEL");
        }

        [Fact]
        public void HandlerExceptionAfterPublicationBecomesDefinedRetryableRecoveryFailure()
        {
            var contract = ContractWith(new ThrowAfterPublicationHandler());

            var result = contract.Dispatch(
                "system.describe",
                ContractVersionRange.Exact(new ContractVersion(1, 0)),
                Empty(),
                InvocationResourceBudget.Start());

            AssertFailure(
                result,
                "contract.handler_failure_after_publication",
                retryable: true,
                publicationCommitted: true,
                "HK01_AFTER_PUBLICATION_SENTINEL");
            Assert.Contains(
                "Inspect the current canonical anchor",
                result.Error!.RepairHint ?? string.Empty,
                StringComparison.Ordinal);
        }

        private static ComposedContract ContractWith(ICanonicalCapabilityHandler handler)
        {
            var baseContribution = BaseContract.CreateContribution();
            var definition = Assert.Single(baseContribution.Definitions.Where(candidate =>
                string.Equals(candidate.Key.Name, "system.describe", StringComparison.Ordinal) &&
                candidate.Key.Version.Equals(new ContractVersion(1, 0))));
            var definitions = new Dictionary<CapabilityKey, CapabilityDefinition>
            {
                [definition.Key] = definition
            };
            var routes = new Dictionary<CapabilityKey, CapabilityRoute>
            {
                [definition.Key] = new CapabilityRoute("arkus.base", definition.Key, handler)
            };

            var constructor = Assert.Single(typeof(ComposedContract).GetConstructors(
                BindingFlags.Instance | BindingFlags.NonPublic));
            return Assert.IsType<ComposedContract>(constructor.Invoke(new object[] { definitions, routes }));
        }

        private static void AssertFailure(
            CapabilityInvocationResult result,
            string machineCode,
            bool retryable,
            bool publicationCommitted,
            string internalSentinel)
        {
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(machineCode, result.Error!.MachineCode);
            Assert.Equal("$", result.Error.Path);
            Assert.Equal(retryable, result.Error.Retryable);
            Assert.Equal("system.describe@1.0", result.Error.Context["capability"]);
            Assert.Equal("System.InvalidOperationException", result.Error.Context["exceptionType"]);
            Assert.Equal(publicationCommitted, (bool)result.Error.Context["publicationCommitted"]!);
            Assert.DoesNotContain(internalSentinel, result.Error.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(internalSentinel, result.Error.RepairHint ?? string.Empty, StringComparison.Ordinal);
        }

        private static IReadOnlyDictionary<string, object?> Empty()
        {
            return new Dictionary<string, object?>(StringComparer.Ordinal);
        }

        private sealed class ThrowBeforePublicationHandler : ICanonicalCapabilityHandler
        {
            public CapabilityInvocationResult Invoke(
                CapabilityInvocationContext context,
                IReadOnlyDictionary<string, object?> request)
            {
                throw new InvalidOperationException("HK01_BEFORE_PUBLICATION_SENTINEL");
            }
        }

        private sealed class ThrowAfterPublicationHandler : ICanonicalCapabilityHandler
        {
            public CapabilityInvocationResult Invoke(
                CapabilityInvocationContext context,
                IReadOnlyDictionary<string, object?> request)
            {
                context.ResourceBudget.MarkPublicationCommitted();
                throw new InvalidOperationException("HK01_AFTER_PUBLICATION_SENTINEL");
            }
        }
    }
}
