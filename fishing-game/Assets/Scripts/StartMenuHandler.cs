using UnityEngine;
using UnityEngine.SceneManagement; 

public class StartMenuHandler : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("game");
    }

    public void OpenOptions()
    {
        Debug.Log("opened optoins");
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
}
