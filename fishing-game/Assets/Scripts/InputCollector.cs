using UnityEngine; 
using System.Collections.Generic; 
using System; 

// input collector recieves event broadcasts from the InputHandler and collects
// all inputs into an array for each "skill check" in order to compare it against the 
// required inputs 
public class InputCollector
{
    
    // event for a collected input to tell the visual generator the check and display accordingly
    public static event Action<List<InputNames>> OnInputCollected; 

    // the array of inputs
    private List<InputNames> _inputCollection = new(); 

    // initialize/destroy to subscribe/unsubscribe
    public void Initialize()
    {
        InputHandler.OnRegisteredInput += CollectInput; 
    }

    public void Destroy()
    {
        InputHandler.OnRegisteredInput -= CollectInput; 
        _inputCollection.Clear(); 
    }

    private void CollectInput(InputNames input)
    {
        _inputCollection.Add(input); 
        OnInputCollected?.Invoke(_inputCollection); 
        Debug.Log(input); 
    }
}