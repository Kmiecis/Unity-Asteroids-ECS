using UnityEngine;
using UnityEngine.UI;

namespace Asteroids
{
    public class FramesPerSecond : MonoBehaviour
    {
        [SerializeField]
        protected Text text;

        private int _frames = 0;
        private float _sum = 0.0f;

        private void Start()
        {
            text.text = 0.ToString();
            text.color = Color.red;
        }

        private void Update()
        {
            var deltaTime = Time.smoothDeltaTime;

            _frames++;
            _sum += deltaTime;

            if (_sum > 0.5f)
            {
                var fps = _frames / _sum;

                text.text = fps.ToString("F2");
                if (fps > 30.0f)
                {
                    text.color = Color.green;
                }
                else if (fps > 15.0f)
                {
                    text.color = Color.yellow;
                }
                else
                {
                    text.color = Color.red;
                }

                _frames = 0;
                _sum = 0.0f;
            }
        }
    }
}
