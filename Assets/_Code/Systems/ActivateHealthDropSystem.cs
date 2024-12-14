using Latios;
using Latios.Psyshock;
using Latios.Psyshock.Anna;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static Unity.Entities.SystemAPI;
using Collider = Latios.Psyshock.Collider;
using Physics = Latios.Psyshock.Physics;

namespace SV
{
    public partial struct ActivateHealthDropSystem : ISystem
    {
        private LatiosWorldUnmanaged _latiosWorld;
        private BuildCollisionLayerTypeHandles _handles;
        private EntityQuery _healthDropQuery;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _latiosWorld = state.GetLatiosWorldUnmanaged();
            _handles = new BuildCollisionLayerTypeHandles(ref state);
            _healthDropQuery = state.Fluent().With<HealthDrop>(true)
                                    .WithDisabled<CanBeCollected>().PatchQueryForBuildingCollisionLayer()
                                    .Build();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var settings = _latiosWorld.GetPhysicsSettings();
            _handles.Update(ref state);
            state.Dependency = Physics.BuildCollisionLayer(_healthDropQuery, _handles)
                                      .WithSettings(settings.collisionLayerSettings)
                                      .ScheduleParallel(out var layer, state.WorldUpdateAllocator, state.Dependency);
            new Job
            {
                layer = layer,
                canBeCollectedLookup = GetComponentLookup<CanBeCollected>(),
            }.Schedule();
        }


        [BurstCompile]
        partial struct Job : IJobEntity
        {
            [ReadOnly]
            public CollisionLayer layer;

            public ComponentLookup<CanBeCollected> canBeCollectedLookup;


            public void Execute(ref Health health, in WorldTransform transform, in Collider collider, in Player player)
            {
                var search = Physics.AabbFrom(collider, transform.worldTransform);
                foreach (var hit in Physics.FindObjects(search, layer))
                {
                    if (Physics.DistanceBetween(collider, transform.worldTransform, hit.collider, hit.transform, 1f,
                            out var result))
                    {
                        canBeCollectedLookup.SetComponentEnabled(hit.entity, true);
                        Debug.Log("can be collected now");
                    }
                }

            }
        }
    }
}