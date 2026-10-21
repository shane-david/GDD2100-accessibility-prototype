using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using UnityEngine.SceneManagement; 

public class OptionsMenuHandler : IScaleable
{
    [Header("Dropdowns")]
    [SerializeField] private TMP_Dropdown _peripheralDropdown; 
    [SerializeField] private TMP_Dropdown _difficultyDropdown; 
    [SerializeField] private TMP_Dropdown _guiScaleDropdown; 

    [Header("Other UI References")]
    [SerializeField] private GameObject _menuPanel; 
    [SerializeField] private GameObject _tutorialButton; 
    [SerializeField] private RectTransform _totalTransform; 

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
        _difficultyDropdown.onValueChanged.AddListener(ChangeDifficulty);
        _guiScaleDropdown.onValueChanged.AddListener(ChangeGUIScale);
    }

    private void OnDisable()
    {
        _peripheralDropdown.onValueChanged.RemoveListener(ChangePeripheral); 
        _difficultyDropdown.onValueChanged.RemoveListener(ChangeDifficulty);
        _guiScaleDropdown.onValueChanged.RemoveListener(ChangeGUIScale);
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

    public void ChangeDifficulty(int index)
    {
        string option = _difficultyDropdown.options[index].text;
        Debug.Log(option);
        GameSettings.difficulty = option; 
        EventHandler.InvokeDifficultyChange(); 
    }

    public void ChangeGUIScale(int index)
    {
        string option = _guiScaleDropdown.options[index].text; 
        Debug.Log(option[0]); 
        GameSettings.guiScale = option[0]; 
        ChangeScale(option[0]); 
        EventHandler.InvokeScaleChange(option[0]); 
    }

    public void Show()
    {
        _menuPanel.SetActive(true); 
    }

    // activate the tutorial 
    public void TutorialSceneButton()
    {
        GameManager.Instance.ActivateTutorial(); 
        CloseOptions(); 
    }

    // interface methods
    public override void ChangeScale(char scale)
    {   
        // log what the scale is changing to 
        Debug.Log($"Changing scale to {scale}"); 

        // switch the scale of the total transform based on the values set in inspector 
        switch (scale)
        {
            case '1':
                _totalTransform.localScale = _scale1; 
                break;
            case '2':
                _totalTransform.localScale = _scale2; 
                break; 
            case '3':
                _totalTransform.localScale = _scale3;
                break; 
            default:
                break; 
        }
    }
}
