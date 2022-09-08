using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct FiringCooldown : IComponentData
    {
        public float value;
    }
}
