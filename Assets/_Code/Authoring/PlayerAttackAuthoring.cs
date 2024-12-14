using Latios.Authoring;
using Latios.Transforms.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class PlayerAttackAuthoring : MonoBehaviour
    {
    }

    public class PlayerAttackAuthoringBaker : Baker<PlayerAttackAuthoring>
    {
        [BakingType] struct PreviousRequest : IRequestPreviousTransform { }

        public override void Bake(PlayerAttackAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent<PreviousRequest>(        entity);
            AddComponent<PlayerAttackColliderTag>(entity);
        }
    }
}

