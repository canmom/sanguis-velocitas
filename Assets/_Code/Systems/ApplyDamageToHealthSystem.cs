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
    public partial struct ApplyDamageToHealthSystem : ISystem
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
            new Job { dcb = latiosWorld.syncPoint.CreateDestroyCommandBuffer() }.Schedule();
        }

        [BurstCompile]
        partial struct Job : IJobEntity
        {
            public DestroyCommandBuffer dcb;
            public void Execute(Entity entity, ref DamageThisFrame damage, ref Health health)
            {
                if (health.currentHealth >= health.maxHealth)
                {
                    // Win
                }
                else
                {
                    health.currentHealth -= damage.damageFromPropulsion;
                    health.currentHealth -= damage.damageFromPoison;

                    if (health.currentHealth <= 0f)
                    {
                        dcb.Add(entity);
                    }
                }
                health.currentHealth = math.clamp(health.currentHealth, 0f, health.maxHealth);
            }
        }
    }
}

