using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class InputVisualGenerator : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private GameObject _inputLayout; 
    [SerializeField] private GameObject _lullRight;
    [SerializeField] private GameObject _lullLeft; 

    [Header("RNG Values")]
    [SerializeField] private float _maxLullTime; 
    [SerializeField] private float _minLullTime; 
    [Tooltip("The chance is 1 out of whatever number this is every half second")]
    [SerializeField] private int _rollChance; 
    [SerializeField] private int _inputCeiling = 18;
    [SerializeField] private int _minInputs;
    [SerializeField] private int _maxInputs; 

    [SerializeField] private List<PressableSO> _possibleInputs = new(); 

    private List<InputNames> _generatedInputs = new(); 
    private InputCollector _inputCollector; 
    private bool _decoySpawned = false; 
    private PressableSO _decoy; 
    private int _badPresses; 
    private int _goodPresses; 
    private GameObject _decoyObject; 

    private void Awake()
    {
        // null check possible inputs
        if (_possibleInputs == null)
        {
            Debug.LogWarning("[InputVisualGenerator] NEED TO PUT POSSIBLE INPUTS IN!"); 
        }

        // create and the input collector
        _inputCollector = new();
    }

    // subscribe/unsubscribe for both this object and the input collector
    private void OnEnable()
    {   
        StartLull(); 
        InputCollector.OnInputCollected += CheckInputs;
    }

    private void OnDisable()
    {
        InputCollector.OnInputCollected -= CheckInputs;
    }

    private void StartLull()
    {   
        _badPresses = 0; 
        InputHandler.OnRegisteredInput += CheckDecoyInput; 
        float lullTime = Random.Range(_minLullTime, _maxLullTime); 
        StartCoroutine(Lull(lullTime)); 
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
        } else if (leftOrRight == 1) {
            _decoyObject = Instantiate(_decoy.buttonPrefab, _lullRight.transform); 
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
            Destroy(_decoyObject);
            _decoy = null; 
            _decoySpawned = false; 
            _goodPresses++; 
        } else
        {
            _badPresses++; 
            Debug.Log($"Bad Press, you have {_badPresses} bad presses!"); 
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
                EndCatch("lose"); 
                return; 
            }
        }

        // if we mad it out of the loop the inputs are an exact match so far and if the sizes are the same the player wins
        if (collectedInputs.Count == _generatedInputs.Count) {
            EndCatch("win"); 
        }
    }

    private void GenerateInputs()
    {   
        // start feeding input into input collector instead of this 
        _inputCollector.Initialize(); 
        InputHandler.OnRegisteredInput -= CheckDecoyInput; 

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
        randomInputs = Mathf.Clamp(randomInputs, _minInputs, _inputCeiling); 

        // spawn that many inputs
        for (int i = 0; i < randomInputs; i++)
        {
            // get random input 
            int randomInputIndex = Random.Range(0, _possibleInputs.Count);
            Instantiate(_possibleInputs[randomInputIndex].buttonPrefab, _inputLayout.transform); 
            _generatedInputs.Add(_possibleInputs[randomInputIndex].inputId); 
        }

    }

    // when we end a catch we need to disable the visual generator and clean up our inputs then let the Game Manage deal with the transitions 
    private void EndCatch(string type)
    {   
        _inputCollector.Destroy(); 
        StartCoroutine(DestroyImages(2f, type)); 
    }

    private IEnumerator DestroyImages(float delay, string type)
    {
        yield return new WaitForSeconds(delay); 

        foreach (Transform child in _inputLayout.transform)
        {
            Destroy(child.gameObject); 
        }

        // handle the proper transition 
        if (type == "win") {
            GameManager.Instance.SucceedCatch(); 
        } else if (type == "lose") {
            GameManager.Instance.FailCatch(); 
        }
    }
}
