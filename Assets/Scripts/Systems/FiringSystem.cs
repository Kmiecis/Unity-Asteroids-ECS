using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids
{
    public partial class FiringSystem : SystemBase
    {
        private EntityCommandBufferSystem _commands;

        protected override void OnCreate()
        {
            base.OnCreate();

            _commands = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commands = _commands.CreateCommandBuffer().AsParallelWriter();
            var deltaTime = World.Time.DeltaTime;

            Entities
                .ForEach((int entityInQueryIndex, ref FiringCooldown cooldown, in FiringInterval interval, in FiringSpawner spawner, in LocalToWorld toWorld) =>
                {
                    cooldown.value -= deltaTime;

                    if (cooldown.value <= 0.0f)
                    {
                        var projectile = commands.Instantiate(entityInQueryIndex, spawner.prefab);

                        var projectileTransform = new LocalTransform
                        {
                            Position = math.transform(toWorld.Value, new float3(spawner.offset, 0.0f)),
                            Rotation = quaternion.identity,
                            Scale = spawner.radius * 2.0f
                        };
                        var projectileDirection = new MovementDirection { value = toWorld.Up.xy };

                        commands.SetComponent(entityInQueryIndex, projectile, projectileTransform);
                        commands.SetComponent(entityInQueryIndex, projectile, projectileDirection);

                        cooldown.value = interval.value;
                    }
                })
                .ScheduleParallel();

            _commands.AddJobHandleForProducer(this.Dependency);
        }
    }
}
