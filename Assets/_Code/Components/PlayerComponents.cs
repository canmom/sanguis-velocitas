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
        public float dragCoefficient;
        public float healthMassMultiplier;
        public float healthFlowRate;
    }

    public struct Health : IComponentData
    {
        public float currentHealth;
        public float maxHealth;
    }

    public struct DamageThisFrame : IComponentData
    {
        public float damageFromPropulsion;
        public float damageFromPoison;
        public float heal;
    }

    public struct PlayerAttackColliderTag : IComponentData
    {
    }

    public struct PlayerTailRef : IComponentData
    {
        public EntityWith<PlayerTail> tail;
    }

    public struct PlayerTail : IComponentData
    {
        public bool isThrusting;
    }

    public struct PlayerTailSound : IComponentData
    {
        public EntityWith<Prefab> soundPrefab;
        public float              period;
        public float              currentTime;
        public float              periodJitter;
        public float              volumeJitter;
    }
}

