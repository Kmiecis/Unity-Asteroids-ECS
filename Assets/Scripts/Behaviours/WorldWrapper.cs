using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids
{
    public class WorldWrapper : MonoBehaviour
    {
        private World _world;
        private EntityManager _entityManager;

        private float _maximumDeltaTime;

        private void Awake()
        {
            _world = World.DefaultGameObjectInjectionWorld;
            _entityManager = _world.EntityManager;

            _maximumDeltaTime = _world.MaximumDeltaTime;
        }

        public void Pause()
        {
            _world.MaximumDeltaTime = 0.0f;
        }

        public void Unpause()
        {
            _world.MaximumDeltaTime = _maximumDeltaTime;
        }

        public void Teardown()
        {
            Unpause();

            _entityManager.DestroyEntity(_entityManager.UniversalQuery);
        }

        public void Reload()
        {
            Teardown();

            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
