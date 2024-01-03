using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class FiringIntervalAuthoring : MonoBehaviour
    {
        public float value;

        public FiringInterval AsComponent
        {
            get => new FiringInterval
            {
                value = value
            };
        }
    }

    public struct FiringInterval : IComponentData
    {
        public float value;
    }

    public class FiringIntervalBaker : Baker<FiringIntervalAuthoring>
    {
        public override void Bake(FiringIntervalAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
