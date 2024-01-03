using System;
using Unity.Entities;

namespace Asteroids
{
    [Serializable]
    public struct SceneLoadRequest : IComponentData
    {
        public Hash128 guid;
        public SceneLoadFlags flags;
        public int priority;
    }
}