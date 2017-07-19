using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HideAndSeek
{
    public enum GAME_STRING { ROT, NO_SHOVEL, GEM_START,
        LIMIT_SHOVEL,
        GET_SHOVEL,
        WAIT,
        DAMAGE_TIME,
        FLOOR_SHOW_ALL,
        FLOOR_SHOW_ITEM,
        FLOOR_SHOW_NEAR,
        FLOOR_SHOW_MONSTER,
        FLOOR_SHOW_TRAP,
        INC_MAXHP,
        LIMIT_MAXHP,
        INC_BAG,
        LIMIT_BAG,
        NO_SPACE_BAG,
        FOUND_ITEM,
        LACK_GEM,
        LACK_TIME,
        HIDE_BROKEN_DAMAGE,
        HIDE_BROKEN_USE,
        HIDE_BROKEN_FLOOR,
        HIDE_BROKEN_BUMP,
        HIDE_BROKEN_GEM,
        HIDE_BROKEN_PICK_ITEM,
    };
    public class LocalizationManager : MonoBehaviour
    {        
        public static LocalizationManager instance = null;

        public Dictionary<GAME_STRING, Dictionary<SystemLanguage, string>> contents = new Dictionary<GAME_STRING, Dictionary<SystemLanguage, string>>();
        public Dictionary<ITEM_ID, Dictionary<SystemLanguage, string>> itemNames = new Dictionary<ITEM_ID, Dictionary<SystemLanguage, string>>();
        public Dictionary<ITEM_ID, Dictionary<SystemLanguage, string>> itemDiscriptions = new Dictionary<ITEM_ID, Dictionary<SystemLanguage, string>>();        

        public SystemLanguage locallanguage;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
//                locallanguage = Application.systemLanguage;
                locallanguage = SystemLanguage.English;
                SetupContents();
                SetupItemContents();
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public string GetLocalString(GAME_STRING gs)
        {
            return contents[gs][locallanguage];
        }

        public string GetItemName(ITEM_ID itemId)
        {
            return itemNames[itemId][locallanguage];
        }

        public string GetItemDiscription(ITEM_ID itemId)
        {
            return itemDiscriptions[itemId][locallanguage];
        }

        void SetupItemContents()
        {
            itemNames[ITEM_ID.HEAL1] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "회복1"},
                {SystemLanguage.English,  "heal1"},
            };

            itemNames[ITEM_ID.HEAL2] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "회복2"},
                {SystemLanguage.English,  "heal2"},
            };

            itemNames[ITEM_ID.HEAL3] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "회복3"},
                {SystemLanguage.English,  "heal3"},
            };

            itemNames[ITEM_ID.FRAGMENT_NEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(주변)"},
                {SystemLanguage.English,  "a fragment of view around "},
            };

            itemNames[ITEM_ID.FRAGMENT_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(괴물)"},
                {SystemLanguage.English,  "a fragment of view monsters"},
            };

            itemNames[ITEM_ID.FRAGMENT_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(함정)"},
                {SystemLanguage.English,  "a fragment of view traps"},
            };

            itemNames[ITEM_ID.FRAGMENT_ALL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(전체)"},
                {SystemLanguage.English,  "a fragment of view all"},
            };

            itemNames[ITEM_ID.FRAGMENT_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(보물)"},
                {SystemLanguage.English,  "a fragment of view items"},
            };

            itemNames[ITEM_ID.DESTROY_4D] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "파괴 스크롤 (4방향)"},
                {SystemLanguage.English,  "a destroy scroll (4 way)"},
            };

            itemNames[ITEM_ID.DESTROY_LR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "파괴 스크롤 (좌우방향)"},
                {SystemLanguage.English, "a destroy scroll (left, right side)"},
            };

            itemNames[ITEM_ID.DESTROY_UD] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "파괴 스크롤 (위아래방향)"},
                {SystemLanguage.English,  "a destroy scroll (up, down side)"},
            };

            itemNames[ITEM_ID.ADD_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "시간추가 30초"},
                {SystemLanguage.English, "a time shift (30sce)"},
            };

            itemNames[ITEM_ID.STOP_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "시간멈춤"},
                {SystemLanguage.English, "a time stop"},
            };

            itemNames[ITEM_ID.HIDE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "두건"},
                {SystemLanguage.English, "a hood"},
            };

            itemNames[ITEM_ID.EXTEND_MAX_HP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "보호핼멧"},
                {SystemLanguage.English, "a helm"},
            };

            itemNames[ITEM_ID.EXTEND_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "가방"},
                {SystemLanguage.English, "a bag"},
            };

            itemNames[ITEM_ID.ESCAPE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "탈출 티켓"},
                {SystemLanguage.English, "a ticket of escape"},
            };
        }

        void SetupContents()
        {
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

            contents[GAME_STRING.LIMIT_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "이미 도굴삽이 5개 이상일때는 더 받을 수 없다..."},
                {SystemLanguage.English,  "You can not get more when you have 5 or more shovels ..."},
            };

            contents[GAME_STRING.GET_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "도굴삽을 하나 구했다!"},
                {SystemLanguage.English,  "I got a shovel!"},
            };

            contents[GAME_STRING.WAIT] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "더 기다려야한다..."},
                {SystemLanguage.English,  "I have to wait ..."},
            };

            contents[GAME_STRING.DAMAGE_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시간이 다되어서 피해를 입었다..."},
                {SystemLanguage.English,  "I was out of time and I was damaged ...."},
            };

            contents[GAME_STRING.FLOOR_SHOW_ALL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "검은색 바닥을 밟으니 방 전체가 보인다."},
                {SystemLanguage.English,  "I stepped on the black floor and the whole chamber is visible."},
            };

            contents[GAME_STRING.FLOOR_SHOW_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "노란색 바닥을 밟으니 아이템들이 보인다."},
                {SystemLanguage.English,  "I stepped on the yellow floor and the Items in the chamber is visible."},
            };

            contents[GAME_STRING.FLOOR_SHOW_NEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "초록색 바닥을 밟으니 주변이 보인다."},
                {SystemLanguage.English,  "I stepped on the green floor and around is visible."},
            };

            contents[GAME_STRING.FLOOR_SHOW_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "빨간색 바닥을 밟으니 괴물들이 보인다."},
                {SystemLanguage.English,  "I stepped on the red floor and the Monsters in the chamber is visible."},
            };

            contents[GAME_STRING.FLOOR_SHOW_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "파란색 바닥을 밟으니 함정들이 보인다."},
                {SystemLanguage.English,  "I stepped on the blue floor and the Traps in the chamber is visible."},
            };

            contents[GAME_STRING.INC_MAXHP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "최대 체력이 늘어났다."},
                {SystemLanguage.English,  "Maximum HP increased."},
            };

            contents[GAME_STRING.LIMIT_MAXHP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "이미 최대 체력이다."},
                {SystemLanguage.English,  "It is already maximum HP."},
            };

            contents[GAME_STRING.INC_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "가방 공간이 늘어났다."},
                {SystemLanguage.English,  "The bag space increased."},
            };

            contents[GAME_STRING.LIMIT_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "이미 최대치이다..."},
                {SystemLanguage.English,  "It is already maximum..."},
            };

            contents[GAME_STRING.NO_SPACE_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "가방에 공간이 부족하다."},
                {SystemLanguage.English,  "There is not enough space in the bag."},
            };

            contents[GAME_STRING.FOUND_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "{0} 을 발견했다."},
                {SystemLanguage.English,  "I found the {0}."},
            };

            contents[GAME_STRING.LACK_GEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "보석이 부족합니다."},
                {SystemLanguage.English,  "There is not enough gem."},
            };

            contents[GAME_STRING.LACK_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시간이 부족하다..."},
                {SystemLanguage.English,  "Time is running out ..."},
            };

            contents[GAME_STRING.HIDE_BROKEN_DAMAGE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "피해를 입어서 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by damage."},
            };

            contents[GAME_STRING.HIDE_BROKEN_USE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "아이템 사용하는 소리에 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by using sound of the item."},
            };

            contents[GAME_STRING.HIDE_BROKEN_FLOOR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "특수 바닥을 밟아서 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by stepping on the speacial floor."},
            };

            contents[GAME_STRING.HIDE_BROKEN_BUMP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "무언가에 부딪혀서  은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by runnig into something."},
            };

            contents[GAME_STRING.HIDE_BROKEN_GEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "보석을 줍는 소리에 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by getting sound of the gem."},
            };

            contents[GAME_STRING.HIDE_BROKEN_PICK_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "아이템을 줍는 소리에 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by getting sound of the item."},
            };

            //contents[GAME_STRING.] = new Dictionary<SystemLanguage, string>()
            //{
            //    {SystemLanguage.Korean,  ""},
            //    {SystemLanguage.English,  ""},
            //};
        }
    }
}