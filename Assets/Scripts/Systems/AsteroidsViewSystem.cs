using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class AsteroidsViewSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;
        private EntityArchetype _archetype;

        protected override void OnCreate()
        {
            base.OnCreate();
            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _archetype = EntityManager.CreateArchetype(
                typeof(Translation),
                typeof(Rotation),
                typeof(NonUniformScale),
                typeof(Collider),
                typeof(ViewHidden)
            );
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();

            var viewBoundsQuery = GetEntityQuery(typeof(ViewBounds), typeof(Translation));
            var viewBounds = viewBoundsQuery.GetSingleton<ViewBounds>();
            var viewBoundsTranslation = viewBoundsQuery.GetSingleton<Translation>();
            viewBounds.min += viewBoundsTranslation.Value;
            viewBounds.max += viewBoundsTranslation.Value;

            var asteroidPrefabQuery = GetEntityQuery(typeof(AsteroidPrefab));
            var asteroidPrefab = asteroidPrefabQuery.GetSingleton<AsteroidPrefab>();

            var archetype = _archetype;

            Entities
                .WithAll<ViewVisible>()
                .ForEach((int entityInQueryIndex, Entity entity, in Translation translation, in Rotation rotation, in NonUniformScale scale, in Collider collider) =>
                {
                    if (
                        math.any(viewBounds.min > translation.Value) ||
                        math.any(translation.Value > viewBounds.max)
                    )
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);

                        var asteroid = commands.CreateEntity(entityInQueryIndex, archetype);
                        commands.SetComponent(entityInQueryIndex, asteroid, translation);
                        commands.SetComponent(entityInQueryIndex, asteroid, rotation);
                        commands.SetComponent(entityInQueryIndex, asteroid, scale);
                        commands.SetComponent(entityInQueryIndex, asteroid, collider);
                    }
                })
                .ScheduleParallel();

            Entities
                .WithAll<ViewHidden>()
                .ForEach((int entityInQueryIndex, Entity entity, in Translation translation, in Rotation rotation, in NonUniformScale scale, in Collider collider) =>
                {
                    if (
                        math.all(viewBounds.min <= translation.Value) &&
                        math.all(translation.Value <= viewBounds.max)
                    )
                    {
                        commands.DestroyEntity(entityInQueryIndex, entity);

                        var asteroid = commands.Instantiate(entityInQueryIndex, asteroidPrefab.value);
                        commands.SetComponent(entityInQueryIndex, asteroid, translation);
                        commands.SetComponent(entityInQueryIndex, asteroid, rotation);
                        commands.SetComponent(entityInQueryIndex, asteroid, scale);
                        commands.SetComponent(entityInQueryIndex, asteroid, collider);
                    }
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
