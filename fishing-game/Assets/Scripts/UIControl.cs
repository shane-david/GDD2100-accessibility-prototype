using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems; 
using UnityEngine.InputSystem; 
using System.Collections.Generic; 
using TMPro; 

public class UIControl : MonoBehaviour
{
    // data for input restrictions 
    [SerializeField] private float _deadzone = 0.5f; 
    [SerializeField] private float _repeatDelay = 0.25f; 

    // data for buttons 
    private readonly List<Selectable> _items = new(); 
    private int _index; 

    // data for input 
    private int _enabledFrame; 
    private bool _isStickHeld; 
    private float _nextMoveTime; 

    // dropdown list state
    private TMP_Dropdown _openDropdown;
    private readonly List<Toggle> listItems = new();
    private int _listIndex; 


    // Unity Lifetime

    // get all the buttons that are children of this object and set selected to the first one 
    void OnEnable()
    {
        // fill the buttons list 
        _items.Clear(); 
        GetComponentsInChildren(false, _items); 

        // remove the buttons that are not interactable
        _items.RemoveAll(s => !s.interactable); 

        // set the selected button to the first one 
        _index = 0; 
        if (_items.Count > 0) Select(); 

        _enabledFrame = Time.frameCount; 
        _isStickHeld = false; 
        _openDropdown = null; 
    }

    // sense for and resolve controller input 
    void Update()
    {
        PollInput(); 
    }

    // private helpers

    private void Select()
    {
        EventSystem.current.SetSelectedGameObject(_items[_index].gameObject); 
    }

    private void OpenList(TMP_Dropdown dd)
    {
        dd.Show();

        // the spawned list lives under the dropdown; the inactive template is skipped
        listItems.Clear();
        dd.GetComponentsInChildren(false, listItems);
        if (listItems.Count == 0) { dd.Hide(); return; }

        _openDropdown = dd;
        _listIndex = Mathf.Clamp(dd.value, 0, listItems.Count - 1);
        EventSystem.current.SetSelectedGameObject(listItems[_listIndex].gameObject);
        _isStickHeld = true; // don't let a stick already held move the list instantly
        _nextMoveTime = Time.unscaledTime + _repeatDelay; 
    }

    private void UpdateList(Gamepad pad)
    {
        float y = pad.leftStick.ReadValue().y;

        if (Mathf.Abs(y) < _deadzone)
        {
            _isStickHeld = false;
        }
        else if (!_isStickHeld || Time.unscaledTime >= _nextMoveTime)
        {
            _listIndex = Mathf.Clamp(_listIndex + (y < 0 ? 1 : -1), 0, listItems.Count - 1);
            EventSystem.current.SetSelectedGameObject(listItems[_listIndex].gameObject);
            _isStickHeld = true;
            _nextMoveTime = Time.unscaledTime + _repeatDelay;
        }

        if (pad.buttonSouth.wasPressedThisFrame)
        {
            _openDropdown.value = _listIndex;
            CloseList();
        }
        else if (pad.buttonEast.wasPressedThisFrame)
        {
            CloseList();
        }
    }

    private void CloseList()
    {
        if (_openDropdown.IsExpanded) _openDropdown.Hide();
        _openDropdown = null;
        _isStickHeld = false;
        Select(); 
    }

    private void PollInput()
    {
        // get the current game pad, do nothing if it does not exist
        Gamepad controller = Gamepad.current; 
        if (controller == null || _items.Count == 0) return; 

        // update the list instead of the menu if a dropdown is opne
        if (_openDropdown != null)
        {
            if (_openDropdown.IsExpanded)
            {
                UpdateList(controller); 
                return; 
            }
            CloseList(); 
        }

        // read the value of the left stick 
        float leftStickY = controller.leftStick.ReadValue().y; 

        // set stick held if it is not held
        if (Mathf.Abs(leftStickY) < _deadzone)
        {
            _isStickHeld = false; 
        } else if (!_isStickHeld || Time.unscaledTime >= _nextMoveTime)
        {
            // move to the previous or next button if the input is up or down
            _index = (_index + (leftStickY < 0 ? 1 : -1) + _items.Count) % _items.Count; 

            // select the current button 
            Select(); 

            // set stikc held and move time
            _isStickHeld = true; 
            _nextMoveTime = Time.unscaledTime + _repeatDelay; 
        }

        // if the south button was pressed, click the button 
        if (controller.buttonSouth.wasPressedThisFrame && Time.frameCount > _enabledFrame)
        {
            switch (_items[_index])
            {
                case Button b : b.onClick.Invoke(); break; 
                case TMP_Dropdown dd: OpenList(dd); break; 
            }
        }
    }
}
