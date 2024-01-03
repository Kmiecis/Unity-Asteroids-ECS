using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = World.Time.DeltaTime;

            Entities
                .ForEach((ref LocalTransform transform, in MovementDirection direction, in MovementSpeed speed) =>
                {
                    var deltaPosition = deltaTime * speed.value * direction.value;
                    transform.Position += new float3(deltaPosition, 0.0f);
                })
                .ScheduleParallel();
        }
    }
}
