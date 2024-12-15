using Unity.Entities;
using UnityEngine;
using Latios;
using Latios.Authoring;
using Latios.Kinemation;
using Latios.Kinemation.Authoring;
using Unity.Collections;

namespace SV
{
    public class PlayerAnimationAuthoring : MonoBehaviour
    {
        public AnimationClip biteAnimation;
    }

    [TemporaryBakingType]
    public struct PlayerAnimationSmartBakeItem : ISmartBakeItem<PlayerAnimationAuthoring>
    {
        SmartBlobberHandle<SkeletonClipSetBlob> blob;

        public bool Bake(PlayerAnimationAuthoring authoring, IBaker baker)
        {
            baker.AddComponent<PlayerAnimations>(baker.GetEntity(TransformUsageFlags.Dynamic));
            var clips = new NativeArray<SkeletonClipConfig>(1, Allocator.Temp);
            clips[0] = new SkeletonClipConfig { clip = authoring.biteAnimation, settings = SkeletonClipCompressionSettings.kDefaultSettings };
            blob = baker.RequestCreateBlobAsset(baker.GetComponent<Animator>(), clips);
            return true;
        }

        public void PostProcessBlobRequests(EntityManager entityManager, Entity entity)
        {
            entityManager.SetComponentData(entity, new PlayerAnimations { blob = blob.Resolve(entityManager) });
        }
    }
    
    public class PlayerAnimationBaker : SmartBaker<PlayerAnimationAuthoring, PlayerAnimationSmartBakeItem>
    {
        
    }
}