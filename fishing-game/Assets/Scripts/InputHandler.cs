using UnityEngine;
using UnityEngine.InputSystem; 
using System; 
using TMPro; 

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
    public static event Action OnPauseInput; 

    [SerializeField] private TextMeshProUGUI _errorTextObject; 
    [SerializeField] private float stickPullThreshold = 0.5f; 
    private bool rightStickPulled = false; 

    // update function to poll for the different inputs and invoke events
    private void Update()
    {   
        if (_errorTextObject.gameObject.activeSelf) {
            _errorTextObject.gameObject.SetActive(false); 
        }
        
        InputPoll(); 
    }

    // poll for inputs 
    private void InputPoll()
    {   

        // alwayd do escape for a pause input
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnPauseInput?.Invoke(); 
        }
        
        // pool for inputs depending on the peripheral 
        if (GameSettings.peripheralType == "Keyboard") {
            KeyboardInputs(); 
        } else if (GameSettings.peripheralType == "Controller") {
            ControllerInputs(); 
        }
    }

    private void KeyboardInputs()
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

    private void ControllerInputs()
    {
        if (Gamepad.current == null) {
            _errorTextObject.text = "Please connect a controller or press escape to switch to keyboard to continue!";
            _errorTextObject.gameObject.SetActive(true); 
            return; 
        } 

        // input pool 
        if (Gamepad.current.buttonWest.wasPressedThisFrame) {
            OnRegisteredInput?.Invoke(InputNames.left); 
        } else if (Gamepad.current.buttonEast.wasPressedThisFrame) {
            OnRegisteredInput?.Invoke(InputNames.right); 
        } else if (Gamepad.current.buttonSouth.wasPressedThisFrame) {
            OnRegisteredInput?.Invoke(InputNames.down); 
        } else if (Gamepad.current.buttonNorth.wasPressedThisFrame) {
            OnRegisteredInput?.Invoke(InputNames.up); 
        } else if (!rightStickPulled && Gamepad.current.rightStick.ReadValue().magnitude >= stickPullThreshold) {
            rightStickPulled = true; 
            OnCastInput?.Invoke("pressed"); 
        } else if (rightStickPulled && Gamepad.current.rightStick.ReadValue().magnitude < stickPullThreshold) {
            rightStickPulled = false; 
            OnCastInput?.Invoke("released"); 
        }
    }
}
