using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.Validation;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    internal static class WorldValidationBindings
    {
        public static IReadOnlyList<CapabilityRoute> CreateRoutes(IWorldValidationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldValidationCurrentHandler(service)),
                CapabilityRoute.FromHandler(new WorldValidationProposedHandler(service))
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
