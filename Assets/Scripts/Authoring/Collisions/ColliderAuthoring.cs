using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class ColliderAuthoring : MonoBehaviour
    {
        public float radius;
        public float2 offset;

        public Collider AsComponent
        {
            get => new Collider
            {
                radius = radius,
                offset = offset
            };
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

    public struct Collider : IComponentData
    {
        public float radius;
        public float2 offset;
    }

    public class ColliderBaker : Baker<ColliderAuthoring>
    {
        public override void Bake(ColliderAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
