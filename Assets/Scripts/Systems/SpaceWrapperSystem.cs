using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    [UpdateBefore(typeof(MovementSystem))]
    public partial class SpaceWrapperSystem : SystemBase
    {
        private EntityQuery _spaceBoundsQuery;

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

        protected override void OnCreate()
        {
            base.OnCreate();

            _spaceBoundsQuery = GetSpaceBoundsQuery();
        }

        protected override void OnUpdate()
        {
            if (!TryGetSpaceBounds(out var spaceBounds))
            {
                return;
            }

            Entities
                .ForEach((ref LocalTransform transform) =>
                {
                    var position = transform.Position.xy;
                    var spaceBoundsSize = spaceBounds.Size;

                    while (position.x < spaceBounds.min.x)
                        position.x += spaceBoundsSize.x;
                    while (position.x > spaceBounds.max.x)
                        position.x -= spaceBoundsSize.x;

                    while (position.y < spaceBounds.min.y)
                        position.y += spaceBoundsSize.y;
                    while (position.y > spaceBounds.max.y)
                        position.y -= spaceBoundsSize.y;

                    transform.Position = new float3(position, 0.0f);
                })
                .ScheduleParallel();
        }
    }
}
