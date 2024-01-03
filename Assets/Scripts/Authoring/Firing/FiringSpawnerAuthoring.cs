using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class FiringSpawnerAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public float2 offset;
        public float radius;

        public bool MarkDependencies(IBaker baker)
        {
            baker.DependsOn(prefab);
            return prefab != null;
        }

        public FiringSpawner AsComponent(IBaker baker)
        {
            return new FiringSpawner
            {
                prefab = baker.GetEntity(prefab, TransformUsageFlags.Dynamic),
                offset = offset,
                radius = radius
            };
        }
    }

    public struct FiringSpawner : IComponentData
    {
        public Entity prefab;
        public float2 offset;
        public float radius;
    }

    public class FiringSpawnerBaker : Baker<FiringSpawnerAuthoring>
    {
        public override void Bake(FiringSpawnerAuthoring authoring)
        {
            if (!authoring.MarkDependencies(this))
                return;

            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent(this));
        }
    }
}
