namespace Arkus.Harness.Runtime
{
    /// <summary>
    /// Marks a canonical handler that is public only inside an explicitly admitted higher-layer
    /// composition. H0 route-universe discovery deliberately excludes these handlers; the admitted
    /// composition must prove its own route completeness against the accepted admission policy.
    /// </summary>
    public interface IAdmittedCapabilityHandler : ICanonicalCapabilityHandler
    {
    }
}
