using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [Serializable]
    public struct Bounds : IComponentData
    {
        public float2 min;
        public float2 max;

        public float2 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (max + min) * 0.5f;
        }

        public float2 Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (max - min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bounds Translated(float2 dt)
        {
            return new Bounds
            {
                min = min + dt,
                max = max + dt
            };
        }
    }
}
