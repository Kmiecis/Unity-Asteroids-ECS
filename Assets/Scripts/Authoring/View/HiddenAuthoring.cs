using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class HiddenAuthoring : MonoBehaviour
    {
        public Hidden AsComponent
        {
            get => new Hidden();
        }
    }

    public struct Hidden : IComponentData
    {
    }

    public class HiddenBaker : Baker<HiddenAuthoring>
    {
        public override void Bake(HiddenAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
