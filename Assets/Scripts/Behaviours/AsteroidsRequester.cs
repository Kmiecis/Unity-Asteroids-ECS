using System;
using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public struct SpawnAsteroidsRequest : IComponentData
    {
        public int width;
        public int height;
        public Entity prefab;
    }

    [Serializable]
    public struct SpawnAsteroidsRequestRaw
    {
        public int width;
        public int height;
        public GameObject prefab;
    }

    public class AsteroidsRequester : MonoBehaviour
    {
        public SpawnAsteroidsRequestRaw request;
        public Entity entity;

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                var world = World.DefaultGameObjectInjectionWorld;
                var settings = new GameObjectConversionSettings(world, GameObjectConversionUtility.ConversionFlags.AddEntityGUID);
                var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(request.prefab, settings);

                var entityManager = world.EntityManager;
                var entity = entityManager.CreateEntity();
                var component = new SpawnAsteroidsRequest
                {
                    width = request.width,
                    height = request.height,
                    prefab = prefab
                };
                entityManager.AddComponentData(entity, component);
            }
        }
    }
}
