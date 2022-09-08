using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct MovementDirection : IComponentData
    {
        public float3 value;
    }
}
