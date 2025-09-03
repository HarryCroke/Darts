using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SwipeInput Player;
    
    public GameObject DartPrefab;
    public Vector3 InitialDartPosition;

    private int dartCount = 1, roundCount = 1, totalScore;
    [SerializeField]
    private TextMeshProUGUI DartText, RoundText, ScoreText;
    
    private List<GameObject> dartList = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        Dart.HitBoard += BoardHit;
        SwipeInput.DartThrown += OnDartThrown;
        CreateNewDart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BoardHit(int score, bool isDouble)
    {
        CreateNewDart();
        dartCount++;
        if (dartCount > 3)
        {
            dartCount = 1;
            roundCount++;
        }
        
        totalScore += score;
        
        DartText.text = "Dart: " + dartCount;
        RoundText.text = "Round: " + roundCount;
        ScoreText.text = "Total: " + totalScore;
        
    }

    private void OnDartThrown(Dart dart)
    {
        if(roundCount != 1 && dartCount == 1) ClearDartsOnBoard();
    }

    private void ClearDartsOnBoard()
    {
        GameObject newestDart = dartList[3];
        dartList.Remove(newestDart);
        foreach (var dart in dartList)
        {
            Destroy(dart);
        }
        
        dartList = new List<GameObject>() {newestDart};
    }

    private void GameEnd()
    {
        
    }

    private void ResetGame()
    {
        
    }
    
    /// <summary>
    /// </summary>
    private void CreateNewDart()
    {
        GameObject newDart = Instantiate(DartPrefab, InitialDartPosition, Quaternion.identity);
        Player.CurrentDart = newDart.GetComponent<Dart>();
        dartList.Add(newDart);
    }
}
