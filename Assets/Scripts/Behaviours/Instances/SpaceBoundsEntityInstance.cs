using Unity.Entities;

namespace Asteroids
{
    public class SpaceBoundsEntityInstance : EntityInstance
    {
        public override ComponentType[] Archetype
        {
            get => new ComponentType[]
            {
                typeof(SpaceBounds)
            };
        }
    }
}