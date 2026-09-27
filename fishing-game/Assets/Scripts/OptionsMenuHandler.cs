using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class OptionsMenuHandler : MonoBehaviour
{
    
    [SerializeField] TMP_Dropdown _peripheralDropdown; 
    [SerializeField] GameObject _menuPanel; 

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
}
