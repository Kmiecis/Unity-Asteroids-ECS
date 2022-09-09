using Unity.Entities;

namespace Asteroids
{
    [UpdateAfter(typeof(CollisionSystem))]
    public partial class BulletsCollisionsSystem : SystemBase
    {
        // Increase score on collision before being destroyed
        protected override void OnUpdate()
        {
        }
    }
}
