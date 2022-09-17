using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class SimulationSystem : SystemBase
    {
        private EntityCommandBufferSystem _commands;
        private EntityQuery _viewBoundsQuery;
        private EntityQuery _spaceBoundsQuery;
        private EntityQuery _asteroidDataQuery;

        private EntityQuery GetViewBoundsQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<ViewBounds>(),
                ComponentType.ReadOnly<Bounds>(),
                ComponentType.ReadOnly<Translation>()
            );
        }

        private Bounds GetViewBounds()
        {
            var bounds = _viewBoundsQuery.GetSingleton<Bounds>();
            var translation = _viewBoundsQuery.GetSingleton<Translation>();
            return bounds.Translated(translation.Value.xy);
        }

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

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _viewBoundsQuery = GetViewBoundsQuery();
            _spaceBoundsQuery = GetSpaceBoundsQuery();
            _asteroidDataQuery = GetAsteroidDataQuery();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var viewBounds = GetViewBounds();
            var spaceBounds = GetSpaceBounds();
            var asteroidData = GetAsteroidData();

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, in SimulateRequest request) =>
                {
                    var random = new Random(request.seed);

                    for (float y = spaceBounds.min.y; y <= spaceBounds.max.y; y += 1.0f)
                    {
                        for (float x = spaceBounds.min.x; x <= spaceBounds.max.x; x += 1.0f)
                        {
                            var position = new float2(x, y);
                            var radius = random.NextFloat(asteroidData.minRadius, asteroidData.maxRadius);
                            var direction = random.NextFloat2Direction();
                            var speed = random.NextFloat(asteroidData.minSpeed, asteroidData.maxSpeed);

                            var translation = new Translation { Value = new float3(position, 0.0f) };
                            var rotation = new Rotation { Value = quaternion.identity };
                            var scale = new NonUniformScale { Value = new float3(radius * 2.0f) };
                            var collider = new Collider { radius = radius };
                            var movementDirection = new MovementDirection { value = direction };
                            var movementSpeed = new MovementSpeed { value = speed };

                            var visible = math.all(viewBounds.min <= position) && math.all(position <= viewBounds.max);
                            var asteroid = visible ?
                                commands.Instantiate(entityInQueryIndex, asteroidData.prefab) :
                                commands.CreateEntity(entityInQueryIndex, asteroidData.archetype);

                            commands.SetComponent(entityInQueryIndex, asteroid, translation);
                            commands.SetComponent(entityInQueryIndex, asteroid, rotation);
                            commands.SetComponent(entityInQueryIndex, asteroid, scale);
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
