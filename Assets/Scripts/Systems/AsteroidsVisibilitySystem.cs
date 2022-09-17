using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Asteroids
{
    [UpdateAfter(typeof(VisibilitySystem))]
    public partial class AsteroidsVisibilitySystem : SystemBase
    {
        private EntityCommandBufferSystem _commands;
        private EntityQuery _entityQuery;
        private EntityQuery _asteroidDataQuery;

        [BurstCompile]
        private struct VisibilityJob : IJobEntityBatchWithIndex
        {
            [ReadOnly]
            public EntityTypeHandle entityType;
            [ReadOnly]
            public ComponentTypeHandle<Visible> visibleType;
            [ReadOnly]
            public ComponentTypeHandle<Hidden> hiddenType;
            [ReadOnly]
            public ComponentTypeHandle<Visibility> visibilityType;
            [ReadOnly]
            public ComponentTypeHandle<Translation> translationType;
            [ReadOnly]
            public ComponentTypeHandle<Rotation> rotationType;
            [ReadOnly]
            public ComponentTypeHandle<NonUniformScale> scaleType;
            [ReadOnly]
            public ComponentTypeHandle<Collider> colliderType;
            [ReadOnly]
            public ComponentTypeHandle<MovementDirection> directionType;
            [ReadOnly]
            public ComponentTypeHandle<MovementSpeed> speedType;
            [ReadOnly]
            public AsteroidData asteroidData;

            public EntityCommandBuffer.ParallelWriter commands;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex, int indexOfFirstEntityInQuery)
            {
                var entities = batchInChunk.GetNativeArray(entityType);
                var visibilities = batchInChunk.GetNativeArray(visibilityType);
                var translations = batchInChunk.GetNativeArray(translationType);
                var rotations = batchInChunk.GetNativeArray(rotationType);
                var scales = batchInChunk.GetNativeArray(scaleType);
                var colliders = batchInChunk.GetNativeArray(colliderType);
                var directions = batchInChunk.GetNativeArray(directionType);
                var speeds = batchInChunk.GetNativeArray(speedType);

                if (batchInChunk.Has(visibleType))
                {
                    for (int i = 0; i < batchInChunk.Count; ++i)
                    {
                        var visibility = visibilities[i];

                        if (!visibility.value)
                        {
                            int entityInQueryIndex = i + indexOfFirstEntityInQuery;
                            commands.DestroyEntity(entityInQueryIndex, entities[i]);

                            var asteroid = commands.CreateEntity(i, asteroidData.archetype);
                            commands.SetComponent(entityInQueryIndex, asteroid, translations[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, rotations[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, scales[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, colliders[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, directions[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, speeds[i]);
                        }
                    }
                }
                else if (batchInChunk.Has(hiddenType))
                {
                    for (int i = 0; i < batchInChunk.Count; ++i)
                    {
                        var visibility = visibilities[i];

                        if (visibility.value)
                        {
                            int entityInQueryIndex = i + indexOfFirstEntityInQuery;
                            commands.DestroyEntity(entityInQueryIndex, entities[i]);

                            var asteroid = commands.Instantiate(i, asteroidData.prefab);
                            commands.SetComponent(entityInQueryIndex, asteroid, translations[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, rotations[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, scales[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, colliders[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, directions[i]);
                            commands.SetComponent(entityInQueryIndex, asteroid, speeds[i]);
                        }
                    }
                }
            }
        }

        private EntityQuery GetEntityQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<Asteroid>(),
                ComponentType.ReadOnly<Visibility>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Rotation>(),
                ComponentType.ReadOnly<NonUniformScale>(),
                ComponentType.ReadOnly<Collider>(),
                ComponentType.ReadOnly<MovementDirection>(),
                ComponentType.ReadOnly<MovementSpeed>()
            );
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
            _entityQuery = GetEntityQuery();
            _asteroidDataQuery = GetAsteroidDataQuery();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var asteroidData = GetAsteroidData();

            var job = new VisibilityJob
            {
                entityType = GetEntityTypeHandle(),
                visibleType = GetComponentTypeHandle<Visible>(),
                hiddenType = GetComponentTypeHandle<Hidden>(),
                visibilityType = GetComponentTypeHandle<Visibility>(),
                translationType = GetComponentTypeHandle<Translation>(),
                rotationType = GetComponentTypeHandle<Rotation>(),
                scaleType = GetComponentTypeHandle<NonUniformScale>(),
                colliderType = GetComponentTypeHandle<Collider>(),
                directionType = GetComponentTypeHandle<MovementDirection>(),
                speedType = GetComponentTypeHandle<MovementSpeed>(),
                asteroidData = asteroidData,
                commands = commands
            };
            this.Dependency = job.ScheduleParallel(_entityQuery, this.Dependency);

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
