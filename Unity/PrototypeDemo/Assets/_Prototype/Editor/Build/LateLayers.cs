using UnityEngine;

namespace Proto.Build
{
    /// Layers 3..6 on top of terrain/rasantes/surfaces.
    public static class LateLayers
    {
        public static void Build(int maxLayer, Transform[] L)
        {
            var lay = DemoBuild.Lay; var tb = DemoBuild.Tb;
            Kit.Reset(); SignKit.Reset(); CharacterKit.Reset();
            if (maxLayer < 3) return;
            var hb = new HouseBuilder(tb, lay);
            var housesRoot = maxLayer >= 4 ? L[4] : L[3];
            foreach (var h in lay.Houses) hb.Build(h, housesRoot, maxLayer >= 4);
            hb.Plinths.Build("Plinths_Zocalos_Peldanos", L[3], true);
            Debug.Log("[Proto] houses built: " + lay.Houses.Count);
            if (maxLayer < 5) return;
            new Encounters(tb, lay, L[5]).BuildAll();
            Debug.Log("[Proto] encounters built");
            if (maxLayer < 6) return;
            new Dressing(tb, lay, L[6]).BuildAll();
            var bd = new Backdrop(tb, L[6]);
            bd.BuildMountains();
            bd.Forests(tb.Terrain);
            bd.DistantRoofs();
            Debug.Log("[Proto] dressing + backdrop built");
            new GameplayBuild(tb, lay, L[6]).Build();
            Debug.Log("[Proto] gameplay built");
        }
    }
}
