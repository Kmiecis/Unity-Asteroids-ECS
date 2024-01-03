using Unity.Collections;
using Unity.Entities;

namespace Asteroids
{
    public static class EntityQueryExtensions
    {
        public static T GetSingletonSafely<T>(this EntityQuery self)
            where T : unmanaged, IComponentData
        {
            using (var array = self.ToComponentDataArray<T>(Allocator.Temp))
            {
                return array[0];
            }
        }

        public static bool TryGetSingletonSafely<T>(this EntityQuery self, out T result)
            where T : unmanaged, IComponentData
        {
            using (var array = self.ToComponentDataArray<T>(Allocator.Temp))
            {
                result = array.Length > 0 ? array[0] : default;
                return array.Length > 0;
            }
        }
    }
}