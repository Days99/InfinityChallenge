using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class PathfindingGenetic : MonoBehaviour
{
    private Individual currentMaze;
    


    float Heuristic(Vector2Int current, Vector2Int end)
    {
        return Vector2Int.Distance(current, end);
    }

    List<Vector2Int> Reconstruct(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current, Vector2Int start)
    {
        List<Vector2Int> finalPath = new List<Vector2Int>();
        finalPath.Add(current);
        while(current != start)
        {
            current = cameFrom[current];
            finalPath.Add(current);
        }
        finalPath.Reverse();

        return finalPath;
    }

     public List<Vector2Int> AStar(Individual individual, int width, int height, Vector2Int start, Vector2Int goal)
    {
        currentMaze = individual;
        List<Vector2Int> closedSet = new List<Vector2Int>();
        SimplePriorityQueue<Vector2Int> openSet = new SimplePriorityQueue<Vector2Int>();

        openSet.Enqueue(start, Heuristic(start, goal));

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();

        
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                gScore.Add(new Vector2Int(i,j), Mathf.Infinity);
            }
        }
        gScore[start] = 0;

        while(openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();
            if(current == goal)
            {
                return Reconstruct(cameFrom, current, start);
            }
            closedSet.Add(current);
            foreach(Vector2Int neighboor in GetVector2Ints(current, individual.maze, width, height))
            {
                if (closedSet.Contains(neighboor))
                    continue;

                if (!openSet.Contains(neighboor))
                    openSet.Enqueue(neighboor, gScore[neighboor] + Heuristic(neighboor, goal));

                float tentaticeScore = gScore[current] + Heuristic(current, neighboor);

                if (tentaticeScore >= gScore[neighboor])
                    continue;

                cameFrom[neighboor] = current;
                gScore[neighboor] = tentaticeScore + (individual.maze[neighboor.x,neighboor.y]);
                openSet.UpdatePriority(neighboor, gScore[neighboor] + Heuristic(neighboor, goal));

            }

        }
        return new List<Vector2Int>();

    }

    private List<Vector2Int> GetVector2Ints(Vector2Int pos, int[,] maze, int width, int height)
    {
        List<Vector2Int> neighboors = new List<Vector2Int>();
        if ((pos.x + 1 < width) && (maze[pos.x + 1, pos.y] <= 0))
            neighboors.Add(new Vector2Int(pos.x + 1, pos.y));
        if ((pos.x - 1 > 0) && (maze[pos.x - 1, pos.y] <= 0))
            neighboors.Add(new Vector2Int(pos.x - 1, pos.y));
        if ((pos.y + 1 < height) && (maze[pos.x, pos.y + 1] <= 0))
            neighboors.Add(new Vector2Int(pos.x, pos.y + 1));        
        if ((pos.y - 1 > 0) && (maze[pos.x, pos.y - 1] <= 0))
            neighboors.Add(new Vector2Int(pos.x, pos.y - 1));
        return neighboors;
    }

}
