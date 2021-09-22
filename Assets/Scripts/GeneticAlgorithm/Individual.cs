using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Individual
{
    public int[,] maze;
    public Vector2Int[] starts;
    public Vector2Int[] ends;
    public float evaluation;

    public Individual(int[,] maze_, Vector2Int[] starts_, Vector2Int[] ends_, float evaluation_)
    {
        maze = maze_;
        starts = starts_;
        ends = ends_;
        evaluation = evaluation_;
    }
}
