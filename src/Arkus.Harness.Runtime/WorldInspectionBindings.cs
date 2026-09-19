using System;
using System.Collections.Generic;
using Arkus.Game.Authoring;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Runtime
{
    internal static class WorldInspectionBindings
    {
        public static IReadOnlyList<CapabilityRoute> CreateRoutes(IWorldInspectionService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return new List<CapabilityRoute>
            {
                CapabilityRoute.FromHandler(new WorldSummaryHandler(service)),
                CapabilityRoute.FromHandler(new WorldObjectGetHandler(service)),
                CapabilityRoute.FromHandler(new WorldObjectQueryHandler(service)),
                CapabilityRoute.FromHandler(new WorldReferenceQueryHandler(service)),
                CapabilityRoute.FromHandler(new WorldExtensionQueryHandler(service)),
                CapabilityRoute.FromHandler(new WorldExtensionReadHandler(service))
            }.AsReadOnly();
        }
    }

    internal abstract class WorldInspectionHandlerBase : ICanonicalCapabilityHandler
    {
        protected WorldInspectionHandlerBase(IWorldInspectionService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected IWorldInspectionService Service { get; }

        public abstract CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request);

        protected static void Validate(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldInspectionContract.SummaryName, "1.0")]
    internal sealed class WorldSummaryHandler : WorldInspectionHandlerBase
    {
        public WorldSummaryHandler(IWorldInspectionService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Service.Summary(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldInspectionContract.ObjectGetName, "1.0")]
    internal sealed class WorldObjectGetHandler : WorldInspectionHandlerBase
    {
        public WorldObjectGetHandler(IWorldInspectionService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Service.GetObject(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldInspectionContract.ObjectQueryName, "1.0")]
    internal sealed class WorldObjectQueryHandler : WorldInspectionHandlerBase
    {
        public WorldObjectQueryHandler(IWorldInspectionService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Service.QueryObjects(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldInspectionContract.ReferenceQueryName, "1.0")]
    internal sealed class WorldReferenceQueryHandler : WorldInspectionHandlerBase
    {
        public WorldReferenceQueryHandler(IWorldInspectionService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Service.QueryReferences(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldInspectionContract.ExtensionQueryName, "1.0")]
    internal sealed class WorldExtensionQueryHandler : WorldInspectionHandlerBase
    {
        public WorldExtensionQueryHandler(IWorldInspectionService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Service.QueryExtensions(request);
        }
    }

    [PublicCapabilityRoute("arkus.base", WorldInspectionContract.ExtensionReadName, "1.0")]
    internal sealed class WorldExtensionReadHandler : WorldInspectionHandlerBase
    {
        public WorldExtensionReadHandler(IWorldInspectionService service) : base(service) { }

        public override CapabilityInvocationResult Invoke(
            CapabilityInvocationContext context,
            IReadOnlyDictionary<string, object?> request)
        {
            Validate(context, request);
            return Service.ReadExtension(request);
        }
    }
}
