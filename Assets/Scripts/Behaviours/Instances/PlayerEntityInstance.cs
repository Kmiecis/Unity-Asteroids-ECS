using Unity.Entities;

namespace Asteroids
{
    public class PlayerEntityInstance : EntityInstance
    {
        public override ComponentType[] Archetype
        {
            get => new ComponentType[]
            {
                typeof(Player)
            };
        }
    }
}