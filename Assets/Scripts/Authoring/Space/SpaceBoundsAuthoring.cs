using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class SpaceBoundsAuthoring : MonoBehaviour
    {
        public SpaceBounds AsComponent
        {
            get => new SpaceBounds();
        }
    }

    public struct SpaceBounds : IComponentData
    {
    }

    public class SpaceBoundsBaker : Baker<SpaceBoundsAuthoring>
    {
        public override void Bake(SpaceBoundsAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
