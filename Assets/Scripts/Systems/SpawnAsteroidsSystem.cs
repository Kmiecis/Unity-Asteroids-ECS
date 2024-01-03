using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class SpawnAsteroidsSystem : SystemBase
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
            if (!TryGetViewBounds(out var viewBounds) ||
                !TryGetSpaceBounds(out var spaceBounds) ||
                !TryGetAsteroidData(out var asteroidData))
            {
                return;
            }

            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var asteroidArchetype = _asteroidArchetype;

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, in SpawnAsteroidsRequest request) =>
                {
                    var random = new Random(request.seed);

                    for (float y = spaceBounds.min.y; y <= spaceBounds.max.y; y += 1.0f)
                    {
                        for (float x = spaceBounds.min.x; x <= spaceBounds.max.x; x += 1.0f)
                        {
                            var dx = random.NextFloat(-request.maxOffset, request.maxOffset);
                            var dy = random.NextFloat(-request.maxOffset, request.maxOffset);
                            var position = new float2(x + dx, y + dy);
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
                        }
                    }

                    commands.DestroyEntity(entityInQueryIndex, entity);
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
