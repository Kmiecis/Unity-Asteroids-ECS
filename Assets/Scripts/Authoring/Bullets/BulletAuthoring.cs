using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class BulletAuthoring : MonoBehaviour
    {
        public Bullet AsComponent
        {
            get => new Bullet();
        }
    }

    public struct Bullet : IComponentData
    {
    }

    public class BulletBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
