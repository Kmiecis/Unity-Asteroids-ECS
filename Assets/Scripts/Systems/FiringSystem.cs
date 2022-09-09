using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class FiringSystem : SystemBase
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
            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((int entityInQueryIndex, ref FiringCooldown cooldown, in FiringInterval interval, in FiringSpawner spawner, in LocalToWorld transform) =>
                {
                    cooldown.value -= deltaTime;

                    if (cooldown.value <= 0.0f)
                    {
                        var projectile = commands.Instantiate(entityInQueryIndex, spawner.prefab);

                        var projectileTranslation = new Translation { Value = math.transform(transform.Value, spawner.offset) };
                        var projectileDirection = new MovementDirection { value = transform.Up.xy };

                        commands.SetComponent(entityInQueryIndex, projectile, projectileTranslation);
                        commands.SetComponent(entityInQueryIndex, projectile, projectileDirection);

                        cooldown.value = interval.value;
                    }
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
