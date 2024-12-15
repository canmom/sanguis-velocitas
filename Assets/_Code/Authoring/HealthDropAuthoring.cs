using Unity.Entities;
using UnityEngine;

namespace SV
{
    [DisallowMultipleComponent]
    public class HealthDropAuthoring : MonoBehaviour
    {
        public float amount;
        internal class HealthDropBaker : Baker<HealthDropAuthoring>
        {
            public override void Bake(HealthDropAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                AddComponent(entity, new HealthDrop()
                {
                    amount = authoring.amount,
                });
                
                AddComponent<CanBeCollected>(entity);
                SetComponentEnabled<CanBeCollected>(entity, false);
            }
        }
    }
}

