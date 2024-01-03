using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class VisibilityAuthoring : MonoBehaviour
    {
        public bool value;

        public Visibility AsComponent
        {
            get => new Visibility()
            {
                value = value
            };
        }
    }

    public struct Visibility : IComponentData
    {
        public bool value;
    }

    public class VisibilityBaker : Baker<VisibilityAuthoring>
    {
        public override void Bake(VisibilityAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
