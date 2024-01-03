using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class EntityEmitter<T> : MonoBehaviour
        where T : unmanaged, IComponentData
    {
        public T data;

        protected EntityManager _entityManager;

        public void Emit()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, data);
        }

        protected virtual void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected virtual void OnValidate()
        {
        }
    }
}
