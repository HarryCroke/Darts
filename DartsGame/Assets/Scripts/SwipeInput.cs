using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeInput : MonoBehaviour
{
    private TouchControls touchControls;
    private Vector2 swipeStart, swipeEnd;
    public Rigidbody rb;

    private void Awake()
    {
        touchControls = new TouchControls();
        touchControls.Enable();
        touchControls.Touch.Press.started += TouchStarted;
        touchControls.Touch.Press.canceled += TouchEnded;
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
    }
    
    private void TouchEnded(InputAction.CallbackContext context)
    {
        swipeEnd = touchControls.Touch.Position.ReadValue<Vector2>();
        Vector2 swipeVector = swipeStart - swipeEnd;
        float swipeMagnitude = swipeVector.magnitude;
        Vector2 swipeDirection = swipeVector.normalized;
        print(swipeStart + " + " + swipeEnd + " = " + swipeVector);
        print(swipeDirection + "     " + swipeMagnitude);
        
        Launch(swipeDirection, swipeMagnitude / 100);
        
    }

    private void Launch(Vector2 direction, float force)
    {
        Vector3 forceVector = new Vector3(direction.x, -direction.y, 0) * force;
        rb.AddForce(forceVector, ForceMode.Impulse);
    }


}
