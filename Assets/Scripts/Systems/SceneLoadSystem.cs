using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;

namespace Asteroids
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SceneLoadSystem : SystemBase
    {
        private EntityQuery _sceneLoadRequestQuery;

        private EntityQuery GetSceneLoadRequestQuery()
        {
            return GetEntityQuery(
                ComponentType.ReadOnly<SceneLoadRequest>()
            );
        }

        private NativeArray<SceneLoadRequest> GetSceneLoadRequests()
        {
            return _sceneLoadRequestQuery.ToComponentDataArray<SceneLoadRequest>(Allocator.Temp);
        }

        private void ClearSceneLoadRequests()
        {
            EntityManager.DestroyEntity(_sceneLoadRequestQuery);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _sceneLoadRequestQuery = GetSceneLoadRequestQuery();
        }

        protected override void OnUpdate()
        {
            var requests = GetSceneLoadRequests();

            for (int i = 0; i < requests.Length; ++i)
            {
                var request = requests[i];

                var parameters = new SceneSystem.LoadParameters
                {
                    Flags = request.flags,
                    Priority = request.priority
                };
                SceneSystem.LoadSceneAsync(World.Unmanaged, request.guid, parameters);
            }

            requests.Dispose();

            ClearSceneLoadRequests();
        }
    }
}