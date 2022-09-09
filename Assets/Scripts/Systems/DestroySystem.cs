using Unity.Entities;

namespace Asteroids
{
    public partial class DestroySystem : SystemBase
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
                .WithAll<Destroy>()
                .ForEach((int entityInQueryIndex, Entity entity) =>
                {
                    commands.DestroyEntity(entityInQueryIndex, entity);
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
