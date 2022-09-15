using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [Serializable]
    public struct AsteroidRequest : IComponentData
    {
        public float2 position;
        public float radius;
        public float2 direction;
        public float speed;
    }
}
