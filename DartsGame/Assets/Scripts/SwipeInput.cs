using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeInput : MonoBehaviour
{
    private TouchControls touchControls;
    private Vector2 swipeStart, swipeEnd;
    private float startTime, endTime;
    public Dart currentDart;
    private Vector3 defaultDartPosition;
    private Quaternion defaultDartRotation;
    public GameObject DartPrefab;

    private void Awake()
    {
        touchControls = new TouchControls();
        touchControls.Enable();
        touchControls.Touch.Press.started += TouchStarted;
        touchControls.Touch.Press.canceled += TouchEnded;
        //touchControls.Touch.Reset.performed += Reset;
    }

    private void Start()
    {
        defaultDartPosition = currentDart.transform.position;
        defaultDartRotation = currentDart.transform.rotation;
        Dart.HitBoard += CreateNewDart;
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    private void TouchStarted(InputAction.CallbackContext context)
    {
        swipeStart = touchControls.Touch.Position.ReadValue<Vector2>();
        startTime = Time.time;
    }
    
    private void TouchEnded(InputAction.CallbackContext context)
    {
        endTime = Time.time;
        float timeHeld = endTime - startTime;
        if(timeHeld < 0.1f) return;
        
        swipeEnd = touchControls.Touch.Position.ReadValue<Vector2>();
        Vector2 swipeVector = swipeStart - swipeEnd;
        float swipeMagnitude = CorrectForScreenSize(swipeVector).magnitude;
        Vector2 swipeDirection = swipeVector.normalized;
        print(swipeStart + " + " + swipeEnd + " = " + swipeVector);
        print(swipeDirection + "     " + swipeMagnitude);
        
        currentDart.Launch(swipeDirection, swipeMagnitude * 50, timeHeld);
    }
    
    private Vector2 CorrectForScreenSize(Vector2 vector)
    {
        return new Vector2(vector.x / Screen.width, vector.y / Screen.height);
    }

    public void Reset()
    {
        currentDart.Reset();
    }

    private void CreateNewDart()
    {
        currentDart = Instantiate(DartPrefab, defaultDartPosition, 
            defaultDartRotation).GetComponent<Dart>();
    }
}
