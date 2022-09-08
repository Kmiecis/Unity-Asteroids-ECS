using Unity.Entities;
using Unity.Transforms;

namespace Asteroids
{
    public partial class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((ref Translation translation, in MovementDirection direction, in MovementSpeed speed) =>
                {
                    translation.Value += deltaTime * speed.value * direction.value;
                })
                .ScheduleParallel();
        }
    }
}
