using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public abstract class EntityInstance : MonoBehaviour
    {
        protected EntityManager _manager;
        protected EntityQuery _query;

        public abstract ComponentType[] Archetype { get; }

        public bool TryGetEntity(out Entity entity)
        {
            return _query.TryGetSingletonEntity<Entity>(out entity);
        }

        public bool TryGetData<T>(out T data)
            where T : unmanaged, IComponentData
        {
            if (TryGetEntity(out var entity) && _manager.HasComponent<T>(entity))
            {
                data = _manager.GetComponentData<T>(entity);
                return true;
            }

            data = default;
            return false;
        }

        public bool TrySetData<T>(T data)
            where T : unmanaged, IComponentData
        {
            if (TryGetEntity(out var entity) && _manager.HasComponent<T>(entity))
            {
                _manager.SetComponentData(entity, data);
                return true;
            }
            return false;
        }

        public bool TryAddData<T>(T data)
            where T : unmanaged, IComponentData
        {
            if (TryGetEntity(out var entity))
            {
                return _manager.AddComponentData(entity, data);
            }
            return false;
        }

        private void Awake()
        {
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Start()
        {
            _query = _manager.CreateEntityQuery(Archetype);
        }
    }
}
