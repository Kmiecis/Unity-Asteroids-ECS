using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class RotateSystem : SystemBase
    {
        private static readonly float3 kRotationAxis = new float3(0.0f, 0.0f, 1.0f);

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((ref Rotation rotation, in RotateSpeed speed, in LocalToWorld transform) =>
                {
                    var angle = deltaTime * math.radians(speed.value);
                    rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(kRotationAxis, angle));
                })
                .ScheduleParallel();
        }
    }
}
