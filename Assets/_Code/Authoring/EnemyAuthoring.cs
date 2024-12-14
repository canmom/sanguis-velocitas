using Latios.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class EnemyAuthoring : MonoBehaviour
    {
    }

    public class EnemyAuthoringBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<EnemyTag>(entity);
        }
    }
}

