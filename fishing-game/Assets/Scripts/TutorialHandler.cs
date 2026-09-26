using UnityEngine;
using TMPro; 
using System.Collections.Generic; 

public class TutorialHandler : MonoBehaviour
{
   // necessary references
   [SerializeField] private TextMeshProUGUI _tutorialText;
   [SerializeField] private GameObject      _tutorialButton; 
   [SerializeField] private GameObject _tutorialPanel; 

    [SerializeField] private List<string> _tutorialMessages = new(); 

    private int _tutorialIndex = 0; 

    public void StartTutorial()
    {
        _tutorialButton.SetActive(false); 
        _tutorialPanel.SetActive(true); 
        _tutorialText.text = _tutorialMessages[_tutorialIndex];
    }

    public void GoNext()
    {
        _tutorialIndex++; 
        if (_tutorialIndex >= _tutorialMessages.Count)
        {
            _tutorialPanel.SetActive(false); 
            return; 
        }

        _tutorialText.text = _tutorialMessages[_tutorialIndex];
    }
}
