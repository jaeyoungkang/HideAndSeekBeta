using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.UI;                   //Allows us to use UI.

using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
using System.IO;
using System;

namespace HideAndSeek
{
    public enum GAME_STATE { START, LOBBY, SHOP, LEVEL, MAP, PLAY, RESULT, OVER }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;
        [HideInInspector]
        public bool playersTurn = true;

        private GameObject titleImage;
        private GameObject lobbyImage;
        private GameObject dungeonImage;
        private GameObject resultImage;
        private GameObject shopImage;
        private GameObject controller;
        private GameObject dungeonMap;        

        private Button dungeonABtn;
        private Button dungeonBBtn;
        private Button dungeonCBtn;
        private Button resultButton;
        private Text dungeonText;
        private Text resultText;
        private Text goldText;
        
        private Button level1Btn;
        private Button level2Btn;
        private Button level3Btn;
        private Button level4Btn;
        private Button startButton;
        private Button shopBtn;

        private Image GemImage;
        private Text HpText;
        private Text GemText;
        private Text TimeText;

        private DungeonManager dungeonManager;
        private BoardManager boardScript;
        private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
        private bool enemiesMoving;                             //Boolean to check if enemies are moving.
                
        private GAME_STATE gameState;
                
        public List<GameObject> trapsOnStage = new List<GameObject>();
        public List<GameObject> objsOnStage = new List<GameObject>();
        public List<GameObject> tilesOnStage = new List<GameObject>();

        public int playerHp = 20;
        public int playerGem = 0;
        public float timeLimit;

        public int playerGold = 40;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            enemies = new List<Enemy>();
            boardScript = GetComponent<BoardManager>();
            dungeonManager = GetComponent<DungeonManager>();
            dungeonManager.InitDungeons();

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

        private Dungeon curDungeon;

        public void ShowResult()
        {
            curDungeon.clearCurLevel();
            if(curDungeon.IsEnd())
            {
                resultText.text = "Gold: " + curDungeon.GetReward().ToString();
                playerGold += curDungeon.GetReward();
            }
            GameManager.instance.ChangeState(GAME_STATE.RESULT);
        }

        void setupPage()
        {
            bool bTitle = false;
            bool bLobby = false;
            bool bShop = false;
            bool bDungeon = false;
            bool bResult = false;
            bool bPlay = false;
            bool bMap = false;

            switch (gameState)
            {
                case GAME_STATE.START: bTitle = true; break;
                case GAME_STATE.SHOP: bShop = true; break;
                case GAME_STATE.LOBBY: bLobby = true; break;
                case GAME_STATE.LEVEL: bDungeon = true; break;
                case GAME_STATE.MAP: bMap = true; break;
                case GAME_STATE.PLAY: bPlay = true; break;
                case GAME_STATE.RESULT: bResult = true; break;
                case GAME_STATE.OVER: bResult = true; break;
            }

            shopImage.SetActive(bShop);
            lobbyImage.SetActive(bLobby);
            titleImage.SetActive(bTitle);
            dungeonImage.SetActive(bDungeon);
            resultImage.SetActive(bResult);
            controller.SetActive(bPlay);
            dungeonMap.SetActive(bMap);

            if (bMap || bDungeon || bResult || bPlay)
            {
                HpText.enabled = true;
                GemText.enabled = true;
                TimeText.enabled = true;
                GemImage.enabled = true;
            }
            else
            {
                HpText.enabled = false;
                GemText.enabled = false;
                TimeText.enabled = false;
                GemImage.enabled = true;
            }

            goldText.text = "Gold: " + playerGold;
        }

        public void SetHPText(Color msgColor)
        {
            HpText.text = "HP:" + playerHp;
            HpText.color = msgColor;
        }

        public void SetGemText(Color gColor)
        {
            GemText.text = playerGem.ToString();
            GemText.color = gColor;
        }

        void setupLevel()
        {
            enemies.Clear();
            boardScript.SetupScene(curDungeon.GetCurLevel());
            ChangeState(GAME_STATE.PLAY);
        }

        public void EnterDungeon(Dungeon dungeon)
        {
            curDungeon = dungeon;
            timeLimit = curDungeon.TimeLimit();
            playerGem = 0;
            playerHp = 20;
            ChangeState(GAME_STATE.MAP);
        }

        void EnterDungeonA()
        {
            EnterDungeon(dungeonManager.DungeonA());
        }

