﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.UI;                   //Allows us to use UI.

using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
using System.IO;
using System;


namespace HideAndSeek
{
    public struct GAME_INFO
    {
        public int playerHPDecrease;
        public int playerHPIncrease;

        public int moveCount;
        public int showCount;
        public int waitCount;

        public int goldGet;

        public int skillShow;
        public int skillHP;
        public int skillHide;
        public int skillDestroy;

        public int totalSkillHP;
        public int totalSkillHide;
        public int totalSkillShow;
        public int totalSkillDestroy;

        public float totalTime;
        public float averageTime;

        public float deltaTime;

        public void ResetLevelInfo()
        {
            playerHPDecrease = 0;
            playerHPIncrease = 0;

            moveCount = 0;
            showCount = 0;
            waitCount = 0;

            goldGet = 0;

            skillHP = 0;
            skillHide = 0;
            skillShow = 0;
            skillDestroy = 0;

            deltaTime = 0;
        }

        public void Init()
        {
            ResetLevelInfo();
            totalSkillHP = 0;
            totalSkillHide = 0;
            totalSkillShow = 0;
            totalSkillDestroy = 0;
            totalTime = 0;
            averageTime = 0;
        }
    }

    public class Level
    {
        public int index;
        public int trap;
        public int enemy;
        public int strongEnemy;
        public int thief;
        public int gem;
        public bool clear;

        public Level(int _index, int _trap, int _enemy, int _strongEnemy, int _thief, int _gem)
        {
            index = _index;
            trap = _trap;
            enemy = _enemy;
            strongEnemy = _strongEnemy;
            thief = _thief;
            gem = _gem;
            clear = false;
        }
    }

    public class Dungeon
    {
        private Level[] levels;
        public int curLevel;
        public int lastLevel;
        private int cost;
        private int gold;

        public Dungeon(Level[] _levels, int _cost, int _gold)
        {
            levels = _levels;
            cost = _cost;
            lastLevel = levels.Length;
            gold = _gold;
        }

        public override string ToString() { return curLevel + "/" + lastLevel; }

        public int Cost() { return cost; }

        public Level GetCurLevel() { return levels[curLevel - 1]; }
        public Level[] GetLevels() { return levels; }

        public void clearCurLevel() { levels[curLevel - 1].clear = true; }
        public bool IsEnd() { return levels[lastLevel-1].clear; }

        public void SetLevel(int _level)
        {
            curLevel = _level;
        }
    }

    class Region
    {
        public List<Dungeon> dungeons;
    }

    public class GameManager : MonoBehaviour
    {
        private int Version = 5;
        public string fileName;

        //        public GAME_INFO gameInfo;

        public List<int> waitTimes = new List<int>();

        public int gameOverCount = 0;
        public int gameEndCount = 0;

        float prevTime = 0f;

        public float levelStartDelay = 2f;
        public int playerHp = 30;
        public int playerGem = 0;
        public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector]
        public bool playersTurn = true;

        private GameObject titleImage;
        private GameObject lobbyImage;
        private GameObject dungeonImage;
        private GameObject resultImage;
        private GameObject controller;

        private Text dungeonText;

        private Button startButton;
        private Button resultButton;
        private Button shopBtn;

        private Button dungeonABtn;
        private Button dungeonBBtn;
        private Button dungeonCBtn;

        private BoardManager boardScript;
        private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
        private bool enemiesMoving;                             //Boolean to check if enemies are moving.

        private Dungeon curDungeon;
        private Dungeon dungeonA;
        private Dungeon dungeonB;
        private Dungeon dungeonC;

        public enum GAME_STATE { START, LOBBY, LEVEL, MAP, PLAY, RESULT, OVER }
        private GAME_STATE gameState;

        private GameObject dungeonMap;
        private Button level1Btn;
        private Button level2Btn;
        private Button level3Btn;
        private Button level4Btn;

