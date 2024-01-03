using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    [UpdateBefore(typeof(RotateSystem))]
    public partial class PlayerRotateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = World.Time.DeltaTime;

            var turnLeft = Input.GetKey(KeyCode.A);
            var turnRight = Input.GetKey(KeyCode.D);

            Entities
                .ForEach((ref RotateSpeed speed, in Player player) =>
                {
                    var turnAcceleration = deltaTime * player.turnAcceleration;
                    var turnDeacceleration = deltaTime * player.turnDeacceleration;
                    var turnSpeedLimit = player.turnSpeedLimit;

                    if (turnLeft)
                    {
                        speed.value = math.min(speed.value + turnAcceleration, turnSpeedLimit);
                    }

                    if (turnRight)
                    {
                        speed.value = math.max(speed.value - turnAcceleration, -turnSpeedLimit);
                    }

                    if (
                        (!turnLeft && !turnRight) ||
                        (turnLeft && turnRight)
                    )
                    {
                        if (speed.value < 0.0f)
                        {
                            speed.value = math.min(speed.value - turnDeacceleration, 0.0f);
                        }
                        else if (speed.value > 0.0f)
                        {
                            speed.value = math.max(speed.value + turnDeacceleration, 0.0f);
                        }
                    }
                })
                .ScheduleParallel();
        }
    }
}
