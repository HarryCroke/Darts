using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : MonoBehaviour
{
    private Rigidbody rb;
    private bool onBoard;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    public delegate void HitBoardEventHandler();
    public static HitBoardEventHandler HitBoard;

    public void Launch(Vector2 direction, float force, float time)
    {
        rb.isKinematic = false;
        //force /= time;
        Vector3 forceVector = new Vector3(direction.x, -direction.y, -5) * force;
        rb.AddForce(forceVector, ForceMode.Impulse);
    }
    
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

    // private void OnCollisionEnter(Collision other)
    // {
    //     if(!other.gameObject.CompareTag("Dartboard")) return;
    //     rb.isKinematic = true;
    //     print("Collision");
    //     HitBoard?.Invoke();
    // }
}
