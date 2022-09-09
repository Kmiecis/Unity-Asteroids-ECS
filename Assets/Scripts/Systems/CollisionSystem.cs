using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Asteroids
{
    [UpdateAfter(typeof(MovementSystem))]
    public partial class CollisionSystem : SystemBase
    {
        private EntityQuery _collidersQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            _collidersQuery = GetCollidersQuery();
        }

        private EntityQuery GetCollidersQuery()
        {
            var desc = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<Translation>(),
                    ComponentType.ReadOnly<Collider>()
                }
            };
            return GetEntityQuery(desc);
        }

        protected override void OnUpdate()
        {
            var count = _collidersQuery.CalculateEntityCount();
            var translations = _collidersQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var colliders = _collidersQuery.ToComponentDataArray<Collider>(Allocator.TempJob);

            Entities
                .WithReadOnly(translations)
                .WithReadOnly(colliders)
                .ForEach((int entityInQueryIndex, Entity entity, ref Collided collided, in Translation translation, in Collider collider) =>
                {
                    for (int i = 0; !collided.value && i < count; ++i)
                    {
                        if (entityInQueryIndex == i)
                            continue;

                        var otherTranslation = translations[i];
                        var otherCollider = colliders[i];

                        var dt = otherTranslation.Value - translation.Value;
                        var sr = otherCollider.radius + collider.radius;

                        collided.value = dt.x * dt.x + dt.y * dt.y <= sr * sr;
                    }
                })
                .WithDisposeOnCompletion(translations)
                .WithDisposeOnCompletion(colliders)
                .ScheduleParallel();
        }
    }
}
