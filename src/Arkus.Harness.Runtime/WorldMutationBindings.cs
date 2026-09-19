using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Game.Validation;
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

            // Plan/dry-run receive an attenuated facade whose runtime object is not the
            // authoritative session. Only apply receives the Authoring-owned commit capability.
            var planner = new WorldMutationPlannerView(service);
            var committer = CanonicalWorldMutationAuthority.Bind(service);
            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldChangePlanHandler(planner)),
                CapabilityRoute.FromHandler(new WorldChangeDryRunHandler(planner)),
                CapabilityRoute.FromHandler(new WorldChangeApplyHandler(committer))
            }.AsReadOnly();
        }
    }

    /// <summary>
    /// Capability attenuation boundary: exposes only non-committing authoring operations and
    /// deliberately does not expose or implement canonical commit authority even when the wrapped
    /// service does. HK05 reuses the same accepted attenuation boundary for explicit validation.
    /// </summary>
    internal sealed class WorldMutationPlannerView : IWorldMutationService, IWorldValidationService
    {
        private readonly IWorldMutationService _service;
        private readonly IWorldValidationService _validation;

        public WorldMutationPlannerView(IWorldMutationService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _validation = service as IWorldValidationService ?? new UnavailableWorldValidationService();
        }

        public CapabilityInvocationResult Plan(IReadOnlyDictionary<string, object?> request)
        {
            return _service.Plan(request);
        }

        public CapabilityInvocationResult DryRun(IReadOnlyDictionary<string, object?> request)
        {
            return _service.DryRun(request);
        }

        public CapabilityInvocationResult ValidateCurrent(IReadOnlyDictionary<string, object?> request)
        {
            return _validation.ValidateCurrent(request);
        }

        public CapabilityInvocationResult ValidateProposed(IReadOnlyDictionary<string, object?> request)
        {
            return _validation.ValidateProposed(request);
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
        private readonly ICanonicalWorldMutationCommitter _committer;

        public WorldChangeApplyHandler(ICanonicalWorldMutationCommitter committer)
        {
            _committer = committer ?? throw new ArgumentNullException(nameof(committer));
        }

        public CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (request == null) throw new ArgumentNullException(nameof(request));
            return _committer.Apply(request);
        }
    }
}
