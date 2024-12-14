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
            state.CompleteDependency();
           ref var uiValues = ref GetSingletonRW<UiValues>().ValueRW;
           var playerQuery = QueryBuilder().WithAll<Player>().WithAll<Health>().Build();
           var health = playerQuery.GetSingleton<Health>();
           uiValues.health = health.currentHealth;
           uiValues.goalHealth = health.maxHealth;
            
            
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
                var fullInt = (int)math.floor(values.health);
                var fraction = (int) ((values.health - fullInt) * 100.0f + 0.5f);
                
                FixedString512Bytes temp       = $"Health: {fullInt}.{fraction}   Goal: {values.goalHealth}";
                var                 healthText = new CalliString(textLookup[references.healthText]);
                healthText.Clear();
                healthText.Append(temp);
            }
        }
    }
}

