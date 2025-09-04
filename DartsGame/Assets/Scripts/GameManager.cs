using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    
    private AudioSource audioSource;
    public AudioClip HitAudio, ThrowAudio, BellAudio;
    
    // Start is called before the first frame update
    void Start()
    {
        Dart.HitBoard += OnBoardHit;
        SwipeInput.DartThrown += OnDartThrown;
        Dart.UpdateScoreText += UpdateScoreText;
        
        CountUpButton.onClick.AddListener(delegate {StartGame(0);});
        ThreeOhOneButton.onClick.AddListener(delegate {StartGame(1);});
        
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called when a game mode is selected
    /// <param name="gameModeIndex">The chosen game mode</param>
    /// </summary>
    private void StartGame(int gameModeIndex)
    {
        currentGameMode = GameModes[gameModeIndex];
        currentGameMode.Manager = this;
        currentGameMode.StartGame();
    }

    /// <summary>
    /// Called when a dart either hit the board or missed
    /// <param name="score">The total score of the dart</param>
    /// <param name="successfulHit">Did the dart successfully hit the dartboard?</param>
    /// </summary>
    private void OnBoardHit(int score, bool successfulHit)
    {
        currentGameMode.OnBoardHit(score, successfulHit);
        if(successfulHit) PlayAudioClip(HitAudio);
    }

    /// <summary>
    /// Called when a dart is thrown
    /// </summary>
    private void OnDartThrown(Dart dart)
    {
        currentGameMode.OnDartThrown(dart);
        PlayAudioClip(ThrowAudio);
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

    /// <summary>
    /// Play an audio clip with a random pitch between 0.9-1.1
    /// </summary>
    public void PlayAudioClip(AudioClip audioClip)
    {
       float pitch = Random.Range(0.9f, 1.1f);
       audioSource.pitch = pitch;
       audioSource.PlayOneShot(audioClip);
    }
}
