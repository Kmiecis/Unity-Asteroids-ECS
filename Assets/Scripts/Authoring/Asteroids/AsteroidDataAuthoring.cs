using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidDataAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public float minRadius;
        public float maxRadius;
        public float minSpeed;
        public float maxSpeed;

        public bool MarkDependencies(IBaker baker)
        {
            baker.DependsOn(prefab);
            return prefab != null;
        }

        public AsteroidData AsComponent(IBaker baker)
        {
            return new AsteroidData
            {
                prefab = baker.GetEntity(prefab, TransformUsageFlags.Dynamic),
                minRadius = minRadius,
                maxRadius = maxRadius,
                minSpeed = minSpeed,
                maxSpeed = maxSpeed
            };
        }
    }

    public struct AsteroidData : IComponentData
    {
        public Entity prefab;
        public float minRadius;
        public float maxRadius;
        public float minSpeed;
        public float maxSpeed;

        public static EntityArchetype CreateAsteroidArchetype(EntityManager entityManager)
        {
            return entityManager.CreateArchetype(
                typeof(Asteroid),
                typeof(LocalTransform),
                typeof(Collider),
                typeof(Collided),
                typeof(MovementDirection),
                typeof(MovementSpeed),
                typeof(Hidden),
                typeof(Visibility)
            );
        }
    }

    public class AsteroidDataBaker : Baker<AsteroidDataAuthoring>
    {
        public override void Bake(AsteroidDataAuthoring authoring)
        {
            if (!authoring.MarkDependencies(this))
                return;

            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent(this));
        }
    }
}
