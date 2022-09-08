using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct Collider : IComponentData
    {
        public float value;
    }
}
