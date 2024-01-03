using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Asteroids
{
    [UpdateAfter(typeof(VisibilitySystem))]
    public partial class AsteroidsVisibilitySystem : SystemBase
    {
        private EntityCommandBufferSystem _commands;
        private EntityQuery _entityQuery;
        private EntityQuery _asteroidDataQuery;
        private EntityArchetype _asteroidArchetype;

        [BurstCompile]
        private struct VisibilityJob : IJobChunk
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
            public ComponentTypeHandle<LocalTransform> transformType;
            [ReadOnly]
            public ComponentTypeHandle<Collider> colliderType;
            [ReadOnly]
            public ComponentTypeHandle<MovementDirection> directionType;
            [ReadOnly]
            public ComponentTypeHandle<MovementSpeed> speedType;
            [ReadOnly]
            public AsteroidData asteroidData;
            [ReadOnly]
            public EntityArchetype asteroidArchetype;

            public EntityCommandBuffer.ParallelWriter commands;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var entities = chunk.GetNativeArray(entityType);
                var visibilities = chunk.GetNativeArray(ref visibilityType);
                var transforms = chunk.GetNativeArray(ref transformType);
                var colliders = chunk.GetNativeArray(ref colliderType);
                var directions = chunk.GetNativeArray(ref directionType);
                var speeds = chunk.GetNativeArray(ref speedType);

                if (chunk.Has(ref visibleType))
                {
                    for (int i = 0; i < chunk.Count; ++i)
                    {
                        var visibility = visibilities[i];

                        if (!visibility.value)
                        {
                            commands.DestroyEntity(unfilteredChunkIndex, entities[i]);

                            var asteroid = commands.CreateEntity(unfilteredChunkIndex, asteroidArchetype);
                            commands.SetComponent(unfilteredChunkIndex, asteroid, transforms[i]);
                            commands.SetComponent(unfilteredChunkIndex, asteroid, colliders[i]);
                            commands.SetComponent(unfilteredChunkIndex, asteroid, directions[i]);
                            commands.SetComponent(unfilteredChunkIndex, asteroid, speeds[i]);
                        }
                    }
                }
                else if (chunk.Has(ref hiddenType))
                {
                    for (int i = 0; i < chunk.Count; ++i)
                    {
                        var visibility = visibilities[i];

                        if (visibility.value)
                        {
                            commands.DestroyEntity(unfilteredChunkIndex, entities[i]);

                            var asteroid = commands.Instantiate(unfilteredChunkIndex, asteroidData.prefab);
                            commands.SetComponent(unfilteredChunkIndex, asteroid, transforms[i]);
                            commands.SetComponent(unfilteredChunkIndex, asteroid, colliders[i]);
                            commands.SetComponent(unfilteredChunkIndex, asteroid, directions[i]);
                            commands.SetComponent(unfilteredChunkIndex, asteroid, speeds[i]);
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
                ComponentType.ReadOnly<LocalTransform>(),
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

        private bool TryGetAsteroidData(out AsteroidData data)
        {
            return _asteroidDataQuery.TryGetSingleton<AsteroidData>(out data);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
            _entityQuery = GetEntityQuery();
            _asteroidDataQuery = GetAsteroidDataQuery();
            _asteroidArchetype = AsteroidData.CreateAsteroidArchetype(EntityManager);
        }

        protected override void OnUpdate()
        {
            if (!TryGetAsteroidData(out var asteroidData))
            {
                return;
            }

            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var asteroidArchetype = _asteroidArchetype;

            var job = new VisibilityJob
            {
                entityType = GetEntityTypeHandle(),
                visibleType = GetComponentTypeHandle<Visible>(),
                hiddenType = GetComponentTypeHandle<Hidden>(),
                visibilityType = GetComponentTypeHandle<Visibility>(),
                transformType = GetComponentTypeHandle<LocalTransform>(),
                colliderType = GetComponentTypeHandle<Collider>(),
                directionType = GetComponentTypeHandle<MovementDirection>(),
                speedType = GetComponentTypeHandle<MovementSpeed>(),
                asteroidData = asteroidData,
                asteroidArchetype = asteroidArchetype,
                commands = commands
            };
            this.Dependency = job.ScheduleParallel(_entityQuery, this.Dependency);

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
