using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct AsteroidData : IComponentData
    {
        public Entity prefab;
        public float radius;
        public float minSpeed;
        public float maxSpeed;
    }
}
