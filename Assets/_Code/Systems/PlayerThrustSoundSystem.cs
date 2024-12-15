using Latios;
using Latios.Myri;
using Latios.Transforms;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Entities.SystemAPI;

namespace SV
{
    [BurstCompile]
    public partial struct PlayerThrustSoundSystem : ISystem
    {
        LatiosWorldUnmanaged latiosWorld;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            latiosWorld = state.GetLatiosWorldUnmanaged();

            state.InitSystemRng("PlayerThrustSoundSystem");
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
                icb               = latiosWorld.syncPoint.CreateInstantiateCommandBuffer<Parent, AudioSourceOneShot>(),
                audioSourceLookup = GetComponentLookup<AudioSourceOneShot>(),
                rng               = state.GetJobRng(),
                dt                = Time.DeltaTime
            }.Schedule();
        }

        [BurstCompile]
        partial struct Job : IJobEntity, IJobEntityChunkBeginEnd
        {
            public InstantiateCommandBuffer<Parent, AudioSourceOneShot> icb;
            [ReadOnly] public ComponentLookup<AudioSourceOneShot>       audioSourceLookup;
            public SystemRng                                            rng;
            public float                                                dt;

            public void Execute(Entity entity, ref PlayerTailSound state, in PlayerTail thrusting)
            {
                state.currentTime -= dt;
                if (thrusting.isThrusting && state.currentTime <= 0f)
                {
                    var source                                      = audioSourceLookup[state.soundPrefab];
                    var nextTime                                    = state.period + rng.currentSequence.NextFloat(-state.periodJitter, state.periodJitter);
                    source.volume                                  *= 1f + rng.currentSequence.NextFloat(-state.volumeJitter, state.volumeJitter);
                    icb.Add(state.soundPrefab, new Parent { parent  = entity }, source);
                    state.currentTime                               = nextTime;
                }
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                rng.BeginChunk(unfilteredChunkIndex);
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask, bool chunkWasExecuted)
            {
            }
        }
    }
}

