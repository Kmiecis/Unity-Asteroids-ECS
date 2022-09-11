using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class ColliderAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float radius;
        public float2 offset;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new Collider
            {
                radius = radius,
                offset = offset
            };
            dstManager.AddComponentData(entity, data);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var position = transform.position + new Vector3(offset.x, offset.y, 0.0f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, radius);
        }
#endif
    }
}
