using Unity.Entities;

namespace Asteroids
{
    public class PlayerScoreEntityInstance : EntityInstance
    {
        public override ComponentType[] Archetype
        {
            get => new ComponentType[]
            {
                typeof(PlayerScore)
            };
        }
    }
}