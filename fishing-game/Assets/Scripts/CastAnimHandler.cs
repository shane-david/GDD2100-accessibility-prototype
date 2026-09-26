using UnityEngine; 
using System.Collections; 

public class CastAnimHandler : MonoBehaviour
{   
    // get necessary references 
    [SerializeField] private Animator _animator; 

    // subscribe/unsubscribe 
    private void OnEnable()
    {
        InputHandler.OnCastInput += OnCastButtonPressed; 
    }

    private void OnDisable()
    {
        InputHandler.OnCastInput -= OnCastButtonPressed; 
    }

    private void Awake()
    {
        _animator.speed = 0f; 
    }

    // stop the animation when the cast button is released and start it when it is realased 
    public void OnCastButtonPressed(string inputType)
    {   
        if (inputType == "pressed") {
            _animator.speed = 3f; 
        } else if (inputType == "released") {   
            _animator.speed = 0f; 
            StartCoroutine(TransitionDelay(1f));  
        }
    }

    private IEnumerator TransitionDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.FinishCast(); 
    }

}