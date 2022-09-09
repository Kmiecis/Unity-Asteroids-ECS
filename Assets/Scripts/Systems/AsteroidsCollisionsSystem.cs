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
        private EntityArchetype _requestArchetype;

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _viewBoundsQuery = GetViewBoundsQuery();
            _spaceBoundsQuery = GetSpaceBoundsQuery();
            _requestArchetype = GetAsteroidRequestArchetype();
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
            var result = _spaceBoundsQuery.GetSingleton<SpaceBounds>();
            var translation = _spaceBoundsQuery.GetSingleton<Translation>();
            result.min += translation.Value;
            result.max += translation.Value;
            return result;
        }

        private EntityArchetype GetAsteroidRequestArchetype()
        {
            return EntityManager.CreateArchetype(
                typeof(AsteroidRequest)
            );
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var viewBounds = GetViewBounds();
            var spaceBounds = GetSpaceBounds();
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

                        var position = random.NextFloat2(spaceBounds.min.xy, spaceBounds.max.xy);
                        while (math.all(viewBounds.min.xy <= position) && math.all(position <= viewBounds.max.xy))
                        {
                            position = random.NextFloat2(spaceBounds.min.xy, spaceBounds.max.xy);
                        }

                        var direction = random.NextFloat2Direction();
                        var speed = random.NextFloat(0.0f, 1.0f);

                        var request = new AsteroidRequest
                        {
                            position = position,
                            direction = direction,
                            speed = speed,
                            timeleft = 1.0f
                        };

                        var requestEntity = commands.CreateEntity(entityInQueryIndex, requestArchetype);
                        commands.SetComponent(entityInQueryIndex, requestEntity, request);
                    }
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
