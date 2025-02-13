using UnityEngine;

public enum GameStateEnum
{
    NotStarted,
    InGame,
    Finish
}
public class GameState
{
    public GameStateEnum currentState;
}
