using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((ref Translation translation, in MovementDirection direction, in MovementSpeed speed) =>
                {
                    translation.Value += new float3(deltaTime * speed.value * direction.value, 0.0f);
                })
                .ScheduleParallel();
        }
    }
}
