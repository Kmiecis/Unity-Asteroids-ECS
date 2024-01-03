using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class VisibilitySystem : SystemBase
    {
        private EntityQuery _viewBoundsQuery;

        private EntityQuery GetViewBoundsQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<ViewBounds>(),
                ComponentType.ReadOnly<Bounds>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }

        private bool TryGetViewBounds(out Bounds bounds)
        {
            if (_viewBoundsQuery.TryGetSingleton<Bounds>(out bounds) &&
                _viewBoundsQuery.TryGetSingletonSafely<LocalTransform>(out var transform))
            {
                bounds = bounds.Translated(transform.Position.xy);
                return true;
            }
            return false;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _viewBoundsQuery = GetViewBoundsQuery();
        }

        protected override void OnUpdate()
        {
            if (!TryGetViewBounds(out var viewBounds))
            {
                return;
            }

            Entities
                .ForEach((ref Visibility visibility, in LocalTransform transform) =>
                {
                    var position = transform.Position.xy;

                    visibility.value = (
                        math.all(viewBounds.min <= position) &&
                        math.all(position <= viewBounds.max)
                    );
                })
                .ScheduleParallel();
        }
    }
}
