using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class SimulationSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;
        private EntityQuery _boundsQuery;
        private EntityArchetype _requestArchetype;

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _boundsQuery = GetSpaceBoundsQuery();
            _requestArchetype = GetAsteroidRequestArchetype();
        }

        private EntityQuery GetSpaceBoundsQuery()
        {
            var desc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<SpaceBounds>(),
                    ComponentType.ReadOnly<Translation>()
                }
            };
            return GetEntityQuery(desc);
        }

        private SpaceBounds GetSpaceBounds()
        {
            var result = _boundsQuery.GetSingleton<SpaceBounds>();
            var translation = _boundsQuery.GetSingleton<Translation>();
            result.min += translation.Value.xy;
            result.max += translation.Value.xy;
            return result;
        }

        private EntityArchetype GetAsteroidRequestArchetype()
        {
            return EntityManager.CreateArchetype(
                typeof(AsteroidRequest),
                typeof(RequestDelay)
            );
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var bounds = GetSpaceBounds();
            var requestArchetype = _requestArchetype;

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, in SimulateRequest request) =>
                {
                    var random = new Random(request.seed);

                    for (float y = bounds.min.y; y <= bounds.max.y; y += 1.0f)
                    {
                        for (float x = bounds.min.x; x <= bounds.max.x; x += 1.0f)
                        {
                            var translation = new float2(x, y);
                            var direction = random.NextFloat2Direction();
                            var speed = random.NextFloat(0.0f, 1.0f);

                            var asteroidRequest = new AsteroidRequest
                            {
                                position = translation,
                                direction = direction,
                                speed = speed
                            };

                            var asteroidRequestEntity = commands.CreateEntity(entityInQueryIndex, requestArchetype);
                            commands.SetComponent(entityInQueryIndex, asteroidRequestEntity, asteroidRequest);
                        }
                    }

                    commands.DestroyEntity(entityInQueryIndex, entity);
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
