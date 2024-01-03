using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class LifetimeAuthoring : MonoBehaviour
    {
        public float value;

        public Lifetime AsComponent
        {
            get => new Lifetime
            {
                value = value
            };
        }
    }

    public struct Lifetime : IComponentData
    {
        public float value;
    }

    public class LifetimeBaker : Baker<LifetimeAuthoring>
    {
        public override void Bake(LifetimeAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
