using UnityEngine;

namespace Asteroids
{
    public class Quitter : MonoBehaviour
    {
        public KeyCode quitKey = KeyCode.Escape;

        private void Update()
        {
            if (Input.GetKey(quitKey))
            {
                Application.Quit();
            }
        }
    }
}
