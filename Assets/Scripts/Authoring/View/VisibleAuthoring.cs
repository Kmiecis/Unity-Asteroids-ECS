using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class VisibleAuthoring : MonoBehaviour
    {
        public Visible AsComponent
        {
            get => new Visible();
        }
    }

    public struct Visible : IComponentData
    {
    }

    public class VisibleBaker : Baker<VisibleAuthoring>
    {
        public override void Bake(VisibleAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
