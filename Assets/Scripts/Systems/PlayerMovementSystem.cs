using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Asteroids
{
    [UpdateBefore(typeof(MovementSystem))]
    public partial class PlayerMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = World.Time.DeltaTime;

            var accelerate = Input.GetKey(KeyCode.W);

            Entities
                .ForEach((ref MovementSpeed speed, ref MovementDirection direction, in Player player, in LocalToWorld transform) =>
                {
                    if (accelerate)
                    {
                        var currentDirection = direction.value;
                        var currentSpeed = speed.value;

                        var playerDirection = transform.Up.xy;
                        var playerAcceleration = deltaTime * player.movementAcceleration;

                        var vector = currentSpeed * currentDirection + playerAcceleration * playerDirection;
                        var length = math.length(vector);
                        if (length > 0)
                        {
                            speed.value = math.clamp(length, 0.0f, player.movementSpeedLimit);
                            direction.value = math.normalize(vector);
                        }
                    }
                    else if (speed.value > 0.0f)
                    {
                        var playerDeacceleration = deltaTime * player.movementDeacceleration;
                        speed.value = math.max(speed.value + playerDeacceleration, 0.0f);
                    }
                })
                .ScheduleParallel();
        }
    }
}