        public List<GameObject> trapsOnStage = new List<GameObject>();
        public List<GameObject> objsOnStage = new List<GameObject>();
        public List<GameObject> tilesOnStage = new List<GameObject>();

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                instance = this;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
#else
//                fileName = "log/gameLog_" + DateTime.Now.ToString("M_d_hh_mm") + "_VER_" + Version.ToString() + ".txt";
//                string info = "Level: " + "\tMove: " + "\tHP Inc: " + "\tHP Dec: " + "\tGoldGet: " + "\tShow: " + "\tWait: " + "\tSkill HP: " + "\tSkill Hide: " + "\tSkill Show: " + "\tSkill Destroy: " + "\tTime: ";
//                WriteFile(fileName, info);
#endif
            }
            else if (instance != this)
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            enemies = new List<Enemy>();

            //Get a component reference to the attached BoardManager script
            boardScript = GetComponent<BoardManager>();

            InitDungeons();
            InitUI();
            ChangeState(GAME_STATE.START);
        }

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (instance != null)
            {
                instance.InitUI();
                instance.setupLevel();
            }
        }

        void InitDungeons()
        {
            // TAKE ABOUT 1 MINUTE
            Level[] dungeonAInfo = new Level[] {
                                   new Level(0, 8, 0, 0, 0, 1),
                                   new Level(1, 9, 0, 0, 0, 1),
                                    new Level(2, 10, 2, 0, 0, 3),
                                    new Level(3, 12, 3, 0, 0, 1)
                                };
            dungeonA = new Dungeon(dungeonAInfo, 0, 10);

            // TAKE ABOUT 3 MINUTES
            Level[] dungeonBInfo = new Level[] {
                                   new Level(0, 6, 1, 0, 0, 1),
                                   new Level(1, 7, 1, 0, 0, 1),
                                    new Level(2, 8, 1, 0, 0, 1),
                                    new Level(3, 8, 1, 0, 0, 1),
                                    new Level(4, 8, 1, 0, 0, 1),
                                    new Level(5, 8, 1, 0, 0, 1),
                                    new Level(6, 8, 1, 0, 0, 1),
                                    new Level(7, 8, 1, 0, 0, 1),
                                    new Level(8, 9, 1, 0, 0, 0)
            };
            dungeonB = new Dungeon(dungeonBInfo, 2, 40);

            // TAKE ABOUT 5 MINUTES
            Level[] dungeonCInfo = new Level[] {
                                   new Level(0, 6, 1, 0, 0, 1),
                                   new Level(1, 7, 1, 0, 0, 1),
                                    new Level(2, 8, 1, 0, 0, 1),
                                    new Level(3, 8, 1, 0, 0, 1),
                                    new Level(4, 8, 1, 0, 0, 1),
                                    new Level(5, 8, 1, 0, 0, 1),
                                    new Level(6, 8, 1, 0, 0, 1),
                                    new Level(7, 8, 1, 0, 0, 1),
                                    new Level(8, 8, 1, 0, 0, 1),
                                    new Level(9, 8, 1, 0, 0, 1),
                                    new Level(10, 8, 1, 0, 0, 1),
                                    new Level(11, 8, 1, 0, 0, 1),
                                    new Level(12, 8, 1, 0, 0, 1),
                                    new Level(13, 8, 1, 0, 0, 1),
                                    new Level(14, 8, 1, 0, 0, 1),
                                    new Level(15, 8, 1, 0, 0, 1)
            };
            dungeonC= new Dungeon(dungeonCInfo, 3, 100);
        }

        void EnterDungeon(Dungeon dungeon)
        {
            if (dungeon.Cost() <= playerGem)
            {
                playerGem = playerGem - dungeon.Cost();
                curDungeon = dungeon;
                ChangeState(GAME_STATE.MAP);
            }
            else
            {

            }
        }

        void GotoDungeonMap()
        {
            if(curDungeon.IsEnd()) ChangeState(GAME_STATE.LOBBY);
            else ChangeState(GAME_STATE.MAP);
        }


        void setupPage()
        {
            bool bTitle = false;
            bool bLobby = false;
            bool bDungeon = false;
            bool bResult = false;
            bool bPlay = false;
            bool bMap = false;

            switch (gameState)
            {
                case GAME_STATE.START: bTitle = true; break;
                case GAME_STATE.LOBBY: bLobby = true; break;
                case GAME_STATE.LEVEL: bDungeon = true; break;
                case GAME_STATE.MAP: bMap = true; break;
                case GAME_STATE.PLAY: bPlay = true; break;
                case GAME_STATE.RESULT: bResult = true; break;
                case GAME_STATE.OVER: bResult = true; break;
            }

            lobbyImage.SetActive(bLobby);
            titleImage.SetActive(bTitle);
            dungeonImage.SetActive(bDungeon);
            resultImage.SetActive(bResult);
            controller.SetActive(bPlay);
            dungeonMap.SetActive(bMap);
        }

        void setupLevel()
        {
            enemies.Clear();
            boardScript.SetupScene(curDungeon.GetCurLevel());
            ChangeState(GAME_STATE.PLAY);
        }

        void InitiateLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        void EnterDungeonA()
        {
            EnterDungeon(dungeonA);
        }

        void EnterDungeonB()
        {
            EnterDungeon(dungeonB);
        }

        void EnterDungeonC()
        {
            EnterDungeon(dungeonC);
        }

        void EnterShop()
        {

        }

        public void ShowResult()
        {
            curDungeon.clearCurLevel();
            ChangeState(GAME_STATE.RESULT);
        }

        void GoToLobby()
        {
            ChangeState(GAME_STATE.LOBBY);
        }

        void EnterLevel1()
        {
            curDungeon.SetLevel(1);
            ChangeState(GAME_STATE.LEVEL);
            dungeonText.text = "Level " + curDungeon.ToString();
            Invoke("InitiateLevel", 2f);
        }

        void EnterLevel2()
        {
            curDungeon.SetLevel(2);
            ChangeState(GAME_STATE.LEVEL);
            dungeonText.text = "Level " + curDungeon.ToString();
            Invoke("InitiateLevel", 2f);
        }

        void EnterLevel3()
        {
            curDungeon.SetLevel(3);
            ChangeState(GAME_STATE.LEVEL);
            dungeonText.text = "Level " + curDungeon.ToString();
            Invoke("InitiateLevel", 2f);
        }

        void EnterLevel4()
        {
            curDungeon.SetLevel(4);
            ChangeState(GAME_STATE.LEVEL);
            dungeonText.text = "Level " + curDungeon.ToString();
            Invoke("InitiateLevel", 2f);
        }

        void InitUI()
        {
            dungeonMap = GameObject.Find("DungeonMap");
            level1Btn = GameObject.Find("level1").GetComponent<Button>();
            level2Btn = GameObject.Find("level2").GetComponent<Button>();
            level3Btn = GameObject.Find("level3").GetComponent<Button>();
            level4Btn = GameObject.Find("level4").GetComponent<Button>();

            level1Btn.onClick.AddListener(EnterLevel1);
            level2Btn.onClick.AddListener(EnterLevel2);
            level3Btn.onClick.AddListener(EnterLevel3);
            level4Btn.onClick.AddListener(EnterLevel4);

            titleImage = GameObject.Find("FrontPageImage");
            lobbyImage = GameObject.Find("LobbyImage");
            dungeonImage = GameObject.Find("DungeonImage");
            resultImage = GameObject.Find("ResultImage");
            controller = GameObject.Find("Controller");

            dungeonText = GameObject.Find("DungeonText").GetComponent<Text>();

            shopBtn = GameObject.Find("ShopButton").GetComponent<Button>();
            dungeonABtn = GameObject.Find("DungeonABtn").GetComponent<Button>();
            dungeonBBtn = GameObject.Find("DungeonBBtn").GetComponent<Button>();
            dungeonCBtn = GameObject.Find("DungeonCBtn").GetComponent<Button>();
            
            resultButton = GameObject.Find("ResultButton").GetComponent<Button>();
            startButton = GameObject.Find("FrontPageButton").GetComponent<Button>();

            dungeonABtn.onClick.AddListener(EnterDungeonA);
            dungeonBBtn.onClick.AddListener(EnterDungeonB);
            dungeonCBtn.onClick.AddListener(EnterDungeonC);

            shopBtn.onClick.AddListener(EnterShop);

            resultButton.onClick.AddListener(GotoDungeonMap);
            startButton.onClick.AddListener(GoToLobby);
        }

        public void ChangeState(GAME_STATE nextState)
        {
            gameState = nextState;
            setupPage();
        }

        void Update()
        {
            if (gameState == GAME_STATE.PLAY)
            {
                if (!playersTurn && !enemiesMoving)
                    StartCoroutine(MoveEnemies());
            }
        }

        public void AddEnemyToList(Enemy script)
        {
            enemies.Add(script);
        }


        public void GameOver()
        {
            ChangeState(GAME_STATE.OVER);
        }

        bool bShowing = false;
        public bool IsShowing() { return bShowing; }

        public void ShowMap(bool bShow)
        {
            bShowing = bShow;
            ShowAllUnits(bShow);
            ShowObjects(bShow);
        }

        public void ShowAllUnits(bool bShow)
        {
            ShowEnemies(bShow);
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "Thief") enemies[i].Show(bShow);
            }
        }

        public void ShowEnemies(bool bShow)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "Enemy") enemies[i].Show(bShow);
            }
        }

        public List<Enemy> SearchEnemies(Vector3[] range)
        {
            List<Enemy> result = new List<Enemy>();

            foreach (Vector3 v in range)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].tag == "Thief") continue;
                    if (enemies[i].transform.position == v) result.Add(enemies[i]);
                }
            }

            return result;
        }

        public void DestoryEnemies(Vector3 targetPos)
        {
            Vector3[] range = new Vector3[]
            {
                new Vector3(targetPos.x-1, targetPos.y, 0  ),
                new Vector3(targetPos.x+1, targetPos.y, 0  ),
                new Vector3(targetPos.x, targetPos.y-1, 0  ),
                new Vector3(targetPos.x, targetPos.y+1, 0  )
            };

            List<GameObject> targetTiles = new List<GameObject>();

            foreach (GameObject obj in tilesOnStage)
            {
                foreach (Vector3 v in range)
                {
                    if (obj.tag == "Wall") continue;
                    if (obj.transform.position == v) targetTiles.Add(obj);
                }
            }

            foreach (GameObject obj in trapsOnStage)
            {
                foreach (Vector3 v in range)
                {
                    if (obj.transform.position == v) targetTiles.Add(obj);
                }
            }

            List<Enemy> targetEnemies = SearchEnemies(range);
            StartCoroutine(DestroyEffectFloor(targetTiles));
            StartCoroutine(DestroyEffect(targetEnemies));
        }

        IEnumerator DestroyEffectFloor(List<GameObject> targetTiles)
        {
            foreach (GameObject obj in targetTiles)
            {
                SpriteRenderer spRenderer = obj.GetComponent<SpriteRenderer>();
                if (spRenderer)
                {
                    Color color = spRenderer.color;
                    color = new Vector4(1, 0.5f, 0.5F, 1);
                    spRenderer.color = color;
                }
            }
            yield return new WaitForSeconds(0.5f);

            foreach (GameObject obj in targetTiles)
            {
                SpriteRenderer spRenderer = obj.GetComponent<SpriteRenderer>();
                if (spRenderer)
                {
                    Color color = spRenderer.color;
                    color = new Vector4(1, 1, 1, 1);
                    spRenderer.color = color;
                }
            }
        }

        IEnumerator DestroyEffect(List<Enemy> targetEnemies)
        {
            foreach (Enemy en in targetEnemies) en.Show(true);
            yield return new WaitForSeconds(0.1f);
            foreach (Enemy en in targetEnemies) en.Show(false);
            yield return new WaitForSeconds(0.1f);
            foreach (Enemy en in targetEnemies) en.Show(true);
            yield return new WaitForSeconds(0.05f);
            foreach (Enemy en in targetEnemies) en.Show(false);
            yield return new WaitForSeconds(0.05f);
            foreach (Enemy en in targetEnemies) en.Show(true);
            yield return new WaitForSeconds(0.1f);
            foreach (Enemy en in targetEnemies) en.gameObject.SetActive(false);
        }

        public void SetSearchEnemies(bool value)
        {
            foreach (Enemy en in enemies)
            {
                en.SetSearch(value);
            }
        }

        IEnumerator MoveEnemies()
        {
            float totalTime = 0.24f;
            enemiesMoving = true;

            yield return new WaitForSeconds(0.08f);

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].gameObject.activeSelf) enemies[i].MoveEnemy();

                yield return new WaitForSeconds(0.03f);
                totalTime -= 0.03f;
            }

            if (totalTime > 0) yield return new WaitForSeconds(totalTime);
            playersTurn = true;

            enemiesMoving = false;
        }

        public bool[,] UpdateMap(Vector3 playerPos)
        {
            bool[,] map = new bool[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    map[i, j] = true;
                }
            }

            foreach (Enemy en in enemies)
            {
                Vector2 pos = en.transform.position;
                int x = (int)pos.x;
                int y = (int)pos.y;
                map[x, y] = false;
            }

            map[(int)playerPos.x, (int)playerPos.y] = false;

            return map;
        }

        public bool IsAvailablePos(Vector2 dest)
        {
            foreach (Enemy en in enemies)
            {
                Vector2 pos = en.transform.position;
                if (pos == dest) return false;
            }
            return true;
        }

        public GameObject IsTrap(float x, float y)
        {
            foreach (GameObject obj in trapsOnStage)
            {
                if (obj.transform.position.x == x && obj.transform.position.y == y) return obj;
            }

            return null;
        }

        public void ShowObjects(bool bShow)
        {
            foreach (GameObject obj in objsOnStage)
            {
                if (obj == null) continue;
                Renderer renderer = obj.GetComponent<SpriteRenderer>();
                if (renderer) renderer.enabled = bShow;
            }

            foreach (GameObject obj in trapsOnStage)
            {
                if (obj == null) continue;
                Renderer renderer = obj.GetComponent<SpriteRenderer>();
                if (renderer) renderer.enabled = bShow;
            }

            foreach (GameObject obj in tilesOnStage)
            {
                if (obj == null) continue;
                Renderer renderer = obj.GetComponent<SpriteRenderer>();
                if (renderer)
                {
                    if (bShow)
                    {
                        Color color = renderer.material.color;
                        color.a = 1f;
                        renderer.material.color = color;
                    }
                    else
                    {
                        Color color = renderer.material.color;
                        color.a = 0.6f;
                        renderer.material.color = color;
                    }
                }
            }
        }

        public void writeLog()
        {
            //            gameInfo.deltaTime = Time.time - prevTime;
            //			prevTime = Time.time;
            //            foreach (int time in waitTimes)
            //            {
            //                if (time > 0) gameInfo.waitCount++;
            //            }            

            //            gameInfo.totalTime += gameInfo.deltaTime;            
            //            gameInfo.totalSkillHP += gameInfo.skillHP;
            //            gameInfo.totalSkillShow += gameInfo.skillShow;
            //            gameInfo.totalSkillHide += gameInfo.skillHide;
            //            gameInfo.totalSkillDestroy += gameInfo.skillDestroy;

            ////            gameInfo.averageTime = gameInfo.totalTime / level;

            //            Dictionary<string, object> eventInfo = new Dictionary<string, object>
            //                        {
            ////                            { "level", level},
            //                            { "move", gameInfo.moveCount},
            //                            { "HP Inc", gameInfo.playerHPIncrease},
            //                            { "HP Dec", gameInfo.playerHPDecrease},
            //                            { "GoldGet", gameInfo.goldGet},
            //                            { "show", gameInfo.showCount},                            
            //                            { "Wait", gameInfo.waitCount},
            //                            { "time", (int)gameInfo.deltaTime}
            //                        };

            //            Analytics.CustomEvent("Level info", eventInfo);

            //            Dictionary<string, object> skillInfo = new Dictionary<string, object>
            //                        {
            ////                            { "level", level},
            //                            { "Skill HP", gameInfo.skillHP},
            //                            { "Skill Hide", gameInfo.skillHide},
            //                            { "Skill Show", gameInfo.skillShow},
            //                            { "Skill Destory", gameInfo.skillDestroy}
            //                         };
            //            Analytics.CustomEvent("Skill info", skillInfo);

            //#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            //#else
            //            string info = 
            ////                level.ToString() + "\t" +
            //                gameInfo.moveCount.ToString()
            //                + "\t" + gameInfo.playerHPIncrease.ToString()
            //                + "\t" + gameInfo.playerHPDecrease.ToString()
            //                + "\t" + gameInfo.goldGet.ToString()
            //                + "\t" + gameInfo.showCount.ToString()
            //                + "\t" + gameInfo.waitCount.ToString()
            //                + "\t" + gameInfo.skillHP.ToString()
            //                + "\t" + gameInfo.skillHide.ToString()
            //                + "\t" + gameInfo.skillShow.ToString()
            //                + "\t" + gameInfo.skillDestroy.ToString()
            //                + "\t" + ((int)gameInfo.deltaTime).ToString();
            //            WriteFile(fileName, info);            
            //#endif
            //            gameInfo.ResetLevelInfo();
        }

        public void WriteFile(string fileName, string info)
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