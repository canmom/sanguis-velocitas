using Latios;
using Latios.Psyshock;
using Latios.Psyshock.Anna;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Entities.SystemAPI;

namespace SV
{
    [BurstCompile]
    public partial struct AoEDamagePlayerSystem : ISystem
    {
        LatiosWorldUnmanaged           latiosWorld;
        BuildCollisionLayerTypeHandles handles;
        EntityQuery                    m_aoeQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();
            handles     = new BuildCollisionLayerTypeHandles(ref state);
            m_aoeQuery  = state.Fluent().With<AoE>(true).PatchQueryForBuildingCollisionLayer().Build();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var settings = latiosWorld.GetPhysicsSettings();
            handles.Update(ref state);
            state.Dependency = Physics.BuildCollisionLayer(m_aoeQuery, handles).WithSettings(settings.collisionLayerSettings)
                               .ScheduleParallel(out var layer, state.WorldUpdateAllocator, state.Dependency);
            new Job
            {
                layer     = layer,
                aoeLookup = GetComponentLookup<AoE>(true),
                dt        = Time.DeltaTime
            }.Schedule();
        }

        [BurstCompile]
        partial struct Job : IJobEntity
        {
            [ReadOnly] public CollisionLayer       layer;
            [ReadOnly] public ComponentLookup<AoE> aoeLookup;
            public float                           dt;

            public void Execute(ref Health health, in WorldTransform transform, in Collider collider)
            {
                var   search = Physics.AabbFrom(collider, transform.worldTransform);
                float damage = 0f;
                foreach (var hit in Physics.FindObjects(search, layer))
                {
                    if (Physics.DistanceBetween(collider, transform.worldTransform, hit.collider, hit.transform, 0f, out _))
                    {
                        damage += aoeLookup[hit.entity].dps;
                        //damage = math.max(damage, aoeLookup[hit.entity].dps);
                    }
                }

                // Subtract health, but only if we haven't finished the level yet. Otherwise, clamp it.
                if (health.health < health.goalHealth)
                    health.health -= damage * dt;
                else
                    health.health = health.goalHealth;
            }
        }
    }
}

