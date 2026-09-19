using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    /// <summary>
    /// Marker for effective public handlers that can commit canonical authorable state.
    /// It is executable-surface metadata, not a second capability registry.
    /// </summary>
    public interface ITransactionalMutationHandler : ICanonicalCapabilityHandler
    {
    }

    internal static class WorldMutationBindings
    {
        public static IReadOnlyList<CapabilityRoute> CreateRoutes(IWorldMutationService service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldChangePlanHandler(service)),
                CapabilityRoute.FromHandler(new WorldChangeDryRunHandler(service)),
                CapabilityRoute.FromHandler(new WorldChangeApplyHandler(service))
            }.AsReadOnly();
        }
    }

    internal abstract class WorldMutationHandlerBase : ICanonicalCapabilityHandler
    {
        protected WorldMutationHandlerBase(IWorldMutationService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected IWorldMutationService Service { get; }

        public abstract CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request);

        protected static void Validate(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldMutationContract.PlanName, "1.0")]
    internal sealed class WorldChangePlanHandler : WorldMutationHandlerBase
    {
        public WorldChangePlanHandler(IWorldMutationService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Service.Plan(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldMutationContract.DryRunName, "1.0")]
    internal sealed class WorldChangeDryRunHandler : WorldMutationHandlerBase
    {
        public WorldChangeDryRunHandler(IWorldMutationService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Service.DryRun(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldMutationContract.ApplyName, "1.0")]
    internal sealed class WorldChangeApplyHandler : ITransactionalMutationHandler
    {
        private readonly IWorldMutationService _service;

        public WorldChangeApplyHandler(IWorldMutationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
            return _service.Apply(request);
        }
    }
}
