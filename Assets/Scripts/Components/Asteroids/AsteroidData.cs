using Unity.Entities;

namespace Asteroids
{
    public struct AsteroidData : IComponentData
    {
        public Entity prefab;
        public EntityArchetype archetype;
        public float minRadius;
        public float maxRadius;
        public float minSpeed;
        public float maxSpeed;
    }
}
