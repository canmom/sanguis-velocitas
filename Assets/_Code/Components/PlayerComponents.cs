using Latios;
using Latios.Psyshock;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SV
{
    public struct Player : IComponentData
    {
        public float rotationSpeed;
        public float rotationSharpness;
        public float thrustForce;
    }

    public struct Health : IComponentData
    {
        public float health;
    }

    public struct PlayerAttackColliderTag : IComponentData
    {
    }
}

