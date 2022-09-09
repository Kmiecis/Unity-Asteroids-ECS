using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct MovementDirection : IComponentData
    {
        public float2 value;
    }
}
