using Latios;
using Latios.Psyshock.Anna;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Entities.SystemAPI;

namespace SV
{
    public partial class PlayerControlSystem : SubSystem
    {
        // Input Action Map variable

        protected override void OnCreate()
        {
            base.OnCreate();

            // Create Input Actions
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Destroy Input Actions
        }

        protected override void OnUpdate()
        {
            CompleteDependency();

            // Read Input Actions

            foreach ((var transform, var rigidbody, var impulseBuffer) in Query<WorldTransform, RefRW<RigidBody>, DynamicBuffer<AddImpulse> >().WithAll<PlayerTag>())
            {
                // Add linear force
                //impulseBuffer.Add(new AddImpulse(math.right() * SystemAPI.Time.DeltaTime));

                // Trick to add torque (apply a linear impulse in the opposite direction of the point impulse)
                // This example shows clockwise torque.
                //impulseBuffer.Add(new AddImpulse(transform.position + math.up(), math.right() * SystemAPI.Time.DeltaTime));
                //impulseBuffer.Add(new AddImpulse(-math.right() * SystemAPI.Time.DeltaTime));
            }
        }
    }
}

