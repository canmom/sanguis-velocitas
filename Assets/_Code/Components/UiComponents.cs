using Latios;
using Latios.Calligraphics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace SV
{
    public struct UiReferences : IComponentData
    {
        public EntityWithBuffer<CalliByte>     healthText;
        public EntityWith<HealthbarProperties> healthbar;
        public EntityWithBuffer<CalliByte>     hostText;
    }

    public struct UiHealthbarAnimation : IComponentData
    {
        public float poisonRampUpRate;
        public float poisonRampDownRate;
        public float propulsionRampUpRate;
        public float propulsionRampDownRate;
        public float gainRate;
    }

    [MaterialProperty("_healthbar")]
    public struct HealthbarProperties : IComponentData
    {
        public float healthFraction;
        public float poisonAnimationFraction;
        public float propulsionAnimationFraction;
        public float gainAnimationFraction;
    }
}

