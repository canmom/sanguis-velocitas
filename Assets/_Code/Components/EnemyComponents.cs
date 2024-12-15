using Unity.Entities;

namespace SV
{
    public struct FollowPlayer : IComponentData
    {
        public float maxDistance;
        public float impulseStrength;
        public float proportionalGain;
        public float maxSpeed;
    }
}