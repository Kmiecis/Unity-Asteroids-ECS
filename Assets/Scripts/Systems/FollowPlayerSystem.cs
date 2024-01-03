using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    [UpdateAfter(typeof(PlayerMovementSystem))]
    public partial class FollowPlayerSystem : SystemBase
    {
        private EntityQuery _playerQuery;

        private EntityQuery GetPlayerQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<Player>(),
                ComponentType.ReadOnly<LocalTransform>()
            );
        }

        private bool TryGetPlayerTransform(out LocalTransform transform)
        {
            return _playerQuery.TryGetSingletonSafely<LocalTransform>(out transform);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _playerQuery = GetPlayerQuery();
        }

        protected override void OnUpdate()
        {
            if (!TryGetPlayerTransform(out var playerTransform))
            {
                return;
            }

            Entities
                .ForEach((ref LocalTransform transform, in FollowPlayer follow) =>
                {
                    transform.Position = playerTransform.Position + new float3(follow.offset, 0.0f);
                })
                .ScheduleParallel();
        }
    }
}