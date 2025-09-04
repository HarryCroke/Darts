using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    /// <summary>
    /// Called by the manager when a new game of this game-mode started
    /// </summary>
    public abstract void StartGame();

    /// <summary>
    /// Called by the game-mode when the game-mode ends
    /// </summary>
    public abstract void EndGame();

    /// <summary>
    /// Called when a dart hits the dartboard
    /// <param name="score">The score awarded by the dart that just hit the board</param>
    /// <param name="isDouble">Did the dart land in a double zone?</param>
    /// </summary>
    public abstract void OnBoardHit(int score, bool isDouble);
    
    /// <summary>
    /// Called when a dart is launched by the player
    /// <param name="dart">A reference to the dart launched by the player</param>
    /// </summary>
    public abstract void OnDartThrown(Dart dart);
    
    [NonSerialized] // A reference to the Game Manager
    public GameManager Manager;

    [SerializeField, Tooltip("The UI which displays scores and other data relevant to this game mode")] 
    protected GameObject GameUI;
}
