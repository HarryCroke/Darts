using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SwayingObject : MonoBehaviour
{
    [SerializeField, Tooltip("Animation curves to control the X and Y positions of the dartboard")] 
    private AnimationCurve VerticalCurve, HorizontalCurve;
    
    // The current progress through the animation curve, from 0-1
    private float verticalProgress, horizontalProgress;
    
    [SerializeField, Tooltip("The max distance to move the dartboard")] 
    private float Intensity;
    [SerializeField, Tooltip("The time it takes to complete an animation curve")] 
    private float Duration;
    
    [SerializeField, Tooltip("The slider which controls the Intensity and duration of the sway")]
    private Slider SwaySlider;
    
    private Vector3 initialPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        SwaySlider.onValueChanged.AddListener(delegate{ChangeIntensity();});
    }

    private void FixedUpdate()
    {
        // Progress through each animation curve
        verticalProgress += Time.fixedDeltaTime / Duration;
        horizontalProgress += Time.fixedDeltaTime / Duration;
        if(verticalProgress >= 1) verticalProgress = 0;
        if(horizontalProgress >= 1) horizontalProgress = 0;
        
        // Move this object based on the progress of each curve multiplied by the intensity
        transform.position = new Vector3(initialPosition.x + (Intensity * HorizontalCurve.Evaluate(horizontalProgress)), 
            initialPosition.y + ((Intensity/2) * VerticalCurve.Evaluate(verticalProgress)), transform.position.z);
    }

    /// <summary>
    /// Called when sway slider changes, set intensity and duration based on the slider's value
    /// </summary>
    private void ChangeIntensity()
    {
        Intensity = SwaySlider.value;
        Duration = 100 - (SwaySlider.value * 20);
        
        verticalProgress = Random.Range(0f, 1f);
        horizontalProgress = Random.Range(0f, 1f);
    }
}
