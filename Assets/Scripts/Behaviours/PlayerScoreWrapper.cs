using UnityEngine;
using UnityEngine.UI;

namespace Asteroids
{
    public class PlayerScoreWrapper : MonoBehaviour
    {
        public EntityInstance playerScoreInstance;
        public Text playerScoreText;

        private int _score;

        private void Start()
        {
            playerScoreText.text = _score.ToString();
        }

        private void Update()
        {
            if (playerScoreInstance.TryGetData(out PlayerScore data))
            {
                if (_score != data.value)
                {
                    _score = data.value;

                    playerScoreText.text = _score.ToString();
                }
            }
        }
    }
}
