using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct AsteroidPrefab : IComponentData
    {
        public Entity value;
    }
}
