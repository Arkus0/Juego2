namespace Juego2.Foundation
{
    /// <summary>Project rendering-layer convention. Bit 0 is Unity's Default; bit 1 receives URP decals.</summary>
    public static class J2RenderingLayers
    {
        public const uint Default = 1u << 0;
        public const uint DecalReceiver = 1u << 1;
        public const string DecalReceiverName = "DecalReceiver";
    }
}
