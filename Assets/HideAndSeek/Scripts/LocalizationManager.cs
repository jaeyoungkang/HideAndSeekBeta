using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HideAndSeek
{
    public enum UI_STRING { FRONT_TITLE, FRONT_WARNING, FRONT_BUTTON,
    LOBBY_TITLE, LOBBY_BUTTON_AD, LOBBY_BUTTON_PURCHASE, LOBBY_TIME_REMAIN }

    public enum GAME_STRING { ROT, NO_SHOVEL, GEM_START, LIMIT_SHOVEL, GET_SHOVEL, WAIT, DAMAGE_TIME, FLOOR_SHOW_ALL, FLOOR_SHOW_ITEM, FLOOR_SHOW_NEAR, FLOOR_SHOW_MONSTER, FLOOR_SHOW_TRAP,
        INC_MAXHP, LIMIT_MAXHP, INC_BAG, LIMIT_BAG, NO_SPACE_BAG, FOUND_ITEM, LACK_GEM, LACK_TIME, HIDE_BROKEN_DAMAGE, HIDE_BROKEN_USE, HIDE_BROKEN_FLOOR, HIDE_BROKEN_BUMP, HIDE_BROKEN_GEM,
        HIDE_BROKEN_PICK_ITEM };

    public class LocalizationManager : MonoBehaviour
    {        
        public static LocalizationManager instance = null;

        public Dictionary<DUNGEON_ID, Dictionary<SystemLanguage, string>> dungeonStrings = new Dictionary<DUNGEON_ID, Dictionary<SystemLanguage, string>>();
        
        public Dictionary<UI_STRING, Dictionary<SystemLanguage, string>> uiStrings = new Dictionary<UI_STRING, Dictionary<SystemLanguage, string>>();

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
                SetupUIStrings();
                SetupDungeonStrings();
                SetupItemStrings();
                SetupItemDiscriptionStrings();
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

        public string GetLocalUIString(UI_STRING us)
        {
            return uiStrings[us][locallanguage];
        }

        public string GetDungeonString(DUNGEON_ID did)
        {
            return dungeonStrings[did][locallanguage];
        }

        void SetupDungeonStrings()
        {
            dungeonStrings[DUNGEON_ID.TUTORIAL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시험의 던전"},
                {SystemLanguage.English,  "DUNGEON OF THE TEST"},
            };

            dungeonStrings[DUNGEON_ID.RUIN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "고대 유적지"},
                {SystemLanguage.English,  "ANCIENT RUIN"},
            };

            dungeonStrings[DUNGEON_ID.TOMB] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "왕의 무덤"},
                {SystemLanguage.English,  "KING'S TOMB"},
            };

            dungeonStrings[DUNGEON_ID.MAZE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "신비한 미로"},
                {SystemLanguage.English,  "MAZE"},
            };

            dungeonStrings[DUNGEON_ID.HELLGATE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "지옥의 입구"},
                {SystemLanguage.English,  "HELL GATE"},
            };
        }

        void SetupUIStrings()
        {
            uiStrings[UI_STRING.FRONT_TITLE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "던전 마스터"},
                {SystemLanguage.English,  "DUNGEON MASTER"},
            };

            uiStrings[UI_STRING.FRONT_WARNING] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "※ 주의!\n본 게임은 클라이언트 기반의 게임입니다.\n게임을 삭제하면 게임기록도 같이 삭제됩니다."},
                {SystemLanguage.English,  "※ Warning!\nThis game is based on the client,\n If you delete this game application, your game history is also deleted."},
            };

            uiStrings[UI_STRING.FRONT_BUTTON] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시작"},
                {SystemLanguage.English,  "START"},
            };

            uiStrings[UI_STRING.LOBBY_TITLE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "로비"},
                {SystemLanguage.English,  "LOBBY"},
            };

            uiStrings[UI_STRING.LOBBY_BUTTON_AD] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "광고"},
                {SystemLanguage.English,  "AD"},
            };


            uiStrings[UI_STRING.LOBBY_BUTTON_PURCHASE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "구입"},
                {SystemLanguage.English,  "PURCHASE"},
            };

            uiStrings[UI_STRING.LOBBY_TIME_REMAIN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "다음 획득 기회까지 남은시간 : "},
                {SystemLanguage.English,  "Time remaining until next chance : "},
            };

            //uiStrings[UI_STRING] = new Dictionary<SystemLanguage, string>()
            //{
            //    {SystemLanguage.Korean,  ""},
            //    {SystemLanguage.English,  ""},
            //};
        }

        void SetupItemStrings()
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

        void SetupItemDiscriptionStrings()
        {
            itemDiscriptions[ITEM_ID.HEAL1] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "HP를 1칸 회복한다."},
                {SystemLanguage.English,  "Recover HP 1 point."},
            };

            itemDiscriptions[ITEM_ID.HEAL2] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "HP를 2칸 회복한다."},
                {SystemLanguage.English,  "Recover HP 2 point."},
            };

            itemDiscriptions[ITEM_ID.HEAL3] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "HP를 3칸 회복한다."},
                {SystemLanguage.English,  "Recover HP 3 point."},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_NEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "주위 3칸반경을 밝힌다."},
                {SystemLanguage.English,  "Can view around(3blocks)"},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "전 영역의 괴물의 위치를 본다. "},
                {SystemLanguage.English,  "Can view monsters"},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "전 영역의 함정의 위치를 본다."},
                {SystemLanguage.English,  "Can view traps"},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_ALL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "전체를 본다."},
                {SystemLanguage.English,  "Can veiw all"},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "전 영역에 있는 보석이나 아이템을 본다. "},
                {SystemLanguage.English,  "Can view items and gems"},
            };

            itemDiscriptions[ITEM_ID.DESTROY_4D] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "상하좌우 1칸의 괴물이나 함정을 제거한다."},
                {SystemLanguage.English,  "Destory monsters or traps from up, down, left, and right."},
            };

            itemDiscriptions[ITEM_ID.DESTROY_LR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "좌우 2칸씩 괴물이나 함정을 제거한다."},
                {SystemLanguage.English, "Destory monsters or traps from left, and right. (two blocks)"},
            };

            itemDiscriptions[ITEM_ID.DESTROY_UD] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "상하 2칸씩 괴물이나 함정을 제거한다."},
                {SystemLanguage.English,  "Destory monsters or traps from up, and down. (two blocks)"},
            };

            itemDiscriptions[ITEM_ID.ADD_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "제한시간이 30초 늘어난다."},
                {SystemLanguage.English, "The time limit is increased by 30 seconds."},
            };

            itemDiscriptions[ITEM_ID.STOP_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "움직이지 않을동안 시간을 멈춘다."},
                {SystemLanguage.English, "The time stop while not moving."},
            };

            itemDiscriptions[ITEM_ID.HIDE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "적이 나를 못본다. 하지만 쉽게 풀린다!"},
                {SystemLanguage.English, "it makes Monster can't find me, but it is broken easily"},
            };

            itemDiscriptions[ITEM_ID.EXTEND_MAX_HP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "최대 HP가 1칸 늘어난다."},
                {SystemLanguage.English, "Increas maximum HP  1 point."},
            };

            itemDiscriptions[ITEM_ID.EXTEND_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "가방 슬롯이 1칸 늘어난다. "},
                {SystemLanguage.English, "Increas one space in the bag"},
            };

            itemDiscriptions[ITEM_ID.ESCAPE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "방을 탈출해서 방선택화면으로 이동."},
                {SystemLanguage.English, "Escape to the chamber select phase."},
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