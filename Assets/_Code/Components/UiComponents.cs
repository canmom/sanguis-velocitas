using Latios;
using Latios.Calligraphics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SV
{
    public struct UiValues : IComponentData
    {
        public float health;
        public float   goalHealth;
    }

    public struct UiReferences : IComponentData
    {
        public EntityWithBuffer<CalliByte> healthText;
    }
}

