using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public float movementAcceleration;
        public float movementDeacceleration;
        public float movementSpeedLimit;

        public float turnAcceleration;
        public float turnDeacceleration;
        public float turnSpeedLimit;

        public Player AsComponent
        {
            get => new Player
            {
                movementAcceleration = movementAcceleration,
                movementDeacceleration = movementDeacceleration,
                movementSpeedLimit = movementSpeedLimit,

                turnAcceleration = turnAcceleration,
                turnDeacceleration = turnDeacceleration,
                turnSpeedLimit = turnSpeedLimit
            };
        }
    }

    public struct Player : IComponentData
    {
        public float movementAcceleration;
        public float movementDeacceleration;
        public float movementSpeedLimit;

        public float turnAcceleration;
        public float turnDeacceleration;
        public float turnSpeedLimit;
    }

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var target = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(target, authoring.AsComponent);
        }
    }
}
