using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class RotateSpeedAuthoring : MonoBehaviour
    {
        public float value;

        public RotateSpeed AsComponent
        {
            get => new RotateSpeed
            {
                value = value
            };
        }
    }

    public struct RotateSpeed : IComponentData
    {
        public float value;
    }

    public class RotateSpeedBaker : Baker<RotateSpeedAuthoring>
    {
        public override void Bake(RotateSpeedAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
