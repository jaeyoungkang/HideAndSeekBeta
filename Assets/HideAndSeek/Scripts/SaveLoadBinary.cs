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
        public bool isClearTutorial = false;
        public int bagSize = 3;
        public int invenSize = 4;
        
        public int invenGem = 0;
    }

    public static class SaveLoad
    {
        public static List<GameInfo> savedGames = new List<GameInfo>();

        public static void Save()
        {
            savedGames.Add(GameManager.instance.info);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/games2.gd");
            bf.Serialize(file, SaveLoad.savedGames);
            file.Close();
        }

        public static void Load()
        {
            if (File.Exists(Application.persistentDataPath + "/games2.gd"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/games2.gd", FileMode.Open);
                SaveLoad.savedGames = (List<GameInfo>)bf.Deserialize(file);
                file.Close();
                if(SaveLoad.savedGames.Count>0)
                {
                    GameManager.instance.info = SaveLoad.savedGames[0];
                }
            }
        }

        public static void SaveFile()
        {
            string fileName = "/savedGames.gd";
            //if (File.Exists(fileName))
            //{
            //    Debug.Log(fileName + " already exists.");
            //    return;
            //}
            var sr = File.CreateText(fileName);
            sr.WriteLine(GameManager.instance.info.isClearTutorial);
            sr.Close();
        }

        public static void LoadFile()
        {
            string fileName = "/savedGames.gd";
            if (File.Exists(fileName))
            {
                var sr = File.OpenText(fileName);
                string line = sr.ReadLine();
                while (line != null)
                {
                    GameManager.instance.info.isClearTutorial = Convert.ToBoolean(line);
                    Debug.Log(line); // prints each line of the file
                    line = sr.ReadLine();
                }
            }
            else
            {
                Debug.Log("Could not Open the file: " + fileName + " for reading.");
                return;
            }
        }
    }
}