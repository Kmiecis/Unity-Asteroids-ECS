using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct Visibility : IComponentData
    {
        public bool value;
    }
}
