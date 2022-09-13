using System;
using Unity.Entities;

namespace Asteroids
{
    [Serializable]
    public struct RequestDelay : IComponentData
    {
        public float value;
    }
}
