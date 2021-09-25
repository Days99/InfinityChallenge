using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Individual
{
    public int[,] maze;
    public Vector2Int[] starts;
    public Vector2Int[] ends;
    public List<Vector2Int> path;
    public float evaluation;
    public int width;
    public int heigth;

    public Individual(int[,] maze_, Vector2Int[] starts_, Vector2Int[] ends_, float evaluation_, List<Vector2Int> path_, int width_, int heigth_)
    {
        maze = maze_;
        starts = starts_;
        ends = ends_;
        evaluation = evaluation_;
        path = path_;
        width = width_;
        heigth = heigth_;
    }
}
