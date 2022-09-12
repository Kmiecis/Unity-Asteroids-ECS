using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class BoundsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float2 min;
        public float2 max;

        public Bounds Data
        {
            get => new Bounds
            {
                min = min,
                max = max
            };
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, Data);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var bounds = this.Data;
            var position = (float3)transform.position;
            var center = new float3(bounds.Center, 0.0f) + position;
            var size = new float3(bounds.Size, 0.0f);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}
