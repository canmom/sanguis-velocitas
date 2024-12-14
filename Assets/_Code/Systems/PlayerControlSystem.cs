using Latios;
using Latios.Psyshock.Anna;
using Latios.Transforms;
using SV.Input;
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

            float2 aim    = (float2) _input.Player.Aim.ReadValue<UnityEngine.Vector2>();
            float  thrust = _input.Player.Thrust.ReadValue<float>();

            aim -= new float2 (UnityEngine.Screen.width / 2f, UnityEngine.Screen.height / 2f);

            var aimQuaternion = quaternion.LookRotationSafe(new float3(aim.y, -aim.x, 0f), new float3(0f, 0f, 1f));

            foreach ((var rigidbody, var impulseBuffer, var transform, var player, var health, var damage)
                     in Query<RefRW<RigidBody>, DynamicBuffer<AddImpulse>, WorldTransform, Player, Health, RefRW<DamageThisFrame> >())
            {
                rigidbody.ValueRW.inverseMass = math.max(player.healthMassMultiplier / health.currentHealth, 0.001f);

                var quaternionDifference = math.mul(math.inverse(transform.rotation), aimQuaternion);
                var angleDifference      = math.angle(quaternionDifference, quaternion.identity) - math.PIHALF;  // I don't know why our angle is 180 degrees off, but it is.
                var angleAttenuation     = math.saturate(angleDifference / (player.rotationSpeed * SystemAPI.Time.DeltaTime));
                //UnityEngine.Debug.Log($"angle difference: {angleDifference}, angle attenuation: {angleAttenuation}, angular.z: {rigidbody.ValueRO.velocity.angular.z}");

                //quaternion difference should always be around the z axis, so we should be able to ignore other components
                //value is proportional to the sine of the angle
                var targetAngularVelocity =
                    new float3(0f, 0f, player.rotationSpeed * angleAttenuation * math.sign(quaternionDifference.value.z) * math.sign(quaternionDifference.value.w));
                rigidbody.ValueRW.velocity.angular = targetAngularVelocity +
                                                     (rigidbody.ValueRO.velocity.angular - targetAngularVelocity) *
                                                     math.exp( -player.rotationSharpness * SystemAPI.Time.DeltaTime);

                if (thrust > 0f)
                {
                    impulseBuffer.Add(new AddImpulse(player.thrustForce * transform.rightDirection * SystemAPI.Time.DeltaTime));

                    damage.ValueRW.damageFromPropulsion += player.healthFlowRate * SystemAPI.Time.DeltaTime;
                }

                impulseBuffer.Add(new AddImpulse(-rigidbody.ValueRO.velocity.linear * player.dragCoefficient * SystemAPI.Time.DeltaTime));
            }

            if (_input.Player.Restart.WasPressedThisFrame())
            {
                var currentScene = worldBlackboardEntity.GetComponentData<CurrentScene>();
                EntityManager.AddComponentData(sceneBlackboardEntity, new RequestLoadScene
                {
                    newScene = currentScene.current
                });
            }
        }
    }
}

