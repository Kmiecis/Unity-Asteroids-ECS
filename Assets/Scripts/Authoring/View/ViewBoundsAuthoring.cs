using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class ViewBoundsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float3 min;
        public float3 max;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var component = new ViewBounds { min = min, max = max };
            dstManager.AddComponentData(entity, component);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var position = (float3)transform.position;
            var center = (min + max) * 0.5f + position;
            var size = (max - min);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}
