using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct Lifetime : IComponentData
    {
        public float value;
    }
}
