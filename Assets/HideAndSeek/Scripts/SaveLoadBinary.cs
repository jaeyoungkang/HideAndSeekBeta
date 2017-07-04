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
    }

    public static class SaveLoad
    {
        public static string fileName = "/games14.gd";
        public static List<GameInfo> savedGames = new List<GameInfo>();

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