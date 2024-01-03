using System.Runtime.CompilerServices;
using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class BoundsAuthoring : MonoBehaviour
    {
        public float2 min;
        public float2 max;

        public Bounds AsComponent
        {
            get => new Bounds
            {
                min = min,
                max = max
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var bounds = this.AsComponent;
            var position = (float3)transform.position;
            var center = new float3(bounds.Center, 0.0f) + position;
            var size = new float3(bounds.Size, 0.0f);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }

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

    public class BoundsBaker : Baker<BoundsAuthoring>
    {
        public override void Bake(BoundsAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
