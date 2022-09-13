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
                ComponentType.ReadOnly<Translation>()
            );
        }

        private Bounds GetSpaceBounds()
        {
            var bounds = _spaceBoundsQuery.GetSingleton<Bounds>();
            var translation = _spaceBoundsQuery.GetSingleton<Translation>();
            return bounds.Translated(translation.Value.xy);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _spaceBoundsQuery = GetSpaceBoundsQuery();
        }

        protected override void OnUpdate()
        {
            var spaceBounds = GetSpaceBounds();

            Entities
                .ForEach((ref Translation translation) =>
                {
                    var position = translation.Value.xy;
                    var spaceBoundsSize = spaceBounds.Size;

                    while (position.x < spaceBounds.min.x)
                        position.x += spaceBoundsSize.x;
                    while (position.x > spaceBounds.max.x)
                        position.x -= spaceBoundsSize.x;

                    while (position.y < spaceBounds.min.y)
                        position.y += spaceBoundsSize.y;
                    while (position.y > spaceBounds.max.y)
                        position.y -= spaceBoundsSize.y;

                    translation.Value = new float3(position, 0.0f);
                })
                .ScheduleParallel();
        }
    }
}
