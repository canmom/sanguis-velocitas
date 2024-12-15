using Latios;
using Latios.Kinemation;
using Latios.Psyshock;
using Latios.Psyshock.Anna;
using Latios.Transforms;
using static Unity.Entities.SystemAPI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SV
{
    [BurstCompile]
    public partial struct ChompSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float t = (float)Time.ElapsedTime;

            foreach ((var bones, var animations) in Query<DynamicBuffer<BoneReference>, RefRO<PlayerAnimations> >())
            {
                ref var clip     = ref animations.ValueRO.blob.Value.clips[0];
                var     clipTime = clip.LoopToClipTime(t);

                for (int i = 1; i < bones.Length; i++)
                {
                    var boneSampledLocalTransform          = clip.SampleBone(i, clipTime);
                    var boneTransformAspect                = GetAspect<TransformAspect>(bones[i].bone);
                    boneTransformAspect.localTransformQvvs = boneSampledLocalTransform;
                }
            }
        }
    }

    [BurstCompile]
    public partial struct ChompSystem2 : ISystem
    {
        LatiosWorldUnmanaged           latiosWorld;
        BuildCollisionLayerTypeHandles handles;
        TransformAspect.Lookup         lookup;
        EntityQuery                    m_enemyQuery;

        public void OnCreate(ref SystemState state)
        {
            latiosWorld  = state.GetLatiosWorldUnmanaged();
            handles      = new BuildCollisionLayerTypeHandles(ref state);
            m_enemyQuery = state.Fluent().WithAnyEnabled<EnemyTag, CanBeCollected>(true).PatchQueryForBuildingCollisionLayer().Build();
            lookup       = new TransformAspect.Lookup(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var settings = latiosWorld.GetPhysicsSettings();
            handles.Update(ref state);
            lookup.Update(ref state);
            state.Dependency = Physics.BuildCollisionLayer(m_enemyQuery, handles).WithSettings(settings.collisionLayerSettings)
                               .ScheduleParallel(out var layer, state.WorldUpdateAllocator, state.Dependency);
            new Job
            {
                enemyLayer      = layer,
                transformLookup = lookup,
                dt              = Time.DeltaTime,
            }.Schedule();
        }

        [BurstCompile]
        partial struct Job : IJobEntity
        {
            [ReadOnly] public CollisionLayer enemyLayer;
            public TransformAspect.Lookup    transformLookup;
            public float                     dt;

            public void Execute(Entity entity, ref PlayerAnimations animation, ref DynamicBuffer<BoneReference> bones, in TwoAgoTransform previous)
            {
                if (animation.animationTime >= animation.openTime)
                {
                    //UnityEngine.Debug.Log($"Playing through animation: {animation.animationTime}, {animation.openTime}");
                    // Play through the rest of the animation.
                    animation.animationTime += dt * animation.snapMultiplier;
                    if (animation.animationTime > animation.blob.Value.clips[0].duration)
                        animation.animationTime = 0f;
                }
                else
                {
                    var currentTransform     = transformLookup[entity].worldTransform;
                    var anticipationDistance =
                        animation.closeDistance + (math.distance(previous.position, currentTransform.position) * animation.chompAnticipateTime / dt);
                    var   testPoint    = currentTransform.position;
                    var   search       = new Aabb(testPoint - anticipationDistance, testPoint + anticipationDistance);
                    float bestDistance = float.MaxValue;
                    var   forward      = math.rotate(currentTransform.rotation, math.back());

                    foreach (var candidate in Physics.FindObjects(search, enemyLayer))
                    {
                        if (!Physics.DistanceBetween(testPoint, candidate.collider, candidate.transform, anticipationDistance, out var hitResult))
                            continue;
                        var ab = hitResult.hitpoint - testPoint;
                        if (math.dot(math.normalizesafe(ab), forward) < animation.cosFov)
                            continue;
                        bestDistance = math.min(bestDistance, math.length(ab));
                    }
                    var previousAnimationTime = animation.animationTime;
                    if (bestDistance >= anticipationDistance)
                    {
                        animation.animationTime = 0f;
                        //UnityEngine.Debug.Log("Resetting");
                    }
                    else if (bestDistance <= animation.closeDistance)
                        animation.animationTime = animation.openTime;
                    else
                        animation.animationTime = animation.openTime * math.unlerp(anticipationDistance, animation.closeDistance, bestDistance);
                    if (animation.animationTime < previousAnimationTime - 0.5f * animation.openTime)
                    {
                        // Just step the animation forward to avoid a discontinuity
                        animation.animationTime = previousAnimationTime + dt * animation.snapMultiplier;
                        //UnityEngine.Debug.Log("Stepping");
                    }
                    //UnityEngine.Debug.Log($"{animation.animationTime}, {previousAnimationTime}, {anticipationDistance}, {bestDistance}");
                }

                ref var clip     = ref animation.blob.Value.clips[0];
                var     clipTime = animation.animationTime;

                for (int i = 1; i < bones.Length; i++)
                {
                    var boneSampledLocalTransform          = clip.SampleBone(i, clipTime);
                    var boneTransformAspect                = transformLookup[bones[i].bone];
                    boneTransformAspect.localTransformQvvs = boneSampledLocalTransform;
                }
            }
        }
    }
}

