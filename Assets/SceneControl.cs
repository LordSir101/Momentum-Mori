using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
   public static void Quit()
   {
        Debug.Log("quit");
        Application.Quit();
   }

//    public void QuitWrapper()
//    {
//         SceneControl.Quit();
//    }

   public static void LoadScene(string sceneName)
   {
        SceneManager.LoadScene(sceneName);
   }
}