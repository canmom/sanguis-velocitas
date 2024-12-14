using Latios;
using Latios.Transforms;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SV
{
    public struct EnemyTag : IComponentData
    {
    }

    public struct DropOnDeath : IBufferElementData
    {
        public EntityWith<Prefab> prefab;
    }

    public struct DropOnDeathCleanup : ICleanupBufferElementData
    {
        public EntityWith<Prefab> prefab;
    }

    public struct DropOnDeathTransformCleanup : ICleanupComponentData
    {
        public TransformQvvs transform;
    }

    public struct AoE : IComponentData
    {
        public float dps;
    }
}

