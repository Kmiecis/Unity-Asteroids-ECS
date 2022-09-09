using Unity.Entities;

namespace Asteroids
{
    [UpdateAfter(typeof(CollisionSystem))]
    public partial class BulletsCollisionsSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        // Increase score on collision before being destroyed
        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithAll<Bullet>()
                .ForEach((int entityInQueryIndex, Entity entity, in Collided collided) =>
                {
                    if (collided.value)
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);
                    }
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
