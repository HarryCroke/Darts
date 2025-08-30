using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeInput : MonoBehaviour
{
    private TouchControls touchControls;

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
        print(touchControls.Touch.Position.ReadValue<Vector2>());
    }
    
    private void TouchEnded(InputAction.CallbackContext context)
    {
        print(touchControls.Touch.Position.ReadValue<Vector2>());
    }
    
    
}
