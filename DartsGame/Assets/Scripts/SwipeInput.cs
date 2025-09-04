using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SwipeInput : MonoBehaviour
{
    // Input / Swipe
    private TouchControls touchControls;
    private Vector2 swipeStart, swipeEnd;
    private float startTime, endTime;
    
    // Darts
    public Dart CurrentDart;
    private Vector3 defaultDartPosition;
    private Quaternion defaultDartRotation;
    public GameObject DartPrefab;
    
    // Called when a dart is launched
    public delegate void DartThrownEventHandler(Dart dart);
    public static DartThrownEventHandler DartThrown;
    
    // Sensitivity
    private float sensitivity = 0.6f;
    [SerializeField]
    private Slider SensitivitySlider;
    
    [NonSerialized]
    public bool GameActive;

    private void Awake()
    {
        // Initialise player's input
        touchControls = new TouchControls();
        touchControls.Enable();
        
        touchControls.Touch.Press.started += TouchStarted;
        touchControls.Touch.Press.canceled += TouchEnded;
        
        SensitivitySlider.onValueChanged.AddListener(delegate{ChangeSensitivity();});
    }
    
    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    /// <summary>
    /// Called when player starts touching the screen to begin a swipe
    /// </summary>
    private void TouchStarted(InputAction.CallbackContext context)
    {
        swipeStart = touchControls.Touch.Position.ReadValue<Vector2>();
        startTime = Time.time;
    }
    
    /// <summary>
    /// Called when player stops touching the screen, ending the swipe and launching a dart
    /// </summary>
    private void TouchEnded(InputAction.CallbackContext context)
    {
        // Ignore swipe if in main menu or if dart is already travelling
        if(!GameActive || !CurrentDart.Rb.isKinematic) return;
        
        // Calculate time length of swipe, return if swipe too long or short
        endTime = Time.time;
        float timeHeld = endTime - startTime;
        if(timeHeld < 0.05f || timeHeld > 1) return;
        
        swipeEnd = touchControls.Touch.Position.ReadValue<Vector2>();
        
        Vector2 swipeVector = swipeStart - swipeEnd;
        float swipeMagnitude = CorrectForScreenSize(swipeVector).magnitude * sensitivity;
        Vector2 swipeDirection = swipeVector.normalized;
        
        CurrentDart.Launch(swipeDirection, swipeMagnitude, timeHeld);
        DartThrown?.Invoke(CurrentDart);
    }
    
    /// <summary>
    /// Convert Vector from length in pixels to % of screen
    /// <param name="vector">Input Vector in pixels </param>
    /// <returns>Converted Vector in % of screen</returns>
    /// </summary>
    private static Vector2 CorrectForScreenSize(Vector2 vector)
    {
        return new Vector2(vector.x / Screen.width, vector.y / Screen.height);
    }

    private void ChangeSensitivity()
    {
        sensitivity = SensitivitySlider.value / 10;
    }
}
