using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct FiringSpawner : IComponentData
    {
        public Entity prefab;
        public float3 offset;
    }
}
