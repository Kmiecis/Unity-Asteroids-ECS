using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class AsteroidsSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;
        private EntityQuery _viewBoundsQuery;
        private EntityQuery _asteroidDataQuery;
        private EntityArchetype _asteroidArchetype;

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

        private EntityQuery GetAsteroidPrefabQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<AsteroidData>()
            );
        }

        private AsteroidData GetAsteroidData()
        {
            return _asteroidDataQuery.GetSingleton<AsteroidData>();
        }

        private EntityArchetype GetAsteroidArchetype()
        {
            return EntityManager.CreateArchetype(
                typeof(Asteroid),
                typeof(Translation),
                typeof(Rotation),
                typeof(NonUniformScale),
                typeof(Collider),
                typeof(Collided),
                typeof(MovementDirection),
                typeof(MovementSpeed),
                typeof(ViewHidden)
            );
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _viewBoundsQuery = GetViewBoundsQuery();
            _asteroidDataQuery = GetAsteroidPrefabQuery();
            _asteroidArchetype = GetAsteroidArchetype();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();

            var viewBounds = GetViewBounds();
            var asteroidData = GetAsteroidData();
            var asteroidArchetype = _asteroidArchetype;

            var deltaTime = Time.DeltaTime;
            // Create asteroids from requests
            Entities
                .ForEach((int entityInQueryIndex, Entity entity, ref RequestDelay delay, in AsteroidRequest request) =>
                {
                    delay.value -= deltaTime;
                    if (delay.value > 0.0f)
                        return;

                    commands.DestroyEntity(entityInQueryIndex, entity);

                    var translation = new Translation { Value = new float3(request.position, 0.0f) };
                    var rotation = new Rotation { Value = quaternion.identity };
                    var scale = new NonUniformScale { Value = new float3(asteroidData.radius * 2.0f) };
                    var collider = new Collider { radius = asteroidData.radius };
                    var direction = new MovementDirection { value = request.direction };
                    var speed = new MovementSpeed { value = request.speed };

                    var asteroid = (math.all(viewBounds.min <= request.position) && math.all(request.position <= viewBounds.max)) ?
                        commands.Instantiate(entityInQueryIndex, asteroidData.prefab) :
                        commands.CreateEntity(entityInQueryIndex, asteroidArchetype);

                    commands.SetComponent(entityInQueryIndex, asteroid, translation);
                    commands.SetComponent(entityInQueryIndex, asteroid, rotation);
                    commands.SetComponent(entityInQueryIndex, asteroid, scale);
                    commands.SetComponent(entityInQueryIndex, asteroid, collider);
                    commands.SetComponent(entityInQueryIndex, asteroid, direction);
                    commands.SetComponent(entityInQueryIndex, asteroid, speed);
                })
                .ScheduleParallel();

            // Hide asteroids as necessary
            Entities
                .WithAll<ViewVisible>()
                .ForEach((
                    int entityInQueryIndex, Entity entity,
                    in Translation translation, in Rotation rotation, in NonUniformScale scale,
                    in Collider collider, in MovementDirection direction, in MovementSpeed speed
                ) =>
                {
                    var position = translation.Value.xy;
                    if (
                        math.any(viewBounds.min > position) ||
                        math.any(position > viewBounds.max)
                    )
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);

                        var asteroid = commands.CreateEntity(entityInQueryIndex, asteroidArchetype);
                        commands.SetComponent(entityInQueryIndex, asteroid, translation);
                        commands.SetComponent(entityInQueryIndex, asteroid, rotation);
                        commands.SetComponent(entityInQueryIndex, asteroid, scale);
                        commands.SetComponent(entityInQueryIndex, asteroid, collider);
                        commands.SetComponent(entityInQueryIndex, asteroid, direction);
                        commands.SetComponent(entityInQueryIndex, asteroid, speed);
                    }
                })
                .ScheduleParallel();

            // Show asteroids as necessary
            Entities
                .WithAll<ViewHidden>()
                .ForEach((
                    int entityInQueryIndex, Entity entity,
                    in Translation translation, in Rotation rotation, in NonUniformScale scale,
                    in MovementDirection direction, in MovementSpeed speed
                ) =>
                {
                    var position = translation.Value.xy;
                    if (
                        math.all(viewBounds.min <= position) &&
                        math.all(position <= viewBounds.max)
                    )
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);

                        var asteroid = commands.Instantiate(entityInQueryIndex, asteroidData.prefab);
                        commands.SetComponent(entityInQueryIndex, asteroid, translation);
                        commands.SetComponent(entityInQueryIndex, asteroid, rotation);
                        commands.SetComponent(entityInQueryIndex, asteroid, scale);
                        commands.SetComponent(entityInQueryIndex, asteroid, direction);
                        commands.SetComponent(entityInQueryIndex, asteroid, speed);
                    }
                })
                .ScheduleParallel();
            
            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
