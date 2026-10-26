using UnityEngine;

// this class can be attached to any "standard scaler" that is a UI element that just needs to just 
// its own local scale when the scale event happens and does not need to adjust any inner elements 
// or do anything unique 
public class StandardScaler : IScaleable
{   
    // change this transform.localScale to proper scale 
    public override void ChangeScale(char scale)
    {
        switch (scale)
        {
            case '1':
                this.transform.localScale = _scale1; 
                break;
            case '2':
                this.transform.localScale = _scale2; 
                break;
            case '3':
                this.transform.localScale = _scale3; 
                break;
            default:
                break;
        }
    }

    // subscribe to the event 
    private void OnEnable() {
        EventHandler.OnScaleChange += ChangeScale;
        ChangeScale(GameSettings.guiScale); 
    }
    
    private void OnDisable() => EventHandler.OnScaleChange -= ChangeScale;
    
}