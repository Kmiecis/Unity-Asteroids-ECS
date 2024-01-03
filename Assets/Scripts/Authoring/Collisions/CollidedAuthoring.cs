using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class CollidedAuthoring : MonoBehaviour
    {
        public bool value;

        public Collided AsComponent
        {
            get => new Collided
            {
                value = value
            };
        }
    }

    public struct Collided : IComponentData
    {
        public bool value;
    }

    public class CollidedBaker : Baker<CollidedAuthoring>
    {
        public override void Bake(CollidedAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
