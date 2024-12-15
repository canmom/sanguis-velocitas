using Latios;
using Latios.Psyshock.Anna.Systems;
using Latios.Systems;
using Latios.Transforms;
using Latios.Transforms.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace SV
{
    // Spawning structural changes can go here.
    [UpdateInGroup(typeof(LatiosWorldSyncGroup))]
    public partial class SpawnRootSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            //
        }
    }

    // Reactive systems that do structural changes can go here.
    [UpdateInGroup(typeof(LatiosWorldSyncGroup), OrderLast = true)]
    public partial class StructuralReactiveRootSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddUnmanagedSystem<DropOnDeathSystem>();
        }
    }

    // Systems that don't do structural changes, but are responsible for initializing
    // newly spawned entities can go here.
    [UpdateInGroup(typeof(PostSyncPointGroup), OrderFirst = true)]
    public partial class InitializeSpawnsRootSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddManagedSystem<TransformSuperSystem>();
            GetOrCreateAndAddUnmanagedSystem<ResetDamageThisFrameSystem>();
        }
    }

    // Systems which apply movement, physics, and animation can go here.
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AnnaSuperSystem))]
    public partial class PrePhysicsRootSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddManagedSystem<PlayerControlSystem>();
            GetOrCreateAndAddUnmanagedSystem<EnemyFollowsPlayerSystem>();
        }
    }

    // Systems which react to events and other update game logic can go here.
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSuperSystem))]
    public partial class PostTransformsRootSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddUnmanagedSystem<DropOnDeathCopyTransformSystem>();
            GetOrCreateAndAddUnmanagedSystem<CollectHealthDropSystem>();
            GetOrCreateAndAddUnmanagedSystem<ActivateHealthDropSystem>();
            GetOrCreateAndAddUnmanagedSystem<PlayerKillEnemeySystem>();
            GetOrCreateAndAddUnmanagedSystem<AoEDamagePlayerSystem>();
            GetOrCreateAndAddUnmanagedSystem<ApplyDamageToHealthSystem>();
            GetOrCreateAndAddUnmanagedSystem<PlayerThrustSoundSystem>();
            GetOrCreateAndAddUnmanagedSystem<UiSystem>();
        }
    }
}

