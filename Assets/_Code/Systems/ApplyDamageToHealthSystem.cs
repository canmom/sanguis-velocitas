using Latios;
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
    public partial struct ApplyDamageToHealthSystem : ISystem, ISystemNewScene
    {
        LatiosWorldUnmanaged latiosWorld;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();
        }

        public void OnNewScene(ref SystemState state)
        {
            latiosWorld.sceneBlackboardEntity.AddComponent<GameState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new Job
            {
                dcb    = latiosWorld.syncPoint.CreateDestroyCommandBuffer(),
                sbe    = latiosWorld.sceneBlackboardEntity,
                lookup = GetComponentLookup<GameState>()
            }.Schedule();
        }

        [BurstCompile]
        partial struct Job : IJobEntity
        {
            public DestroyCommandBuffer       dcb;
            public Entity                     sbe;
            public ComponentLookup<GameState> lookup;

            public void Execute(Entity entity, ref DamageThisFrame damage, ref Health health)
            {
                health.currentHealth += damage.heal;
                if (health.currentHealth >= health.maxHealth)
                {
                    // Win
                    ref var state = ref lookup.GetRefRW(sbe).ValueRW;
                    state.win     = true;
                }
                else
                {
                    health.currentHealth -= damage.damageFromPropulsion;
                    health.currentHealth -= damage.damageFromPoison;

                    if (health.currentHealth <= 0f)
                    {
                        dcb.Add(entity);
                        ref var state = ref lookup.GetRefRW(sbe).ValueRW;
                        state.lose    = true;
                        if (damage.damageFromPoison > 0f)
                            state.deathByPoison = true;
                        if (damage.damageFromPropulsion > 0f)
                            state.deathByPropulsion = true;
                    }
                }
                health.currentHealth = math.clamp(health.currentHealth, 0f, health.maxHealth);
            }
        }
    }
}

