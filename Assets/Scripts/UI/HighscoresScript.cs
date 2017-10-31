using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HighscoresScript : MonoBehaviour {

    public static HighscoresScript highscores;

    HighscoreContainer container;

    PlayerData zero = new PlayerData();

    private void Awake()
    {
        CreateHighscore();
        CreateContainer();
        LoadAndSort();
    }

    public static void ReturnDataByID(int pID, out int pScore, out string pName)
    {
        if (pID == 0 || pID == 1)
        {
            highscores.LoadAndSort();
        }
        var containerSize = highscores.container.highscoreListData.Count;
        if (containerSize < pID || containerSize == 0)
        {
            pScore = 000000;
            pName = "NULL";
        }
        else
        {
            pScore = highscores.container.highscoreListData[pID].playerScore;
            pName = highscores.container.highscoreListData[pID].playerName;
        }
    }

    public void CreateContainer()
    {
        if (highscores.container == null)
        {
            highscores.container = new HighscoreContainer();
            container = highscores.container;
        }
        else
        {
            container = highscores.container;
        }
    }

    public static void CreateHighscore()
    {
        if (HighscoresScript.highscores == null)
        {
            highscores = new HighscoresScript();
            DontDestroyOnLoad(highscores);
        }
        else
        {
            highscores = HighscoresScript.highscores;
        }
    }

    public static void AddPlayerHighscore(string pName, int pScore)
    {
        PlayerData thisNewPlayer = new PlayerData();
        thisNewPlayer.playerName = pName;
        thisNewPlayer.playerScore = pScore;
        highscores.container.highscoreListData.Add(thisNewPlayer);
        highscores.Save();
        highscores.LoadAndSort();
    }

    public void LoadAndSort()
    {
        Load();
        Sort();
    }

    private void ClearHighscoreList()
    {
        if (File.Exists(Application.persistentDataPath + "/highscoreData.dat"))
        {
            string filepath = Application.persistentDataPath + "/highscoreData.dat";
            File.Delete(filepath);
            FileStream file = File.Create(Application.persistentDataPath + "/highscoreData.dat");
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (!File.Exists(Application.persistentDataPath + "/highscoreData.dat"))
        {
            FileStream file = File.Create(Application.persistentDataPath + "/highscoreData.dat");
            bf.Serialize(file, highscores.container);
            file.Close();
        }
        else
        {

            FileStream file = File.Open(Application.persistentDataPath + "/highscoreData.dat", FileMode.Open);
            bf.Serialize(file, highscores.container);
            file.Close();
        }
    }

    private void Load()
    { 
        if (File.Exists(Application.persistentDataPath + "/highscoreData.dat"))
        {
            HighscoreContainer data = new HighscoreContainer();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/highscoreData.dat", FileMode.Open);
            data = (HighscoreContainer)bf.Deserialize(file);
            file.Close();

            container.highscoreListData = data.highscoreListData;

        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Create(Application.persistentDataPath + "/highscoreData.dat");

            PlayerData easter = new PlayerData();
            easter.playerName = "GOD";
            easter.playerScore = 10000;
            highscores.container.highscoreListData.Add(easter);
            bf.Serialize(file, highscores.container);
            file.Close();
        }
    }

    private void Sort()
    {
        if (File.Exists(Application.persistentDataPath + "/highscoreData.dat"))
        {
            container.highscoreListData.Sort
            (
                delegate (PlayerData firstPair,
                PlayerData secondPair)
                {
                    return secondPair.playerScore.CompareTo(firstPair.playerScore);
                }
            );
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        SceneManager.LoadScene("Main-Menu");
    }

    [Serializable]
    class HighscoreContainer
    {
        public List<PlayerData> highscoreListData = new List<PlayerData>();
    }

    [Serializable]
    class PlayerData
    {
        public string playerName;
        public int playerScore;
    }
}
