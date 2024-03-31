using System;
using UnityEngine;

public static class StaticEventHandler
{
    public static event Action<Room> OnRoomChanged;
    public static void CallRoomChangedEvent(Room room)
    {
        OnRoomChanged?.Invoke(room);
    }
    public static event Action<Room> OnRoomEnemiesDefeated;
    public static void CallRoomEnemiesDefeatedEvent(Room room)
    {
        OnRoomEnemiesDefeated?.Invoke(room);
    }
    public static event Action<int> OnPointsScored;
    public static void CallPointsScoredEvent(int points)
    {
        OnPointsScored?.Invoke(points);
    }
    public static event Action<ScoreChangedArgs> OnScoreChanged;
    public static void CallScoreChangedEvent(long score,int multiplier)
    {
        OnScoreChanged?.Invoke(new ScoreChangedArgs() { score=score ,multiplier = multiplier });
    }
    public static event Action<bool> OnMultiplier;
    public static void CallMultiplierEvent(bool multiplier)
    {
        OnMultiplier?.Invoke(multiplier);
    }
}
public class ScoreChangedArgs : EventArgs
{
    public long score;
    public int multiplier;
}
