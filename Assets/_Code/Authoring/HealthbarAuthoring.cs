using Latios.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class HealthbarAuthoring : MonoBehaviour
    {
        public float poisonRampUpRate       = 0.75f;
        public float poisonRampDownRate     = 0.75f;
        public float propulsionRampUpRate   = 0.75f;
        public float propulsionRampDownRate = 0.75f;
        public float gainRate               = 0.75f;
    }

    public class HealthbarAuthoringBaker : Baker<HealthbarAuthoring>
    {
        public override void Bake(HealthbarAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new UiHealthbarAnimation
            {
                poisonRampUpRate       = authoring.poisonRampUpRate,
                poisonRampDownRate     = authoring.poisonRampDownRate,
                propulsionRampUpRate   = authoring.propulsionRampUpRate,
                propulsionRampDownRate = authoring.poisonRampDownRate,
                gainRate               = authoring.gainRate,
            });
            AddComponent(entity, new HealthbarProperties
            {
                healthFraction = 0.2f  // preview
            });
        }
    }
}

