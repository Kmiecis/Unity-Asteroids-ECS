using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [Serializable]
    public struct ViewBounds : IComponentData
    {
        public float3 min;
        public float3 max;
    }
}
