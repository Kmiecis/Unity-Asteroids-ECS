using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public class FollowEntity : EntityAssignee
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
            transform.position = translation.Value + offset;
        }
    }
}
