using System;
using Unity.Entities;

namespace Asteroids
{
    [Serializable]
    public struct SpawnAsteroidsRequest : IComponentData
    {
        public uint seed;
        public float maxOffset;
    }
}
