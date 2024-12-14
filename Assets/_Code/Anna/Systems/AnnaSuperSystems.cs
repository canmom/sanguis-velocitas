using Latios.Transforms.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Latios.Psyshock.Anna.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSuperSystem))]
    public partial class AnnaSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddUnmanagedSystem<BuildEnvironmentCollisionLayerSystem>();
            GetOrCreateAndAddUnmanagedSystem<BuildKinematicCollisionLayerSystem>();
            GetOrCreateAndAddUnmanagedSystem<BuildRigidBodyCollisionLayerSystem>();
            GetOrCreateAndAddUnmanagedSystem<RigidBodyVsRigidBodySystem>();
            GetOrCreateAndAddUnmanagedSystem<RigidBodyVsEnvironmentSystem>();
            GetOrCreateAndAddUnmanagedSystem<RigidBodyVsKinematicSystem>();
            GetOrCreateAndAddUnmanagedSystem<SolveSystem>();
            GetOrCreateAndAddUnmanagedSystem<IntegrateRigidBodiesSystem>();
        }
    }
}
