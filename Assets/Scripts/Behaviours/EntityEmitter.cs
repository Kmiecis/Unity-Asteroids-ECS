using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class EntityEmitter<T> : MonoBehaviour
        where T : struct, IComponentData
    {
        public T data;

        protected EntityManager _entityManager;

        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public void Emit()
        {
            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, data);
        }
    }
}
