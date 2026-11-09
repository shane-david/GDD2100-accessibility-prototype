using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem; 

/// <summary>
/// Broadcast events for gamepad connection/disconnection 
/// </summary>
public class GamepadConnectionEvents : MonoBehaviour
{
    public UnityEvent onGamepadConnected;
    public UnityEvent onGamepadDisconnected;

    void OnEnable()
    {
        // subscribe on device change to the input system event
        InputSystem.onDeviceChange += OnDeviceChange; 

        // invoke connected if the gamepad was plugged in on start up
        if (Gamepad.current != null)
        {
            onGamepadConnected?.Invoke(); 
        }
    }

    void OnDisable() => InputSystem.onDeviceChange -= OnDeviceChange; 

    // for gamepad device changes invoke the corresponding unity event 
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // only care about gamepad devices
        if (device is not Gamepad) return; 

        // switch-case the change
        switch (change)
        {
            
            // invoke connected for added or reconnected events
            case InputDeviceChange.Added:
            case InputDeviceChange.Reconnected:
                onGamepadConnected?.Invoke();
                break; 

            // invoke disconnected for disconnected or removed events
            case InputDeviceChange.Disconnected:
            case InputDeviceChange.Removed:
                onGamepadDisconnected?.Invoke();
                break; 
        }
    }
}
