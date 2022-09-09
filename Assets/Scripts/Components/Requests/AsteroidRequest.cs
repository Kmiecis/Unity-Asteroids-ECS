using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    public struct AsteroidRequest : IComponentData
    {
        public float2 position;
        public float2 direction;
        public float speed;
        public float timeleft;
    }
}
