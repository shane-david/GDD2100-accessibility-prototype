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
        OptionsMenuHandler.Instance.Show(); 
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
}
