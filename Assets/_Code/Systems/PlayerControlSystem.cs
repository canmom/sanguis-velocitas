using Latios;
using Latios.Psyshock.Anna;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using SV.Input;

using static Unity.Entities.SystemAPI;

namespace SV
{
    public partial class PlayerControlSystem : SubSystem
    {
        ChompyInputActions _input;

        protected override void OnCreate()
        {
            base.OnCreate();

            _input = new ChompyInputActions();
        }

        protected override void OnStartRunning()
        {
            _input.Enable();
        }

        protected override void OnStopRunning()
        {
            _input.Disable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _input.Dispose();
        }

        protected override void OnUpdate()
        {
            CompleteDependency();

            float2 aim = (float2) _input.Player.Aim.ReadValue<UnityEngine.Vector2>();
            float thrust = _input.Player.Thrust.ReadValue<float>();

            aim -= new float2 (UnityEngine.Screen.width/2f, UnityEngine.Screen.height/2f);

            var aimQuaternion = quaternion.LookRotationSafe(new float3(aim.y, -aim.x, 0f), new float3(0f, 0f, 1f));

            foreach ((var rigidbody, var impulseBuffer, var transform, var player) in Query<RefRW<RigidBody>, DynamicBuffer<AddImpulse>, WorldTransform, Player >())
            {
                var mass = 1f/rigidbody.ValueRO.inverseMass;

                var quaternionDifference = math.mul(math.inverse(transform.rotation), aimQuaternion);

                //quaternion difference should always be around the z axis, so we should be able to ignore other components
                //value is proportional to the sine of the angle
                var targetAngularVelocity = new float3(0f, 0f, player.rotationSpeed * math.sign(quaternionDifference.value.z) * math.sign(quaternionDifference.value.w));
                rigidbody.ValueRW.velocity.angular = targetAngularVelocity +
                    (rigidbody.ValueRO.velocity.angular - targetAngularVelocity)
                    * math.exp( -player.rotationSharpness * SystemAPI.Time.DeltaTime);


                if (thrust > 0f)
                {
                    impulseBuffer.Add(new AddImpulse(player.thrustForce * transform.rightDirection * SystemAPI.Time.DeltaTime));
                }

                //var cameraTransform = SystemAPI.GetAspect<TransformAspect>(player.camera).localTransform;

                //cameraTransform.position.x = transform.position.x;
                //cameraTransform.position.y = transform.position.y;

                //SystemAPI.GetAspect<TransformAspect>(player.camera).localTransform = cameraTransform;

                // Trick to add torque (apply a linear impulse in the opposite direction of the point impulse)
                // This example shows clockwise torque.
                //impulseBuffer.Add(new AddImpulse(transform.position + math.up(), math.right() * SystemAPI.Time.DeltaTime));
                //impulseBuffer.Add(new AddImpulse(-math.right() * SystemAPI.Time.DeltaTime));
            }
        }
    }
}
