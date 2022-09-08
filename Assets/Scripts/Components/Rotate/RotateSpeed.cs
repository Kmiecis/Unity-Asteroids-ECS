using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct RotateSpeed : IComponentData
    {
        public float value;
    }
}
