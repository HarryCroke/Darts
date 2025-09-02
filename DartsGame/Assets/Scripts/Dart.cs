using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    
    // Distance / Multiplier
    private static Dictionary<float, int> multiplierDistances = new Dictionary<float, int>
    {
        {0.3f, 2}, // Double Bull
        {0.55f, 1}, // Single bull
        {2.55f, 1}, // Singles 1
        {2.85f, 3}, // Triples
        {4.15f, 1}, // Singles 2
        {4.45f,2} // Doubles
        //{1000f, 0} // Out
    };
    
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
        HitBoard?.Invoke();
        Dartboard dartboard = other.transform.parent.GetComponent<Dartboard>();
        float scoreMultiplier = GetScoreFromDistance(GetDistanceFromBullseye(dartboard.Bullseye));
        GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = scoreMultiplier.ToString();

    }

    private float GetDistanceFromBullseye(GameObject bullseye)
    {
        Vector2 dartLocation = new Vector2(transform.position.x, transform.position.y);
        Vector2 bullseyeLocation = new Vector2(bullseye.transform.position.x, bullseye.transform.position.y);
        return Vector2.Distance(dartLocation, bullseyeLocation);
    }

    private float GetScoreFromDistance(float distance)
    {
        foreach (var item in multiplierDistances)
        {
            if (distance < item.Key)
            {
                return item.Value;
            }
        }

        return 0;
    }
    
}
