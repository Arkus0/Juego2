using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.Validation;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    internal static class WorldValidationBindings
    {
        public static IReadOnlyList<CapabilityRoute> CreateRoutes(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            // Explicit validation is read-only but the authoritative session also owns commit.
            // Route it through the same HK04 attenuation facade used by plan/dry-run so these
            // handlers never carry the internal canonical committer themselves.
            var validation = (IWorldValidationService)new WorldMutationPlannerView(service);
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldValidationCurrentHandler(validation)),
                CapabilityRoute.FromHandler(new WorldValidationProposedHandler(validation))
            }.AsReadOnly();
        }
    }

    internal abstract class WorldValidationHandlerBase : ICanonicalCapabilityHandler
    {
        protected WorldValidationHandlerBase(IWorldValidationService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected IWorldValidationService Service { get; }

        public abstract CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request);

        protected static void ValidateArguments(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldValidationContract.CurrentName, "1.0")]
    internal sealed class WorldValidationCurrentHandler : WorldValidationHandlerBase
    {
        public WorldValidationCurrentHandler(IWorldValidationService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            ValidateArguments(context, request);
            return Service.ValidateCurrent(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldValidationContract.ProposedName, "1.0")]
    internal sealed class WorldValidationProposedHandler : WorldValidationHandlerBase
    {
        public WorldValidationProposedHandler(IWorldValidationService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            ValidateArguments(context, request);
            return Service.ValidateProposed(request);
        }
    }
}
