using UnityEngine;
using TMPro; 
using System.Collections.Generic; 

public class TutorialHandler : IScaleable
{
   // necessary references
   [SerializeField] private TextMeshProUGUI _tutorialText;
   [SerializeField] private List<string> _tutorialMessages = new();     
   [SerializeField] private GameObject _tutorialPanel; 


    // subscribe and unsubscribe to on start tutorial 
    private void OnEnable() 
    {
        EventHandler.OnTutorialEvent  += GoNext; 
        EventHandler.OnScaleChange += ChangeScale;
        ChangeScale(GameSettings.guiScale);  
    }
    private void OnDisable()
    {
        EventHandler.OnTutorialEvent -= GoNext; 
        EventHandler.OnScaleChange -= ChangeScale;  
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
    
        // set the current one to true and activate the message 
        _tutorialText.text = _tutorialMessages[index]; 
    }

    // disable the tutorial panel and all arrows
    private void DisablePanel()
    {
        // deactivate the panel and all the arrows
        _tutorialPanel.SetActive(false);
    }

    public override void ChangeScale(char scale)
    {
        switch (scale)
        {
            case '1':
                _tutorialPanel.transform.localScale = _scale1; 
                break;
            case '2':
                _tutorialPanel.transform.localScale = _scale2;
                break;
            case '3':
                _tutorialPanel.transform.localScale = _scale3;
                break;
            default:
                break;  
        }
    }
}
