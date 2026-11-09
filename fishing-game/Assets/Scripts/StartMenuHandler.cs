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
        Debug.Log("here"); 
        GetComponent<UIControl>().enabled = false; 
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
}
