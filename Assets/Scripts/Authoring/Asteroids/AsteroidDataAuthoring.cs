using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject prefab;
        public float minRadius;
        public float maxRadius;
        public float minSpeed;
        public float maxSpeed;

        private static EntityArchetype GetAsteroidArchetype(EntityManager entityManager)
        {
            return entityManager.CreateArchetype(
                typeof(Asteroid),
                typeof(Translation),
                typeof(Rotation),
                typeof(NonUniformScale),
                typeof(Collider),
                typeof(Collided),
                typeof(MovementDirection),
                typeof(MovementSpeed),
                typeof(Hidden),
                typeof(Visibility)
            );
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new AsteroidData
            {
                prefab = conversionSystem.GetPrimaryEntity(prefab),
                archetype = GetAsteroidArchetype(dstManager),
                minRadius = minRadius,
                maxRadius = maxRadius,
                minSpeed = minSpeed,
                maxSpeed = maxSpeed
            };

            dstManager.AddComponentData(entity, data);
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefab);
        }
    }
}
