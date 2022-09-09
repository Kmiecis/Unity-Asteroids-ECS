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
            var deltaTime = Time.DeltaTime;

            var turnLeft = Input.GetKey(KeyCode.A);
            var turnRight = Input.GetKey(KeyCode.D);

            Entities
                .ForEach((ref RotateSpeed speed, in Player player) =>
                {
                    var turnAcceleration = math.radians(player.turnAcceleration);
                    var turnDeacceleration = math.radians(player.turnDeacceleration);
                    var turnSpeedLimit = math.radians(player.turnSpeedLimit);

                    if (turnLeft)
                    {
                        speed.value = math.min(speed.value + deltaTime * turnAcceleration, turnSpeedLimit);
                    }

                    if (turnRight)
                    {
                        speed.value = math.max(speed.value - deltaTime * turnAcceleration, -turnSpeedLimit);
                    }

                    if (
                        !turnLeft && !turnRight ||
                        turnLeft && turnRight
                    )
                    {
                        if (speed.value < 0.0f)
                        {
                            speed.value = math.min(speed.value - deltaTime * turnDeacceleration, 0.0f);
                        }
                        else if (speed.value > 0.0f)
                        {
                            speed.value = math.max(speed.value + deltaTime * turnDeacceleration, 0.0f);
                        }
                    }
                })
                .ScheduleParallel();
        }
    }
}