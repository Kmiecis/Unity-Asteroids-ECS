using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct SpaceBounds : IComponentData
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

        public SpaceBounds Translated(float2 dt)
        {
            return new SpaceBounds
            {
                min = min + dt,
                max = max + dt
            };
        }
    }
}
