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
using UnityEngine;

namespace SV
{
    public partial struct ActivateHealthDropSystem : ISystem
    {
        private LatiosWorldUnmanaged           _latiosWorld;
        private BuildCollisionLayerTypeHandles _handles;
        private EntityQuery                    _healthDropQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _latiosWorld     = state.GetLatiosWorldUnmanaged();
            _handles         = new BuildCollisionLayerTypeHandles(ref state);
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
                layer                = layer,
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
                var hitSet = new NativeHashSet<Entity>(layer.count, Allocator.Temp);

                var search       = Physics.AabbFrom(collider, transform.worldTransform);
                var maxDistance  = 0.001f;
                search.min      -= maxDistance;
                search.max      += maxDistance;
                foreach (var hit in Physics.FindObjects(search, layer))
                {
                    if (!Physics.DistanceBetween(collider, transform.worldTransform, hit.collider, hit.transform, maxDistance, out _))
                    {
                        SetCanBeCollected(hit.entity);
                    }
                    hitSet.Add(hit.entity);
                }

                foreach (var body in layer.colliderBodies)
                {
                    if (!hitSet.Contains(body.entity))
                    {
                        SetCanBeCollected(body.entity);
                    }
                }
            }

            void SetCanBeCollected(Entity entity)
            {
                canBeCollectedLookup.SetComponentEnabled(entity, true);
                Debug.Log("can be collected now");
            }
        }
    }
}

