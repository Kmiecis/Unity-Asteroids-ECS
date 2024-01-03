using Unity.Entities;

namespace Asteroids
{
    [UpdateAfter(typeof(CollisionSystem))]
    public partial class PlayerCollisionSystem : SystemBase
    {
        private EntityCommandBufferSystem _commands;

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
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
