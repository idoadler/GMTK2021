using UnityEngine;

public class ExitApp : MonoBehaviour
{
#if !UNITY_WEBGL
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else            
            Application.Quit();
#endif
        }
    }
#endif 
}
