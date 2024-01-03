using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class MovementDirectionAuthoring : MonoBehaviour
    {
        public float2 value;

        public MovementDirection AsComponent
        {
            get => new MovementDirection
            {
                value = value
            };
        }
    }

    public struct MovementDirection : IComponentData
    {
        public float2 value;
    }

    public class MovementDirectionBaker : Baker<MovementDirectionAuthoring>
    {
        public override void Bake(MovementDirectionAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
