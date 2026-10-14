using UnityEngine;
using System; 
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
    [SerializeField] private GameObject _tutorialPanel; 

    public bool _tutorialActive = false; 



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

    private void Start()
    {
        if (_tutorialActive) {
            _tutorialPanel.SetActive(true);
            _tutorialPanel.GetComponent<TutorialHandler>().enabled = true; 
            EventHandler.InvokeTutorialEvent(0); 
        }
    }

    private void OnEnable() => InputHandler.OnPauseInput += Pause; 
    private void OnDisable() => InputHandler.OnPauseInput -= Pause; 

    public void ActivateTutorial()
    {
        _tutorialActive = true; 
        _tutorialPanel.SetActive(true);
        _tutorialPanel.GetComponent<TutorialHandler>().enabled = true; 
        EventHandler.InvokeTutorialEvent(0); 
    }
    
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
        if (_tutorialActive) EventHandler.InvokeTutorialEvent(3);
    }

    public void SucceedCatch()
    {
        _visualGenerator.enabled = false; 
        _restartButton.SetActive(true); 
        _rodImage.SetActive(false); 
        _stageText.text = "You caught it!"; 
        if (_tutorialActive) EventHandler.InvokeTutorialEvent(3);
    }

    public void StartCast()
    {
        _restartButton.SetActive(false);  
        _castBar.SetActive(true); 
        _stageText.enabled = false; 
        if (_tutorialActive) EventHandler.InvokeTutorialEvent(4); 
    }
    
    public void FinishCast()
    {
        _castBar.SetActive(false); 
        _visualGenerator.enabled = true; 
        _rodImage.SetActive(true); 
        if (_tutorialActive) EventHandler.InvokeTutorialEvent(1); 
    }

    public void StarBite()
    {
        _stageText.enabled = true; 
        _stageText.text = "You got a bite!"; 
    }
}
