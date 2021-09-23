using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(GeneticAlgorith))]
public class GeneticAlgorithmEditor : Editor
{
    private GeneticAlgorith geneticAlgorithm;
    public override void OnInspectorGUI()
    {
        MonoBehaviour monoBehaviour = (MonoBehaviour)target;
        geneticAlgorithm = monoBehaviour.GetComponent<GeneticAlgorith>();

        DrawDefaultInspector();
        if (GUILayout.Button("Build Grid"))
        {
            geneticAlgorithm.BuildLevel();
        }
        if(GUILayout.Button("Save Level"))
        {
            geneticAlgorithm.SaveLevel();
        }
        if (GUILayout.Button("Load Level"))
        {
            geneticAlgorithm.LoadLevel();
        }
        if (GUILayout.Button("Generate Puzzle"))
        {
            geneticAlgorithm.GeneratePuzzle();
        }
        if (GUILayout.Button("Randomize Puzzle"))
        {
            geneticAlgorithm.RandomizeSolution();
        }
    }
}

#endif