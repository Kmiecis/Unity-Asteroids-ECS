using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class PlayerScoreAuthoring : MonoBehaviour
    {
        public int value;

        public PlayerScore AsComponent
        {
            get => new PlayerScore
            {
                value = value
            };
        }
    }

    public struct PlayerScore : IComponentData
    {
        public int value;
    }

    public class PlayerScoreBaker : Baker<PlayerScoreAuthoring>
    {
        public override void Bake(PlayerScoreAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
