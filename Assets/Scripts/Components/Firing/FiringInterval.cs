using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct FiringInterval : IComponentData
    {
        public float value;
    }
}
