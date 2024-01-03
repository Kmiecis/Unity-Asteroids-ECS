using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class PlayerDataAuthoring : MonoBehaviour
    {
        public GameObject prefab;

        public bool MarkDependencies(IBaker baker)
        {
            baker.DependsOn(prefab);
            return prefab != null;
        }

        public PlayerData AsComponent(IBaker baker)
        {
            return new PlayerData
            {
                prefab = baker.GetEntity(prefab, TransformUsageFlags.Dynamic)
            };
        }
    }

    public struct PlayerData : IComponentData
    {
        public Entity prefab;
    }

    public class PlayerDataBaker : Baker<PlayerDataAuthoring>
    {
        public override void Bake(PlayerDataAuthoring authoring)
        {
            if (!authoring.MarkDependencies(this))
                return;

            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent(this));
        }
    }
}