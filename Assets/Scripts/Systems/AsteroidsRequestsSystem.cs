using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class AsteroidsRequestsSystem : SystemBase
    {
        private EntityCommandBufferSystem _commands;
        private EntityQuery _viewBoundsQuery;
        private EntityQuery _spaceBoundsQuery;
        private EntityQuery _asteroidDataQuery;
        private EntityArchetype _asteroidArchetype;

        private EntityQuery GetViewBoundsQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<ViewBounds>(),
                ComponentType.ReadOnly<Bounds>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }

        private bool TryGetViewBounds(out Bounds bounds)
        {
            if (_viewBoundsQuery.TryGetSingleton<Bounds>(out bounds) &&
                _viewBoundsQuery.TryGetSingletonSafely<LocalTransform>(out var transform))
            {
                bounds = bounds.Translated(transform.Position.xy);
                return true;
            }
            return false;
        }

        private EntityQuery GetSpaceBoundsQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<SpaceBounds>(),
                ComponentType.ReadOnly<Bounds>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }

        private bool TryGetSpaceBounds(out Bounds bounds)
        {
            if (_spaceBoundsQuery.TryGetSingleton<Bounds>(out bounds) &&
                _spaceBoundsQuery.TryGetSingletonSafely<LocalTransform>(out var transform))
            {
                bounds = bounds.Translated(transform.Position.xy);
                return true;
            }
            return false;
        }

        private EntityQuery GetAsteroidDataQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<AsteroidData>()
            );
        }

        private bool TryGetAsteroidData(out AsteroidData data)
        {
            return _asteroidDataQuery.TryGetSingleton<AsteroidData>(out data);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
            _viewBoundsQuery = GetViewBoundsQuery();
            _spaceBoundsQuery = GetSpaceBoundsQuery();
            _asteroidDataQuery = GetAsteroidDataQuery();
            _asteroidArchetype = AsteroidData.CreateAsteroidArchetype(EntityManager);
        }

        protected override void OnUpdate()
        {
            var deltaTime = World.Time.DeltaTime;

            if (!TryGetViewBounds(out var viewBounds) ||
                !TryGetSpaceBounds(out var spaceBounds) ||
                !TryGetAsteroidData(out var asteroidData))
            {
                return;
            }

            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var asteroidArchetype = _asteroidArchetype;

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, ref RequestDelay delay, in AsteroidRequest request) =>
                {
                    delay.value -= deltaTime;
                    if (delay.value > 0.0f)
                        return;

                    commands.DestroyEntity(entityInQueryIndex, entity);

                    var random = new Random(request.seed);

                    var position = random.NextFloat2(spaceBounds.min, spaceBounds.max);
                    if (math.all(spaceBounds.min < viewBounds.min) && math.all(viewBounds.max < spaceBounds.max))
                        while (math.all(viewBounds.min <= position) && math.all(position <= viewBounds.max))
                            position = random.NextFloat2(spaceBounds.min, spaceBounds.max);
                    var radius = random.NextFloat(asteroidData.minRadius, asteroidData.maxRadius);
                    var direction = random.NextFloat2Direction();
                    var speed = random.NextFloat(asteroidData.minSpeed, asteroidData.maxSpeed);

                    var transform = new LocalTransform
                    {
                        Position = new float3(position, 0.0f),
                        Rotation = quaternion.identity,
                        Scale = radius * 2.0f
                    };
                    var collider = new Collider { radius = radius };
                    var movementDirection = new MovementDirection { value = direction };
                    var movementSpeed = new MovementSpeed { value = speed };

                    var visible = (
                        math.all(viewBounds.min <= position) &&
                        math.all(position <= viewBounds.max)
                    );
                    var asteroid = visible ?
                        commands.Instantiate(entityInQueryIndex, asteroidData.prefab) :
                        commands.CreateEntity(entityInQueryIndex, asteroidArchetype);

                    commands.SetComponent(entityInQueryIndex, asteroid, transform);
                    commands.SetComponent(entityInQueryIndex, asteroid, collider);
                    commands.SetComponent(entityInQueryIndex, asteroid, movementDirection);
                    commands.SetComponent(entityInQueryIndex, asteroid, movementSpeed);
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
