using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HideAndSeek
{
    public enum UI_STRING { FRONT_TITLE, FRONT_WARNING, FRONT_BUTTON,
    LOBBY_TITLE, LOBBY_BUTTON_SHOVEL, LOBBY_BUTTON_PURCHASE, LOBBY_TIME_REMAIN, PURCHASE_TITLE, PURCHASE_BTN, PURCHASE_NOTICE, RETURN_BTN, ENTER_BTN, ENTER_SHOP_BTN,
    GOTO_LOBBY_BTN, GOTO_MAP_BTN, SHOP_TITLE, SHOP_DISPLAY, SHOP_BAG
    }

    public enum GAME_STRING { ROT, NO_SHOVEL, GEM_START, LIMIT_SHOVEL, GET_A_SHOVEL, WAIT, DAMAGE_TIME, FLOOR_SHOW_ALL, FLOOR_SHOW_ITEM, FLOOR_SHOW_NEAR, FLOOR_SHOW_MONSTER, FLOOR_SHOW_TRAP,
        INC_MAXHP, LIMIT_MAXHP, INC_BAG, LIMIT_BAG, NO_SPACE_BAG, FOUND_ITEM, LACK_GEM, LACK_TIME, HIDE_BROKEN_DAMAGE, HIDE_BROKEN_USE, HIDE_BROKEN_FLOOR, HIDE_BROKEN_BUMP, HIDE_BROKEN_GEM,
        HIDE_BROKEN_PICK_ITEM };

    public enum DUNGEON_STRING { TUTORIAL, RUIN, TOMB, MAZE, HELLGATE, CHAMBER, INFO_NEED_SHOVEL, INFO_FREE_ENTER, INFO_TIME_LIMIT, INFO_NUM_CHAMBER, INFO_NUM_SHOVEL, INFO_NUM_CHALLANGE, INFO_NUM_CLEAR,
      CLEAR_CHAMBER, CLEAR, FAIL,
      RESULT_GET_GEM, RESULT_GET_ITEM, RESULT_USE_ITEM, RESULT_BUY_ITEM, RESULT_SELL_ITEM, RESULT_DAMAGED_MONSTER, RESULT_DAMAGED_TRAP, RESULT_DAMAGED_TIME, RESULT_DESTROY_MONSTER, RESULT_DESTROY_TRAP,
      CHAMBER_INFO_SKELETON, CHAMBER_INFO_SKELETON2, CHAMBER_INFO_TRAP, CHAMBER_INFO_GEM, CHAMBER_INFO_TILE_NEAR, CHAMBER_INFO_TILE_MONSTER, CHAMBER_INFO_TILE_TRAP, CHAMBER_INFO_TILE_ITEM,
      CHAMBER_INFO_TILE_ALL, CHAMBER_INFO_TILE_INFO
    };

    public class LocalizationManager : MonoBehaviour
    {        
        public static LocalizationManager instance = null;

        public Dictionary<DUNGEON_STRING, Dictionary<SystemLanguage, string>> dungeonStrings = new Dictionary<DUNGEON_STRING, Dictionary<SystemLanguage, string>>();
        
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
				locallanguage = Application.systemLanguage;
//				locallanguage = SystemLanguage.English;              
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

        public string GetDungeonString(DUNGEON_STRING did)
        {
            return dungeonStrings[did][locallanguage];
        }

        void SetupDungeonStrings()
        {
            dungeonStrings[DUNGEON_STRING.TUTORIAL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시험의 던전"},
                {SystemLanguage.English,  "DUNGEON OF THE TEST"},
            };

            dungeonStrings[DUNGEON_STRING.RUIN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "고대 유적지"},
                {SystemLanguage.English,  "ANCIENT RUIN"},
            };

            dungeonStrings[DUNGEON_STRING.TOMB] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "왕의 무덤"},
                {SystemLanguage.English,  "KING'S TOMB"},
            };

            dungeonStrings[DUNGEON_STRING.MAZE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "신비한 미로"},
                {SystemLanguage.English,  "MAZE"},
            };

            dungeonStrings[DUNGEON_STRING.HELLGATE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "지옥의 입구"},
                {SystemLanguage.English,  "HELL GATE"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "방"},
                {SystemLanguage.English,  "Chamber"},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NEED_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "입장하는데 1개의 도굴삽이 필요"},
                {SystemLanguage.English,  "I must have a shovel to enter."},
            };

            dungeonStrings[DUNGEON_STRING.INFO_FREE_ENTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "무료 입장"},
                {SystemLanguage.English,  "Free admission"},
            };

            dungeonStrings[DUNGEON_STRING.INFO_TIME_LIMIT] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "제한시간: "},
                {SystemLanguage.English,  "Time limit: "},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NUM_CHAMBER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "방수: "},
                {SystemLanguage.English,  "Number of Chambers: "},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NUM_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "보유 도굴삽: "},
                {SystemLanguage.English,  "Number of shovels I have: "},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NUM_CHALLANGE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "도전 회수 : "},
                {SystemLanguage.English,  "Number of challenge: "},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NUM_CLEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "정복 회수 : "},
                {SystemLanguage.English,  "Number of clear: "},
            };

            dungeonStrings[DUNGEON_STRING.CLEAR_CHAMBER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "탐색 완료!"},
                {SystemLanguage.English,  "CLEAR!"},
            };

            dungeonStrings[DUNGEON_STRING.CLEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  " 정복!"},
                {SystemLanguage.English,  " CLEAR!"},
            };

            dungeonStrings[DUNGEON_STRING.FAIL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  " 실패!"},
                {SystemLanguage.English,  "FAIL!"},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_GET_GEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "획득한 보석 : "},
                {SystemLanguage.English,  "Count of gems I picked up :"},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_GET_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "획득한 아이템 수 : "},
                {SystemLanguage.English,  "Count of items I picked up : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_USE_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "사용한 아이템 수 : "},
                {SystemLanguage.English,  "Count of items I used : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_BUY_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "구입한 아이템 수 : "},
                {SystemLanguage.English,  "Count of items I bought : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_SELL_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "판매한 아이템 수 : "},
                {SystemLanguage.English,  "Count of items I sold : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DAMAGED_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "괴물에 의한 피해 : "},
                {SystemLanguage.English,  "Damage Points by Monsters : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DAMAGED_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "함정에 의한 피해 : "},
                {SystemLanguage.English,  "Damage Points by Traps : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DAMAGED_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "시간에 부족에의한 피해 : " },
                {SystemLanguage.English,  "Damage Points by Out of time : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DESTROY_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "제거한 괴물 수 : " },
                {SystemLanguage.English,  "Count of Monsters that destroied : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DESTROY_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "제거한 함정 수 : " },
                {SystemLanguage.English,  "Count of Traps that destroied : "},
            };
                        
            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_SKELETON] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "해골 : " },
                {SystemLanguage.English,  "SKELETON : "},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_SKELETON2] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "해골 LV2 :" },
                {SystemLanguage.English,  "SKELETON LV2 : "},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "함정 수 :" },
                {SystemLanguage.English,  "Number of Traps : "},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_GEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "보석 수 :" },
                {SystemLanguage.English,  "Number of Gems : "},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_NEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "근처보기" },
                {SystemLanguage.English,  "VEIW NEAR"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "함정보기" },
                {SystemLanguage.English,  "VIEW TRAPS"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "괴물보기" },
                {SystemLanguage.English,  "VIEW MONSTERS"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "아이템보기" },
                {SystemLanguage.English,  "VIEW ITEMS"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_ALL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "전체보기" },
                {SystemLanguage.English,  "VIEW ALL"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_INFO] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "특수 바닥 정보" },
                {SystemLanguage.English,  "Inforamtion of speacial floors"},
            };
        }

        void SetupUIStrings()
        {
            uiStrings[UI_STRING.FRONT_TITLE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "DUNGEON MASTER\n던전 마스터"},
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
                {SystemLanguage.Korean,  "던전목록"},
                {SystemLanguage.English,  "DUNGEON LIST"},
            };

            uiStrings[UI_STRING.LOBBY_BUTTON_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "삽받기"},
                {SystemLanguage.English,  "GET"},
            };

            uiStrings[UI_STRING.LOBBY_BUTTON_PURCHASE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "구입"},
                {SystemLanguage.English,  "BUY"},
            };

            uiStrings[UI_STRING.LOBBY_TIME_REMAIN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "다음 획득 기회까지 남은시간 : "},
                {SystemLanguage.English,  "Time remaining until next chance : "},
            };

            uiStrings[UI_STRING.PURCHASE_TITLE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "도굴삽 구입"},
                {SystemLanguage.English,  "PURCHASE SHOVELS"},
            };

            uiStrings[UI_STRING.PURCHASE_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "구입"},
                {SystemLanguage.English,  "BUY"},
            };

            uiStrings[UI_STRING.PURCHASE_NOTICE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "앱을 삭제 하면\n구입 내역도 같이 삭제 되니\n주의하세요!!!"},
                {SystemLanguage.English,  "If you delet this application,\nyour purchase history will \nbe deleted! Be careful!"},
            };

            uiStrings[UI_STRING.RETURN_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "뒤로"},
                {SystemLanguage.English,  "BACK"},
            };

            uiStrings[UI_STRING.ENTER_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "입장"},
                {SystemLanguage.English,  "ENTER"},
            };

            uiStrings[UI_STRING.ENTER_SHOP_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "상점"},
                {SystemLanguage.English,  "SHOP"},
            };

            uiStrings[UI_STRING.GOTO_LOBBY_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "던전목록으로 돌아가기"},
                {SystemLanguage.English,  "GO TO THE DUNGEON LIST"},
            };

            uiStrings[UI_STRING.GOTO_MAP_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "던전 지도로 돌아가기"},
                {SystemLanguage.English,  "GO TO THE DUNGEONMAP"},
            };

            uiStrings[UI_STRING.SHOP_TITLE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "상점"},
                {SystemLanguage.English,  "SHOP"},
            };

            uiStrings[UI_STRING.SHOP_DISPLAY] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "상품목록"},
                {SystemLanguage.English,  "DISPLAY"},
            };

            uiStrings[UI_STRING.SHOP_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "가방"},
                {SystemLanguage.English,  "BAG"},
            };
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

            contents[GAME_STRING.GET_A_SHOVEL] = new Dictionary<SystemLanguage, string>()
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