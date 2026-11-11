using UnityEngine;
using System.Collections.Generic; 

public class SoundManager : MonoBehaviour
{
    [Header("Sound References")]
    [SerializeField] private AudioSource _audioSource; 
    [SerializeField] private Dictionary<string, AudioClip> _clips = new(); 

    // subscribe and unsubscribe to events 
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    // play one shot getting it from the dictionary 
    private void PlayOneShot(string sound)
    {
        
    }
}
