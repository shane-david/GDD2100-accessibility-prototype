using UnityEngine;
using System.Collections.Generic; 

public class SoundManager : MonoBehaviour
{
    [Header("Sound References")]
    [SerializeField] private AudioSource _audioSource; 
    [SerializeField] private AudioSource _backgroundMusic; 
    [SerializeField] private Dictionary<string, AudioClip> _clips = new(); 

    // set voluems from game settings on awake
    void Awake()
    {
        _audioSource.volume = GameSettings.sfxVolume; 
        _backgroundMusic.volume = GameSettings.musicVolume; 
    }
    // subscribe and unsubscribe to events 
    private void OnEnable()
    {
        EventHandler.OnSoundEvent += PlayOneShot; 
        EventHandler.OnMusicVolumeChanged += ChangeMusicVolume; 
        EventHandler.OnSFXVolumeChanged += ChangeSFXVolume; 
    }

    private void OnDisable()
    {
        EventHandler.OnSoundEvent -= PlayOneShot; 
        EventHandler.OnMusicVolumeChanged -= ChangeMusicVolume; 
        EventHandler.OnSFXVolumeChanged -= ChangeSFXVolume; 
    }

    // play one shot getting it from the dictionary 
    private void PlayOneShot(string sound)
    {
        _audioSource.PlayOneShot(_clips[sound]); 
    }

    // change clips volume
    private void ChangeSFXVolume(float vol)
    {
        _audioSource.volume = vol; 
    }

    // change music volue
    private void ChangeMusicVolume(float vol)
    {
        _backgroundMusic.volume = vol; 
    }

    // click sound to attach to button presses
    public void ClickSound()
    {
        _audioSource.PlayOneShot(_clips["click"]); 
    }
}
