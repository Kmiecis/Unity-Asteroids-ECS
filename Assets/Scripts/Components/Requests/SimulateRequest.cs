using System;
using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    [Serializable]
    public struct SimulateRequest : IComponentData
    {
        public uint seed;
    }
}
