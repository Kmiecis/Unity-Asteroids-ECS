using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidAuthoring : MonoBehaviour
    {
        public Asteroid AsComponent
        {
            get => new Asteroid();
        }
    }

    public struct Asteroid : IComponentData
    {
    }

    public class AsteroidBaker : Baker<AsteroidAuthoring>
    {
        public override void Bake(AsteroidAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
