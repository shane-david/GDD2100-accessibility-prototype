using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine.UI; 
using TMPro;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class InputVisualGenerator : IScaleable
{

    [Header("UI References")]
    [SerializeField] private GameObject _inputLayout; 
    [SerializeField] private GameObject _lullRight;
    [SerializeField] private GameObject _lullLeft; 
    [SerializeField] private GameObject _badInput; 
    [SerializeField] private GameObject _goodInput; 
    private HorizontalLayoutGroup _layoutGroup; 

    [Header("RNG Values")]
    [SerializeField] private float _maxLullTime; 
    [SerializeField] private float _minLullTime; 
    [Tooltip("The chance is 1 out of whatever number this is every half second")]
    [SerializeField] private int _rollChance; 
    [SerializeField] private int _inputCeiling = 18;
    [SerializeField] private int _minInputs;
    [SerializeField] private int _maxInputs; 

    [Header("Timer Values")]
    [SerializeField] private float _easyTime;
    [SerializeField] private float _mediumTime;
    [SerializeField] private float _hardTime;
    private float _timePerInput; 
    [SerializeField] private GameObject _timerObject; 
    private Coroutine _catchCountDown; 

    [Header("Input Values")]
    [SerializeField] private List<PressableSO> _possibleInputs = new(); 

    private List<InputNames> _generatedInputs = new(); 
    private InputCollector _inputCollector; 
    private bool _decoySpawned = false; 
    private PressableSO _decoy; 
    private int _badPresses; 
    private int _goodPresses; 
    private GameObject _decoyObject; 
    private bool _tutorialLull = false; 
    private Vector2 _buttonScale = new Vector2(1f, 1f); 

    private void Awake()
    {
        // null check possible inputs
        if (_possibleInputs == null)
        {
            Debug.LogWarning("[InputVisualGenerator] NEED TO PUT POSSIBLE INPUTS IN!"); 
        }

        // create and the input collector
        _inputCollector = new();

        // cache the layout group
        _layoutGroup = GetComponent<HorizontalLayoutGroup>(); 

        // set difficulty 
        SetDifficulty(); 
        EventHandler.OnDifficultyChange += SetDifficulty; 
    }

    // subscribe/unsubscribe for both this object and the input collector
    private void OnEnable()
    {   
        // only start the lull on enable if the tutorial is not active
        if (!GameManager.Instance._tutorialActive)
        {
            StartLull(); 
        } else {
            StartTutorialLull(); 
        }

        // make sure to change the scale on enable if it is set while this script is not enabled
        ChangeScale(GameSettings.guiScale); 
        
        InputCollector.OnInputCollected += CheckInputs;
        EventHandler.OnScaleChange += ChangeScale; 
    }

    private void OnDisable()
    {
        InputCollector.OnInputCollected -= CheckInputs;
        InputHandler.OnRegisteredInput -= CheckDecoyInput; 
        EventHandler.OnScaleChange -= ChangeScale; 
    }

    private void OnDestroy()
    {
        EventHandler.OnDifficultyChange -= SetDifficulty; 
    }

    // set the timer based on the games difficulty 
    private void SetDifficulty()
    {
        Debug.Log("chaning size"); 
        switch (GameSettings.difficulty)
        {
            case "Easy":
                _timePerInput = _easyTime;
                break;
            case "Medium":
                _timePerInput = _mediumTime;
                break;
            case "Hard":
                _timePerInput = _hardTime;
                break; 
        }

        Debug.Log(_timePerInput); 
    }

    public override void ChangeScale(char scale)
    {
        // switch the scale of the total transform based on the values set in inspector 
        switch (scale)
        {
            case '1':
                _buttonScale = _scale1; 
                _layoutGroup.spacing = -40; 
                break;
            case '2':
                _buttonScale = _scale2; 
                _layoutGroup.spacing = 30; 
                break; 
            case '3':
                _buttonScale = _scale3;
                _layoutGroup.spacing = 68; 
                break; 
            default:
                break; 
        }
    }

    private void StartLull()
    {   
        Debug.Log("lull started"); 
        _badPresses = 0; 
        _goodPresses = 0; 
        InputHandler.OnRegisteredInput += CheckDecoyInput; 
        float lullTime = Random.Range(_minLullTime, _maxLullTime); 
        StartCoroutine(Lull(lullTime)); 
    }

    private void StartTutorialLull()
    {   
        _tutorialLull = true; 

        // pick a random input 
        int randomInputRNG = Random.Range(0, _possibleInputs.Count); 
        _decoy = _possibleInputs[randomInputRNG]; 
        _decoyObject = Instantiate(_decoy.buttonPrefab, _lullLeft.transform);
        _decoyObject.transform.localScale = _buttonScale; 
        _decoySpawned = true; 

        // subscribe to input checking 
        InputHandler.OnRegisteredInput += CheckDecoyInput; 
    }

    private void EndTutorialLull()
    {   
        _tutorialLull = false; 

        // unsubscribe so that StartLull can execute normally
        InputHandler.OnRegisteredInput -= CheckDecoyInput; 

        // broadcast a wait event to the Tutorialhandler
        EventHandler.InvokeTutorialEvent(-1); 

        // start the normal lull 
        StartLull(); 
        
    }

    private IEnumerator Lull(float duration)
    {   

        // keep track of the time until it gets to the duration
        float elapsed = 0f; 
        while (elapsed < duration)
        {   
            // wait half a second between rolls
            yield return new WaitForSeconds(0.5f); 

            // increase the elapsed itme
            elapsed += 0.5f; 

            // roll a number and if the decoy is not spawned and its 0 spawn a decoy
            int roll = Random.Range(0, _rollChance); 
            if (roll == 0 && !_decoySpawned)
            {
                SpawnRandomDecoy(); 
            }
        }

        yield return new WaitForSeconds(1f); 

        // spawn the inputs after lulling is over 
        GenerateInputs(); 
    }


    private void SpawnRandomDecoy()
    {   
        // pick a random input 
        int randomInputRNG = Random.Range(0, _possibleInputs.Count); 
        _decoy = _possibleInputs[randomInputRNG]; 

        // pick left or right
        int leftOrRight = Random.Range(0,2);
        if (leftOrRight == 0) {
            _decoyObject = Instantiate(_decoy.buttonPrefab, _lullLeft.transform); 
            _decoyObject.transform.localScale = _buttonScale; 
        } else if (leftOrRight == 1) {
            _decoyObject = Instantiate(_decoy.buttonPrefab, _lullRight.transform); 
            _decoyObject.transform.localScale = _buttonScale; 
        }

        // set the var
        _decoySpawned = true; 
    }

    private void CheckDecoyInput(InputNames input)
    {   
        // if there is no decoy no need to check decoy input 
        if (!_decoySpawned) return; 

        // if the decoy input is equal to the input destroy the decoy, if it is not add to the bad press counter 
        if (_decoy.inputId == input)
        {   
            _decoyObject.GetComponent<Image>().color = Color.green; 
            Instantiate(_goodInput, _decoyObject.transform);
            Destroy(_decoyObject, 0.2f);
            _decoy = null; 
            _decoySpawned = false; 
            _goodPresses++; 
        } else { 
            _decoyObject.GetComponent<Image>().color = Color.red;
            Instantiate(_badInput, _decoyObject.transform);
            Destroy(_decoyObject, 0.2f); 
            _decoy = null; 
            _decoySpawned = false; 
            _badPresses++; 
        }

        // if this is the tutorial decoy input go to the next phase 
        if (_tutorialLull)
        {
            EndTutorialLull(); 
        }
    }

    private void CheckInputs(List<InputNames> collectedInputs)
    {
        // see if the collected and generated inputs are euqal
        for(int i = 0; i < collectedInputs.Count; i++)
        {
             
            // if we ever get bigger than the generated inputs break otu 
            if (i >= _generatedInputs.Count) break; 

            // compare the inputs and return if they are not the same 
            if (collectedInputs[i] != _generatedInputs[i]) {
                _inputLayout.transform.GetChild(i).GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f); 
                Instantiate(_badInput, _inputLayout.transform.GetChild(i).transform);
                EndCatch("lose"); 
                return; 
            
            // if they are the same make that instance green 
            } else {
                _inputLayout.transform.GetChild(i).GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f); 
                Instantiate(_goodInput, _inputLayout.transform.GetChild(i).transform);
            }
        }

        // if we made it out of the loop the inputs are an exact match so far and if the sizes are the same the player wins
        if (collectedInputs.Count == _generatedInputs.Count) {
            EndCatch("win"); 
        }
    }

    // if we are checking final the timer ran out before the player pressed all inputs so make them red and end the catch
    private void CheckInputsFinal()
    {
        for (int i = 0; i < _generatedInputs.Count; i++)
        {
            _inputLayout.transform.GetChild(i).GetComponent<Image>().color = Color.red; 
        }

        EndCatch("lose"); 
    }

    private void GenerateInputs()
    {   
        // broadcast the tutorial event if the tutorial is active
        if (GameManager.Instance._tutorialActive)
        {
            EventHandler.InvokeTutorialEvent(2); 
        }
        
        // start feeding input into input collector instead of this 
        _inputCollector.Initialize(); 
        InputHandler.OnRegisteredInput -= CheckDecoyInput; 

        // spawn the bite text
        GameManager.Instance.StarBite(); 

        // destroy any decoys
        if (_decoyObject != null)
        {
            Destroy(_decoyObject); 
            _decoy = null;
            _decoySpawned = false;  
        }

        // reset the list
        _generatedInputs.Clear(); 

        // get random amount of inputs and add bad pressses to it and clamp it at the celing
        int randomInputs = Random.Range(_minInputs, _maxInputs+1); 
        randomInputs += _badPresses; 
        randomInputs -= _goodPresses; 
        Debug.Log(randomInputs); 
        randomInputs = Mathf.Clamp(randomInputs, _minInputs, _inputCeiling); 

        // spawn that many inputs
        for (int i = 0; i < randomInputs; i++)
        {
            // get random input 
            int randomInputIndex = Random.Range(0, _possibleInputs.Count);
            GameObject inputObject = Instantiate(_possibleInputs[randomInputIndex].buttonPrefab, _inputLayout.transform); 
            inputObject.transform.localScale = _buttonScale; 
            _generatedInputs.Add(_possibleInputs[randomInputIndex].inputId); 
        }

        // start timer coroutine if it is not the tutorial
        if (!GameManager.Instance._tutorialActive) {
            _catchCountDown = StartCoroutine(CatchCountDown(randomInputs));
        }  
    }

    // timer coroutine, count down from input time (based on difficulty) * # of inputs and fail when at end 
    private IEnumerator CatchCountDown(int numInputs)
    {   
        // calculate the total time remaining 
        float timeRemaining = numInputs * _timePerInput; 

        // active the text object and get text component 
        _timerObject.SetActive(true); 
        TextMeshProUGUI timerText = _timerObject.GetComponentInChildren<TextMeshProUGUI>(); 

        // count down 
        while (timeRemaining > 0)
        {
            // format the text 
            timerText.text = timeRemaining.ToString("F1"); 

            // substract the time passed since last frame
            timeRemaining -= Time.deltaTime; 

            // wait until the next frame until looping agon
            yield return null; 
        }

        // once the timer is over disable the object and check inputs 
        _timerObject.SetActive(false); 
        CheckInputsFinal(); 
    }

    // when we end a catch we need to disable the visual generator and clean up our inputs then let the Game Manage deal with the transitions 
    private void EndCatch(string type)
    {   
        // if the coroutine is still happening terminate it 
        if (_catchCountDown != null)
        {
            StopCoroutine(_catchCountDown);
            _catchCountDown = null; 
            _timerObject.SetActive(false); 
        }

        _inputCollector.Destroy(); 
        StartCoroutine(DestroyImages(2f, type)); 
    }

    private IEnumerator DestroyImages(float delay, string type)
    {

        // turn every image red to indicate failure if there was failure
        if (type == "lose") {
            foreach (Transform child in _inputLayout.transform)
            {
                Image image = child.GetComponent<Image>(); 
                if (image != null)
                {
                    image.color = Color.red; 
                    Instantiate(_badInput, child); 
                }
            }
        }

        yield return new WaitForSeconds(delay); 

        foreach (Transform child in _inputLayout.transform)
        {
            Destroy(child.gameObject); 
        }

        Debug.Log(type); 

        // handle the proper transition 
        if (type == "win") {
            GameManager.Instance.SucceedCatch(); 
        } else if (type == "lose") {
            GameManager.Instance.FailCatch(); 
        }
    }
}
