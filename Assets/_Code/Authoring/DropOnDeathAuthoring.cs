using System.Collections.Generic;
using Latios.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class DropOnDeathAuthoring : MonoBehaviour
    {
        public List<GameObject> prefabsToSpawn;
    }

    public class DropOnDeathAuthoringBaker : Baker<DropOnDeathAuthoring>
    {
        public override void Bake(DropOnDeathAuthoring authoring)
        {
            if (authoring.prefabsToSpawn != null && authoring.prefabsToSpawn.Count > 0)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var buffer = AddBuffer<DropOnDeath>(entity);
                foreach (var prefab in authoring.prefabsToSpawn)
                    buffer.Add(new DropOnDeath { prefab = GetEntity(prefab, TransformUsageFlags.Renderable) });
            }
        }
    }
}

