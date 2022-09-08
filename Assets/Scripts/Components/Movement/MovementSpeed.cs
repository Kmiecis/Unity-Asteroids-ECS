using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct MovementSpeed : IComponentData
    {
        public float value;
    }
}
