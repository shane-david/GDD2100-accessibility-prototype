using UnityEngine; 

[CreateAssetMenu(fileName = "ScriptableObjects/Pressables", menuName = "NewPressable")]
public class PressableSO : ScriptableObject
{
    public GameObject buttonPrefab; 
    public InputNames inputId; 
}