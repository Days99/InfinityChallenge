using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelObject", menuName = "Level", order = 1)]
public class LevelSO : ScriptableObject
{
    public int levelIndex;

    public Vector2Int start;
    public Vector2Int end;
    public int[,] maze;

    public int[,] getMaze() { return maze; }
    public Vector2Int getStart() { return start; }
    public Vector2Int getEnd() { return end; }
    public void CreateLevel(Vector2Int start_, Vector2Int end_, int[,] maze_)
    {
        start = start_;
        end = end_;
        maze = maze_;
    }
        
}
