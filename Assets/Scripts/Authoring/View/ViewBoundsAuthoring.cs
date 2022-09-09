using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class ViewBoundsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float2 min;
        public float2 max;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var component = new ViewBounds { min = min, max = max };
            dstManager.AddComponentData(entity, component);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var position = (float3)transform.position;
            var center = new float3((min + max) * 0.5f, 0.0f) + position;
            var size = new float3(max - min, 0.0f);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}
