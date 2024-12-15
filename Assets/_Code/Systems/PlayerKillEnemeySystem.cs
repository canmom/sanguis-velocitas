using Latios;
using Latios.Psyshock;
using Latios.Psyshock.Anna;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace SV
{
    [BurstCompile]
    public partial struct PlayerKillEnemeySystem : ISystem
    {
        LatiosWorldUnmanaged           latiosWorld;
        BuildCollisionLayerTypeHandles handles;
        EntityQuery                    m_enemyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld  = state.GetLatiosWorldUnmanaged();
            handles      = new BuildCollisionLayerTypeHandles(ref state);
            m_enemyQuery = state.Fluent().With<EnemyTag>(true).PatchQueryForBuildingCollisionLayer().Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var settings = latiosWorld.GetPhysicsSettings();
            handles.Update(ref state);
            state.Dependency = Physics.BuildCollisionLayer(m_enemyQuery, handles).WithSettings(settings.collisionLayerSettings)
                               .ScheduleParallel(out var layer, state.WorldUpdateAllocator, state.Dependency);
            new Job { enemyLayer = layer, dcb = latiosWorld.syncPoint.CreateDestroyCommandBuffer() }.Schedule();
        }

        [WithAll(typeof(PlayerAttackColliderTag))]
        [BurstCompile]
        partial struct Job : IJobEntity
        {
            [ReadOnly] public CollisionLayer enemyLayer;
            public DestroyCommandBuffer      dcb;

            public void Execute(in WorldTransform worldTransform, in PreviousTransform previousTransform, in Collider collider)
            {
                var search = Physics.AabbFrom(collider, previousTransform.worldTransform, worldTransform.position);
                foreach (var hit in Physics.FindObjects(search, enemyLayer))
                {
                    if (Physics.DistanceBetween(collider, previousTransform.worldTransform, hit.collider, hit.transform, 0f, out _))
                    {
                        dcb.Add(hit.entity);
                        continue;
                    }
                    if (Physics.ColliderCast(collider, previousTransform.worldTransform, worldTransform.position, hit.collider, hit.transform, out _))
                    {
                        dcb.Add(hit.entity);
                    }
                }
            }
        }
    }
}

