using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct PlayerScore : IComponentData
    {
        public int value;
    }
}
