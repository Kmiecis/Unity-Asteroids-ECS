using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Asteroids
{
    public class FollowEntity : MonoBehaviour
    {
        public float3 offset;
        public EntityInstance entityInstance;

        private EntityManager _manager;

        private void Awake()
        {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void LateUpdate()
        {
            if (entityInstance.TryGetEntity(out var entity))
            {
                var transform = _manager.GetComponentData<LocalTransform>(entity);
                this.transform.position = transform.Position + offset;
            }
        }
    }
}
