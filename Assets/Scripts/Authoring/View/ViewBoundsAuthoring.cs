using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class ViewBoundsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public ViewBounds viewBounds;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, viewBounds);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var position = (float3)transform.position;
            var center = (viewBounds.min + viewBounds.max) * 0.5f + position;
            var size = (viewBounds.max - viewBounds.min);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}
