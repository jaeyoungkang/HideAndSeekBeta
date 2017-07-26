using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace HideAndSeek
{
    [System.Serializable]
    public class GameInfo
    {
        public List<int> dungeonIdsOpened = new List<int>();
        public bool purchaseUser = false;
        public int enableCount = 10;

        public Dictionary<int, int> dungeonTryCount = new Dictionary<int, int>();
        public Dictionary<int, int> dungeonClearCount = new Dictionary<int, int>();

        //public Dictionary<int, int> dungeonGetGemsCount = new Dictionary<int, int>();

        //public Dictionary<int, int> dungeonGetItemsCount = new Dictionary<int, int>();
        //public Dictionary<int, int> dungeonUseItemsCount = new Dictionary<int, int>();
        //public Dictionary<int, int> dungeonBuyItemsCount = new Dictionary<int, int>();
        //public Dictionary<int, int> dungeonSellItemsCount = new Dictionary<int, int>();

        //public Dictionary<int, int> dungeonDestoryTrapCount = new Dictionary<int, int>();
        //public Dictionary<int, int> dungeonDestoryEnemyCount = new Dictionary<int, int>();
        //public Dictionary<int, int> dungeonDamagedByEnemyCount = new Dictionary<int, int>();
        //public Dictionary<int, int> dungeonDamagedByTrapCount = new Dictionary<int, int>();
        //public Dictionary<int, int> dungeonDamagedByTimeCount = new Dictionary<int, int>();
    }

    public static class SaveLoad
    {
        public static string fileName2 = "/timerecord.gd";
        public static string fileName = "/dm.gd";
        public static List<GameInfo> savedGames = new List<GameInfo>();

        public static void SaveTime()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + fileName2);
            bf.Serialize(file, GameManager.instance.preGenTime);
            file.Close();
        }

        public static void LoadTime()
        {
            if (File.Exists(Application.persistentDataPath + fileName2))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + fileName2, FileMode.Open);
                GameManager.instance.preGenTime = (DateTime)bf.Deserialize(file);
                file.Close();
            }
        }

        public static void Save()
        {
            savedGames.Add(GameManager.instance.info);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + fileName);
            bf.Serialize(file, SaveLoad.savedGames);
            file.Close();
        }

        public static void Load()
        {
            if (File.Exists(Application.persistentDataPath + fileName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
                SaveLoad.savedGames = (List<GameInfo>)bf.Deserialize(file);
                file.Close();
                if(SaveLoad.savedGames.Count>0)
                {
                    GameManager.instance.info = SaveLoad.savedGames[0];
                }
            }
        }

        public static void WriteFile(string fileName, string info)
        {
            if (File.Exists(fileName))
            {
                File.AppendAllText(fileName, info + Environment.NewLine);
                return;
            }

            var sr = File.CreateText(fileName);
            sr.WriteLine(info);
            sr.Close();
        }
    }
}