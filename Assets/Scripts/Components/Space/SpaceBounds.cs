using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct SpaceBounds : IComponentData
    {
        public float2 min;
        public float2 max;
    }
}
