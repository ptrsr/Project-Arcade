using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class HighscoresScript : MonoBehaviour {

    public static HighscoresScript highscores;

    HighscoreContainer container = new HighscoreContainer();

    private string testThis;

    PlayerData testPlayerOne = new PlayerData();
    PlayerData testPlayerTwo = new PlayerData();
    PlayerData testPlayerThree = new PlayerData();
    PlayerData testPlayerFour = new PlayerData();
    PlayerData testPlayerFive = new PlayerData();

    private void Awake()
    {
        if (highscores == null)
        {
            DontDestroyOnLoad(gameObject);
            highscores = this;
        }
        else if (highscores != this)
        {
            Destroy(gameObject);
        }

        testPlayerOne.playerName = "VochtigeMiem";
        testPlayerOne.playerScore = 420;
        testPlayerTwo.playerName = "DankeMemmesboi";
        testPlayerTwo.playerScore = 890;
        testPlayerThree.playerName = "IvanIsBlackSheep";
        testPlayerThree.playerScore = 10;
        testPlayerFour.playerName = "EtienneIsGod";
        testPlayerFour.playerScore = 9999;
        testPlayerFive.playerName = "RuudSenpai";
        testPlayerFive.playerScore = 666;

        container.highscoreListData.Add(testPlayerOne);
        container.highscoreListData.Add(testPlayerTwo);
        container.highscoreListData.Add(testPlayerThree);
        container.highscoreListData.Add(testPlayerFour);
        container.highscoreListData.Add(testPlayerFive);
    }

    public void AddPlayerHighscore()
    {
        PlayerData thisNewPlayer = new PlayerData();
        thisNewPlayer.playerName = "";
        thisNewPlayer.playerScore = 0;
        container.highscoreListData.Add(thisNewPlayer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Save();
            LoadAndSort();
        }
    }

    private void LoadAndSort()
    {
        Load();
        Sort();

        testThis = "";

        foreach(PlayerData player in container.highscoreListData)
        {
            testThis += player.playerName + " " + player.playerScore + " ";
        }

        Debug.Log(testThis);
    }

    private void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (!File.Exists(Application.persistentDataPath + "/highscoreinfo.dat"))
        {
            FileStream file = File.Create(Application.persistentDataPath + "/highscoreInfo.dat");
            bf.Serialize(file, container);
            file.Close();
        }
        else
        {
            FileStream file = File.Open(Application.persistentDataPath + "/highscoreInfo.dat", FileMode.Open);
            bf.Serialize(file, container);
            file.Close();
        }


    }

    private void Load()
    { 
        if (File.Exists(Application.persistentDataPath + "/highscoreInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/highscoreinfo.dat", FileMode.Open);

            HighscoreContainer data = (HighscoreContainer)bf.Deserialize(file);
            file.Close();

            container.highscoreListData = data.highscoreListData;
        }
    }

    private void Sort()
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
