using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : MonoBehaviour
{
    private Rigidbody rb;
    private bool onBoard;
    
    // Called when the dart hits the dartboard
    public delegate void HitBoardEventHandler();
    public static HitBoardEventHandler HitBoard;
    
    // Dart Stats
    [SerializeField, Range(0f, 100f), Tooltip("Multiplied by input vector magnitude to determine force")]
    private float ForceMagnifier;
    [SerializeField, Range(0f, 100f), Tooltip("Z Force applied to dart when launched")]
    private float ForwardForce;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    ///<summary>
    /// Apply force to the dart based on the player's input
    /// <param name="direction">The normalised direction vector of the player's swipe </param>
    /// <param name="force">The magnitude of the player's swipe </param>
    /// <param name="time">The time the player was touching the screen while swiping</param>
    /// </summary>
    public void Launch(Vector2 direction, float force, float time)
    {
        // Don't launch Dart if already in air
        if(rb.isKinematic == false) return;

        force *= ForceMagnifier;
        
        rb.isKinematic = false;
        Vector3 forceVector = new Vector3(direction.x * force, -direction.y * force, -ForwardForce);
        rb.AddForce(forceVector, ForceMode.Impulse);
    }
    
    /// <summary>
    /// Reset this dart to its starting location
    /// </summary>
    public void Reset()
    {
        rb.isKinematic = true;
        rb.transform.position = new Vector3(0, -1, 6);
        rb.transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Dartboard") || onBoard) return;
        rb.isKinematic = true;
        onBoard = true;
        print("Hit");
        HitBoard?.Invoke();
    }

}
