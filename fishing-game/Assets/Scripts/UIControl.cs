using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems; 
using UnityEngine.InputSystem; //OK 
using System.Collections.Generic; 

public class UIControl : MonoBehaviour
{
    // data for input restrictions 
    [SerializeField] private float _deadzone = 0.5f; 
    [SerializeField] private float _repeatDelay = 0.25f; 

    // data for buttons 
    private readonly List<Button> _buttons = new(); 
    private int _index; 

    // data for input 
    private bool _isStickHeld; 
    private float _nextMoveTime; 


    // Unity Lifetime

    // get all the buttons that are children of this object and set selected to the first one 
    void OnEnable()
    {
        // fill the buttons list 
        _buttons.Clear(); 
        GetComponentsInChildren(false, _buttons); 

        // remove the buttons that are not interactable
        _buttons.RemoveAll(b => !b.interactable); 

        // set the selected button to the first one 
        _index = 0; 
        if (_buttons.Count > 0) Select(); 

        _isStickHeld = false; 
    }

    // sense for and resolve controller input 
    void Update()
    {
        PollInput(); 
    }

    // private helpers

    private void Select()
    {
        EventSystem.current.SetSelectedGameObject(_buttons[_index].gameObject); 
    }

    private void PollInput()
    {
        // get the current game pad, do nothing if it does not exist
        Gamepad controller = Gamepad.current; 
        if (controller == null || _buttons.Count == 0) return; 

        // read the value of the left stick 
        float leftStickY = controller.leftStick.ReadValue().y; 

        // set stick held if it is not held
        if (Mathf.Abs(leftStickY) < _deadzone)
        {
            _isStickHeld = false; 
        } else if (!_isStickHeld || Time.unscaledTime >= _nextMoveTime)
        {
            // move to the previous or next button if the input is up or down
            _index = (_index + (leftStickY < 0 ? 1 : -1) + _buttons.Count) % _buttons.Count; 

            // select the current button 
            Select(); 

            // set stikc held and move time
            _isStickHeld = true; 
            _nextMoveTime = Time.unscaledTime + _repeatDelay; 
        }

        // if the south button was pressed, click the button 
        if (controller.buttonSouth.wasPressedThisFrame)
        {
            _buttons[_index].onClick.Invoke(); 
        }
    }
}
