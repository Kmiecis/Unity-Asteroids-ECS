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

        public float2 Center
        {
            get => (max + min) * 0.5f;
        }

        public float2 Size
        {
            get => (max - min);
        }

        public ViewBounds Translated(float2 dt)
        {
            return new ViewBounds
            {
                min = min + dt,
                max = max + dt
            };
        }
    }
}
