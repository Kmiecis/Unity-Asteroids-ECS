using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class LivetimeAuthoring : MonoBehaviour
    {
        public float value;

        public Livetime AsComponent
        {
            get => new Livetime
            {
                value = value
            };
        }
    }

    public struct Livetime : IComponentData
    {
        public float value;
    }

    public class LivetimeBaker : Baker<LivetimeAuthoring>
    {
        public override void Bake(LivetimeAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
