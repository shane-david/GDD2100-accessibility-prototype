using UnityEngine;
using TMPro; 
using System.Collections.Generic; 

public class TutorialHandler : MonoBehaviour
{
   // necessary references
   [SerializeField] private TextMeshProUGUI _tutorialText;
   [SerializeField] private List<GameObject> _tutorialArrows;  
   [SerializeField] private List<string> _tutorialMessages = new();     
   [SerializeField] private GameObject _tutorialPanel; 


    // subscribe and unsubscribe to on start tutorial 
    private void OnEnable() => EventHandler.OnTutorialEvent  += GoNext; 
    private void OnDisable() => EventHandler.OnTutorialEvent -= GoNext; 

    // make sure arrows and tutorial messages align 
    private void Awake()
    {
        if (_tutorialArrows.Count != _tutorialMessages.Count)
        {
            Debug.LogWarning("[TutorialHandler] tutorial arrows and messages do not have the same amount!"); 
        }
    }

    // activate the current text, set the previous arrow to false and the next to true 
    private void GoNext(int index)
    {   
        // a -1 broadcast means to wait for a tutorial event so disable the panel
        if (index == -1)
        {   
            DisablePanel();  
            return; 
        }

        // guard out of bounds errors 
        if (index < 0 || index > _tutorialMessages.Count) return; 

        // if the event is the exact count, it is the end of the tutorial
        if (index == _tutorialMessages.Count)
        {
            GameManager.Instance._tutorialActive = false; 
            DisablePanel();
            this.enabled = false; 
            return; 
        }

        // enable the panel 
        _tutorialPanel.SetActive(true); 

        // if it is not 0, set the previous one false
        if (index != 0)
        {
            _tutorialArrows[index - 1].SetActive(false); 
        }
    
        // set the current one to true and activate the message 
        _tutorialArrows[index].SetActive(true); 
        _tutorialText.text = _tutorialMessages[index]; 
    }

    // disable the tutorial panel and all arrows
    private void DisablePanel()
    {
        // deactivate the panel and all the arrows
        _tutorialPanel.SetActive(false);
        foreach (var arrow in _tutorialArrows)
        {
            arrow.SetActive(false); 
        }
    }
}
