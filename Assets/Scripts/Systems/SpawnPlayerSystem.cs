using Unity.Entities;

namespace Asteroids
{
    public partial class SpawnPlayerSystem : SystemBase
    {
        private EntityCommandBufferSystem _commands;
        private EntityQuery _playerDataQuery;

        private EntityQuery GetPlayerDataQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<PlayerData>()
            );
        }

        private bool TryGetPlayerData(out PlayerData data)
        {
            return _playerDataQuery.TryGetSingleton<PlayerData>(out data);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
            _playerDataQuery = GetPlayerDataQuery();
        }

        protected override void OnUpdate()
        {
            if (!TryGetPlayerData(out var playerData))
            {
                return;
            }

            var commands = _commands.CreateCommandBuffer().AsParallelWriter();

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, in SpawnPlayerRequest request) =>
                {
                    var player = commands.Instantiate(entityInQueryIndex, playerData.prefab);

                    commands.DestroyEntity(entityInQueryIndex, entity);
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}