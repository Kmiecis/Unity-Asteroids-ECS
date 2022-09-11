using UnityEngine;
using UnityEngine.Events;

namespace Asteroids
{
    public class GameOver : MonoBehaviour
    {
        public EntityInstance entityInstance;
        public UnityEvent onGameOver;

        private bool _startedExisting = false;
        private bool _stoppedExisting = false;

        private void Update()
        {
            if (!_startedExisting)
            {
                _startedExisting = entityInstance.TryGetEntity(out var _);
            }

            if (_startedExisting && !_stoppedExisting)
            {
                _stoppedExisting = !entityInstance.TryGetEntity(out var _);
            }

            if (_startedExisting && _stoppedExisting)
            {
                onGameOver?.Invoke();
                enabled = false;
            }
        }
    }
}
