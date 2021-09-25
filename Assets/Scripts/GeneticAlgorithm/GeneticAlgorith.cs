using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticAlgorith : MonoBehaviour
{
    public int mazeWitdth = 10;
    public int mazeHeight = 10;
    public int paths = 1;
    public int popSize = 100;
    public int indivMutationProb = 5;
    public int geneMutationProb = 51;
    public int elitismFactor = 10;
    public int maxGeneration = 100;
    public GameObject straightPieceObject;
    public GameObject curvedPieceObject;
    public GameObject endObj;
    public GameObject tileObj;
    public GameObject pathObj;




    public LevelSO levelSO;


    private GameObject currentMaze;
    private PathfindingGenetic Astar;
    public Individual currentIndividual;
    public List<GameObject> pathObjects;



    //Genetic Algorithm
    private int[,] GenerateIndividual(){
        int[,] newMaze = new int[mazeWitdth, mazeHeight];
        for (int i = 0; i < mazeWitdth; i++)
        {
            for (int j = 0; j < mazeHeight; j++)
            {
                if (i == 0 || i == mazeWitdth - 1 || j == 0 || j == mazeHeight - 1)
                    newMaze[i, j] = 1;
                else
                    newMaze[i, j] = Random.Range(-1,2);
            }
        }
        return newMaze;
    }

    private Vector2Int ValidatePosition(int[,] maze, Vector2Int pos)
    {
        while (maze[pos.x, pos.y] > 0) {
            pos = new Vector2Int(Random.Range(1, mazeWitdth), Random.Range(1, mazeHeight));
        }
        return pos;
        
    }

    private List<Individual> GeneratePopulation()
    {
        List<Individual> newPopulation = new List<Individual>();
        for(int i = 0; i < popSize; i++)
        {
            int[,] maze = GenerateIndividual();
            Vector2Int[] starts = new Vector2Int[paths];
            Vector2Int[] ends = new Vector2Int[paths];

            for (int j = 0; j < paths; j++)
            {
                Vector2Int start = ValidatePosition(maze, new Vector2Int(Random.Range(1, mazeWitdth), Random.Range(1, mazeHeight)));
                starts[j] = start;
                Vector2Int end = ValidatePosition(maze, new Vector2Int(Random.Range(1, mazeWitdth), Random.Range(1, mazeHeight)));
                ends[j] = end;

            }
            newPopulation.Add(new Individual(maze, starts, ends, -1, new List<Vector2Int>(), mazeWitdth, mazeHeight));
        }
        return newPopulation;
    }

    private void EvaluatePopulation(List<Individual> pop)
    {
        foreach(Individual p in pop)
        {
            if(p.evaluation == -1)
            {
                int totalFitness = 0;
                for (int i = 0; i < paths; i++)
                {
                    List<Vector2Int> path = Astar.AStar(p, mazeWitdth, mazeHeight, p.starts[i], p.ends[i]);
                    p.path = path;

                    foreach (Vector2Int v in path)
                        p.maze[v.x, v.y] = 2;
                    int fitness = 1;
                    if (path.Count > 0)
                    {
                        fitness = path.Count;
                        totalFitness += fitness;
                    }
                }
                if (totalFitness == 0)
                    totalFitness = 1;

                p.evaluation = totalFitness;
            }
        }
    }



    private (int parentA, int parentB) Selection(List<Individual> population)
    {
        float totalEvaluation = 0;
        for (int i = 0; i < popSize; i++)
        {
            totalEvaluation += population[i].evaluation;
        }

        int selectedA = -1;
        int selectedB = -1;
        int selected = Random.Range(1, 101);
        float cumulativeChance = 0;

        for (int i = 0; i < popSize; i++)
        {
            float chance = (population[i].evaluation * 100) / totalEvaluation;
            if (selected <= chance + cumulativeChance)
            {
                selectedA = i;
                totalEvaluation -= population[i].evaluation;
                break;
            }

            cumulativeChance += chance;
        }

        selected = Random.Range(1, 101);
        cumulativeChance = 0;

        for (int i = 0; i < popSize; i++)
        {
            if (i == selectedA)
                continue;

            float chance = population[i].evaluation / totalEvaluation * 100;
            if (selected <= chance + cumulativeChance)
            {
                selectedB = i;
                break;
            }

            cumulativeChance += chance;
        }

        if (selectedA == -1 && selectedB == -1)
        {
            selectedA = popSize - 1;
            selectedB = popSize - 2;
        }
        else if (selectedA == -1)
        {
            selectedA = popSize - 1;
        }
        else if (selectedB == -1)
        {
            selectedB = popSize - 1;
        }

        return (selectedA, selectedB);
    }

    private (Individual child1, Individual child2) Crossover(Individual parent1, Individual parent2)
    {
        int crossoverPoint = Random.Range(1, mazeWitdth - 1);

        Individual child1 = new Individual(new int[mazeWitdth, mazeHeight], new Vector2Int[paths], new Vector2Int[paths], -1, new List<Vector2Int>(), mazeWitdth, mazeHeight);
        Individual child2 = new Individual(new int[mazeWitdth, mazeHeight], new Vector2Int[paths], new Vector2Int[paths], -1, new List<Vector2Int>(), mazeWitdth, mazeHeight);

        for(int x = 0; x < mazeWitdth; x++)
        {
            for(int y = 0; y < mazeHeight; y++)
            {
                if (x < crossoverPoint)
                {
                    child1.maze[x, y] = parent1.maze[x, y];
                    child2.maze[x, y] = parent2.maze[x, y];
                }
                else
                {
                    child1.maze[x, y] = parent2.maze[x, y];
                    child2.maze[x, y] = parent1.maze[x, y];
                }
            }
        }

        int startend = Random.Range(0, 4);

        for (int i = 0; i < paths; i++)
        {
            if (startend == 0)
            {
                child1.starts[i] = ValidatePosition(child1.maze, parent1.starts[i]);
                child1.ends[i] = ValidatePosition(child1.maze, parent1.ends[i]);
                child2.starts[i] = ValidatePosition(child2.maze, parent2.starts[i]);
                child2.ends[i] = ValidatePosition(child2.maze, parent2.ends[i]);
            }
            else if (startend == 1)
            {
                child1.starts[i] = ValidatePosition(child1.maze, parent1.starts[i]);
                child1.ends[i] = ValidatePosition(child1.maze, parent2.ends[i]);
                child2.starts[i] = ValidatePosition(child2.maze, parent2.starts[i]);
                child2.ends[i] = ValidatePosition(child2.maze, parent1.ends[i]);
            }
            else if (startend == 2)
            {
                child1.starts[i] = ValidatePosition(child1.maze, parent2.starts[i]);
                child1.ends[i] = ValidatePosition(child1.maze, parent1.ends[i]);
                child2.starts[i] = ValidatePosition(child2.maze, parent1.starts[i]);
                child2.ends[i] = ValidatePosition(child2.maze, parent2.ends[i]);
            }
            else
            {
                child1.starts[i] = ValidatePosition(child1.maze, parent2.starts[i]);
                child1.ends[i] = ValidatePosition(child1.maze, parent2.ends[i]);
                child2.starts[i] = ValidatePosition(child2.maze, parent1.starts[i]);
                child2.ends[i] = ValidatePosition(child2.maze, parent1.ends[i]);
            }
        }
        return (child1, child2);
    }
    

    private Individual Mutation (Individual individual)
    {
        int r = Random.Range(0, 100);
        if(r < indivMutationProb)
        {
            for(int x = 1; x < mazeWitdth - 1; x++)
            {
                for(int y = 1; y< mazeHeight - 1; y++)
                {
                    int g = Random.Range(0, 100);
                    if(g < geneMutationProb)
                    {
                        if (individual.maze[x, y] == 1)
                            individual.maze[x, y] = 0;
                        else
                            individual.maze[x, y] = 1;
                    }
                }
            }
            int changeStart = Random.Range(0, 2);
            int changeEnd = Random.Range(0, 2);
            for (int i = 0; i < paths; i++)
            {
                if (changeStart == 0)
                    individual.starts[i] = ValidatePosition(individual.maze, new Vector2Int(Random.Range(1, mazeWitdth - 1), Random.Range(1, mazeHeight - 1)));
                else
                    individual.starts[i] = ValidatePosition(individual.maze, individual.starts[i]);
                if (changeEnd == 0)
                    individual.ends[i] = ValidatePosition(individual.maze, new Vector2Int(Random.Range(1, mazeWitdth - 1), Random.Range(1, mazeHeight - 1)));
                else
                    individual.ends[i] = ValidatePosition(individual.maze, individual.ends[i]);
            }
        }
        return individual;
    }

    Individual RunGeneticAlgorithm()
    {
        List<Individual> initPop = GeneratePopulation();
        int generationCount = 0;
        while (true)
        {
            generationCount++;
            EvaluatePopulation(initPop);
            initPop.Sort((x, y) => y.evaluation.CompareTo(x.evaluation));
            UnityEngine.Debug.Log("Generation " + generationCount + "Best I " + initPop[0].evaluation);

            if (generationCount >= maxGeneration)
                break;

            List<Individual> newPop = new List<Individual>();
            while (newPop.Count < popSize - elitismFactor)
            {
                (int sel1, int sel2) = Selection(initPop);
                (Individual c1, Individual c2) = Crossover(initPop[sel1], initPop[sel2]);
                 newPop.Add(Mutation(c1));
                 newPop.Add(Mutation(c2));
                
            }
            int childCount = 0;
            while(newPop.Count < popSize)
            {
                newPop.Add(initPop[childCount]);
                childCount++;
            }

            initPop = newPop;



        }

        return initPop[0];
    }

    //Generate Level Geometry
    private void GenerateLevelGeometry(Individual i)
    {
        if (currentMaze != null)
            DestroyImmediate(currentMaze);


        Vector2Int center = new Vector2Int(mazeWitdth / 2, mazeHeight / 2);
        GameObject g = new GameObject();
        BoxCollider box = g.AddComponent<BoxCollider>();
        box.center = new Vector3(0, -1, 0);
        box.size = new Vector3(mazeWitdth - 2, 1, mazeHeight - 2);
        g.layer = 8;

        g.name = "Maze";
        g.tag = "Maze";
        currentMaze = g;

        int[,] newMaze = new int[mazeWitdth, mazeHeight];

        for (int x = 0; x < mazeWitdth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (i.maze[x, y] <= 0)
                {
                    GameObject wall = GameObject.Instantiate(tileObj);
                    wall.transform.position = new Vector3(x - center.x + 0.5f, 0, y - center.y + 0.5f);
                    wall.transform.localScale = new Vector3(1, 1, 1);
                    wall.name = "Ground_" + x + "_" + y;
                    wall.transform.SetParent(currentMaze.transform);
                    wall.layer = 9;
                }
                else if (i.maze[x, y] == 2)
                {
                    GameObject wall = GameObject.Instantiate(pathObj);
                    wall.transform.position = new Vector3(x - center.x + 0.5f, 0, y - center.y + 0.5f);
                    wall.transform.localScale = new Vector3(1, 1, 1);
                    wall.name = "Path_" + x + "_" + y;
                    wall.transform.SetParent(currentMaze.transform);
                    wall.layer = 9;
                }
                for (int j = 0; j < paths; j++)
                {
                    if (x == i.starts[j].x && y == i.starts[j].y)
                    {
                        GameObject start = Instantiate(endObj);
                        start.transform.position = new Vector3(x - center.x + 0.5f, 1, y - center.y + 0.5f);
                        start.transform.localScale = new Vector3(1, 1, 1);
                        start.name = "" + j + "Start_" + x + "_" + y;
                        start.transform.SetParent(currentMaze.transform);
                        start.layer = 10;
                    }
                    else if (x == i.ends[j].x && y == i.ends[j].y)
                    {
                        GameObject end = Instantiate(endObj);
                        end.transform.position = new Vector3(x - center.x + 0.5f, 1, y - center.y + 0.5f);
                        end.transform.localScale = new Vector3(1, 1, 1);
                        end.name = "" + j + "End_" + x + "_" + y;
                        end.transform.SetParent(currentMaze.transform);
                        end.layer = 10;

                    }
                }

            }

        }
    }
    public void BuildLevel()
    {
        Astar = new PathfindingGenetic();
        currentIndividual = RunGeneticAlgorithm();
        GenerateLevelGeometry(currentIndividual);

    }

    //Load and Save Scriptable Objects
    public void SaveLevel()
    {
        var level = ScriptableObject.CreateInstance<LevelSO>();
        level.CreateLevel(currentIndividual.starts[0], currentIndividual.ends[0], currentIndividual.maze, currentIndividual.path, currentIndividual.width, currentIndividual.heigth);

        string[] levelFiles = Directory.GetFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories);
        level.levelIndex = levelFiles.Length;