        void EnterDungeonB()
        {
            EnterDungeon(dungeonManager.DungeonB());
        }

        void EnterDungeonC()
        {
            EnterDungeon(dungeonManager.DungeonC());
        }

        void EnterShop()
        {
            ChangeState(GAME_STATE.SHOP);
        }
        
        void InitUI()
        {
            titleImage = GameObject.Find("FrontPageImage");
            lobbyImage = GameObject.Find("LobbyImage");
            shopImage = GameObject.Find("ShopImage");
            dungeonImage = GameObject.Find("DungeonImage");
            resultImage = GameObject.Find("ResultImage");
            controller = GameObject.Find("Controller");
            dungeonMap = GameObject.Find("DungeonMap");

            shopBtn = GameObject.Find("ShopButton").GetComponent<Button>();
            startButton = GameObject.Find("FrontPageButton").GetComponent<Button>();
            
            level1Btn = GameObject.Find("level1").GetComponent<Button>();
            level2Btn = GameObject.Find("level2").GetComponent<Button>();
            level3Btn = GameObject.Find("level3").GetComponent<Button>();
            level4Btn = GameObject.Find("level4").GetComponent<Button>();
            
            dungeonText = GameObject.Find("DungeonText").GetComponent<Text>();
            resultText = GameObject.Find("ResultText").GetComponent<Text>();
            goldText = GameObject.Find("GoldText").GetComponent<Text>();

            HpText = GameObject.Find("HpText").GetComponent<Text>();
            GemText = GameObject.Find("GemText").GetComponent<Text>();
            TimeText = GameObject.Find("TimeText").GetComponent<Text>();
            GemImage = GameObject.Find("GemImage").GetComponent<Image>();

            resultButton = GameObject.Find("ResultButton").GetComponent<Button>();
            
            dungeonABtn = GameObject.Find("DungeonABtn").GetComponent<Button>();
            dungeonBBtn = GameObject.Find("DungeonBBtn").GetComponent<Button>();
            dungeonCBtn = GameObject.Find("DungeonCBtn").GetComponent<Button>();

            resultButton.onClick.AddListener(GotoDungeonMap);

            level1Btn.onClick.AddListener(EnterLevel1);
            level2Btn.onClick.AddListener(EnterLevel2);
            level3Btn.onClick.AddListener(EnterLevel3);
            level4Btn.onClick.AddListener(EnterLevel4);

            dungeonABtn.onClick.AddListener(EnterDungeonA);
            dungeonBBtn.onClick.AddListener(EnterDungeonB);
            dungeonCBtn.onClick.AddListener(EnterDungeonC);

            shopBtn.onClick.AddListener(EnterShop);
            startButton.onClick.AddListener(GoToLobby);
        }


        void GotoDungeonMap()
        {
            if (curDungeon.IsEnd()) GameManager.instance.ChangeState(GAME_STATE.LOBBY);
            else if (playerHp == 0) GameManager.instance.ChangeState(GAME_STATE.LOBBY);
            else GameManager.instance.ChangeState(GAME_STATE.MAP);
        }

        void EnterLevel1()
        {
            curDungeon.SetLevel(1);
            GameManager.instance.ChangeState(GAME_STATE.LEVEL);
            dungeonText.text = "Level " + curDungeon.ToString();
            Invoke("InitiateLevel", 2f);
        }

        void EnterLevel2()
        {
            curDungeon.SetLevel(2);
            GameManager.instance.ChangeState(GAME_STATE.LEVEL);
            dungeonText.text = "Level " + curDungeon.ToString();
            Invoke("InitiateLevel", 2f);
        }

        void EnterLevel3()
        {
            curDungeon.SetLevel(3);
            GameManager.instance.ChangeState(GAME_STATE.LEVEL);
            dungeonText.text = "Level " + curDungeon.ToString();
            Invoke("InitiateLevel", 2f);
        }

        void EnterLevel4()
        {
            curDungeon.SetLevel(4);
            GameManager.instance.ChangeState(GAME_STATE.LEVEL);
            dungeonText.text = "Level " + curDungeon.ToString();
            Invoke("InitiateLevel", 2f);
        }

        void InitiateLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        void GoToLobby()
        {
            ChangeState(GAME_STATE.LOBBY);
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
                timeLimit -= Time.deltaTime;
                TimeText.text = Mathf.Floor(timeLimit).ToString();
                if (timeLimit <= 10) TimeText.color = Color.red;
                else TimeText.color = Color.white;

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