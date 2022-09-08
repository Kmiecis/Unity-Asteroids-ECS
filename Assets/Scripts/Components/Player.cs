using Unity.Entities;

namespace Asteroids
{
    [GenerateAuthoringComponent]
    public struct Player : IComponentData
    {
        public float movementAcceleration;
        public float movementDeacceleration;
        public float movementSpeedLimit;

        public float turnAcceleration;
        public float turnDeacceleration;
        public float turnSpeedLimit;
    }
}
