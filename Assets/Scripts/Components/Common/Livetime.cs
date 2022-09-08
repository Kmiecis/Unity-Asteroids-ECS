using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct Livetime : IComponentData
    {
        public float value;
    }
}
