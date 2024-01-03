using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class MovementSpeedAuthoring : MonoBehaviour
    {
        public float value;

        public MovementSpeed AsComponent
        {
            get => new MovementSpeed
            {
                value = value
            };
        }
    }

    public struct MovementSpeed : IComponentData
    {
        public float value;
    }

    public class MovementSpeedBaker : Baker<MovementSpeedAuthoring>
    {
        public override void Bake(MovementSpeedAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
