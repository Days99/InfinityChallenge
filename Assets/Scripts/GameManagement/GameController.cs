using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

[Serializable]
class SaveData
{
    public int currentLevel;
    public int[] scores = new int[10];
}
public class GameController : MonoBehaviour
{
    public int currentLevel;
    public int[] scores = new int[10];
    public AudioClip[] clips;
    public GameObject winningEffect;

    private AudioSource source;
    
    // Start is called before the first frame update
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }
        source = GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if(!LoadGame())
        SaveGame(true, 1, scores);
    }

    public void SaveGame(bool firstSave, int currentLevel, int[] scores)
    {
        SaveData data = new SaveData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (firstSave)
        {
            file = File.Create(Application.persistentDataPath + "/PlayerData.dat");
        }
        else
            file = File.Open(Application.persistentDataPath + "/PlayerData.dat", FileMode.Open);

        data.currentLevel = currentLevel;
        data.scores = scores;
        this.currentLevel = currentLevel;
        this.scores = scores;
        bf.Serialize(file, data);
        file.Close();

        Debug.Log(Application.persistentDataPath  + "Game data saved!");
    }

    public bool LoadGame() 
    {
        if (File.Exists(Application.persistentDataPath
               + "/PlayerData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/PlayerData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            currentLevel = data.currentLevel;
            scores = data.scores;
            Debug.Log("Game data loaded!");
            return true;
        }
        else
        {
            Debug.LogError("There is no save data!");
            return false;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void LoadLevel(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void PlaySoundEffect(int i)
    {
        source.PlayOneShot(clips[i]);
    }

    public void LoadNextLevel()
    {
        int nextLevelindex = SceneManager.GetActiveScene().buildIndex + 1;
        if(nextLevelindex > currentLevel)
        SaveGame(false, nextLevelindex, scores);
        SceneManager.LoadScene(nextLevelindex);

    }

    public void ActivateWinningEffect()
    {
        GameObject w = Instantiate(winningEffect, GameObject.FindGameObjectWithTag("Maze").transform);
        w.GetComponent<WinningEffectController>().Init();
        w.GetComponent<WinningEffectController>().StartMoving();
    }

}


