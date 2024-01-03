using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class ViewBoundsAuthoring : MonoBehaviour
    {
        public ViewBounds AsComponent
        {
            get => new ViewBounds();
        }
    }

    public struct ViewBounds : IComponentData
    {
    }

    public class ViewBoundsBaker : Baker<ViewBoundsAuthoring>
    {
        public override void Bake(ViewBoundsAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
