using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountUp : GameMode
{
    // In the Count-up game mode, you play 5 rounds of 3 darts in order to get the highest score possible
    private int dartCount = 1, roundCount = 1, totalScore;
    
    [SerializeField, Tooltip("Reference to the UI that stores the score and current round/dart")]
    private TextMeshProUGUI DartText, RoundText, ScoreText;
    
    public override void StartGame()
    {
        // Initialise this game mode
        Manager.Player.GameActive = true;
        dartCount = 1;
        roundCount = 1;
        totalScore = 0;
        Manager.DartList.Clear();
        Manager.CreateNewDart();
        
        // Set up this game mode's UI
        Manager.MainMenu.SetActive(false);
        GameUI.SetActive(true);
        DartText.text = "Dart: " + dartCount;
        RoundText.text = "Round: " + roundCount;
        ScoreText.text = "Total: " + totalScore;
    }

    public override void EndGame()
    {
        // Reset the game state and clear the dartboard
        Manager.Player.GameActive = false;
        Manager.ClearDartsOnBoard();
        Destroy(Manager.DartList[0].gameObject);
        Manager.DartList.Clear();
        
        // Reset the UI back to the main menu
        Manager.MainMenu.SetActive(true);
        GameUI.SetActive(false);
        Manager.UpdateScoreText("Final Score: " + totalScore);
    }

    public override void OnBoardHit(int score, bool successfulHit)
    {
        totalScore += score;
        
        dartCount++;
        if (dartCount > 3)
        {
            dartCount = 1;
            roundCount++;
            Manager.PlayAudioClip(Manager.BellAudio);
            if(roundCount > 5) EndGame();
        }
        
        if(Manager.Player.GameActive) Manager.CreateNewDart();
        
        // Update UI
        DartText.text = "Dart: " + dartCount;
        RoundText.text = "Round: " + roundCount;
        ScoreText.text = "Total: " + totalScore;
    }

    public override void OnDartThrown(Dart dart)
    {
        // Clear dartboard when the first dart of a new round is thrown
        if(roundCount != 1 && dartCount == 1) Manager.ClearDartsOnBoard();
    }
}
