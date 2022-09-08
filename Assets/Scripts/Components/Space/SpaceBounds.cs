using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct SpaceBounds : IComponentData
    {
        public int3 min;
        public int3 max;
    }
}
