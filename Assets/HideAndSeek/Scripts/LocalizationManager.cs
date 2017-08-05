using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HideAndSeek
{
    public enum UI_STRING { FRONT_TITLE, FRONT_WARNING, FRONT_BUTTON,
    LOBBY_TITLE, LOBBY_BUTTON_SHOVEL, LOBBY_BUTTON_PURCHASE, LOBBY_TIME_REMAIN, PURCHASE_TITLE, PURCHASE_BTN, PURCHASE_NOTICE, RETURN_BTN, ENTER_BTN, ENTER_SHOP_BTN,
    GOTO_LOBBY_BTN, GOTO_MAP_BTN, SHOP_TITLE, SHOP_DISPLAY, SHOP_BAG, LEAVE_DUNGEON, ITEM_HELP, YES_BTN, NO_BTN, QUIT
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
                //                locallanguage = SystemLanguage.Japanese;
                locallanguage = Application.systemLanguage;
                if (locallanguage != SystemLanguage.Korean && locallanguage != SystemLanguage.Japanese &&
                    locallanguage != SystemLanguage.Chinese && locallanguage != SystemLanguage.ChineseSimplified && locallanguage != SystemLanguage.ChineseTraditional)
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
                {SystemLanguage.Chinese,  "测试室"},
                {SystemLanguage.ChineseSimplified,  "测试室"},
                {SystemLanguage.ChineseTraditional,  "測試室"},
                {SystemLanguage.Japanese,  "試験のダンジョン"},

            };

            dungeonStrings[DUNGEON_STRING.RUIN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "고대 유적지"},
                {SystemLanguage.English,  "ANCIENT RUIN"},
                {SystemLanguage.Chinese,  "古代叛乱"},
                {SystemLanguage.ChineseSimplified,  "古代叛乱"},
                {SystemLanguage.ChineseTraditional,  "古代叛亂"},
                {SystemLanguage.Japanese,  "古代遺跡"},
            };

            dungeonStrings[DUNGEON_STRING.TOMB] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "왕의 무덤"},
                {SystemLanguage.English,  "KING'S TOMB"},
                {SystemLanguage.Chinese,  "国王的汤姆"},
                {SystemLanguage.ChineseSimplified,  "国王的汤姆"},
                {SystemLanguage.ChineseTraditional,  "國王的湯姆"},
                {SystemLanguage.Japanese,  "王の墓"},
            };

            dungeonStrings[DUNGEON_STRING.MAZE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "신비한 미로"},
                {SystemLanguage.English,  "MAZE"},
                {SystemLanguage.Chinese,  "迷宫"},
                {SystemLanguage.ChineseSimplified,  "迷宫"},
                {SystemLanguage.ChineseTraditional,  "迷宮"},
                {SystemLanguage.Japanese,  "不思議な迷路"},
            };

            dungeonStrings[DUNGEON_STRING.HELLGATE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "지옥의 입구"},
                {SystemLanguage.English,  "HELL GATE"},
                {SystemLanguage.Chinese,  "地狱的入口"},
                {SystemLanguage.ChineseSimplified,  "地狱的入口"},
                {SystemLanguage.ChineseTraditional,  "地獄的入口"},
                {SystemLanguage.Japanese,  "地獄の入り口"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "방"},
                {SystemLanguage.English,  "Chamber"},
                {SystemLanguage.Chinese,  "房间"},
                {SystemLanguage.ChineseSimplified,  "房间"},
                {SystemLanguage.ChineseTraditional,  "房間"},
                {SystemLanguage.Japanese,  "部屋"},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NEED_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "입장하는데 1개의 도굴삽이 필요."},
                {SystemLanguage.English,  "I must have a shovel to enter."},
                {SystemLanguage.Chinese,  "我必须有一把铲子进入."},
                {SystemLanguage.ChineseSimplified,  "我必须有一把铲子进入."},
                {SystemLanguage.ChineseTraditional,  "我必須有一把鏟子進入."},
                {SystemLanguage.Japanese,  "入場するのに1つのシャベルが必要."},
            };

            dungeonStrings[DUNGEON_STRING.INFO_FREE_ENTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "무료 입장"},
                {SystemLanguage.English,  "Free admission"},
                {SystemLanguage.Chinese,  "免费入场"},
                {SystemLanguage.ChineseSimplified,  "免费入场"},
                {SystemLanguage.ChineseTraditional,  "免費入場"},
                {SystemLanguage.Japanese,  "入場無料"},
            };

            dungeonStrings[DUNGEON_STRING.INFO_TIME_LIMIT] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "제한시간: "},
                {SystemLanguage.English,  "Time limit: "},
                {SystemLanguage.Chinese,  "时限: "},
                {SystemLanguage.ChineseSimplified,  "时限: "},
                {SystemLanguage.ChineseTraditional,  "時限: "},
                {SystemLanguage.Japanese,  "制限時間: "},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NUM_CHAMBER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "방수: "},
                {SystemLanguage.English,  "Number of Chambers: "},
                {SystemLanguage.Chinese,  "房间数量: "},
                {SystemLanguage.ChineseSimplified,  "房间数量: "},
                {SystemLanguage.ChineseTraditional,  "房間數量: "},
                {SystemLanguage.Japanese,  "部屋の数: "},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NUM_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "보유 도굴삽: "},
                {SystemLanguage.English,  "Number of shovels I have: "},
                {SystemLanguage.Chinese,  "铲数量: "},
                {SystemLanguage.ChineseSimplified,  "铲数量: "},
                {SystemLanguage.ChineseTraditional,  "鏟數量: "},
                {SystemLanguage.Japanese,  "保有盗掘シャベル: "},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NUM_CHALLANGE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "도전 횟수 : "},
                {SystemLanguage.English,  "Number of challenge: "},
                {SystemLanguage.Chinese,  "挑战数量: "},
                {SystemLanguage.ChineseSimplified,  "挑战数量: "},
                {SystemLanguage.ChineseTraditional,  "挑戰數量: "},
                {SystemLanguage.Japanese,  "挑戦回数: "},
            };

            dungeonStrings[DUNGEON_STRING.INFO_NUM_CLEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "정복 횟수 : "},
                {SystemLanguage.English,  "Number of clear: "},
                {SystemLanguage.Chinese,  "征服数量: "},
                {SystemLanguage.ChineseSimplified,  "征服数量: "},
                {SystemLanguage.ChineseTraditional,  "征服數量: "},
                {SystemLanguage.Japanese,  "征服回収: "},
            };

            dungeonStrings[DUNGEON_STRING.CLEAR_CHAMBER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "탐색 완료!"},
                {SystemLanguage.English,  "CLEAR!"},
                {SystemLanguage.Chinese,  "完整的导航!"},
                {SystemLanguage.ChineseSimplified,  "完整的导航!"},
                {SystemLanguage.ChineseTraditional,  "完整的導航!"},
                {SystemLanguage.Japanese,  "ナビゲーション完了!"},
            };

            dungeonStrings[DUNGEON_STRING.CLEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "정복!"},
                {SystemLanguage.English,  "CLEAR!"},
                {SystemLanguage.Chinese,  "征服!"},
                {SystemLanguage.ChineseSimplified,  "征服!"},
                {SystemLanguage.ChineseTraditional,  "征服!"},
                {SystemLanguage.Japanese,  "征服!"},
            };

            dungeonStrings[DUNGEON_STRING.FAIL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "실패!"},
                {SystemLanguage.English,  "FAIL!"},
                {SystemLanguage.Chinese,  "失败!"},
                {SystemLanguage.ChineseSimplified,  "失败!"},
                {SystemLanguage.ChineseTraditional,  "失敗!"},
                {SystemLanguage.Japanese,  "失敗!"},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_GET_GEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "획득한 보석 : "},
                {SystemLanguage.English,  "Count of gems I picked up :"},
                {SystemLanguage.Chinese,  "宝贝的数量我拿起来 : "},
                {SystemLanguage.ChineseSimplified,  "宝贝的数量我拿起来 : "},
                {SystemLanguage.ChineseTraditional,  "寶貝的數量我拿起來 : "},
                {SystemLanguage.Japanese,  "獲得した宝石 : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_GET_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "획득한 아이템 수 : "},
                {SystemLanguage.English,  "Count of items I picked up : "},
                {SystemLanguage.Chinese,  "我拿起的物品数量 : "},
                {SystemLanguage.ChineseSimplified,  "我拿起的物品数量 : "},
                {SystemLanguage.ChineseTraditional,  "我拿起的物品數量 : :"},
                {SystemLanguage.Japanese,  "獲得したアイテム数 : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_USE_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "사용한 아이템 수 : "},
                {SystemLanguage.English,  "Count of items I used : "},
                {SystemLanguage.Chinese,  "我使用的项目数量 : "},
                {SystemLanguage.ChineseSimplified,  "我使用的项目数量 : "},
                {SystemLanguage.ChineseTraditional,  "我使用的項目數量 : "},
                {SystemLanguage.Japanese,  "使用したアイテム数 : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_BUY_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "구입한 아이템 수 : "},
                {SystemLanguage.English,  "Count of items I bought : "},
                {SystemLanguage.Chinese,  "我买的物品数量 : "},
                {SystemLanguage.ChineseSimplified,  "我买的物品数量 : "},
                {SystemLanguage.ChineseTraditional,  "我買的物品數量 : "},
                {SystemLanguage.Japanese,  "購入したアイテム数 : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_SELL_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "판매한 아이템 수 : "},
                {SystemLanguage.English,  "Count of items I sold : "},
                {SystemLanguage.Chinese,  "我出售的物品数量 : "},
                {SystemLanguage.ChineseSimplified,  "我出售的物品数量 : "},
                {SystemLanguage.ChineseTraditional,  "我出售的物品數量 : "},
                {SystemLanguage.Japanese,  "販売したアイテム数 : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DAMAGED_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "괴물에 의한 피해 : "},
                {SystemLanguage.English,  "Damage Points by Monsters : "},
                {SystemLanguage.Chinese,  "怪物伤害点 : "},
                {SystemLanguage.ChineseSimplified,  "怪物伤害点 : "},
                {SystemLanguage.ChineseTraditional,  "怪物傷害點 : "},
                {SystemLanguage.Japanese,  "モンスターによる被害 : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DAMAGED_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "함정에 의한 피해 : "},
                {SystemLanguage.English,  "Damage Points by Traps : "},
                {SystemLanguage.Chinese,  "到陷阱接收损坏 : "},
                {SystemLanguage.ChineseSimplified,  "到陷阱接收损坏 : "},
                {SystemLanguage.ChineseTraditional,  "到陷阱接收損壞 : "},
                {SystemLanguage.Japanese,  "トラップによる被害 : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DAMAGED_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "시간에 부족에 의한 피해 : " },
                {SystemLanguage.English,  "Damage Points by Out of time : "},
                {SystemLanguage.Chinese,  "伤害点不合时间 : "},
                {SystemLanguage.ChineseSimplified,  "伤害点不合时间 : "},
                {SystemLanguage.ChineseTraditional,  "傷害點不合時間 : "},
                {SystemLanguage.Japanese,  "時間に不足の被害 : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DESTROY_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "제거한 괴물 수 : " },
                {SystemLanguage.English,  "Count of Monsters was destroyed : "},
                {SystemLanguage.Chinese,  "怪兽的数量被摧毁了 : "},
                {SystemLanguage.ChineseSimplified,  "怪兽的数量被摧毁了 : "},
                {SystemLanguage.ChineseTraditional,  "怪獸的數量被摧毀了 : "},
                {SystemLanguage.Japanese,  "削除モンスター数 : "},
            };

            dungeonStrings[DUNGEON_STRING.RESULT_DESTROY_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "제거한 함정 수 : " },
                {SystemLanguage.English,  "Count of Traps was destroyed : "},
                {SystemLanguage.Chinese,  "陷阱数量被摧毁 : "},
                {SystemLanguage.ChineseSimplified,  "陷阱数量被摧毁 : "},
                {SystemLanguage.ChineseTraditional,  "陷阱數量被摧毀 : "},
                {SystemLanguage.Japanese,  "削除トラップすることができ ： "},
            };
                        
            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_SKELETON] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "해골 : " },
                {SystemLanguage.English,  "SKELETON : "},
                {SystemLanguage.Chinese,  "骨架 : "},
                {SystemLanguage.ChineseSimplified,  "骨架 : "},
                {SystemLanguage.ChineseTraditional,  "骨架 : "},
                {SystemLanguage.Japanese,  "スケルトン : "},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_SKELETON2] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "해골 LV2 : " },
                {SystemLanguage.English,  "SKELETON LV2 : "},
                {SystemLanguage.Chinese,  "骨架 LV2 : "},
                {SystemLanguage.ChineseSimplified,  "骨架 LV2 : "},
                {SystemLanguage.ChineseTraditional,  "骨架 LV2 : "},
                {SystemLanguage.Japanese,  "スケルトン LV2 : "},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "함정 수 : " },
                {SystemLanguage.English,  "Number of Traps : "},
                {SystemLanguage.Chinese,  "陷阱数量 : "},
                {SystemLanguage.ChineseSimplified,  "陷阱数量 : "},
                {SystemLanguage.ChineseTraditional,  "陷阱數量 : "},
                {SystemLanguage.Japanese,  "トラップすることができ : "},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_GEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "보석 수 :" },
                {SystemLanguage.English, "Number of Gems : "},
                {SystemLanguage.Chinese, "宝石数量 :"},
                {SystemLanguage.ChineseSimplified, "宝石数量 :"},
                {SystemLanguage.ChineseTraditional, "寶石數量 :"},
                {SystemLanguage.Japanese, "宝石本数 :"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_NEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "근처보기" },
                {SystemLanguage.English, "VEIW NEAR"},
                {SystemLanguage.Chinese, "查看附近"},
                {SystemLanguage.ChineseSimplified, "查看附近"},
                {SystemLanguage.ChineseTraditional, "查看附近"},
                {SystemLanguage.Japanese, "近くを見る"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "함정보기" },
                {SystemLanguage.English,  "VIEW TRAPS"},
                {SystemLanguage.Chinese,  "见陷阱位置"},
                {SystemLanguage.ChineseSimplified,  "见陷阱位置"},
                {SystemLanguage.ChineseTraditional,  "見陷阱位置"},
                {SystemLanguage.Japanese,  "トラップ位置を表示"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "괴물보기" },
                {SystemLanguage.English,  "VIEW MONSTERS"},
                {SystemLanguage.Chinese,  "查看怪物的位置"},
                {SystemLanguage.ChineseSimplified,  "查看怪物的位置"},
                {SystemLanguage.ChineseTraditional,  "查看怪物的位置"},
                {SystemLanguage.Japanese,  "モンスターの位置を表示"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "아이템보기" },
                {SystemLanguage.English,  "VIEW ITEMS"},
                {SystemLanguage.Chinese,  "见财宝的位置"},
                {SystemLanguage.ChineseSimplified,  "见财宝的位置"},
                {SystemLanguage.ChineseTraditional,  "見財寶的位置"},
                {SystemLanguage.Japanese,  "宝の場所を表示"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_ALL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "전체보기" },
                {SystemLanguage.English,  "VIEW ALL"},
                {SystemLanguage.Chinese,  "查看全部"},
                {SystemLanguage.ChineseSimplified,  "查看全部"},
                {SystemLanguage.ChineseTraditional,  "查看全部"},
                {SystemLanguage.Japanese,  "一覧"},
            };

            dungeonStrings[DUNGEON_STRING.CHAMBER_INFO_TILE_INFO] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "특수 바닥 정보" },
                {SystemLanguage.English,  "Information of speacial floors"},
                {SystemLanguage.Chinese,  "楼层信息"},
                {SystemLanguage.ChineseSimplified,  "楼层信息"},
                {SystemLanguage.ChineseTraditional,  "樓層信息"},
                {SystemLanguage.Japanese,  "特殊床情報"},
            };            
        }

        void SetupUIStrings()
        {
            uiStrings[UI_STRING.FRONT_TITLE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "DUNGEON MASTER\n던전 마스터"},
                {SystemLanguage.English,  "DUNGEON MASTER"},
                {SystemLanguage.Chinese,  "DUNGEON MASTER\n地下城征服"},
                {SystemLanguage.ChineseSimplified,  "DUNGEON MASTER\n地下城征服"},
                {SystemLanguage.ChineseTraditional,  "DUNGEON MASTER\n地下城征服"},
                {SystemLanguage.Japanese,  "DUNGEON MASTER\nダンジョンマスター"},
            };

            uiStrings[UI_STRING.FRONT_WARNING] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "※ 주의!\n본 게임은 클라이언트 기반의 게임입니다.\n게임을 삭제하면 게임기록도 같이 삭제됩니다."},
                {SystemLanguage.English,  "※ Warning!\nThis game is based on the client,\n If you delete this game application, your game history is also deleted."},
                {SystemLanguage.Chinese,  "※ 警告！\n这个游戏是基于客户端的。如果您删除此游戏应用程序，\n您的游戏历史记录也将被删除."},
                {SystemLanguage.ChineseSimplified,  "※ 警告！\n这个游戏是基于客户端的。如果您删除此游戏应用程序，\n您的游戏历史记录也将被删除."},
                {SystemLanguage.ChineseTraditional,  "※ 警告！\n這個遊戲是基於客戶端的。如果您刪除此遊戲應用程序，\n您的遊戲歷史記錄也將被刪除."},
                {SystemLanguage.Japanese,  "※ 注意！\n本ゲームは、クライアントベースのゲームです.\nゲームを削除すると、ゲームの記録も一緒に削除されます"},
            };

            uiStrings[UI_STRING.FRONT_BUTTON] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시작"},
                {SystemLanguage.English,  "START"},
                {SystemLanguage.Chinese,  "开始"},
                {SystemLanguage.ChineseSimplified,  "开始"},
                {SystemLanguage.ChineseTraditional,  "開始"},
                {SystemLanguage.Japanese,  "開始"},
            };

            uiStrings[UI_STRING.LOBBY_TITLE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "던전목록"},
                {SystemLanguage.English,  "DUNGEON LIST"},
                {SystemLanguage.Chinese,  "地下城列表"},
                {SystemLanguage.ChineseSimplified,  "地下城列表"},
                {SystemLanguage.ChineseTraditional,  "地下城列表"},
                {SystemLanguage.Japanese,  "ダンジョンリスト"},
            };

            uiStrings[UI_STRING.LOBBY_BUTTON_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "삽받기"},
                {SystemLanguage.English,  "GET"},
                {SystemLanguage.Chinese,  "得到"},
                {SystemLanguage.ChineseSimplified,  "得到"},
                {SystemLanguage.ChineseTraditional,  "得到"},
                {SystemLanguage.Japanese,  "シャベル"},
            };

            uiStrings[UI_STRING.LOBBY_BUTTON_PURCHASE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "구입"},
                {SystemLanguage.English,  "BUY"},
                {SystemLanguage.Chinese,  "购买"},
                {SystemLanguage.ChineseSimplified,  "购买"},
                {SystemLanguage.ChineseTraditional,  "購買"},
                {SystemLanguage.Japanese,  "購入"},
            };

            uiStrings[UI_STRING.LOBBY_TIME_REMAIN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "새로운 도굴삽 획득까지 남은시간 : "},
                {SystemLanguage.English,  "Time remaining until get a new shovel : "},
                {SystemLanguage.Chinese,  "剩下时间，直到得到一个新的铲子 : "},
                {SystemLanguage.ChineseSimplified,  "剩下时间，直到得到一个新的铲子 : "},
                {SystemLanguage.ChineseTraditional,  "剩下時間，直到得到一個新的鏟子 : "},
                {SystemLanguage.Japanese,  "新しい盗掘シャベル獲得までの残り時間 : "},
            };

            uiStrings[UI_STRING.PURCHASE_TITLE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "도굴삽 구입"},
                {SystemLanguage.English,  "PURCHASE SHOVELS"},
                {SystemLanguage.Chinese,  "购买鞋"},
                {SystemLanguage.ChineseSimplified,  "购买鞋"},
                {SystemLanguage.ChineseTraditional,  "購買鞋"},
                {SystemLanguage.Japanese,  "盗掘シャベルを購入"},
            };

            uiStrings[UI_STRING.PURCHASE_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "구입"},
                {SystemLanguage.English,  "BUY"},
                {SystemLanguage.Chinese,  "购买"},
                {SystemLanguage.ChineseSimplified,  "购买"},
                {SystemLanguage.ChineseTraditional,  "購買"},
                {SystemLanguage.Japanese,  "購入"},
            };

            uiStrings[UI_STRING.PURCHASE_NOTICE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "앱을 삭제 하면\n구입 내역도 같이 삭제 되니\n주의하세요!!!"},
                {SystemLanguage.English,  "If you delet this application,\nyour purchase history will \nbe deleted! Be careful!"},
                {SystemLanguage.Chinese,  "如果您删除此应用程序，\n您的购买记录将被删除！\n小心！!!"},
                {SystemLanguage.ChineseSimplified,  "如果您删除此应用程序，\n您的购买记录将被删除！\n小心！!!"},
                {SystemLanguage.ChineseTraditional,  "如果您刪除此應用程序，\n您的購買記錄將被刪除！\n小心！!!"},
                {SystemLanguage.Japanese,  "アプリを削除すると、\n購入履歴も一緒に削除されます.\n注意してください！!!"},
            };

            uiStrings[UI_STRING.RETURN_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "뒤로"},
                {SystemLanguage.English,  "BACK"},
                {SystemLanguage.Chinese,  "背部"},
                {SystemLanguage.ChineseSimplified,  "背部"},
                {SystemLanguage.ChineseTraditional,  "背部"},
                {SystemLanguage.Japanese,  "バック"},
            };

            uiStrings[UI_STRING.ENTER_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "입장"},
                {SystemLanguage.English,  "ENTER"},
                {SystemLanguage.Chinese,  "输入"},
                {SystemLanguage.ChineseSimplified,  "输入"},
                {SystemLanguage.ChineseTraditional,  "輸入"},
                {SystemLanguage.Japanese,  "入場"},
            };

            uiStrings[UI_STRING.ENTER_SHOP_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "상점"},
                {SystemLanguage.English,  "SHOP"},
                {SystemLanguage.Chinese,  "店"},
                {SystemLanguage.ChineseSimplified,  "店"},
                {SystemLanguage.ChineseTraditional,  "店"},
                {SystemLanguage.Japanese,  "店"},
            };

            uiStrings[UI_STRING.GOTO_LOBBY_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "던전목록으로 돌아가기"},
                {SystemLanguage.English,  "GO TO THE DUNGEON LIST"},
                {SystemLanguage.Chinese,  "去DUNGEON列表"},
                {SystemLanguage.ChineseSimplified,  "去DUNGEON列表"},
                {SystemLanguage.ChineseTraditional,  "去DUNGEON列表"},
                {SystemLanguage.Japanese,  "ダンジョンリストに戻る"},
            };

            uiStrings[UI_STRING.GOTO_MAP_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "던전 지도로 돌아가기"},
                {SystemLanguage.English,  "GO TO THE DUNGEONMAP"},
                {SystemLanguage.Chinese,  "去地下城的地图"},
                {SystemLanguage.ChineseSimplified,  "去地下城的地图"},
                {SystemLanguage.ChineseTraditional,  "去地下城的地圖"},
                {SystemLanguage.Japanese,  "ダンジョンマップに戻る"},
            };

            uiStrings[UI_STRING.SHOP_TITLE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "상점"},
                {SystemLanguage.English,  "SHOP"},
                {SystemLanguage.Chinese,  "店"},
                {SystemLanguage.ChineseSimplified,  "店"},
                {SystemLanguage.ChineseTraditional,  "店"},
                {SystemLanguage.Japanese,  "店"},
            };

            uiStrings[UI_STRING.SHOP_DISPLAY] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "상품목록"},
                {SystemLanguage.English,  "DISPLAY"},
                {SystemLanguage.Chinese,  "显示"},
                {SystemLanguage.ChineseSimplified,  "显示"},
                {SystemLanguage.ChineseTraditional,  "顯示"},
                {SystemLanguage.Japanese,  "商品リスト"},
            };

            uiStrings[UI_STRING.SHOP_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "가방"},
                {SystemLanguage.English,  "BAG"},
                {SystemLanguage.Chinese,  "袋"},
                {SystemLanguage.ChineseSimplified,  "袋"},
                {SystemLanguage.ChineseTraditional,  "袋"},
                {SystemLanguage.Japanese,  "バッグ"},
            };

            uiStrings[UI_STRING.LEAVE_DUNGEON] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "탐험포기"},
                {SystemLanguage.English,  "Abandon"},
                {SystemLanguage.Chinese,  "放弃"},
                {SystemLanguage.ChineseSimplified,  "放弃"},
                {SystemLanguage.ChineseTraditional,  "放棄"},
                {SystemLanguage.Japanese,  "探索放棄"},
            };

            uiStrings[UI_STRING.ITEM_HELP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "아이템 정보"},
                {SystemLanguage.English,  "Item infoamtion"},
                {SystemLanguage.Chinese,  "物品信息"},
                {SystemLanguage.ChineseSimplified,  "物品信息"},
                {SystemLanguage.ChineseTraditional,  "物品信息"},
                {SystemLanguage.Japanese,  "アイテム情報"},
            };

            uiStrings[UI_STRING.YES_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "네"},
                {SystemLanguage.English,  "YES"},
                {SystemLanguage.Chinese,  "是"},
                {SystemLanguage.ChineseSimplified,  "是"},
                {SystemLanguage.ChineseTraditional,  "是"},
                {SystemLanguage.Japanese,  "はい"},
            };

            uiStrings[UI_STRING.NO_BTN] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "아니오"},
                {SystemLanguage.English,  "NO"},
                {SystemLanguage.Chinese,  "没有"},
                {SystemLanguage.ChineseSimplified,  "没有"},
                {SystemLanguage.ChineseTraditional,  "沒有"},
                {SystemLanguage.Japanese,  "いいえ"},
            };
            
            uiStrings[UI_STRING.QUIT] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "게임을 종료하시겠습니까?"},
                {SystemLanguage.English,  "Are you sure you want to quit the game?"},
                {SystemLanguage.Chinese,  "你确定要退出游戏吗？"},
                {SystemLanguage.ChineseSimplified,  "你确定要退出游戏吗？"},
                {SystemLanguage.ChineseTraditional,  "你確定要退出遊戲嗎？"},
                {SystemLanguage.Japanese,  "ゲームを終了しますか？"},
            };
        }

        void SetupItemStrings()
        {
            itemNames[ITEM_ID.HEAL1] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "회복1"},
                {SystemLanguage.English,  "heal1"},
                {SystemLanguage.Chinese,  "治愈1分"},
                {SystemLanguage.ChineseSimplified,  "治愈1分"},
                {SystemLanguage.ChineseTraditional,  "治愈1分"},
                {SystemLanguage.Japanese,  "回復1"},
            };

            itemNames[ITEM_ID.HEAL2] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "회복2"},
                {SystemLanguage.English,  "heal2"},
                {SystemLanguage.Chinese,  "治愈2分"},
                {SystemLanguage.ChineseSimplified,  "治愈2分"},
                {SystemLanguage.ChineseTraditional,  "治愈2分"},
                {SystemLanguage.Japanese,  "回復2"},
            };

            itemNames[ITEM_ID.HEAL3] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "회복3"},
                {SystemLanguage.English,  "heal3"},
                {SystemLanguage.Chinese,  "治愈3分"},
                {SystemLanguage.ChineseSimplified,  "治愈3分"},
                {SystemLanguage.ChineseTraditional,  "治愈3分"},
                {SystemLanguage.Japanese,  "回復3"},
            };

            itemNames[ITEM_ID.FRAGMENT_NEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(주변)"},
                {SystemLanguage.English,  "a fragment of view around "},
                {SystemLanguage.Chinese,  "视线周围的片段"},
                {SystemLanguage.ChineseSimplified,  "视线周围的片段"},
                {SystemLanguage.ChineseTraditional,  "視線周圍的片段"},
                {SystemLanguage.Japanese,  "視野破片（周辺）"},
            };

            itemNames[ITEM_ID.FRAGMENT_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(괴물)"},
                {SystemLanguage.English,  "a fragment of view monsters"},
                {SystemLanguage.Chinese,  "视图怪物片段"},
                {SystemLanguage.ChineseSimplified,  "视图怪物片段"},
                {SystemLanguage.ChineseTraditional,  "視圖怪物片段"},
                {SystemLanguage.Japanese,  "視野破片（モンスター）"},
            };

            itemNames[ITEM_ID.FRAGMENT_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(함정)"},
                {SystemLanguage.English,  "a fragment of view traps"},
                {SystemLanguage.Chinese,  "视图陷阱的片段"},
                {SystemLanguage.ChineseSimplified,  "视图陷阱的片段"},
                {SystemLanguage.ChineseTraditional,  "視圖陷阱的片段"},
                {SystemLanguage.Japanese,  "視野破片（トラップ）"},
            };

            itemNames[ITEM_ID.FRAGMENT_ALL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(전체)"},
                {SystemLanguage.English,  "a fragment of view all"},
                {SystemLanguage.Chinese,  "视图全部"},
                {SystemLanguage.ChineseSimplified,  "视图全部"},
                {SystemLanguage.ChineseTraditional,  "視圖全部"},
                {SystemLanguage.Japanese,  "視野破片（全）"},
            };

            itemNames[ITEM_ID.FRAGMENT_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시야파편(보물)"},
                {SystemLanguage.English,  "a fragment of view items"},
                {SystemLanguage.Chinese,  "视图项目片段"},
                {SystemLanguage.ChineseSimplified,  "视图项目片段"},
                {SystemLanguage.ChineseTraditional,  "視圖項目片段"},
                {SystemLanguage.Japanese,  "視野破片（宝）"},
            };

            itemNames[ITEM_ID.DESTROY_4D] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "파괴 스크롤 (4방향)"},
                {SystemLanguage.English,  "a destroy scroll (4 way)"},
                {SystemLanguage.Chinese,  "破坏滚动（4方向）"},
                {SystemLanguage.ChineseSimplified,  "破坏滚动（4方向）"},
                {SystemLanguage.ChineseTraditional,  "破壞滾動（4方向）"},
                {SystemLanguage.Japanese,  "破壊スクロール（4方向）"},
            };

            itemNames[ITEM_ID.DESTROY_LR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "파괴 스크롤 (좌우방향)"},
                {SystemLanguage.English, "a destroy scroll (left, right side)"},
                {SystemLanguage.Chinese,  "破坏滚动（左，右）"},
                {SystemLanguage.ChineseSimplified,  "破坏滚动（左，右）"},
                {SystemLanguage.ChineseTraditional,  "破壞滾動（左，右）"},
                {SystemLanguage.Japanese,  "破壊スクロール（左右方向）"},
            };

            itemNames[ITEM_ID.DESTROY_UD] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "파괴 스크롤 (위아래방향)"},
                {SystemLanguage.English,  "a destroy scroll (up, down side)"},
                {SystemLanguage.Chinese,  "破坏滚动（上，下）"},
                {SystemLanguage.ChineseSimplified,  "破坏滚动（上，下）"},
                {SystemLanguage.ChineseTraditional,  "破壞滾動（上，下）"},
                {SystemLanguage.Japanese,  "破壊スクロール（上下方向）"},
            };

            itemNames[ITEM_ID.ADD_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "시간추가 30초"},
                {SystemLanguage.English, "a time shift (30sce)"},
                {SystemLanguage.Chinese,  "添加时间30秒"},
                {SystemLanguage.ChineseSimplified,  "添加时间30秒"},
                {SystemLanguage.ChineseTraditional,  "添加時間30秒"},
                {SystemLanguage.Japanese,  "時間を追加30秒"},
            };

            itemNames[ITEM_ID.STOP_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "시간멈춤"},
                {SystemLanguage.English, "a time stop"},
                {SystemLanguage.Chinese,  "时间停止"},
                {SystemLanguage.ChineseSimplified,  "时间停止"},
                {SystemLanguage.ChineseTraditional,  "時間停止"},
                {SystemLanguage.Japanese,  "時間停止"},
            };

            itemNames[ITEM_ID.HIDE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "두건"},
                {SystemLanguage.English, "a hood"},
                {SystemLanguage.Chinese,  "引擎罩"},
                {SystemLanguage.ChineseSimplified,  "引擎罩"},
                {SystemLanguage.ChineseTraditional,  "引擎罩"},
                {SystemLanguage.Japanese,  "フード"},
            };

            itemNames[ITEM_ID.EXTEND_MAX_HP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "보호헬멧"},
                {SystemLanguage.English, "a helm"},
                {SystemLanguage.Chinese,  "舵手"},
                {SystemLanguage.ChineseSimplified,  "舵手"},
                {SystemLanguage.ChineseTraditional,  "舵手"},
                {SystemLanguage.Japanese,  "ヘルメット"},
            };

            itemNames[ITEM_ID.EXTEND_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "가방"},
                {SystemLanguage.English, "a bag"},
                {SystemLanguage.Chinese,  "袋"},
                {SystemLanguage.ChineseSimplified,  "袋"},
                {SystemLanguage.ChineseTraditional,  "袋"},
                {SystemLanguage.Japanese,  "バッグ"},
            };

            itemNames[ITEM_ID.ESCAPE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "탈출 티켓"},
                {SystemLanguage.English, "a ticket of escape"},
                {SystemLanguage.Chinese,  "逃生单"},
                {SystemLanguage.ChineseSimplified,  "逃生单"},
                {SystemLanguage.ChineseTraditional,  "逃生單"},
                {SystemLanguage.Japanese,  "脱出チケット"},
            };
        }

        void SetupItemDiscriptionStrings()
        {
            itemDiscriptions[ITEM_ID.HEAL1] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "HP를 1칸 회복한다."},
                {SystemLanguage.English,  "Recover HP 1 point."},
                {SystemLanguage.Chinese,  "恢复HP 1点."},
                {SystemLanguage.ChineseSimplified,  "恢复HP 1点."},
                {SystemLanguage.ChineseTraditional,  "恢復HP 1點."},
                {SystemLanguage.Japanese,  "HPを1カーン回復する."},
            };

            itemDiscriptions[ITEM_ID.HEAL2] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "HP를 2칸 회복한다."},
                {SystemLanguage.English,  "Recover HP 2 point."},
                {SystemLanguage.Chinese,  "恢复HP 2点."},
                {SystemLanguage.ChineseSimplified,  "恢复HP 2点."},
                {SystemLanguage.ChineseTraditional,  "恢復HP 2點."},
                {SystemLanguage.Japanese,  "HPを2カーン回復する."},
            };

            itemDiscriptions[ITEM_ID.HEAL3] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "HP를 3칸 회복한다."},
                {SystemLanguage.English,  "Recover HP 3 point."},
                {SystemLanguage.Chinese,  "恢复HP 3点."},
                {SystemLanguage.ChineseSimplified,  "恢复HP 3点."},
                {SystemLanguage.ChineseTraditional,  "恢復HP 3點."},
                {SystemLanguage.Japanese,  "HPを3カーン回復する."},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_NEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "주위 3칸반경을 밝힌다."},
                {SystemLanguage.English,  "Can view around.(3blocks)"},
                {SystemLanguage.Chinese,  "可以查看.（三个街区）"},
                {SystemLanguage.ChineseSimplified,  "可以查看.（三个街区）"},
                {SystemLanguage.ChineseTraditional,  "可以查看.（三個街區）"},
                {SystemLanguage.Japanese,  "周辺3間の半径を言う."},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "전 영역의 괴물의 위치를 본다. "},
                {SystemLanguage.English,  "Can view monsters."},
                {SystemLanguage.Chinese,  "可以查看怪物."},
                {SystemLanguage.ChineseSimplified,  "可以查看怪物."},
                {SystemLanguage.ChineseTraditional,  "可以查看怪物."},
                {SystemLanguage.Japanese,  "全領域のモンスターの位置を見る."},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "전 영역의 함정의 위치를 본다."},
                {SystemLanguage.English,  "Can view traps."},
                {SystemLanguage.Chinese,  "可以查看陷阱."},
                {SystemLanguage.ChineseSimplified,  "可以查看陷阱."},
                {SystemLanguage.ChineseTraditional,  "可以查看陷阱."},
                {SystemLanguage.Japanese,  "全領域のトラップの位置を見る."},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_ALL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "전체를 본다."},
                {SystemLanguage.English,  "Can veiw all."},
                {SystemLanguage.Chinese,  "可以查看全部."},
                {SystemLanguage.ChineseSimplified,  "可以查看全部."},
                {SystemLanguage.ChineseTraditional,  "可以查看全部."},
                {SystemLanguage.Japanese,  "全体を見る."},
            };

            itemDiscriptions[ITEM_ID.FRAGMENT_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "전 영역에 있는 보석이나 아이템을 본다. "},
                {SystemLanguage.English,  "Can view items and gems."},
                {SystemLanguage.Chinese,  "可以查看项目和宝石."},
                {SystemLanguage.ChineseSimplified,  "可以查看项目和宝石."},
                {SystemLanguage.ChineseTraditional,  "可以查看項目和寶石."},
                {SystemLanguage.Japanese,  "全領域の宝石やアイテムを見る."},
            };

            itemDiscriptions[ITEM_ID.DESTROY_4D] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "상하좌우 1칸의 괴물이나 함정을 제거한다."},
                {SystemLanguage.English,  "Destory monsters or traps from the four side.(a block)"},
                {SystemLanguage.Chinese,  "来自四面的毁灭性怪物或陷阱.（一块）"},
                {SystemLanguage.ChineseSimplified,  "来自四面的毁灭性怪物或陷阱.（一块）"},
                {SystemLanguage.ChineseTraditional,  "來自四面的毀滅性怪物或陷阱.（一塊）"},
                {SystemLanguage.Japanese,  "上下左右1マスのモンスターやトラップを削除する."},
            };

            itemDiscriptions[ITEM_ID.DESTROY_LR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "좌우 2칸씩 괴물이나 함정을 제거한다."},
                {SystemLanguage.English, "Destory monsters or traps from left, right.(two blocks)"},
                {SystemLanguage.Chinese,  "来自左，右的破坏怪物或陷阱.（两块）"},
                {SystemLanguage.ChineseSimplified,  "来自左，右的破坏怪物或陷阱.（两块）"},
                {SystemLanguage.ChineseTraditional,  "來自左，右的破壞怪物或陷阱.（兩塊）"},
                {SystemLanguage.Japanese,  "左右2ずつモンスターやトラップを削除する."},
            };

            itemDiscriptions[ITEM_ID.DESTROY_UD] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "상하 2칸씩 괴물이나 함정을 제거한다."},
                {SystemLanguage.English,  "Destory monsters or traps from up, down.(two blocks)"},
                {SystemLanguage.Chinese,  "破坏怪物或陷阱从上，下.（两块）"},
                {SystemLanguage.ChineseSimplified,  "破坏怪物或陷阱从上，下.（两块）"},
                {SystemLanguage.ChineseTraditional,  "破壞怪物或陷阱從上，下.（兩塊）"},
                {SystemLanguage.Japanese,  "上下2ずつモンスターやトラップを削除する."},
            };

            itemDiscriptions[ITEM_ID.ADD_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "제한시간이 30초 늘어난다."},
                {SystemLanguage.English, "The time limit is increased by 30 seconds."},
                {SystemLanguage.Chinese,  "时间限制增加30秒."},
                {SystemLanguage.ChineseSimplified,  "时间限制增加30秒."},
                {SystemLanguage.ChineseTraditional,  "時間限制增加30秒."},
                {SystemLanguage.Japanese,  "制限時間が30秒増える."},
            };

            itemDiscriptions[ITEM_ID.STOP_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "움직이지 않을동안 시간을 멈춘다."},
                {SystemLanguage.English, "The time stop while not moving."},
                {SystemLanguage.Chinese,  "时间停止，而不移动."},
                {SystemLanguage.ChineseSimplified,  "时间停止，而不移动."},
                {SystemLanguage.ChineseTraditional,  "時間停止，而不移動."},
                {SystemLanguage.Japanese,  "動かない間の時間を停止する."},
            };

            itemDiscriptions[ITEM_ID.HIDE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "적이 나를 못본다. 하지만 쉽게 풀린다!"},
                {SystemLanguage.English, "it makes Monster can't find me, but it is broken easily!"},
                {SystemLanguage.Chinese,  "怪物不能找到我，但它容易破碎!"},
                {SystemLanguage.ChineseSimplified,  "怪物不能找到我，但它容易破碎!"},
                {SystemLanguage.ChineseTraditional,  "怪物不能找到我，但它容易破碎!"},
                {SystemLanguage.Japanese,  "こと私ない見る。しかし、簡単にロック解除！"},
            };

            itemDiscriptions[ITEM_ID.EXTEND_MAX_HP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "최대 HP가 1칸 늘어난다."},
                {SystemLanguage.English, "Increase maximum HP  1 point."},
                {SystemLanguage.Chinese,  "增加最大HP 1点."},
                {SystemLanguage.ChineseSimplified,  "增加最大HP 1点."},
                {SystemLanguage.ChineseTraditional,  "增加最大HP 1點."},
                {SystemLanguage.Japanese,  "最大HPが1マス増える."},
            };

            itemDiscriptions[ITEM_ID.EXTEND_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "가방 슬롯이 1칸 늘어난다. "},
                {SystemLanguage.English, "Increase one space in the bag"},
                {SystemLanguage.Chinese,  "增加袋中的一个空间."},
                {SystemLanguage.ChineseSimplified,  "增加袋中的一个空间."},
                {SystemLanguage.ChineseTraditional,  "增加袋中的一個空間."},
                {SystemLanguage.Japanese,  "バッグスロットが1マス増える."},
            };

            itemDiscriptions[ITEM_ID.ESCAPE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean, "방을 탈출해서 방선택화면으로 이동."},
                {SystemLanguage.English, "Escape to the chamber select phase."},
                {SystemLanguage.Chinese,  "逃离到室选择阶段."},
                {SystemLanguage.ChineseSimplified,  "逃离到室选择阶段."},
                {SystemLanguage.ChineseTraditional,  "逃離到室選擇階段."},
                {SystemLanguage.Japanese,  "部屋を脱出して部屋の選択画面に移動し."},
            };
        }

        void SetupContents()
        {
            contents[GAME_STRING.ROT] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "무언가 썩고있는 냄새가 난다..."},
                {SystemLanguage.English,  "Something smells rotten..."},
                {SystemLanguage.Chinese,  "闻起来很烂..."},
                {SystemLanguage.ChineseSimplified,  "闻起来很烂..."},
                {SystemLanguage.ChineseTraditional,  "聞起來很爛..."},
                {SystemLanguage.Japanese,  "何か腐っているにおいがする..."},
            };

            contents[GAME_STRING.NO_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "도굴 삽 없이는 들어 갈 수 없다..."},
                {SystemLanguage.English,  "I can not enter without a shovel..."},
                {SystemLanguage.Chinese,  "我不能没有铲子进入..."},
                {SystemLanguage.ChineseSimplified,  "我不能没有铲子进入..."},
                {SystemLanguage.ChineseTraditional,  "我不能沒有鏟子進入..."},
                {SystemLanguage.Japanese,  "盗掘シャベルなして行くことができない..."},
            };

            contents[GAME_STRING.GEM_START] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "{0}개의 보석을 가지고 시작한다."},
                {SystemLanguage.English,  "Start with {0} gem(s)."},
                {SystemLanguage.Chinese,  "从{0}宝石开始."},
                {SystemLanguage.ChineseSimplified,  "从{0}宝石开始."},
                {SystemLanguage.ChineseTraditional,  "從{0}寶石開始."},
                {SystemLanguage.Japanese,  "{0}つの宝石を持って開始する."},
            };

            contents[GAME_STRING.LIMIT_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "이미 도굴삽이 5개 이상일때는 더 받을 수 없다..."},
                {SystemLanguage.English,  "You can not get more when you have 5 or more shovels ..."},
                {SystemLanguage.Chinese,  "当你有5个或更多的铲子时，你不能得到更多的东西..."},
                {SystemLanguage.ChineseSimplified,  "当你有5个或更多的铲子时，你不能得到更多的东西..."},
                {SystemLanguage.ChineseTraditional,  "當你有5個或更多的鏟子時，你不能得到更多的東西..."},
                {SystemLanguage.Japanese,  "盗掘シャベルが5個以上 持っているときは、より受けることができない..."},
            };

            contents[GAME_STRING.GET_A_SHOVEL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "도굴삽을 하나 구했다!"},
                {SystemLanguage.English,  "I got a shovel!"},
                {SystemLanguage.Chinese,  ""},
                {SystemLanguage.ChineseSimplified,  ""},
                {SystemLanguage.ChineseTraditional,  ""},
                {SystemLanguage.Japanese,  ""},
            };

            contents[GAME_STRING.WAIT] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "더 기다려야한다..."},
                {SystemLanguage.English,  "I have to wait ..."},
                {SystemLanguage.Chinese,  "我不得不等待..."},
                {SystemLanguage.ChineseSimplified,  "我不得不等待..."},
                {SystemLanguage.ChineseTraditional,  "我不得不等待..."},
                {SystemLanguage.Japanese,  "さらに待たなければなら..."},
            };

            contents[GAME_STRING.DAMAGE_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시간이 다되어서 피해를 입었다..."},
                {SystemLanguage.English,  "I was out of time and I was damaged ..."},
                {SystemLanguage.Chinese,  "我没时间，我被损坏了..."},
                {SystemLanguage.ChineseSimplified,  "我没时间，我被损坏了..."},
                {SystemLanguage.ChineseTraditional,  "我沒時間，我被損壞了..."},
                {SystemLanguage.Japanese,  "時間がされて被害を受けた..."},
            };

            contents[GAME_STRING.FLOOR_SHOW_ALL] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "검은색 바닥을 밟으니 방 전체가 보인다."},
                {SystemLanguage.English,  "I stepped on the black floor and the whole chamber is visible."},
                {SystemLanguage.Chinese,  "我踩在黑色的地板上，整个房间都是可见的."},
                {SystemLanguage.ChineseSimplified,  "我踩在黑色的地板上，整个房间都是可见的."},
                {SystemLanguage.ChineseTraditional,  "我踩在黑色的地板上，整個房間都是可見的."},
                {SystemLanguage.Japanese,  "黒底を踏んだから部屋全体が見える."},
            };

            contents[GAME_STRING.FLOOR_SHOW_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "노란색 바닥을 밟으니 아이템들이 보인다."},
                {SystemLanguage.English,  "I stepped on the yellow floor and the Items in the chamber is visible."},
                {SystemLanguage.Chinese,  "我走在黄色的地板上，房间里的物品是可见的."},
                {SystemLanguage.ChineseSimplified,  "我走在黄色的地板上，房间里的物品是可见的."},
                {SystemLanguage.ChineseTraditional,  "我走在黃色的地板上，房間裡的物品是可見的."},
                {SystemLanguage.Japanese,  "黄色の床を踏んだからアイテムが見える."},
            };

            contents[GAME_STRING.FLOOR_SHOW_NEAR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "초록색 바닥을 밟으니 주변이 보인다."},
                {SystemLanguage.English,  "I stepped on the green floor and around is visible."},
                {SystemLanguage.Chinese,  "我踩在绿色的地板上，周围是可见的."},
                {SystemLanguage.ChineseSimplified,  "我踩在绿色的地板上，周围是可见的."},
                {SystemLanguage.ChineseTraditional,  "我踩在綠色的地板上，周圍是可見的."},
                {SystemLanguage.Japanese,  "緑の床を踏んだから周りが見える."},
            };

            contents[GAME_STRING.FLOOR_SHOW_MONSTER] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "빨간색 바닥을 밟으니 괴물들이 보인다."},
                {SystemLanguage.English,  "I stepped on the red floor and the Monsters in the chamber is visible."},
                {SystemLanguage.Chinese,  "我踩在红色的地板上，房间里的怪物是可见的."},
                {SystemLanguage.ChineseSimplified,  "我踩在红色的地板上，房间里的怪物是可见的."},
                {SystemLanguage.ChineseTraditional,  "我踩在紅色的地板上，房間裡的怪物是可見的."},
                {SystemLanguage.Japanese,  "赤底を踏んだからモンスターが見える."},
            };

            contents[GAME_STRING.FLOOR_SHOW_TRAP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "파란색 바닥을 밟으니 함정들이 보인다."},
                {SystemLanguage.English,  "I stepped on the blue floor and the Traps in the chamber is visible."},
                {SystemLanguage.Chinese,  "我踩在蓝色的地板上，房间里的陷阱是可见的."},
                {SystemLanguage.ChineseSimplified,  "我踩在蓝色的地板上，房间里的陷阱是可见的."},
                {SystemLanguage.ChineseTraditional,  "我踩在藍色的地板上，房間裡的陷阱是可見的."},
                {SystemLanguage.Japanese,  "青の床を踏んだからトラップが見える."},
            };

            contents[GAME_STRING.INC_MAXHP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "최대 체력이 늘어났다."},
                {SystemLanguage.English,  "Maximum HP increased."},
                {SystemLanguage.Chinese,  "最大HP增加."},
                {SystemLanguage.ChineseSimplified,  "最大HP增加."},
                {SystemLanguage.ChineseTraditional,  "最大HP增加."},
                {SystemLanguage.Japanese,  "最大体力が増えた."},
            };

            contents[GAME_STRING.LIMIT_MAXHP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "이미 최대 체력이다."},
                {SystemLanguage.English,  "It is already maximum HP."},
                {SystemLanguage.Chinese,  "它已经是最高HP了."},
                {SystemLanguage.ChineseSimplified,  "它已经是最高HP了."},
                {SystemLanguage.ChineseTraditional,  "它已經是最高HP了."},
                {SystemLanguage.Japanese,  "すでに最大体力である."},
            };

            contents[GAME_STRING.INC_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "가방 공간이 늘어났다."},
                {SystemLanguage.English,  "The bag space increased."},
                {SystemLanguage.Chinese,  "袋子空间增加."},
                {SystemLanguage.ChineseSimplified,  "袋子空间增加."},
                {SystemLanguage.ChineseTraditional,  "袋子空間增加."},
                {SystemLanguage.Japanese,  "バッグのスペースが増えた."},
            };

            contents[GAME_STRING.LIMIT_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "이미 최대치이다..."},
                {SystemLanguage.English,  "It is already maximum..."},
                {SystemLanguage.Chinese,  "已经是最大..."},
                {SystemLanguage.ChineseSimplified,  "已经是最大..."},
                {SystemLanguage.ChineseTraditional,  "已經是最大..."},
                {SystemLanguage.Japanese,  "既に最大値である..."},
            };

            contents[GAME_STRING.NO_SPACE_BAG] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "가방에 공간이 부족하다."},
                {SystemLanguage.English,  "There is not enough space in the bag."},
                {SystemLanguage.Chinese,  "包里没有足够的空间."},
                {SystemLanguage.ChineseSimplified,  "包里没有足够的空间."},
                {SystemLanguage.ChineseTraditional,  "包裡沒有足夠的空間."},
                {SystemLanguage.Japanese,  "バッグにスペースが不足している."},
            };

            contents[GAME_STRING.FOUND_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "{0} 을 발견했다."},
                {SystemLanguage.English,  "I found the {0}."},
                {SystemLanguage.Chinese,  "我找到了 {0}."},
                {SystemLanguage.ChineseSimplified,  "我找到了 {0}."},
                {SystemLanguage.ChineseTraditional,  "我找到了 {0}."},
                {SystemLanguage.Japanese,  "{0} を発見した."},
            };

            contents[GAME_STRING.LACK_GEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "보석이 부족합니다."},
                {SystemLanguage.English,  "There is not enough gem."},
                {SystemLanguage.Chinese,  "宝石还不够."},
                {SystemLanguage.ChineseSimplified,  "宝石还不够."},
                {SystemLanguage.ChineseTraditional,  "寶石還不夠."},
                {SystemLanguage.Japanese,  "宝石が不足します."},
            };

            contents[GAME_STRING.LACK_TIME] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "시간이 부족하다..."},
                {SystemLanguage.English,  "Time is running out ..."},
                {SystemLanguage.Chinese,  "时间不多了 ..."},
                {SystemLanguage.ChineseSimplified,  "时间不多了 ..."},
                {SystemLanguage.ChineseTraditional,  "時間不多了 ..."},
                {SystemLanguage.Japanese,  "時間が不足している..."},
            };

            contents[GAME_STRING.HIDE_BROKEN_DAMAGE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "피해를 입어서 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by damage..."},
                {SystemLanguage.Chinese,  "隐藏效果被损坏破坏..."},
                {SystemLanguage.ChineseSimplified,  "隐藏效果被损坏破坏..."},
                {SystemLanguage.ChineseTraditional,  "隱藏效果被損壞破壞..."},
                {SystemLanguage.Japanese,  "被害を着潜伏が解けた..."},
            };

            contents[GAME_STRING.HIDE_BROKEN_USE] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "아이템 사용하는 소리에 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by using sound of the item..."},
                {SystemLanguage.Chinese,  "使用项目的声音破坏了隐藏效果..."},
                {SystemLanguage.ChineseSimplified,  "使用项目的声音破坏了隐藏效果..."},
                {SystemLanguage.ChineseTraditional,  "使用項目的聲音破壞了隱藏效果..."},
                {SystemLanguage.Japanese,  "アイテムを使用する音に身を隠し、この解け..."},
            };

            contents[GAME_STRING.HIDE_BROKEN_FLOOR] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "특수 바닥을 밟아서 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by stepping on the speacial floor..."},
                {SystemLanguage.Chinese,  "踩在地板上，隐藏的效果就被打破了..."},
                {SystemLanguage.ChineseSimplified,  "踩在地板上，隐藏的效果就被打破了..."},
                {SystemLanguage.ChineseTraditional,  "踩在地板上，隱藏的效果就被打破了..."},
                {SystemLanguage.Japanese,  "特殊床を踏んで潜伏が解けた..."},
            };

            contents[GAME_STRING.HIDE_BROKEN_BUMP] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "무언가에 부딪혀서  은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by running into something..."},
                {SystemLanguage.Chinese,  "隐藏的效果通过进入某些东西而被打破..."},
                {SystemLanguage.ChineseSimplified,  "隐藏的效果通过进入某些东西而被打破..."},
                {SystemLanguage.ChineseTraditional,  "隱藏的效果通過進入某些東西而被打破..."},
                {SystemLanguage.Japanese,  "何かにぶつかって潜伏が解けた..."},
            };

            contents[GAME_STRING.HIDE_BROKEN_GEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "보석을 줍는 소리에 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by getting sound of the gem..."},
                {SystemLanguage.Chinese,  "通过获得宝石的声音破坏了隐藏效果..."},
                {SystemLanguage.ChineseSimplified,  "通过获得宝石的声音破坏了隐藏效果..."},
                {SystemLanguage.ChineseTraditional,  "通過獲得寶石的聲音破壞了隱藏效果..."},
                {SystemLanguage.Japanese,  "宝石を取る音に身を隠し、この解け..."},
            };

            contents[GAME_STRING.HIDE_BROKEN_PICK_ITEM] = new Dictionary<SystemLanguage, string>()
            {
                {SystemLanguage.Korean,  "아이템을 줍는 소리에 은신이 풀렸다..."},
                {SystemLanguage.English,  "The hiding effect is broken by getting sound of the item..."},
                {SystemLanguage.Chinese,  "隐藏的效果是通过获得该项目的声音而被破坏的..."},
                {SystemLanguage.ChineseSimplified,  "隐藏的效果是通过获得该项目的声音而被破坏的..."},
                {SystemLanguage.ChineseTraditional,  "隱藏的效果是通過獲得該項目的聲音而被破壞的..."},
                {SystemLanguage.Japanese,  "アイテムを拾う音に身を隠し、この解け..."},
            };
        }
    }
}