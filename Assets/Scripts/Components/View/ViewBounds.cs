using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [Serializable]
    public struct ViewBounds : IComponentData
    {
        public float2 min;
        public float2 max;
    }
}
