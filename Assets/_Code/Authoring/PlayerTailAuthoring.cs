using Latios.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class PlayerTailAuthoring : MonoBehaviour
    {
        public GameObject soundPrefab;
        public float      period       = 0.727f;
        public float      periodJitter = 0.01f;
        public float      volumeJitter = 0.25f;
    }

    public class PlayerTailAuthoringBaker : Baker<PlayerTailAuthoring>
    {
        public override void Bake(PlayerTailAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new PlayerTail
            {
                isThrusting = false
            });
            AddComponent(entity, new PlayerTailSound
            {
                soundPrefab  = GetEntity(authoring.soundPrefab, TransformUsageFlags.Dynamic),
                period       = authoring.period,
                currentTime  = 0f,
                periodJitter = authoring.periodJitter,
                volumeJitter = authoring.volumeJitter
            });
        }
    }
}

