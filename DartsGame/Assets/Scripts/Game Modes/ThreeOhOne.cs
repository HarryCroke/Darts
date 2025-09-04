using TMPro;
using UnityEngine;

public class ThreeOhOne : GameMode
{
    // In the 301 game mode, you play repeating rounds of 3 darts trying to reduce your score to exactly 0
    // Going below 0 points means you go bust, and you start the next round with how many points you 
    // started this round with
    private int dartCount = 1, roundCount = 1, remainingScore, roundStartScore;
    
    [SerializeField, Tooltip("Reference to the UI that stores the score and current round/dart")]
    private TextMeshProUGUI DartText, RoundText, ScoreText;
    
    public override void StartGame()
    {
        // Initialise this game mode
        Manager.Player.GameActive = true;
        dartCount = 1;
        roundCount = 1;
        remainingScore = 301;
        roundStartScore = 301;
        Manager.DartList.Clear();
        Manager.CreateNewDart();
        
        // Set up this game mode's UI
        Manager.MainMenu.SetActive(false);
        GameUI.SetActive(true);
        DartText.text = "Dart: " + dartCount;
        RoundText.text = "Round: " + roundCount;
        ScoreText.text = "Remaining: " + remainingScore;
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
        Manager.UpdateScoreText("Rounds taken: " + roundCount);
    }

    public override void OnBoardHit(int score, bool successfulHit)
    {
        remainingScore -= score;
        
        // End the game when at exactly 0 score
        if(remainingScore == 0) EndGame();
        
        dartCount++;
        
        // Start a new round every 3 darts or when bust
        if (dartCount > 3 || remainingScore < 0)
        {
            NewRound();
        }

        roundStartScore = remainingScore;
        
        if(Manager.Player.GameActive) Manager.CreateNewDart();
        
        // Update UI
        DartText.text = "Dart: " + dartCount;
        RoundText.text = "Round: " + roundCount;
        ScoreText.text = "Remaining: " + remainingScore;
    }

    private void NewRound()
    {
        dartCount = 1;
        roundCount++;
        Manager.PlayAudioClip(Manager.BellAudio);

        // Reset points when bust
        if (remainingScore < 0)
        {
            Manager.UpdateScoreText("Bust!");
            remainingScore = roundStartScore;
        } 
        
        roundStartScore = remainingScore;
    }

    public override void OnDartThrown(Dart dart)
    {
        // Clear dartboard when the first dart of a new round is thrown
        if(roundCount != 1 && dartCount == 1) Manager.ClearDartsOnBoard();
    }
}
