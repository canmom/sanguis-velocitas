using Latios;
using Latios.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Entities.SystemAPI;

namespace SV
{
    public partial class DriveParticlesSystem : SubSystem
    {
        protected override void OnUpdate()
        {
            CompleteDependency();
            foreach ((var playerTail, var particles) in Query<PlayerTail, SystemAPI.ManagedAPI.UnityEngineComponent<UnityEngine.ParticleSystem> >())
            {
                var em = particles.Value.emission;
                if (playerTail.isThrusting)
                {
                    em.enabled = true;
                        
                }
                else
                {
                    em.enabled = false;
                }
            }
        }
    }
}

