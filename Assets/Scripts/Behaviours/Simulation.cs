using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class Simulation : MonoBehaviour
    {
        [Min(1)]
        public uint seed = 1;

        private bool _simulated;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_simulated)
            {
                var world = World.DefaultGameObjectInjectionWorld;
                var entityManager = world.EntityManager;
                var entity = entityManager.CreateEntity();
                entityManager.AddComponentData(entity, new SimulateRequest { seed = seed });

                _simulated = true;
            }
        }
    }
}
