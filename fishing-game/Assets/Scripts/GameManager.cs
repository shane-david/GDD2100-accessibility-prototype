using UnityEngine;
using TMPro; 

// game manager is a singleton that will handele all of the transitions
// -when the player wins and can play again
// -when the player loses and can try again 
// -when the player casts and has to bite 
public class GameManager : MonoBehaviour
{

    // necessary references
    [SerializeField] private InputVisualGenerator _visualGenerator; 
    [SerializeField] private GameObject _restartButton; 
    [SerializeField] private GameObject _castBar; 
    [SerializeField] private GameObject _rodImage; 
    [SerializeField] private TextMeshProUGUI _stageText; 

    // make singleton
    public static GameManager Instance { get; private set; }

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

    private void OnEnable() => InputHandler.OnPauseInput += Pause; 
    private void OnDisable() => InputHandler.OnPauseInput -= Pause; 

    private void Pause()
    {
        OptionsMenuHandler.Instance.Show(); 
    }

    public void FailCatch()
    {
        _visualGenerator.enabled = false; 
        _restartButton.SetActive(true); 
        _rodImage.SetActive(false); 
        _stageText.text = "You lost it!"; 
    }

    public void SucceedCatch()
    {
        _visualGenerator.enabled = false; 
        _restartButton.SetActive(true); 
        _rodImage.SetActive(false); 
        _stageText.text = "You caught it!";  
    }

    public void StartCast()
    {
        _restartButton.SetActive(false);  
        _castBar.SetActive(true); 
        _stageText.enabled = false; 
    }
    
    public void FinishCast()
    {
        _castBar.SetActive(false); 
        _visualGenerator.enabled = true; 
        _rodImage.SetActive(true); 
    }

    public void StarBite()
    {
        _stageText.enabled = true; 
        _stageText.text = "You got a bite!"; 
    }
}
