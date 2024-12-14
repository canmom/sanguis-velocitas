using Latios.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class AoEAuthoring : MonoBehaviour
    {
        public float damagePerSecond = 1f;
    }

    public class AoEAuthoringBaker : Baker<AoEAuthoring>
    {
        public override void Bake(AoEAuthoring authoring)
        {
            var entity                         = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new AoE { dps = authoring.damagePerSecond });
        }
    }
}

