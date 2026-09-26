using UnityEngine;
using UnityEngine.InputSystem; 
using System; 

public enum InputNames
{
    left,
    right,
    down,
    up
}

public class InputHandler : MonoBehaviour
{

    // public static events representing the different inputs 
    public static event Action<InputNames> OnRegisteredInput; 
    public static event Action<string> OnCastInput; 

    // update function to poll for the different inputs and invoke events
    private void Update()
    {
        InputPoll(); 
    }

    // poll for inputs 
    private void InputPoll()
    {
        
        // if there is not peripheral ignore keyboard inputs 
        if (Keyboard.current == null) return; 

        // input pool 
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) {
            OnRegisteredInput?.Invoke(InputNames.left); 
        } else if (Keyboard.current.rightArrowKey.wasPressedThisFrame) {
            OnRegisteredInput?.Invoke(InputNames.right); 
        } else if (Keyboard.current.downArrowKey.wasPressedThisFrame) {
            OnRegisteredInput?.Invoke(InputNames.down); 
        } else if (Keyboard.current.upArrowKey.wasPressedThisFrame) {
            OnRegisteredInput?.Invoke(InputNames.up); 
        } else if (Mouse.current.rightButton.wasPressedThisFrame) {
            OnCastInput?.Invoke("pressed"); 
        } else if (Mouse.current.rightButton.wasReleasedThisFrame) {
            OnCastInput?.Invoke("released"); 
        }
    }
}
