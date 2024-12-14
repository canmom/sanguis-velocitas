using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Latios.Psyshock.Anna.Authoring
{
    [DisableAutoCreation]
    public class UnityRigidBodyBaker : Baker<UnityEngine.Rigidbody>
    {
        public override void Bake(UnityEngine.Rigidbody authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RigidBody
            {
                inverseMass = 1f / authoring.mass,
#if UNITY_6000_0_OR_NEWER
                velocity = new UnitySim.Velocity { linear = authoring.linearVelocity, angular = authoring.angularVelocity },
#else
                velocity = new UnitySim.Velocity { linear = authoring.velocity, angular = authoring.angularVelocity },
#endif
                coefficientOfFriction    = (half)0.3f,
                coefficientOfRestitution = (half)0.3f
            });
            AddBuffer<AddImpulse>(entity);
            if (authoring.isKinematic)
                AddComponent<KinematicCollisionTag>(entity);

            if (authoring.constraints != RigidbodyConstraints.None)
            {
                AddComponent(entity, new LockWorldAxesFlags
                {
                    // For some reason, Unity skips 0x1.
                    packedFlags = (byte)((int)authoring.constraints >> 1)
                });
            }
        }
    }
}

