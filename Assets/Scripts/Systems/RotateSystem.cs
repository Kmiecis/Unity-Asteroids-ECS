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
            var deltaTime = World.Time.DeltaTime;

            Entities
                .ForEach((ref LocalTransform transform, in RotateSpeed speed) =>
                {
                    var angle = deltaTime * math.radians(speed.value);
                    transform.Rotation = math.mul(transform.Rotation, quaternion.AxisAngle(kRotationAxis, angle));
                })
                .ScheduleParallel();
        }
    }
}
