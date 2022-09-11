using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Asteroids
{
    public class Countdown : MonoBehaviour
    {
        public float timer;
        public Text text;
        public UnityEvent onCountdownFinish;

        private int _value;

        private void Update()
        {
            timer -= Time.deltaTime;

            var value = Mathf.CeilToInt(timer);
            if (_value != value)
            {
                _value = value;

                text.text = _value.ToString();

                if (_value == 0)
                {
                    onCountdownFinish?.Invoke();
                }
            }
        }
    }
}
