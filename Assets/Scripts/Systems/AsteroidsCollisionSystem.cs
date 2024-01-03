using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class AsteroidsCollisionSystem : SystemBase
    {
        private EntityCommandBufferSystem _commands;
        private EntityArchetype _requestArchetype;

        private EntityArchetype GetAsteroidRequestArchetype()
        {
            return EntityManager.CreateArchetype(
                typeof(AsteroidRequest),
                typeof(RequestDelay)
            );
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
            _requestArchetype = GetAsteroidRequestArchetype();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var requestArchetype = _requestArchetype;
            
            Entities
                .WithAll<Asteroid>()
                .ForEach((int entityInQueryIndex, Entity entity, in LocalTransform transform, in Collided collided) =>
                {
                    if (collided.value)
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);

                        var seed = math.max(math.hash(transform.Position), 1);
                        var request = new AsteroidRequest { seed = seed };
                        var requestDelay = new RequestDelay { value = 1.0f };

                        var requestEntity = commands.CreateEntity(entityInQueryIndex, requestArchetype);
                        commands.SetComponent(entityInQueryIndex, requestEntity, request);
                        commands.SetComponent(entityInQueryIndex, requestEntity, requestDelay);
                    }
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
