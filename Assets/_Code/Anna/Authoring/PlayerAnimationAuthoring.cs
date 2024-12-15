using Latios;
using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class PlayerAnimationAuthoring : MonoBehaviour
    {
        public AnimationClip biteAnimation;
        public float         biteTargetFov      = 90f;
        public float         anticipationTime   = 0.7f;
        public float         openMouthTime      = 0.4f;
        public float         closeMouthDistance = 0.2f;
        public float         snapMultiplier     = 4f;
    }

    [TemporaryBakingType]
    public struct PlayerAnimationSmartBakeItem : ISmartBakeItem<PlayerAnimationAuthoring>
    {
        SmartBlobberHandle<SkeletonClipSetBlob> blob;

        public bool Bake(PlayerAnimationAuthoring authoring, IBaker baker)
        {
            baker.AddComponent(baker.GetEntity(TransformUsageFlags.Dynamic), new PlayerAnimations
            {
                cosFov              = math.cos(math.radians(authoring.biteTargetFov)),
                chompAnticipateTime = authoring.anticipationTime,
                animationTime       = 0f,
                closeDistance       = authoring.closeMouthDistance,
                openTime            = authoring.openMouthTime,
                snapMultiplier      = authoring.snapMultiplier,
            });
            var clips = new NativeArray<SkeletonClipConfig>(1, Allocator.Temp);
            clips[0]  = new SkeletonClipConfig
            {
                clip     = authoring.biteAnimation,
                settings = SkeletonClipCompressionSettings.kDefaultSettings
            };
            blob = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), clips);
            return true;
        }

        public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
        {
            var animation  = entityManager.GetComponentData<PlayerAnimations>(entity);
            animation.blob = blob.Resolve(entityManager);
            entityManager.SetComponentData(entity, animation);
        }
    }

    public class PlayerAnimationBaker : SmartBaker<PlayerAnimationAuthoring, PlayerAnimationSmartBakeItem>
    {
    }
}

