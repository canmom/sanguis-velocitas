using Latios.Psyshock.Anna;
using Latios.Transforms;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace SV
{
    public partial struct EnemyFollowsPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var playerQuery = SystemAPI.QueryBuilder().WithAll<Player>().WithAll<WorldTransform>().Build();
            if (!playerQuery.TryGetSingleton<WorldTransform>(out var playerTransform)) return;
            new EnemyFollowPlayerJob()
            {
                playerPosition = playerTransform.position,
            }.Schedule();
        }

        [BurstCompile]
        private partial struct EnemyFollowPlayerJob : IJobEntity
        {
            public float3 playerPosition;

            private void Execute(Entity entity, 
                                 TransformAspect transform, 
                                 FollowPlayer followPlayer, 
                                 RigidBody rigidBody, 
                                 DynamicBuffer<AddImpulse> impulses
                )
            {
                var distance = math.distance(transform.worldPosition, playerPosition);
                var direction = math.normalize(playerPosition - transform.worldPosition);

                var desiredVelocity
                    = direction * math.min(followPlayer.maxSpeed, distance * followPlayer.proportionalGain);
                
                if (distance > followPlayer.maxDistance)
                {
                    desiredVelocity = float3.zero;
                }
                var steeringForce = (desiredVelocity - rigidBody.velocity.linear) * followPlayer.impulseStrength;

           
                
                impulses.Add(new AddImpulse(steeringForce));
            }
        }
    }
}