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
    public Rigidbody Rb;

    private void Awake()
    {
        touchControls = new TouchControls();
        touchControls.Enable();
        touchControls.Touch.Press.started += TouchStarted;
        touchControls.Touch.Press.canceled += TouchEnded;
        touchControls.Touch.Reset.performed += Reset;
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
        
        Launch(swipeDirection, swipeMagnitude * 50, timeHeld);
        
    }

    private void Launch(Vector2 direction, float force, float time)
    {
        //force /= time;
        Vector3 forceVector = new Vector3(direction.x, -direction.y, -5) * force;
        Rb.AddForce(forceVector, ForceMode.Impulse);
    }

    private Vector2 CorrectForScreenSize(Vector2 vector)
    {
        return new Vector2(vector.x / Screen.width, vector.y / Screen.height);
    }

    private void Reset(InputAction.CallbackContext context)
    {
        Rb.transform.position = new Vector3(0, -1, 6);
        Rb.transform.rotation = Quaternion.identity;
        Rb.velocity = Vector3.zero;
    }


}
