using Unity.Entities;

namespace Asteroids
{
    [UpdateAfter(typeof(CollisionSystem))]
    public partial class PlayerCollisionSystem : SystemBase
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

            Entities
                .WithAll<Player>()
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
