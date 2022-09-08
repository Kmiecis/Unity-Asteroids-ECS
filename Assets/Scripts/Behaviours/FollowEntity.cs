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
            var translation = _manager.GetComponentData<Translation>(entityInstance.entity);
            transform.position = translation.Value + offset;
        }
    }
}
