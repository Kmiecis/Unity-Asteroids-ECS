using Unity.Entities;

namespace Asteroids
{
    public partial class LivetimeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((ref Livetime livetime) =>
                {
                    livetime.value += deltaTime;
                })
                .ScheduleParallel();
        }
    }
}
