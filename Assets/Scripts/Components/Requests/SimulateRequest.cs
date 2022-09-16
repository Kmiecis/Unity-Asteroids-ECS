using System;
using Unity.Entities;

namespace Asteroids
{
    [Serializable]
    public struct SimulateRequest : IComponentData
    {
        public uint seed;
    }
}
