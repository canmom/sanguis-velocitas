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
    public partial struct ResetDamageThisFrameSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new Job().Schedule();
        }

        [BurstCompile]
        partial struct Job : IJobEntity
        {
            public void Execute(ref DamageThisFrame d) => d = default;
        }
    }
}

