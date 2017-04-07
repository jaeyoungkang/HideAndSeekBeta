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

        public int playerSodaUse;
        public int playerSodaGet;

        public int playerHp;
        public int playerSoda;

        public int moveTryCount;
        public int moveCount;

        public int deltaTime;

        public void init()
        {
            playerHPDecrease = 0;
            playerHPIncrease = 0;

            playerSodaUse = 0;
            playerSodaGet = 0;

            playerHp = 0;
            playerSoda = 0;

            moveTryCount = 0;
            moveCount = 0;

            deltaTime = 0;
        }

    }	

    public class GameManager : MonoBehaviour
    {
        public GAME_INFO gameInfo;

        public int gameOverCount = 0;
        public int gameEndCount = 0;        

        float prevTime = 0f;        

        public float levelStartDelay = 2f;
        public int playerHp = 30;
        public int playerIp = 1;
        public int playerGold = 0;
        public int playerCoin = 1;
        public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector] public bool playersTurn = true;

        private Text showTimeText;

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

        public List<GameObject> goldsOnStage = new List<GameObject>();
        public List<GameObject> trapsOnStage = new List<GameObject>();

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
            foreach (GameObject aGold in goldsOnStage)
            {
                if (aGold == null) continue;
                Renderer renderer = aGold.GetComponent<SpriteRenderer>();
                if (renderer) renderer.enabled = bShow;
            }

            foreach (GameObject obj in trapsOnStage)
            {
                if (obj == null) continue;
                Renderer renderer = obj.GetComponent<SpriteRenderer>();
                if (renderer) renderer.enabled = bShow;
            }
        }

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                instance = this;
                gameInfo.init();
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
			gameInfo.deltaTime = (int)(Time.time - prevTime);
			prevTime = Time.time;
            Dictionary<string, object> eventInfo = new Dictionary<string, object>
                        {
                            { "level", level},
                            { "move", gameInfo.moveCount},
                            { "move try", gameInfo.moveTryCount},
                            { "HP", gameInfo.playerHp},
                            { "HP Inc", gameInfo.playerHPIncrease},
                            { "HP Dec", gameInfo.playerHPDecrease},
                            { "Soda", gameInfo.playerSoda},
                            { "SodaUse", gameInfo.playerSodaUse},
                            { "SodaGet", gameInfo.playerSodaGet},
                            { "time", gameInfo.deltaTime}
                        };

            Analytics.CustomEvent("Level info", eventInfo);

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
#else
            string info =
                "Level: " + level.ToString()
                + " Move: " + gameInfo.moveCount.ToString()
                + " Move try: " + gameInfo.moveTryCount.ToString()

                + " HP: " + gameInfo.playerHp.ToString()
                + " HP Inc: " + gameInfo.playerHPIncrease.ToString()
                + " HP Dec: " + gameInfo.playerHPDecrease.ToString()

                + " Soda: " + gameInfo.playerSoda.ToString()
                + " Soda Use: " + gameInfo.playerSodaUse.ToString()
                + " Soda Get: " + gameInfo.playerSodaGet.ToString()

                + " Time: " + gameInfo.deltaTime.ToString();

            string info1 =
                level.ToString()
                + "\t" + gameInfo.moveCount.ToString()
                + "\t" + gameInfo.moveTryCount.ToString()

                + "\t" + gameInfo.playerHp.ToString()
                + "\t" + gameInfo.playerHPIncrease.ToString()
                + "\t" + gameInfo.playerHPDecrease.ToString()

                + "\t" + gameInfo.playerSoda.ToString()
                + "\t" + gameInfo.playerSodaUse.ToString()
                + "\t" + gameInfo.playerSodaGet.ToString()

                + "\t" + gameInfo.deltaTime.ToString();

            print (info);
            WriteFile("gameLog.txt", info1);
#endif
            gameInfo.init();
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
//            sr.WriteLine("I can write ints {0} or floats {1}, and so on.", 1, 4.2);
            sr.Close();
        }

		public void setLevel()
		{
			if (gameState == GAME_STATE.OVER)
                instance.level = 1;
			else 
				instance.level++;
		}

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (instance != null)
            {
				instance.writeLog ();
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

            showTimeText = GameObject.Find("ShowTimeText").GetComponent<Text>();

            ChangeTitleText();

            enemies.Clear();

            //Call the SetupScene function of the BoardManager script, pass it current level number.
            boardScript.SetupScene(level);

            titleImage.SetActive(true);
            if (gameState == GAME_STATE.PLAY)
                Invoke("HideTitleImage", levelStartDelay);
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
                case GAME_STATE.OVER:
                    titleText.text = "GAME OVER";
                    break;

                case GAME_STATE.END:
                    titleText.text = "All levels cleared!";
                    break;

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

        bool IsInput()
        {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            bool touchReleased = false;
            for (int i = 0; i < Input.touches.Length; i++)
            {
                touchReleased = Input.touches[i].phase == TouchPhase.Ended;
                if (touchReleased) break;
            }

            if (touchReleased)
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
                case GAME_STATE.OVER:
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
                    WriteFile("gameLog.txt", "GAME START");
#endif
                    break;
            }

        }
		
		void Update()
		{            
            if (gameState != GAME_STATE.PLAY)
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
        
        public void GameOver()
		{
            gameState = GAME_STATE.OVER;
            playersTurn = false;
            gameOverCount++;
        }

        public void GameContinue()
        {
            gameState = GAME_STATE.PLAY;
            playersTurn = false;
            level--;
        }
        
        public void EndGame()
		{
			level = 1;
            gameState = GAME_STATE.END;
            playersTurn = false;
            gameEndCount++;
        }

        public bool ShowEnemies(bool bShow, bool blong = false)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Show(bShow);
            }

            ShowObjects(bShow);

            return true;
        }


        IEnumerator MoveEnemies()
		{
            float totalTime = 0.3f;
			enemiesMoving = true;
			
			yield return new WaitForSeconds(0.1f);
			
			if (enemies.Count == 0) 
			{
				yield return new WaitForSeconds(0.1f);
			}
			
			for (int i = 0; i < enemies.Count; i++)
			{
                enemies[i].MoveEnemy ();

                yield return new WaitForSeconds(0.02f);
                totalTime -= 0.02f;

            }

            yield return new WaitForSeconds(totalTime);
            playersTurn = true;
			
			enemiesMoving = false;
		}
	}
}

