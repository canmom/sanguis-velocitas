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
    public partial struct DropOnDeathCopyTransformSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new Job().ScheduleParallel();
        }

        [WithChangeFilter(typeof(WorldTransform))]
        [BurstCompile]
        partial struct Job : IJobEntity
        {
            public void Execute(ref DropOnDeathTransformCleanup cleanup, in WorldTransform transform) => cleanup.transform = transform.worldTransform;
        }
    }
}

