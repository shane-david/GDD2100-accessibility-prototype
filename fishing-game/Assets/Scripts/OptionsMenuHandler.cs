using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using UnityEngine.SceneManagement; 

public class OptionsMenuHandler : MonoBehaviour
{
    
    [SerializeField] TMP_Dropdown _peripheralDropdown; 
    [SerializeField] GameObject _menuPanel; 
    [SerializeField] GameObject _tutorialButton; 

    // make singleton
    public static OptionsMenuHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return; 
        }

        Instance = this; 
        DontDestroyOnLoad(gameObject); 

        SceneManager.sceneLoaded += OnFishingSceneLoaded; 
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnFishingSceneLoaded; 
        }
    }

    // when the fishing scene is loaded, show the tutorial button 
    private void OnFishingSceneLoaded(Scene scene, LoadSceneMode mode)  
    {
        if (scene.name == "game")
        {
            _tutorialButton.SetActive(true); 
        }
    }

    // subscribe and unsubscribe from the dropdown changed evernts
    private void OnEnable()
    {
        _peripheralDropdown.onValueChanged.AddListener(ChangePeripheral); 
    }

    private void OnDisable()
    {
        _peripheralDropdown.onValueChanged.RemoveListener(ChangePeripheral); 
    }

    // destroy option
    public void CloseOptions()
    {
        _menuPanel.SetActive(false); 
    }

    public void ChangePeripheral(int index)
    {
        string option = _peripheralDropdown.options[index].text; 
        Debug.Log(option);
        GameSettings.peripheralType = option; 
    }

    public void Show()
    {
        _menuPanel.SetActive(true); 
    }

    public void TutorialSceneButton()
    {
        GameManager.Instance.ActivateTutorial(); 
        CloseOptions(); 
    }
}
