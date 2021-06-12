using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void Run(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
