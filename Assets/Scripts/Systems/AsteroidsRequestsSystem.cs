using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class AsteroidsRequestsSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;
        private EntityQuery _viewBoundsQuery;
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
            _asteroidDataQuery = GetAsteroidDataQuery();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var viewBounds = GetViewBounds();
            var asteroidData = GetAsteroidData();

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, ref RequestDelay delay, in AsteroidRequest request) =>
                {
                    delay.value -= deltaTime;
                    if (delay.value > 0.0f)
                        return;

                    commands.DestroyEntity(entityInQueryIndex, entity);

                    var translation = new Translation { Value = new float3(request.position, 0.0f) };
                    var rotation = new Rotation { Value = quaternion.identity };
                    var scale = new NonUniformScale { Value = new float3(request.radius * 2.0f) };
                    var collider = new Collider { radius = request.radius };
                    var direction = new MovementDirection { value = request.direction };
                    var speed = new MovementSpeed { value = request.speed };

                    var visible = math.all(viewBounds.min <= request.position) && math.all(request.position <= viewBounds.max);
                    var asteroid = visible ?
                        commands.Instantiate(entityInQueryIndex, asteroidData.prefab) :
                        commands.CreateEntity(entityInQueryIndex, asteroidData.archetype);

                    commands.SetComponent(entityInQueryIndex, asteroid, translation);
                    commands.SetComponent(entityInQueryIndex, asteroid, rotation);
                    commands.SetComponent(entityInQueryIndex, asteroid, scale);
                    commands.SetComponent(entityInQueryIndex, asteroid, collider);
                    commands.SetComponent(entityInQueryIndex, asteroid, direction);
                    commands.SetComponent(entityInQueryIndex, asteroid, speed);
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
