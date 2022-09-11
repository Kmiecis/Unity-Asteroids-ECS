using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Asteroids
{
    public class OffsetEntity : MonoBehaviour
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
                var translation = _manager.GetComponentData<Translation>(entity);
                translation.Value = (float3)transform.position + offset;
                _manager.SetComponentData(entity, translation);
            }
        }
    }
}
