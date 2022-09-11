using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids
{
    public class SceneReloader : MonoBehaviour
    {
        public void Reload()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
