using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class SimulationSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;
        private EntityQuery _spaceBoundsQuery;
        private EntityQuery _asteroidDataQuery;
        private EntityArchetype _requestArchetype;

        private EntityQuery GetSpaceBoundsQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<SpaceBounds>(),
                ComponentType.ReadOnly<Bounds>(),
                ComponentType.ReadOnly<Translation>()
            );
        }

        private Bounds GetSpaceBounds()
        {
            var bounds = _spaceBoundsQuery.GetSingleton<Bounds>();
            var translation = _spaceBoundsQuery.GetSingleton<Translation>();
            return bounds.Translated(translation.Value.xy);
        }

        private EntityQuery GetAsteroidDataQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<AsteroidData>()
            );
        }

        private AsteroidData GetAsteroidData()
        {
            return _asteroidDataQuery.GetSingleton<AsteroidData>();
        }

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

            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _spaceBoundsQuery = GetSpaceBoundsQuery();
            _asteroidDataQuery = GetAsteroidDataQuery();
            _requestArchetype = GetAsteroidRequestArchetype();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var spaceBounds = GetSpaceBounds();
            var asteroidData = GetAsteroidData();
            var requestArchetype = _requestArchetype;

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, in SimulateRequest request) =>
                {
                    var random = new Random(request.seed);

                    for (float y = spaceBounds.min.y; y <= spaceBounds.max.y; y += 1.0f)
                    {
                        for (float x = spaceBounds.min.x; x <= spaceBounds.max.x; x += 1.0f)
                        {
                            var position = new float2(x, y);
                            var direction = random.NextFloat2Direction();
                            var speed = random.NextFloat(asteroidData.minSpeed, asteroidData.maxSpeed);

                            var asteroidRequest = new AsteroidRequest
                            {
                                position = position,
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
