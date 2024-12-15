using Latios.Authoring;
using Latios.Transforms.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public float initialHealth        = 10f;
        public int   goalHealth           = 100;
        public float rotationSpeed        = 1f;
        public float rotationSharpness    = 15f;
        public float thrustForce          = 1f;
        public float dragCoefficient      = 0.2f;
        public float healthMassMultiplier = 1f;
        public float healthFlowRate       = 0.1f;

        public PlayerTailAuthoring tail;
    }

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
    {
        [BakingType] struct PreviousRequest : IRequestPreviousTransform { }

        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health
            {
                currentHealth = authoring.initialHealth,
                maxHealth     = authoring.goalHealth,
            });
            AddComponent<DamageThisFrame>(entity);
            AddComponent(                 entity, new Player
            {
                rotationSpeed        = authoring.rotationSpeed,
                rotationSharpness    = authoring.rotationSharpness,
                thrustForce          = authoring.thrustForce,
                dragCoefficient      = authoring.dragCoefficient,
                healthMassMultiplier = authoring.healthMassMultiplier,
                healthFlowRate       = authoring.healthFlowRate,
            });
            AddComponent<PreviousRequest>(entity);
            AddComponent(                 entity, new PlayerTailRef
            {
                tail = GetEntity(authoring.tail, TransformUsageFlags.Renderable)
            });
        }
    }
}

