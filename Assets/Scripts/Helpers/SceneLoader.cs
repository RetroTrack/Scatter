using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scatter.Helpers
{
    public class SceneLoader : MonoBehaviour
    {
        public static void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public static void Quit()
        {
            Application.Quit();
        }

    }
}