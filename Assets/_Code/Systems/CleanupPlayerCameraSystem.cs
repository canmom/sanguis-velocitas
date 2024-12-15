using Latios;
using Latios.Myri;
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
    public partial struct CleanupPlayerCameraSystem : ISystem
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
                childLookup         = GetBufferLookup<Child>(true),
                audioListenerLookup = GetComponentLookup<AudioListener>(true),
            }.ScheduleParallel();
        }

        [WithAll(typeof(PlayerTailRef))]
        [BurstCompile]
        partial struct Job : IJobEntity
        {
            [ReadOnly] public BufferLookup<Child>            childLookup;
            [ReadOnly] public ComponentLookup<AudioListener> audioListenerLookup;

            public void Execute(ref DynamicBuffer<LinkedEntityGroup> leg, in DynamicBuffer<Child> children)
            {
                foreach (var child in children)
                {
                    if (audioListenerLookup.HasComponent(child.child))
                    {
                        if (childLookup.HasBuffer(child.child))
                        {
                            RemoveAllDescendents(ref leg, childLookup[child.child]);
                        }
                        Remove(child.child, ref leg);
                    }
                    else
                    {
                        if (childLookup.HasBuffer(child.child))
                        {
                            Execute(ref leg, childLookup[child.child]);
                        }
                    }
                }
            }

            void RemoveAllDescendents(ref DynamicBuffer<LinkedEntityGroup> leg, in DynamicBuffer<Child> children)
            {
                foreach (var child in children)
                {
                    if (childLookup.HasBuffer(child.child))
                    {
                        RemoveAllDescendents(ref leg, childLookup[child.child]);
                    }
                    Remove(child.child, ref leg);
                }
            }

            void Remove(Entity child, ref DynamicBuffer<LinkedEntityGroup> leg)
            {
                for (int i = 0; i < leg.Length; i++)
                {
                    if (leg[i].Value == child)
                    {
                        leg.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }
}

