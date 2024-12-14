using Unity.Entities;
using UnityEngine;
using Latios.Transforms;
using Latios.Transforms.Authoring;

namespace SV
{
    public class CameraAuthoring : MonoBehaviour
    {
        
        internal class CameraBaker : Baker<CameraAuthoring>
        {
            public override void Bake(CameraAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
                
                this.AddHiearchyModeFlags(entity, HierarchyUpdateMode.Flags.WorldRotation);

            }
        }
    }
}