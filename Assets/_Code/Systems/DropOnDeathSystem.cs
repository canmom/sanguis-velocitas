using Latios;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace SV
{
    [BurstCompile]
    public partial struct DropOnDeathSystem : ISystem
    {
        LatiosWorldUnmanaged latiosWorld;
        EntityQuery          m_newQuery;
        EntityQuery          m_deadQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();
            m_newQuery  = state.Fluent().With<DropOnDeath>(true).Without<DropOnDeathCleanup>().Build();
            m_deadQuery = state.Fluent().With<DropOnDeathCleanup>(true).Without<DropOnDeath>().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deadEntities     = m_deadQuery.ToEntityArray(state.WorldUpdateAllocator);
            int spawnedThisFrame = 0;
            foreach (var entity in deadEntities)
            {
                var transform = state.EntityManager.GetComponentData<DropOnDeathTransformCleanup>(entity).transform;
                foreach (var prefab in state.EntityManager.GetBuffer<DropOnDeathCleanup>(entity, true).ToNativeArray(Allocator.Temp))
                {
                    // On subscene unload, the prefabs may no longer exist.
                    if (!state.EntityManager.Exists(prefab.prefab))
                        continue;

                    spawnedThisFrame++;
                    var newSpawn                                                                       = state.EntityManager.Instantiate(prefab.prefab);
                    state.EntityManager.SetComponentData(newSpawn, new WorldTransform { worldTransform = transform });
                }
            }

            state.EntityManager.RemoveComponent(m_deadQuery,
                                                new TypePack<DropOnDeathCleanup, DropOnDeathTransformCleanup>());

            var newEntities = m_newQuery.ToEntityArray(state.WorldUpdateAllocator);
            state.EntityManager.AddComponent(m_newQuery,
                                             new TypePack<DropOnDeathCleanup, DropOnDeathTransformCleanup>());

            foreach (var entity in newEntities)
            {
                var prefabs = state.EntityManager.GetBuffer<DropOnDeath>(entity, true).Reinterpret<DropOnDeathCleanup>()
                              .AsNativeArray();
                state.EntityManager.GetBuffer<DropOnDeathCleanup>(entity, false).AddRange(prefabs);
                state.EntityManager.SetComponentData(entity,
                                                     new DropOnDeathTransformCleanup { transform = TransformQvvs.identity });
            }
        }
    }
}

