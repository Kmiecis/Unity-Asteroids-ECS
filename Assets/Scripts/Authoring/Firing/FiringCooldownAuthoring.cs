using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class FiringCooldownAuthoring : MonoBehaviour
    {
        public float value;

        public FiringCooldown AsComponent
        {
            get => new FiringCooldown
            {
                value = value
            };
        }
    }

    public struct FiringCooldown : IComponentData
    {
        public float value;
    }

    public class FiringCooldownBaker : Baker<FiringCooldownAuthoring>
    {
        public override void Bake(FiringCooldownAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
