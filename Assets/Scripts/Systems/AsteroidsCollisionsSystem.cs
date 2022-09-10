using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    [UpdateAfter(typeof(CollisionSystem))]
    public partial class AsteroidsCollisionsSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;
        private EntityQuery _viewBoundsQuery;
        private EntityQuery _spaceBoundsQuery;
        private EntityQuery _asteroidDataQuery;
        private EntityArchetype _requestArchetype;

        private EntityQuery GetViewBoundsQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<ViewBounds>(),
                ComponentType.ReadOnly<Translation>()
            );
        }

        private ViewBounds GetViewBounds()
        {
            var viewBounds = _viewBoundsQuery.GetSingleton<ViewBounds>();
            var translation = _viewBoundsQuery.GetSingleton<Translation>();
            return viewBounds.Translated(translation.Value.xy);
        }

        private EntityQuery GetSpaceBoundsQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<SpaceBounds>(),
                ComponentType.ReadOnly<Translation>()
            );
        }

        private SpaceBounds GetSpaceBounds()
        {
            var spaceBounds = _spaceBoundsQuery.GetSingleton<SpaceBounds>();
            var translation = _spaceBoundsQuery.GetSingleton<Translation>();
            return spaceBounds.Translated(translation.Value.xy);
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
            _viewBoundsQuery = GetViewBoundsQuery();
            _spaceBoundsQuery = GetSpaceBoundsQuery();
            _asteroidDataQuery = GetAsteroidDataQuery();
            _requestArchetype = GetAsteroidRequestArchetype();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var viewBounds = GetViewBounds();
            var spaceBounds = GetSpaceBounds();
            var asteroidData = GetAsteroidData();
            var requestArchetype = _requestArchetype;

            Entities
                .WithAll<Asteroid>()
                .ForEach((int entityInQueryIndex, Entity entity, in Translation translation, in Collided collided) =>
                {
                    if (collided.value)
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);

                        var seed = math.max(math.hash(translation.Value), 1);
                        var random = new Random(seed);

                        var position = random.NextFloat2(spaceBounds.min, spaceBounds.max);
                        while (math.all(viewBounds.min <= position) && math.all(position <= viewBounds.max))
                            position = random.NextFloat2(spaceBounds.min, spaceBounds.max);
                        var direction = random.NextFloat2Direction();
                        var speed = random.NextFloat(asteroidData.minSpeed, asteroidData.maxSpeed);

                        var request = new AsteroidRequest
                        {
                            position = position,
                            direction = direction,
                            speed = speed
                        };
                        var requestDelay = new RequestDelay
                        {
                            value = 1.0f
                        };

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
