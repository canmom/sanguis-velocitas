using Latios;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace SV
{
    //[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    //[UpdateAfter(typeof(BeginInitializationEntityCommandBufferSystem))]
    //public partial class DebugRootSuperSystem : RootSuperSystem
    //{
    //    protected override void CreateSystems()
    //    {
    //        GetOrCreateAndAddUnmanagedSystem<LinkedEntityGroupDebugSystem>();
    //    }
    //}

    [BurstCompile]
    public partial struct LinkedEntityGroupDebugSystem : ISystem
    {
        LatiosWorldUnmanaged latiosWorld;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new Job
            {
                esil           = SystemAPI.GetEntityStorageInfoLookup(),
                sceneTagHandle = SystemAPI.GetSharedComponentTypeHandle<SceneTag>()
            }.ScheduleParallel();
        }

        [WithOptions(EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabledEntities)]
        [BurstCompile]
        partial struct Job : IJobEntity
        {
            [ReadOnly] public EntityStorageInfoLookup             esil;
            [ReadOnly] public SharedComponentTypeHandle<SceneTag> sceneTagHandle;

            public void Execute(Entity entity, in SceneTag sceneTag, in DynamicBuffer<LinkedEntityGroup> leg)
            {
                foreach (var l in leg)
                {
                    var chunk = esil[l.Value].Chunk;
                    if (!chunk.Has(sceneTagHandle))
                    {
                        FixedString4096Bytes output          = default;
                        var                  parentArchetype = esil[entity].Chunk.Archetype.GetComponentTypes();
                        foreach (var type in parentArchetype)
                        {
                            output.Append(type.ToFixedString());
                            output.Append('\n');
                        }
                        FixedString32Bytes linked = "linked";
                        output.Append(linked);
                        var linkedArchetype = chunk.Archetype.GetComponentTypes();
                        foreach (var type in linkedArchetype)
                        {
                            output.Append(type.ToFixedString());
                            output.Append('\n');
                        }
                        UnityEngine.Debug.Log($"Found entity in LEG which does not have a SceneTag. Parent:\n{output}");
                    }
                    else
                    {
                        var otherScene = chunk.GetSharedComponent(sceneTagHandle);
                        if (sceneTag.Equals(otherScene))
                            continue;

                        FixedString4096Bytes output          = default;
                        var                  parentArchetype = esil[entity].Chunk.Archetype.GetComponentTypes();
                        foreach (var type in parentArchetype)
                        {
                            output.Append(type.ToFixedString());
                            output.Append('\n');
                        }
                        FixedString32Bytes linked = "linked";
                        output.Append(linked);
                        var linkedArchetype = chunk.Archetype.GetComponentTypes();
                        foreach (var type in linkedArchetype)
                        {
                            output.Append(type.ToFixedString());
                            output.Append('\n');
                        }
                        UnityEngine.Debug.Log($"Found entity in LEG which has wrong SceneTag. Parent:\n{output}");
                    }
                }
            }
        }
    }
}

