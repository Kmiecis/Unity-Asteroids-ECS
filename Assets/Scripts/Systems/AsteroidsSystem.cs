using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class AsteroidsSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;
        private EntityQuery _viewBoundsQuery;
        private EntityQuery _asteroidPrefabQuery;
        private EntityArchetype _asteroidArchetype;

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _viewBoundsQuery = GetViewBoundsQuery();
            _asteroidPrefabQuery = GetAsteroidPrefabQuery();
            _asteroidArchetype = GetAsteroidArchetype();
        }

        private EntityQuery GetViewBoundsQuery()
        {
            var desc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<ViewBounds>(),
                    ComponentType.ReadOnly<Translation>()
                }
            };
            return GetEntityQuery(desc);
        }

        private ViewBounds GetViewBounds()
        {
            var result = _viewBoundsQuery.GetSingleton<ViewBounds>();
            var translation = _viewBoundsQuery.GetSingleton<Translation>();
            result.min += translation.Value;
            result.max += translation.Value;
            return result;
        }

        private EntityQuery GetAsteroidPrefabQuery()
        {
            var desc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<AsteroidPrefab>()
                }
            };
            return GetEntityQuery(desc);
        }

        private Entity GetAsteroidPrefab()
        {
            var asteroidPrefab = _asteroidPrefabQuery.GetSingleton<AsteroidPrefab>();
            return asteroidPrefab.value;
        }

        private EntityArchetype GetAsteroidArchetype()
        {
            return EntityManager.CreateArchetype(
                typeof(Translation),
                typeof(Rotation),
                typeof(NonUniformScale),
                typeof(Collider),
                typeof(MovementDirection),
                typeof(MovementSpeed),
                typeof(ViewHidden)
            );
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();

            var viewBounds = GetViewBounds();
            var asteroidPrefab = GetAsteroidPrefab();
            var asteroidArchetype = _asteroidArchetype;

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, in AsteroidRequest request) =>
                {
                    commands.DestroyEntity(entityInQueryIndex, entity);

                    var translation = new Translation { Value = new float3(request.translation, 0.0f) };
                    var direction = new MovementDirection { value = new float3(request.direction, 0.0f) };
                    var speed = new MovementSpeed { value = request.speed };

                    var asteroid = commands.Instantiate(entityInQueryIndex, asteroidPrefab);
                    commands.SetComponent(entityInQueryIndex, asteroid, translation);
                    commands.SetComponent(entityInQueryIndex, asteroid, direction);
                    commands.SetComponent(entityInQueryIndex, asteroid, speed);
                })
                .ScheduleParallel();

            Entities
                .WithAll<ViewVisible>()
                .ForEach((
                    int entityInQueryIndex, Entity entity,
                    in Translation translation, in Rotation rotation, in NonUniformScale scale,
                    in Collider collider, in MovementDirection direction, in MovementSpeed speed
                ) =>
                {
                    if (
                        math.any(viewBounds.min > translation.Value) ||
                        math.any(translation.Value > viewBounds.max)
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

            Entities
                .WithAll<ViewHidden>()
                .ForEach((
                    int entityInQueryIndex, Entity entity,
                    in Translation translation, in Rotation rotation, in NonUniformScale scale,
                    in MovementDirection direction, in MovementSpeed speed
                ) =>
                {
                    if (
                        math.all(viewBounds.min <= translation.Value) &&
                        math.all(translation.Value <= viewBounds.max)
                    )
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);

                        var asteroid = commands.Instantiate(entityInQueryIndex, asteroidPrefab);
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
