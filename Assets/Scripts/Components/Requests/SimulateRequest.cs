using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct SimulateRequest : IComponentData
    {
        public uint seed;
    }
}
