using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dart : MonoBehaviour
{
    private Rigidbody rb;
    // Is the dart stuck to the dartboard?
    private bool onBoard;
    
    // Called when the dart hits the dartboard
    public delegate void HitBoardEventHandler(int score, bool isDouble);
    public static HitBoardEventHandler HitBoard;
    
    
    public delegate void UpdateScoreTextEventHandler(string newText);
    public static UpdateScoreTextEventHandler UpdateScoreText;
    
    // Dart Stats
    [SerializeField, Range(0f, 100f), Tooltip("Multiplied by input vector magnitude to determine force")]
    private float ForceMagnifier;
    [SerializeField, Range(0f, 100f), Tooltip("Z Force applied to dart when launched")]
    private float ForwardForce;
    
    private int baseScore, totalScore, multiplier;
    
    // Score multiplier is calculated by the dart's distance from the bullseye
    // Key - Maximum distance from Bullseye, Value - respective multiplier
    private static Dictionary<float, int> multiplierDistances = new Dictionary<float, int>
    {
        {0.3f, 2}, // Double Bull
        {0.55f, 1}, // Single bull
        {2.55f, 1}, // Singles 1
        {2.85f, 3}, // Triples
        {4.15f, 1}, // Singles 2
        {4.45f,2} // Doubles
    };

    // Base score is calculated using the angle between the bullseye
    // and the dart to calculate which segment the dart is in
    // This array stores the score of each 18 degree segment
    private static int[] scoreArray = new int[]
    {
        11, 14, 9, 12, 5, 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8

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
        rb.transform.position = new Vector3(0, -0.93f, 4.91f);
        rb.transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Backboard") && !onBoard)
        {
            OnMiss();
        }
        else if (other.CompareTag("Dartboard") && !onBoard)
        {
            OnBoardHit(other);
        }
    }

    /// <summary>
    /// Called when this dart hits the dartboard
    /// </summary>
    private void OnBoardHit(Collider other)
    {
        rb.isKinematic = true;
        onBoard = true;
        Dartboard dartboard = other.transform.parent.GetComponent<Dartboard>();

        totalScore = CalculateScore(dartboard.Bullseye);
        string scoreText = (baseScore + " x " + multiplier + " = " + totalScore);
        UpdateScoreText?.Invoke(scoreText);
        
        // Disable collision once on the board
        GetComponent<BoxCollider>().enabled = false;
        
        HitBoard?.Invoke(totalScore, true);

        gameObject.transform.parent = other.gameObject.transform.parent;
    }
    
    /// <summary>
    /// Called when a dart hits the back wall or falls to the floor
    /// </summary>
    private void OnMiss()
    {
        // Check the dart hasn't already hit the board
        if(onBoard) return;
        
        totalScore = 0;
        onBoard = true;

        UpdateScoreText?.Invoke("Miss!");
        HitBoard?.Invoke(totalScore, false);
    }

    /// <summary>
    /// Calculate and return the score of this dart when hitting the board
    /// <param name="bullseye">Reference to a game-object at the centre of the dartboard</param>
    /// </summary>
    private int CalculateScore(GameObject bullseye)
    {
        baseScore = GetScoreFromAngle(GetAngleFromBullseye(bullseye));
        
        // Base score will be overwritten if a bullseye is hit
        multiplier = GetMultiplierFromDistance(GetDistanceFromBullseye(bullseye));
        
        return baseScore * multiplier;
    }

    /// <summary>
    /// Return the 2D distance between this object and the Bullseye. Z position is ignored
    /// <param name="bullseye">Reference to a game-object at the centre of the dartboard</param>
    /// </summary>
    private float GetDistanceFromBullseye(GameObject bullseye)
    {
        Vector2 dartLocation = new Vector2(transform.position.x, transform.position.y);
        Vector2 bullseyeLocation = new Vector2(bullseye.transform.position.x, bullseye.transform.position.y);
        return Vector2.Distance(dartLocation, bullseyeLocation);
    }

    /// <summary>
    /// The score multiplier (e.g double/triple) is determined by the distance the dart landed from the bullseye.
    /// This method also overrides 'baseScore' if this dart hit a bullseye
    /// <param name="distance">Distance between this dart and the bullseye</param>
    /// </summary>
    private int GetMultiplierFromDistance(float distance)
    {
        foreach (var item in multiplierDistances)
        {
            if (distance < item.Key)
            {
                // Override score if bullseye hit
                if (distance < 0.55)
                {
                    baseScore = 25;
                }
                
                return item.Value;
            }
        }

        return 0;
    }

    /// <summary>
    /// Return the angle of the dart from the bullseye from 0-360, rotated so that 0 is the start of a segment.
    /// Z position is ignored.
    /// <param name="bullseye">Reference to a game-object at the centre of the dartboard</param>
    /// </summary>
    private float GetAngleFromBullseye(GameObject bullseye)
    {
        Vector2 dartLocation = new Vector2(transform.position.x, transform.position.y);
        Vector2 bullseyeLocation = new Vector2(bullseye.transform.position.x, bullseye.transform.position.y);
        Vector2 diff = dartLocation - bullseyeLocation;
        
        // 9 is added as it is half of the angle of each segment, otherwise 0 would fall in the centre of a segment
        float angle =  (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) + 9;
        
        // Convert minus degrees to positive degrees
        if (angle < 0) angle += 360;
        return angle;
    }

    /// <summary>
    /// Return the base score of the segment the dart landed in. 
    /// <param name="angle">The angle of the dart from the bullseye between 0-360</param>
    /// </summary>
    private int GetScoreFromAngle(float angle)
    {
        for (int i = 0; i < scoreArray.Length; i++)
        {
            if (angle < (i + 1) * 18)
            {
                return scoreArray[i];
            }
        }
        return 0;
    }

}
