using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct SpaceBounds : IComponentData
    {
        public float3 min;
        public float3 max;
    }
}
