using Collider = Latios.Psyshock.Collider;
using Latios;
using Latios.Psyshock;
using Latios.Psyshock.Anna;
using Latios.Transforms;
using Physics = Latios.Psyshock.Physics;
using static Unity.Entities.SystemAPI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public partial struct CollectHealthDropSystem : ISystem
    {
        LatiosWorldUnmanaged           latiosWorld;
        BuildCollisionLayerTypeHandles handles;
        EntityQuery                    query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();
            handles     = new BuildCollisionLayerTypeHandles(ref state);
            query       = state.Fluent().With<HealthDrop>(true).WithEnabled<CanBeCollected>().PatchQueryForBuildingCollisionLayer().Build();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var settings = latiosWorld.GetPhysicsSettings();
            handles.Update(ref state);
            state.Dependency = Physics.BuildCollisionLayer(query, handles)
                               .WithSettings(settings.collisionLayerSettings)
                               .ScheduleParallel(out var layer, state.WorldUpdateAllocator, state.Dependency);
            new Job
            {
                layer            = layer,
                healthDropLookup = GetComponentLookup<HealthDrop>(true),
                dcb              = latiosWorld.syncPoint.CreateDestroyCommandBuffer(),
            }.Schedule();
        }

        [BurstCompile]
        partial struct Job : IJobEntity
        {
            [ReadOnly]
            public CollisionLayer layer;

            [ReadOnly]
            public ComponentLookup<HealthDrop> healthDropLookup;
            public DestroyCommandBuffer        dcb;

            public void Execute(ref DamageThisFrame health, in WorldTransform transform, in PreviousTransform previous, in Collider collider, in Player player)
            {
                var search = Physics.AabbFrom(collider, previous.worldTransform, transform.position);
                foreach (var hit in Physics.FindObjects(search, layer))
                {
                    if (Physics.DistanceBetween(collider, previous.worldTransform, hit.collider, hit.transform, 0f, out _))
                    {
                        AddHealth(hit.entity, ref health);
                        continue;
                    }
                    if (Physics.ColliderCast(collider, previous.worldTransform, transform.position, hit.collider, hit.transform, out _))
                    {
                        AddHealth(hit.entity, ref health);
                    }
                }
            }

            void AddHealth(Entity hitEntity, ref DamageThisFrame health)
            {
                var healthDrop  = healthDropLookup[hitEntity];
                health.heal    += healthDrop.amount;
                dcb.Add(hitEntity);
            }
        }
    }
}

