using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HideAndSeek
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager instance = null;

        public enum LOCAL { KOR, ENG };

        public Dictionary<LOCAL, string[]> contents = new Dictionary<LOCAL, string[]>();
        public LOCAL loc = LOCAL.KOR;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                SetupContents();
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }
        

        void SetupContents()
        {
            string[] kor = { "무언가 썩고있는 냄새가 난다..." };
            string[] eng = { "Something smells rotten." };

            contents[LOCAL.KOR] = kor;
            contents[LOCAL.ENG] = eng;
        }


        public string GetLocalString(int index)
        {
            return contents[loc][index];
        }
    }
}