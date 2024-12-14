using Latios.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Latios.Psyshock.Anna.Authoring
{
    public static class AnnaBakingBootstrap
    {
        public static void InstallAnnaUnityBakers(ref CustomBakingBootstrapContext context)
        {
            context.filteredBakerTypes.Add(typeof(UnityRigidBodyBaker));
        }
    }
}

