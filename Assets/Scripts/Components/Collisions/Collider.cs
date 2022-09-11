using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    public struct Collider : IComponentData
    {
        public float radius;
        public float2 offset;
    }
}
