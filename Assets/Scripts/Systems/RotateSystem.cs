using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class RotateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((ref Rotation rotation, in RotateSpeed speed, in LocalToWorld transform) =>
                {
                    var axis = transform.Forward;
                    var angle = math.radians(speed.value);
                    rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(axis, angle));
                })
                .ScheduleParallel();
        }
    }
}
