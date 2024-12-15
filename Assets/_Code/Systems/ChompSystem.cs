using Unity.Entities;
using Unity.Mathematics;
using static Unity.Entities.SystemAPI;
using static Unity.Mathematics.math;
using Latios.Transforms;
using Latios.Kinemation;
using Unity.Burst;

namespace SV
{
    [BurstCompile]
    public partial struct ChompSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float t = (float)Time.ElapsedTime;

            foreach ((var bones, var animations) in Query<DynamicBuffer<BoneReference>, RefRO<PlayerAnimations>>())
            {
                ref var clip = ref animations.ValueRO.blob.Value.clips[0];
                var clipTime = clip.LoopToClipTime(t);

                for (int i=1; i < bones.Length; i++)
                {
                    var boneSampledLocalTransform = clip.SampleBone(i, clipTime);
                    var boneTransformAspect = GetAspect<TransformAspect>(bones[i].bone);
                    boneTransformAspect.localTransformQvvs = boneSampledLocalTransform;
                }
            }
        }
    }
}