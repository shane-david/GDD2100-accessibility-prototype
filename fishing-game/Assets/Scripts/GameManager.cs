using UnityEngine;

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

    public void FailCatch()
    {
        _visualGenerator.enabled = false; 
        _restartButton.SetActive(true); 
        Debug.Log("You lost it"); 
    }

    public void SucceedCatch()
    {
        _visualGenerator.enabled = false; 
        _restartButton.SetActive(true); 
        Debug.Log("You got it"); 
    }

    public void StartCast()
    {
        _restartButton.SetActive(false);
        _castBar.SetActive(true); 
    }
    
    public void FinishCast()
    {
        _castBar.SetActive(false); 
        _visualGenerator.enabled = true; 
    }
}
