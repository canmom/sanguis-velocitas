using Latios;
using Latios.Psyshock;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Latios.Kinemation;

namespace SV
{
    public struct Player : IComponentData
    {
        public float rotationSpeed;
        public float rotationSharpness;
        public float thrustForce;
        public float dragCoefficient;
        public float healthMassMultiplier;
        public float healthFlowRate;
        public float mouthOpenDistance;
        public float mouthClosedDistance;
    }

    public struct PlayerAnimations : IComponentData
    {
        public BlobAssetReference<SkeletonClipSetBlob> blob;
    }

    public struct Health : IComponentData
    {
        public float health;
        public float goalHealth;
    }

    public struct PlayerAttackColliderTag : IComponentData
    {
    }
}

