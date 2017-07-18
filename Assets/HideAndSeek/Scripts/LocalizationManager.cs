using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HideAndSeek
{
    public enum GAME_STRING { ROT, NO_SHOVEL, GEM_START };
    public class LocalizationManager : MonoBehaviour
    {        
        public static LocalizationManager instance = null;

        public Dictionary<GAME_STRING, Dictionary<SystemLanguage, string>> contents = new Dictionary<GAME_STRING, Dictionary<SystemLanguage, string>>();
        public SystemLanguage locallanguage;

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
            locallanguage = Application.systemLanguage;
            print(Application.systemLanguage);
            print(Application.systemLanguage.ToString());

            contents[GAME_STRING.ROT] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "무언가 썩고있는 냄새가 난다..."},
                {SystemLanguage.English,  "Something smells rotten..."},
            };

            contents[GAME_STRING.NO_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "도굴 삽 없이는 들어 갈 수 없다..."},
                {SystemLanguage.English,  "I can not enter without a shovel..."},
            };

            contents[GAME_STRING.GEM_START] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "{0}개의 보석을 가지고 시작한다."},
                {SystemLanguage.English,  "Start with {0} gem(s)."},
            };
        }


        public string GetLocalString(GAME_STRING gs)
        {
            return contents[gs][locallanguage];
        }
    }
}