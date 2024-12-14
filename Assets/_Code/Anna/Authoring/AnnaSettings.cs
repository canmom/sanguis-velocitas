using Latios.Authoring;
using Latios.Psyshock.Anna;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SV
{
    public class AnnaSettings : MonoBehaviour
    {
        public float3 worldAabbMin        = -1f;
        public float3 worldAabbMax        = 1f;
        public int3   subdivisionsPerAxis = new int3(2, 2, 2);
        public float3 gravity             = new float3(0f, -9.81f, 0f);
        public float  linearDamping       = 0.05f;
        public float  angularDamping      = 0.05f;
        public int    solverIterations    = 4;
    }

    public class AnnaSettingsBaker : Baker<AnnaSettings>
    {
        public override void Bake(AnnaSettings authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PhysicsSettings
            {
                collisionLayerSettings = new Latios.Psyshock.CollisionLayerSettings
                {
                    worldAabb                = new Latios.Psyshock.Aabb(authoring.worldAabbMin, authoring.worldAabbMax),
                    worldSubdivisionsPerAxis = authoring.subdivisionsPerAxis
                },
                gravity        = authoring.gravity,
                linearDamping  = (half)authoring.linearDamping,
                angularDamping = (half)authoring.angularDamping,
                numIterations  = (byte)authoring.solverIterations
            });
        }
    }
}

