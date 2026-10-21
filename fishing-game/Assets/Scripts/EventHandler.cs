using System; 

public static class EventHandler
{
    // event for tutorial broadcastes
    public static event Action<int> OnTutorialEvent; 

    // event for difficulty changes 
    public static event Action OnDifficultyChange; 

    // event for changing the GUI scale for all UI elements
    public static event Action<char> OnScaleChange; 

    // invokes for tutorial broadcast event 
    public static void InvokeTutorialEvent(int index) => OnTutorialEvent?.Invoke(index); 

    // invokes for difficuly change event
    public static void InvokeDifficultyChange() => OnDifficultyChange?.Invoke(); 

    public static void InvokeScaleChange(char scale) => OnScaleChange?.Invoke(scale); 
}