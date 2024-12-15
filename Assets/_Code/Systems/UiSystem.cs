using Latios;
using Latios.Calligraphics;
using Latios.Transforms;
using static Unity.Entities.SystemAPI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

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
            var hostBloodRemaining = new NativeReference<float>(state.WorldUpdateAllocator, NativeArrayOptions.ClearMemory);
            new CountDropsJob
            {
                count = hostBloodRemaining
            }.Schedule();
            new CountEnemyDropsJob
            {
                count      = hostBloodRemaining,
                dropLookup = GetComponentLookup<HealthDrop>(true)
            }.Schedule();

            new Job
            {
                textLookup         = GetBufferLookup<CalliByte>(false),
                materialLookup     = GetComponentLookup<HealthbarProperties>(false),
                animationLookup    = GetComponentLookup<UiHealthbarAnimation>(false),
                hostBloodRemaining = hostBloodRemaining,
            }.Schedule();
        }

        [BurstCompile]
        partial struct Job : IJobEntity
        {
            public NativeReference<float>                hostBloodRemaining;
            public BufferLookup<CalliByte>               textLookup;
            public ComponentLookup<HealthbarProperties>  materialLookup;
            public ComponentLookup<UiHealthbarAnimation> animationLookup;

            public void Execute(in Health health, in DamageThisFrame damageThisFrame, in UiReferences references)
            {
                var wholeHealth   = (int)math.floor(health.currentHealth);
                var partialHealth = (int)((health.currentHealth - wholeHealth) * 10);
                // Health Text
                FixedString512Bytes temp =
                    $"{wholeHealth}.{partialHealth} / {(int)math.round(health.maxHealth)}";
                var healthText = new CalliString(textLookup[references.healthText]);
                healthText.Clear();
                healthText.Append(temp);

                // Healthbar
                ref var material  = ref materialLookup.GetRefRW(references.healthbar).ValueRW;
                ref var animation = ref animationLookup.GetRefRW(references.healthbar).ValueRW;

                material.healthFraction               = health.currentHealth / health.maxHealth;
                material.propulsionAnimationFraction += math.select(-animation.propulsionRampDownRate,
                                                                    animation.propulsionRampUpRate, damageThisFrame.damageFromPropulsion > 0f);
                material.poisonAnimationFraction += math.select(-animation.poisonRampDownRate,
                                                                animation.poisonRampUpRate, damageThisFrame.damageFromPoison > 0f);
                material.gainAnimationFraction       -= animation.gainRate;
                material.propulsionAnimationFraction  = math.saturate(material.propulsionAnimationFraction);
                material.poisonAnimationFraction      = math.saturate(material.poisonAnimationFraction);
                material.gainAnimationFraction        = math.select(math.saturate(material.gainAnimationFraction), 1f,
                                                                    damageThisFrame.heal > 0f);

                // Host text
                bool enable   = health.currentHealth + hostBloodRemaining.Value < health.maxHealth;
                var  hostText = new CalliString(textLookup[references.hostText]);
                hostText.Clear();
                if (enable)
                {
                    temp = "The host has run out of blood reserves to sustain your continued existance.\n Press R to restart.";
                    hostText.Append(temp);
                }
            }
        }

        [BurstCompile]
        partial struct CountDropsJob : IJobEntity
        {
            public NativeReference<float> count;

            public void Execute(in HealthDrop drop)
            {
                count.Value += drop.amount;
            }
        }

        [BurstCompile]
        partial struct CountEnemyDropsJob : IJobEntity
        {
            public NativeReference<float>                 count;
            [ReadOnly] public ComponentLookup<HealthDrop> dropLookup;

            public void Execute(in DynamicBuffer<DropOnDeath> droppables)
            {
                foreach (var d in droppables)
                {
                    if (dropLookup.TryGetComponent(d.prefab, out var drop))
                        count.Value += drop.amount;
                }
            }
        }
    }
}

