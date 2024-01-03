using Unity.Collections;
using Unity.Entities;

namespace Asteroids
{
    public partial class BulletsCollisionSystem : SystemBase
    {
        private EntityCommandBufferSystem _commands;

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var collisionsQueue = new NativeQueue<int>(Allocator.TempJob);
            var collisionsQueueWriter = collisionsQueue.AsParallelWriter();

            Entities
                .WithAll<Bullet>()
                .ForEach((int entityInQueryIndex, Entity entity, in Collided collided) =>
                {
                    if (collided.value)
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);

                        collisionsQueueWriter.Enqueue(1);
                    }
                })
                .ScheduleParallel();

            Entities
                .WithReadOnly(collisionsQueue)
                .ForEach((ref PlayerScore playerScore) =>
                {
                    playerScore.value += collisionsQueue.Count;
                })
                .WithDisposeOnCompletion(collisionsQueue)
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
