using System;
using Unity.Entities;

namespace Asteroids
{
    [Serializable]
    public struct AsteroidRequest : IComponentData
    {
        public uint seed;
    }
}
