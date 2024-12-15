using Latios.Authoring;
using Latios.Calligraphics.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class UiAuthoring : MonoBehaviour
    {
        public TextRendererAuthoring healthText;
        public HealthbarAuthoring    healthBar;
    }

    public class UiAuthoringBaker : Baker<UiAuthoring>
    {
        public override void Bake(UiAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new UiReferences
            {
                healthText = GetEntity(authoring.healthText, TransformUsageFlags.Renderable),
                healthbar  = GetEntity(authoring.healthBar, TransformUsageFlags.Renderable)
            });
        }
    }
}

