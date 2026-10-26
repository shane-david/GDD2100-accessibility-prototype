using UnityEngine; 

public abstract class IScaleable : MonoBehaviour
{

    // scale numbers, these should be defined in the inspector for every single 
    // ui class that extends this. 
    [Header("Scale Numbers")]
    [SerializeField] protected Vector2 _scale1; 
    [SerializeField] protected Vector2 _scale2;
    [SerializeField] protected Vector2 _scale3
    ;

    // abstract class to define how the scale changes for each ui-element that extends this class 
    public abstract void ChangeScale(char scale); 
}