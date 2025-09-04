using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public SwipeInput Player;
    public GameObject DartPrefab;
    public Vector3 InitialDartPosition;
    
    [NonSerialized] // List of all the Darts spawned by the game manager
    public List<GameObject> DartList = new List<GameObject>();

    [Tooltip("Reference to the parent game object of the main menu")]
    public GameObject MainMenu;
    
    [Tooltip("Contains a list of available Game Mode components")]
    public GameMode[] GameModes;
    private GameMode currentGameMode;
    
    [SerializeField]
    private Button CountUpButton, ThreeOhOneButton;

    [SerializeField, Tooltip("Reference to the Textbox which displays the final score of the previous game")]
    private TextMeshProUGUI FinalScoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        Dart.HitBoard += OnBoardHit;
        SwipeInput.DartThrown += OnDartThrown;
        Dart.UpdateScoreText += UpdateScoreText;
        
        CountUpButton.onClick.AddListener(delegate {StartGame(0);});
        ThreeOhOneButton.onClick.AddListener(delegate {StartGame(1);});
    }

    private void StartGame(int gameModeIndex)
    {
        currentGameMode = GameModes[gameModeIndex];
        currentGameMode.Manager = this;
        currentGameMode.StartGame();
    }
    
    private void EndGame()
    {
        currentGameMode.EndGame();
    }

    private void OnBoardHit(int score, bool isDouble)
    {
        currentGameMode.OnBoardHit(score, isDouble);
    }

    private void OnDartThrown(Dart dart)
    {
        currentGameMode.OnDartThrown(dart);
    }

    /// <summary>
    /// Destroy any dart currently on the dartboard
    /// </summary>
    public void ClearDartsOnBoard()
    {
        // Newest Dart represents the dart currently available to be launched by the player
        GameObject newestDart = DartList[DartList.Count - 1];
        
        DartList.Remove(newestDart);
        foreach (var dart in DartList)
        {
            Destroy(dart);
        }
        
        DartList = new List<GameObject>() {newestDart};
    }
    
    /// <summary>
    /// Create a new Dart ready for the player to launch
    /// </summary>
    public void CreateNewDart()
    {
        GameObject newDart = Instantiate(DartPrefab, InitialDartPosition, Quaternion.identity);
        Player.CurrentDart = newDart.GetComponent<Dart>();
        DartList.Add(newDart);
    }

    /// <summary>
    /// Update the global score text with new text
    /// </summary>
    public void UpdateScoreText(string newText)
    {
        FinalScoreText.text = newText;
    }
}
