using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class EntityInstance : MonoBehaviour
    {
        public GameObject prefab;

        protected World _world;
        protected EntityManager _entityManager;
        protected Entity _entity;

        public bool TryGetEntity(out Entity entity)
        {
            entity = _entity;
            return (
                entity != Entity.Null &&
                _entityManager.Exists(entity)
            );
        }

        public bool TryGetData<T>(out T data)
            where T : struct, IComponentData
        {
            if (TryGetEntity(out var entity) && _entityManager.HasComponent<T>(entity))
            {
                data = _entityManager.GetComponentData<T>(entity);
                return true;
            }

            data = default;
            return false;
        }

        private void Awake()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;
        }

        private void Start()
        {
            var settings = new GameObjectConversionSettings(_world, GameObjectConversionUtility.ConversionFlags.AssignName);
            var entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
            _entity = _entityManager.Instantiate(entityPrefab);
        }
    }
}
