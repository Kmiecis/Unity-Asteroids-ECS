using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct Collided : IComponentData
    {
        public bool value;
    }
}
