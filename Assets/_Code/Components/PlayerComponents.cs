using Latios;
using Latios.Psyshock;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SV
{
    public struct PlayerTag : IComponentData
    {
    }

    public struct Health : IComponentData
    {
        public float health;
    }

    public struct PlayerAttackColliderTag : IComponentData
    {
    }
}

