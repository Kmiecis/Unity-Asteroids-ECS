using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class FollowPlayerAuthoring : MonoBehaviour
    {
        public float2 offset;

        public FollowPlayer AsComponent
        {
            get => new FollowPlayer()
            {
                offset = offset
            };
        }
    }

    public struct FollowPlayer : IComponentData
    {
        public float2 offset;
    }

    public class FollowPlayerBaker : Baker<FollowPlayerAuthoring>
    {
        public override void Bake(FollowPlayerAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}