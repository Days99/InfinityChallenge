using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelObject", menuName = "Level", order = 1)]
public class LevelSO : ScriptableObject
{
    public int levelIndex;
    public Vector2Int start;
    public Vector2Int end;
    public List<Vector2Int> path;
    public int[,] maze;
    public int width;
    public int heigth;

    public void CreateLevel(Vector2Int start_, Vector2Int end_, int[,] maze_, List<Vector2Int> path_, int width_, int heigth_)
    {
        start = start_;
        end = end_;
        maze = maze_;
        path = path_;
        width = width_;
        heigth = heigth_;
    }
        
}
