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
                ComponentType.ReadOnly<Translation>()
            );
        }

        private Bounds GetViewBounds()
        {
            var bounds = _viewBoundsQuery.GetSingleton<Bounds>();
            var translation = _viewBoundsQuery.GetSingleton<Translation>();
            return bounds.Translated(translation.Value.xy);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _viewBoundsQuery = GetViewBoundsQuery();
        }

        protected override void OnUpdate()
        {
            var viewBounds = GetViewBounds();

            Entities
                .ForEach((ref Visibility visibility, in Translation translation) =>
                {
                    var position = translation.Value.xy;

                    visibility.value = math.all(viewBounds.min <= position) && math.all(position <= viewBounds.max);
                })
                .ScheduleParallel();
        }
    }
}
