using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    [UpdateAfter(typeof(MovementSystem))]
    public partial class CollisionSystem : SystemBase
    {
        private struct QuadrantData
        {
            public Entity entity;
            public float3 position;
            public Collider collider;
        }

        private const int kQuadrantSize = 1;

        private EntityQuery _spaceBoundsQuery;
        private EntityQuery _collidersQuery;
        private NativeArray<int2> _offsets;

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

        private EntityQuery GetCollidersQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<LocalTransform>(),
                ComponentType.ReadOnly<Collider>()
            );
        }

        private NativeArray<int2> CreateOffsets(Allocator allocator)
        {
            var result = new NativeArray<int2>(8, allocator);
            result[0] = new int2(-1,  0);
            result[1] = new int2(-1,  1);
            result[2] = new int2( 0,  1);
            result[3] = new int2( 1,  1);
            result[4] = new int2( 1,  0);
            result[5] = new int2( 1, -1);
            result[6] = new int2( 0, -1);
            result[7] = new int2(-1, -1);
            return result;
        }

        public static int GetQuadrantKey(float2 position, int size)
        {
            return (int)(math.floor(position.x / kQuadrantSize) + math.floor(position.y / kQuadrantSize) * size);
        }

        public static int GetNextQuadrantKey(int key, int dx, int dy, int size)
        {
            return key + dx + dy * size;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _spaceBoundsQuery = GetSpaceBoundsQuery();
            _collidersQuery = GetCollidersQuery();
            _offsets = CreateOffsets(Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
            if (!TryGetSpaceBounds(out var spaceBounds))
            {
                return;
            }

            var count = _collidersQuery.CalculateEntityCount();
            var spaceSize = spaceBounds.Size;

            var quadrantMap = new NativeParallelMultiHashMap<int, QuadrantData>(count * 9, Allocator.TempJob);
            var quadrantMapWriter = quadrantMap.AsParallelWriter();

            var offsets = _offsets;

            Entities
                .WithReadOnly(offsets)
                .ForEach((int entityInQueryIndex, Entity entity, in LocalTransform transform, in Collider collider) =>
                {
                    var quadrantData = new QuadrantData
                    {
                        entity = entity,
                        position = transform.Position,
                        collider = collider
                    };

                    var position = transform.Position.xy;
                    int quadrantKey = GetQuadrantKey(position, (int)spaceSize.y);
                    quadrantMapWriter.Add(quadrantKey, quadrantData);

                    for (int i = 0; i < offsets.Length; ++i)
                    {
                        var offset = offsets[i];
                        var nextQuadrantKey = GetNextQuadrantKey(quadrantKey, offset.x, offset.y, (int)spaceSize.y);
                        quadrantMapWriter.Add(nextQuadrantKey, quadrantData);
                    }
                })
                .ScheduleParallel();

            Entities
                .WithReadOnly(quadrantMap)
                .ForEach((int entityInQueryIndex, Entity entity, ref Collided collided, in LocalTransform transform, in Collider collider) =>
                {
                    var position = transform.Position.xy + collider.offset;
                    var quadrantKey = GetQuadrantKey(position, (int)spaceSize.y);

                    if (quadrantMap.TryGetFirstValue(quadrantKey, out var data, out var iterator))
                    {
                        do
                        {
                            var otherEntity = data.entity;
                            var otherTranslation = data.position;
                            var otherCollider = data.collider;

                            if (entity == otherEntity)
                                continue;

                            var otherPosition = otherTranslation.xy + otherCollider.offset;

                            var dt = otherPosition - position;
                            var sr = otherCollider.radius + collider.radius;

                            collided.value = dt.x * dt.x + dt.y * dt.y <= sr * sr;
                        }
                        while (!collided.value && quadrantMap.TryGetNextValue(out data, ref iterator));
                    }
                })
                .WithDisposeOnCompletion(quadrantMap)
                .ScheduleParallel();
        }

        protected override void OnDestroy()
        {
            _offsets.Dispose();

            base.OnDestroy();
        }
    }
}
