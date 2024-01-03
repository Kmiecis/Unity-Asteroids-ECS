using Unity.Entities;

namespace Asteroids
{
    public class ViewBoundsEntityInstance : EntityInstance
    {
        public override ComponentType[] Archetype
        {
            get => new ComponentType[]
            {
                typeof(ViewBounds)
            };
        }
    }
}