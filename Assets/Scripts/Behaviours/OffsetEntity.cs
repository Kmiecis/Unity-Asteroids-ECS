using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public class OffsetEntity : EntityAssignee
    {
        public float3 offset;

        private EntityManager _manager;

        private void Awake()
        {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void LateUpdate()
        {
            var translation = _manager.GetComponentData<Translation>(entity);
            translation.Value = (float3)transform.position + offset;
            _manager.SetComponentData(entity, translation);
        }
    }
}
