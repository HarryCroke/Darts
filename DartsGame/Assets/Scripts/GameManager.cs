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

    [SerializeField] 
    private GameObject MainMenu, CountUpMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        Dart.HitBoard += BoardHit;
        SwipeInput.DartThrown += OnDartThrown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        MainMenu.SetActive(false);
        CountUpMenu.SetActive(true);
        Player.GameActive = true;
        dartCount = 1;
        roundCount = 1;
        totalScore = 0;
        dartList.Clear();
        
        DartText.text = "Dart: " + dartCount;
        RoundText.text = "Round: " + roundCount;
        ScoreText.text = "Total: " + totalScore;
        
        CreateNewDart();
    }
    
    private void EndGame()
    {
        MainMenu.SetActive(true);
        CountUpMenu.SetActive(false);
        Player.GameActive = false;
        ClearDartsOnBoard();
        Destroy(dartList[0].gameObject);
        dartList.Clear();
    }

    private void BoardHit(int score, bool isDouble)
    {
        dartCount++;
        if (dartCount > 3)
        {
            dartCount = 1;
            roundCount++;
            if(roundCount > 5) EndGame();
        }
        
        CreateNewDart();
        
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
