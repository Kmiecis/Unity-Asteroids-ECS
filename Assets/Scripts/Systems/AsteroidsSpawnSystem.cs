using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class AsteroidsSpawnSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _commands;

        protected override void OnCreate()
        {
            base.OnCreate();
            _commands = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, in SpawnAsteroidsRequest request) =>
                {
                    var xOffset = -request.width * 0.5f + 0.5f;
                    var yOffset = -request.height * 0.5f + 0.5f;

                    for (int y = 0; y < request.height; ++y)
                    {
                        for (int x = 0; x < request.width; ++x)
                        {
                            var asteroid = commands.Instantiate(entityInQueryIndex, request.prefab);

                            var asteroidTranslation = new Translation { Value = new float3(x + xOffset, y + yOffset, 0.0f) };
                            commands.SetComponent(entityInQueryIndex, asteroid, asteroidTranslation);
                        }
                    }

                    commands.DestroyEntity(entityInQueryIndex, entity);
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