#if UNITY_EDITOR

        UnityEditor.AssetDatabase.CreateAsset(level, "Assets/ScriptableObjects/EnergyLevel" + level.levelIndex + ".asset");
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void LoadLevel()
    {
        var starts = new Vector2Int[1];
        var ends = new Vector2Int[1];
        starts[0] = levelSO.start;
        ends[0] = levelSO.end;
        Individual I = new Individual(levelSO.maze, starts, ends, -1, levelSO.path, levelSO.width, levelSO.heigth);
        currentIndividual = I;
        GenerateLevelGeometry(I);
    }

    public void GeneratePuzzle()
    {
        Astar = new PathfindingGenetic();
        pathObjects = new List<GameObject>();
        for (int i = 1; i < currentIndividual.path.Count - 1; i++)
        {

            if ((currentIndividual.path[i].x + 1 == currentIndividual.path[i + 1].x && currentIndividual.path[i].x - 1 == currentIndividual.path[i - 1].x) || (currentIndividual.path[i].x - 1 == currentIndividual.path[i + 1].x && currentIndividual.path[i].x + 1 == currentIndividual.path[i - 1].x))
                GeneratePiece(0, currentIndividual.path[i], straightPieceObject, Vector2.zero, "S0");
            else if ((currentIndividual.path[i].y + 1 == currentIndividual.path[i + 1].y && currentIndividual.path[i].y - 1 == currentIndividual.path[i - 1].y) || (currentIndividual.path[i].y - 1 == currentIndividual.path[i + 1].y && currentIndividual.path[i].y + 1 == currentIndividual.path[i - 1].y))
                GeneratePiece(90, currentIndividual.path[i], straightPieceObject, Vector2.zero, "S1");

            else if ((currentIndividual.path[i].y + 1 == currentIndividual.path[i + 1].y && currentIndividual.path[i].x + 1 == currentIndividual.path[i - 1].x) || currentIndividual.path[i].y + 1 == currentIndividual.path[i - 1].y && currentIndividual.path[i].x + 1 == currentIndividual.path[i + 1].x)
                GeneratePiece(0, currentIndividual.path[i], curvedPieceObject, new Vector2(0.19f, 0.19f), "C0");
            else if((currentIndividual.path[i].y - 1 == currentIndividual.path[i + 1].y && currentIndividual.path[i].x - 1 == currentIndividual.path[i - 1].x) || (currentIndividual.path[i].y - 1 == currentIndividual.path[i - 1].y && currentIndividual.path[i].x - 1 == currentIndividual.path[i + 1].x))
                GeneratePiece(180, currentIndividual.path[i], curvedPieceObject, new Vector2(-0.19f, -0.19f), "C1");

            else if ((currentIndividual.path[i].y + 1 == currentIndividual.path[i + 1].y && currentIndividual.path[i].x - 1 == currentIndividual.path[i - 1].x) || currentIndividual.path[i].y + 1 == currentIndividual.path[i - 1].y && currentIndividual.path[i].x - 1 == currentIndividual.path[i + 1].x)
                GeneratePiece(90, currentIndividual.path[i], curvedPieceObject, new Vector2(-0.19f, 0.19f), "C2");

            else if((currentIndividual.path[i].y - 1 == currentIndividual.path[i + 1].y && currentIndividual.path[i].x + 1 == currentIndividual.path[i - 1].x) || (currentIndividual.path[i].y - 1 == currentIndividual.path[i - 1].y && currentIndividual.path[i].x + 1 == currentIndividual.path[i + 1].x))
                GeneratePiece(-90, currentIndividual.path[i], curvedPieceObject, new Vector2(0.19f, -0.19f), "C3");

         


        }
    }

    //Generate line puzzle and randomize

    private void GeneratePiece(float angle, Vector2Int pos, GameObject newpiece, Vector2 offset, string name)
    {
        Vector2Int center = new Vector2Int(currentIndividual.width / 2, currentIndividual.heigth / 2);
        GameObject piece = Instantiate(newpiece);
        piece.layer = 10;
        piece.GetComponent<PieceController>().offset = offset;
        piece.transform.position = new Vector3(pos.x - center.x + 0.5f + offset.x, 1, pos.y - center.y + 0.5f + offset.y);
        piece.transform.SetParent(currentMaze.transform);
        piece.GetComponent<PieceController>().pieceName = name;

        RaycastHit hit;
        int layerMask = 1 << 9;
        if(Physics.Raycast(piece.transform.position, new Vector3(0,-10,0), out hit, 100, layerMask))
        {
            if (hit.transform.name.Contains("Path"))
            {
                string newName = name + hit.transform.name;
                hit.transform.name = newName;
            }
        }

        if (!piece.name.Contains("Str"))
        {
            piece.transform.localScale = new Vector3(50, 50, 50);
            piece.transform.localEulerAngles = new Vector3(90, 0, angle);
        }
        else
        {
            piece.transform.localEulerAngles = new Vector3(0, angle, 90);
            piece.transform.localScale = new Vector3(0.3f, 0.5f, 0.25f);
        }


        piece.name = "Puzzle_" + pos.x + "_" + pos.y;
        pathObjects.Add(piece);


    }



    public void RandomizeSolution()
    {
        List<Vector2Int> randomPositions = new List<Vector2Int>();
        foreach(GameObject g in pathObjects)
        {
            Vector2Int center = new Vector2Int(currentIndividual.width / 2, currentIndividual.heigth / 2);
            Vector2Int randomPos = GenerateRandomPosition(randomPositions);
            if (randomPos == Vector2Int.zero)
                randomPos = GenerateRandomPosition(randomPositions);

            randomPositions.Add(randomPos);

           g.transform.localPosition =  new Vector3(randomPos.x - center.x + 0.5f + g.GetComponent<PieceController>().offset.x, g.transform.localPosition.y, randomPos.y - center.y + 0.5f + g.GetComponent<PieceController>().offset.y);
        }
    }

    Vector2Int GenerateRandomPosition(List<Vector2Int> positions)
    {
        Vector2Int randomPos = ValidateRandomPosition(currentIndividual.maze, new Vector2Int(Random.Range(1, currentIndividual.width), Random.Range(1, currentIndividual.heigth)));
        foreach (Vector2Int pos in positions) 
        {
            if (pos == randomPos)
                return Vector2Int.zero;
        }
        return randomPos;
    }

    private Vector2Int ValidateRandomPosition(int[,] maze, Vector2Int pos)
    {
        while (maze[pos.x, pos.y] == 1 || currentIndividual.starts[0] == pos || currentIndividual.ends[0] == pos)
        {
            pos = new Vector2Int(Random.Range(1, mazeWitdth), Random.Range(1, mazeHeight));
        }
        return pos;

    }
}
