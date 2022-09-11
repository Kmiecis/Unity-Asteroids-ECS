using Unity.Entities;

namespace Asteroids
{
    public partial class LifetimeSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;

        protected override void OnCreate()
        {
            base.OnCreate();
            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();

            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, ref Lifetime lifetime) =>
                {
                    var lifetimeValue = lifetime.value;
                    lifetime.value -= deltaTime;

                    if (lifetime.value <= 0.0f && lifetimeValue > 0.0f)
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);
                    }
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
