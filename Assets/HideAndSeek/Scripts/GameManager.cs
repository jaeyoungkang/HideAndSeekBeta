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
    public struct GAME_INFO
    {
        public int playerHPDecrease;
        public int playerHPIncrease;
                
        public int playerHp;
        public int moveCount;
        public int showCount;
        public int waitCount;

        public int gold;
        public int goldGet;

        public int skillHP;
        public int skillTime;
        public int skillShow;
        public int skillDestroy;

        public int totalSkillHP;
        public int totalSkillTime;
        public int totalSkillShow;
        public int totalSkillDestroy;

        public float totalTime;
        public float averageTime;

        public float deltaTime;

        public void ResetLevelInfo()
        {
            playerHPDecrease = 0;
            playerHPIncrease = 0;

            playerHp = 0;
            moveCount = 0;
            showCount = 0;
            waitCount = 0;

            gold = 0;
            goldGet = 0;

            skillHP = 0;
            skillTime = 0;
            skillShow = 0;
            skillDestroy = 0;

            deltaTime = 0;
        }

        public void Init()
        {
            ResetLevelInfo();
            totalSkillHP = 0;
            totalSkillTime = 0;
            totalSkillShow = 0;
            totalSkillDestroy = 0;
            totalTime = 0;
            averageTime = 0;
        }
    }	

    public class GameManager : MonoBehaviour
    {
        public int Version = 4;
        public string fileName;

        public GAME_INFO gameInfo;

        public List<int> waitTimes = new List<int>();

        public int gameOverCount = 0;
        public int gameEndCount = 0;        

        float prevTime = 0f;        

        public float levelStartDelay = 2f;
        public int playerHp = 30;
        public int playerGem = 0;
        public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector] public bool playersTurn = true;

        private Text subTitleText;
        private Text titleText;
        private GameObject titleImage;
        private BoardManager boardScript;
        private int level = 1;
        private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
        private bool enemiesMoving;                             //Boolean to check if enemies are moving.
        private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.

        private enum GAME_STATE { START, PLAY, END, OVER }
        private GAME_STATE gameState = GAME_STATE.START;
                
        public List<GameObject> trapsOnStage = new List<GameObject>();
        public List<GameObject> objsOnStage = new List<GameObject>();
        public List<GameObject> tilesOnStage = new List<GameObject>();


        public bool IsAvailablePos(Vector2 dest)
        {
            foreach(Enemy en in enemies)
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

        //Awake is always called before any Start functions
        void Awake()
        {            
            //Check if instance already exists
            if (instance == null)
            {
                instance = this;
                gameInfo.Init();
                fileName = "log/gameLog_" + DateTime.Now.ToString("M_d_hh_mm") + "_VER_" + Version.ToString() + ".txt";
            }
            else if (instance != this)
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            enemies = new List<Enemy>();

            //Get a component reference to the attached BoardManager script
            boardScript = GetComponent<BoardManager>();

            InitGame();           
        }

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

		public void writeLog()
		{
            gameInfo.deltaTime = Time.time - prevTime;
			prevTime = Time.time;
            foreach (int time in waitTimes)
            {
                if (time > 0) gameInfo.waitCount++;
            }            

            gameInfo.totalTime += gameInfo.deltaTime;            
            gameInfo.totalSkillHP += gameInfo.skillHP;
            gameInfo.totalSkillShow += gameInfo.skillShow;
            gameInfo.totalSkillTime += gameInfo.skillTime;
            gameInfo.totalSkillDestroy += gameInfo.skillDestroy;

            gameInfo.averageTime = gameInfo.totalTime / level;

            Dictionary<string, object> eventInfo = new Dictionary<string, object>
                        {
                            { "level", level},
                            { "move", gameInfo.moveCount},
                            { "HP", gameInfo.playerHp},
                            { "HP Inc", gameInfo.playerHPIncrease},
                            { "HP Dec", gameInfo.playerHPDecrease},
                            { "Gold", gameInfo.gold},
                            { "GoldGet", gameInfo.goldGet},
                            { "show", gameInfo.showCount},                            
                            { "Wait", gameInfo.waitCount},
                            { "time", (int)gameInfo.deltaTime}
                        };

            Analytics.CustomEvent("Level info", eventInfo);

            Dictionary<string, object> skillInfo = new Dictionary<string, object>
                        {
                            { "level", level},
                            { "Skill HP", gameInfo.skillHP},
                            { "Skill Time", gameInfo.skillTime},
                            { "Skill Show", gameInfo.skillShow},
                            { "Skill Destory", gameInfo.skillDestroy}
                         };
            Analytics.CustomEvent("Skill info", skillInfo);
            
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
#else
            string info =
                "Level: " + level.ToString()
                + "\tMove: " + gameInfo.moveCount.ToString()
                + "\tHP: " + gameInfo.playerHp.ToString()
                + "\tHP Inc: " + gameInfo.playerHPIncrease.ToString()
                + "\tHP Dec: " + gameInfo.playerHPDecrease.ToString()
                + "\tGold: " + gameInfo.gold.ToString()
                + "\tGoldGet: " + gameInfo.goldGet.ToString()
                + "\tShow: " + gameInfo.showCount.ToString()
                + "\tWait: " + gameInfo.waitCount.ToString()
                + "\tSkill HP: " + gameInfo.skillHP.ToString()
                + "\tSkill Time: " + gameInfo.skillTime.ToString()
                + "\tSkill Show: " + gameInfo.skillShow.ToString()
                + "\tSkill Destroy: " + gameInfo.skillDestroy.ToString()
                + "\tTime: " + ((int)gameInfo.deltaTime).ToString();            
            WriteFile(fileName, info);            
#endif
            gameInfo.ResetLevelInfo();
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

		public void setLevel()
		{
            instance.level++;
		}

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (instance != null)
            {				
				instance.setLevel ();
                instance.InitGame();
            }
        }

		//Initializes the game for each level.
        void InitGame()
        {
            if (level == 19)
                EndGame();

            //While doingSetup is true the player can't move, prevent player from moving while title card is up.
            doingSetup = true;

            titleImage = GameObject.Find("LevelImage");

            titleText = GameObject.Find("LevelText").GetComponent<Text>();
            subTitleText = GameObject.Find("SubTitleText").GetComponent<Text>();

            ChangeTitleText();

            enemies.Clear();

            //Call the SetupScene function of the BoardManager script, pass it current level number.
            boardScript.SetupScene(level);

            titleImage.SetActive(true);
            if (gameState == GAME_STATE.PLAY)
                Invoke("HideTitleImage", levelStartDelay);
        }

        public void StartGame()
        {
            gameState = GAME_STATE.START;
        }

        public bool IsGameOver()
        {
            return gameState == GAME_STATE.OVER;
        }

        public bool Isplaying()
        {
            return (gameState == GAME_STATE.PLAY && !doingSetup);
        }

		void HideTitleImage()
		{
			titleImage.SetActive(false);
			
			//Set doingSetup to false allowing player to move again.
			doingSetup = false;
        }

		void ChangeTitleText ()
		{
			subTitleText.enabled = true;
            switch(gameState)
            {
                case GAME_STATE.END:
                    titleText.text = "All levels cleared!";
                    break;

                case GAME_STATE.OVER:
                case GAME_STATE.START:
                    titleText.text = "Hide and Seek beta";
                    break;

                case GAME_STATE.PLAY:
                    if (level == 16) titleText.text = "Last Level 1/3";
                    else if (level == 17) titleText.text = "Last Level 2/3";
                    else if (level == 18) titleText.text = "Last Level 3/3";
                    else titleText.text = "Level " + level +"/15";
                    subTitleText.enabled = false;
                    break;
            }			
		}

        public bool IsInput()
        {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            bool touchReleased = false;
            for (int i = 0; i < Input.touches.Length; i++)
            {
                touchReleased = Input.touches[i].phase == TouchPhase.Ended;
                if (touchReleased) break;
            }

            if (touchReleased || Input.GetKeyUp(KeyCode.Space))
#else
            if (Input.GetKeyUp(KeyCode.Space))
#endif
                return true;
            else
                return false;
        }

        void TitlePageUpdate()
        {
            if (IsInput() == false) return;

            switch(gameState)
            {
                case GAME_STATE.END:
                    gameState = GAME_STATE.START;
                    ChangeTitleText();
                    break;

                case GAME_STATE.START:
                    gameState = GAME_STATE.PLAY;
                    ChangeTitleText();

                    Invoke("HideTitleImage", levelStartDelay);

                    Analytics.CustomEvent("gameStart", new Dictionary<string, object>
                    {
                        { "GameOverCount", gameOverCount},
                        { "GameEndCount", gameOverCount}
                    });
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
#else
                    WriteFile(fileName, "GAME START");
#endif
                    break;
            }

        }
		
		void Update()
		{
            if (gameState == GAME_STATE.OVER) return;

            if (gameState == GAME_STATE.END || gameState == GAME_STATE.START)
            {
                TitlePageUpdate();
                return;                
            }

            if (playersTurn || enemiesMoving || doingSetup)
                return;

            StartCoroutine(MoveEnemies());
        }
		
		public void AddEnemyToList(Enemy script)
		{
			enemies.Add(script);
		}

        public void saveTotalInfo()
        {
            Dictionary<string, object> TotalInfo = new Dictionary<string, object>
                        {
                            { "Last Level", level},
                            { "Total Skill HP", gameInfo.totalSkillHP},
                            { "Total Skill Time", gameInfo.totalSkillTime},
                            { "Total Skill Show", gameInfo.totalSkillShow},
                            { "Total Skill Destory", gameInfo.totalSkillDestroy},
                            { "Total Time", gameInfo.totalTime},
                            { "Total Average Time", gameInfo.averageTime}
                         };
            Analytics.CustomEvent("Total info", TotalInfo);
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
#else
            string info = "Last Level " + level.ToString()
              + "\tTotal Skill HP: " + gameInfo.totalSkillHP.ToString()
              + "\tTotal Skill Time: " + gameInfo.totalSkillTime.ToString()
              + "\tTotal Skill Show: " + gameInfo.totalSkillShow.ToString()
              + "\tTotal Skill Destroy: " + gameInfo.totalSkillDestroy.ToString()
              + "\tTotal Time: " + ((int)gameInfo.totalTime).ToString()
              + "\tAverage Time: " + ((int)gameInfo.averageTime).ToString();
            WriteFile(fileName, info);
#endif
        }
        
        public void GameOver()
		{
            writeLog();
            saveTotalInfo();
            gameInfo.Init();

            gameState = GAME_STATE.OVER;
            playersTurn = false;
            gameOverCount++;
            level = 0;            
        }        
        
        public void EndGame()
		{
            writeLog();
            saveTotalInfo();
            gameInfo.Init();
            level = 1;
            gameState = GAME_STATE.END;
            playersTurn = false;
            gameEndCount++;
        }

        public void ShowMap(bool bShow)
        {
            ShowAllUnits(bShow);
            ShowObjects(bShow);

            if (bShow) gameInfo.showCount++;            
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

        public void DestoryEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if(enemies[i].tag == "Thief");
            }
            StartCoroutine(DestroyEffect());
        }

        IEnumerator DestroyEffect()
        {
            ShowEnemies(true);
            yield return new WaitForSeconds(0.1f);
            ShowEnemies(false);
            yield return new WaitForSeconds(0.1f);
            ShowEnemies(true);
            yield return new WaitForSeconds(0.05f);
            ShowEnemies(false);
            yield return new WaitForSeconds(0.05f);
            ShowEnemies(true);
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "Enemy") enemies[i].gameObject.SetActive(false);
            }
        }


        IEnumerator MoveEnemies()
		{
            float totalTime = 0.34f;
			enemiesMoving = true;
            
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < enemies.Count; i++)
			{
                if(enemies[i].gameObject.activeSelf) enemies[i].MoveEnemy ();

                yield return new WaitForSeconds(0.06f);
                totalTime -= 0.06f;
            }

            yield return new WaitForSeconds(totalTime);
            playersTurn = true;
			
			enemiesMoving = false;
		}
	}
}

