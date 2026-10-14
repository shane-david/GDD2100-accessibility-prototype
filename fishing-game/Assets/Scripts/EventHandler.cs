using System; 

public static class EventHandler
{
    // event for tutorial broadcastes
    public static event Action<int> OnTutorialEvent; 

    // invokes for tutorial broadcast event 
    public static void InvokeTutorialEvent(int index) => OnTutorialEvent?.Invoke(index); 
}