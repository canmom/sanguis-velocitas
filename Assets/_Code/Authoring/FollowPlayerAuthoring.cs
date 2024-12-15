using Unity.Entities;
using UnityEngine;

namespace SV
{
    [DisallowMultipleComponent]
    public class FollowPlayerAuthoring : MonoBehaviour
    {
        public float impulseStrength = 10f;
        public float proportionalGain = 2f;
        public float maxSpeed = 5f;
        public float maxDistance = 100f;
        internal class FollowPlayerAuthoringBaker : Baker<FollowPlayerAuthoring>
        {
            public override void Bake(FollowPlayerAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new FollowPlayer()
                {
                    impulseStrength =  authoring.impulseStrength,
                    maxSpeed = authoring.maxSpeed,
                    maxDistance = authoring.maxDistance,
                    proportionalGain = authoring.proportionalGain,
                });
            }
        }
    }
}

