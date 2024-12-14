using Latios;
using Latios.Calligraphics;
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
    public partial struct UiSystem : ISystem
    {
        LatiosWorldUnmanaged latiosWorld;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new Job
            {
                textLookup = GetBufferLookup<CalliByte>(false)
            }.Schedule();
        }

        [BurstCompile]
        partial struct Job : IJobEntity
        {
            public BufferLookup<CalliByte> textLookup;

            public void Execute(in UiValues values, in UiReferences references)
            {
                FixedString512Bytes temp       = $"Health: {(int)math.round(values.health)}\tGoal: {values.goalHealth}";
                var                 healthText = new CalliString(textLookup[references.healthText]);
                healthText.Clear();
                healthText.Append(temp);
            }
        }
    }
}

