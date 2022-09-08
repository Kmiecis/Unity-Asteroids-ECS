using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class EntityInstance : MonoBehaviour
    {
        public GameObject prefab;
        public Entity entity;

        private void Awake()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;
            var settings = new GameObjectConversionSettings(world, GameObjectConversionUtility.ConversionFlags.AddEntityGUID);
            var entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
            entity = entityManager.Instantiate(entityPrefab);
        }
    }
}
