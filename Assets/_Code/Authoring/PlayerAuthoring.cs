using Latios.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public float initialHealth = 10f;
        public float rotationSpeed = 1f;
        public float rotationSharpness = 15f;
        public float thrustForce   = 1f;
        public float dragCoefficient = 0.2f;
    }

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health { health = authoring.initialHealth });
            AddComponent(entity, new Player
                {
                      rotationSpeed = authoring.rotationSpeed,
                  rotationSharpness = authoring.rotationSharpness,
                        thrustForce = authoring.thrustForce,
                    dragCoefficient = authoring.dragCoefficient,
                });
        }
    }
}

