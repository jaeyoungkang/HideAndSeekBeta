using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.IO;
using System;


namespace HideAndSeek
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;                   //Allows us to use UI.

    public class GameManager : MonoBehaviour
    {
        public int gameOverCount = 0;
        public int gameEndCount = 0;

        public int playerHPDecrease = 0;
        public int playerHPIncrease = 0;

        public int playerSodaUse = 0;
        public int playerSodaGet = 0;

        public int playerHp = 0;
		public int playerSoda = 0;

		public int moveTryCount = 0;
		public int moveCount = 0;

        int deltaTime = 0;

        float prevTime = 0f;        

        public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
        public float turnDelay = 0.01f;                          //Delay between each Player turn.
        public int playerFoodPoints = 20;						//Starting value for Player food points.
        public int playerSodaPoints = 1;
        public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.

        private Text subTitleText;
        private Text levelText;                                 //Text to display current level number.
        private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
        private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
        private int level = 1;                                  //Current level number, expressed in game as "Day 1".
        private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
        private bool enemiesMoving;                             //Boolean to check if enemies are moving.
        private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
        private bool gameOver = false;
        private bool gameEnd = false;
        private bool gameStart = true;

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //Assign enemies to a new List of Enemy objects.
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
			deltaTime = (int)(Time.time - prevTime);
			prevTime = Time.time;
            Dictionary<string, object> eventInfo = new Dictionary<string, object>
                        {
                            { "level", level},
                            { "move", moveCount},
                            { "move try", moveTryCount},
                            { "HP", playerHp},
                            { "HP Inc", playerHPIncrease},
                            { "HP Dec", playerHPDecrease},
                            { "Soda", playerSoda},
                            { "SodaUse", playerSodaUse},
                            { "SodaGet", playerSodaGet},
                            { "time", deltaTime}
                        };

            Analytics.CustomEvent("Level info", eventInfo);

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
#else
            string info =
                "Level: " + level.ToString()
                + " Move: " + moveCount.ToString()
                + " Move try: " + moveTryCount.ToString()

                + " HP: " + playerHp.ToString()
                + " HP Inc: " + playerHPIncrease.ToString()
                + " HP Dec: " + playerHPDecrease.ToString()

                + " Soda: " + playerSoda.ToString()
                + " Soda Use: " + playerSodaUse.ToString()
                + " Soda Get: " + playerSodaGet.ToString()

                + " Time: " + deltaTime.ToString();

            string info1 =
                level.ToString()
                + "\t" + moveCount.ToString()
                + "\t" + moveTryCount.ToString()

                + "\t" + playerHp.ToString()
                + "\t" + playerHPIncrease.ToString()
                + "\t" + playerHPDecrease.ToString()

                + "\t" + playerSoda.ToString()
                + "\t" + playerSodaUse.ToString()
                + "\t" + playerSodaGet.ToString()

                + "\t" + deltaTime.ToString();

            print (info);
            WriteFile("gameLog.txt", info1);
#endif
            moveCount = 0;
			moveTryCount = 0;

            playerHPDecrease = 0;
            playerHPIncrease = 0;

            playerSodaUse = 0;
            playerSodaGet = 0;
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
			if (gameOver)
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
            if (level == 14)
                EndGame();

            //While doingSetup is true the player can't move, prevent player from moving while title card is up.
            doingSetup = true;

            levelImage = GameObject.Find("LevelImage");

            //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
            levelText = GameObject.Find("LevelText").GetComponent<Text>();
            subTitleText = GameObject.Find("SubTitleText").GetComponent<Text>();

            ChangeTitleText();

            //Clear any Enemy objects in our List to prepare for next level.
            enemies.Clear();

            //Call the SetupScene function of the BoardManager script, pass it current level number.
            boardScript.SetupScene(level);

            levelImage.SetActive(true);
            if (!gameStart && !gameEnd && !gameOver)
                Invoke("HideLevelImage", levelStartDelay);
        }

        public bool Isplaying()
        {
            return (!gameStart && !gameEnd && !gameOver && !doingSetup);
        }

		void HideLevelImage()
		{
			//Disable the levelImage gameObject.
			levelImage.SetActive(false);
			
			//Set doingSetup to false allowing player to move again.
			doingSetup = false;

            ShowEnemies();
        }

		void ChangeTitleText ()
		{
			subTitleText.enabled = true;
			if (gameOver)
				levelText.text = "GAME OVER";
			else if (gameEnd)
				levelText.text = "All levels cleared!";
			else if (gameStart)
				levelText.text = "Hide and Seek beta";
			else {
                if(level == 11) levelText.text = "Last Level 1/3";
                else if (level == 12) levelText.text = "Last Level 2/3";
                else if (level == 13) levelText.text = "Last Level 3/3";
                else levelText.text = "Level " + level;

				subTitleText.enabled = false;
			}
			
		}
		
		//Update is called every frame.
		void Update()
		{
			if (gameEnd) {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                bool touchReleased = false;
                for (int i = 0; i < Input.touches.Length; i++)
                {
                    touchReleased = Input.touches[i].phase == TouchPhase.Ended;
                    if (touchReleased) break;
                }

                if (touchReleased)
#else
                if (Input.GetKeyUp (KeyCode.Space))
#endif
                { 
                    gameEnd = false;
					gameStart = true;
					ChangeTitleText ();
				}
			}
			else if (gameOver) {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
                bool touchReleased = false;
                for (int i = 0; i < Input.touches.Length; i++)
                {
                    touchReleased = Input.touches[i].phase == TouchPhase.Ended;
                    if (touchReleased) break;
                }

                if (touchReleased)
#else
                if (Input.GetKeyUp (KeyCode.Space)) 
#endif
                { 
                    gameOver = false;
					gameStart = true;
					ChangeTitleText ();
				}
			}
			else if (gameStart) {
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
                {
                    gameStart = false;
                    ChangeTitleText();
                    Invoke("HideLevelImage", levelStartDelay);

                    Analytics.CustomEvent("gameStart", new Dictionary<string, object>
                    {
                        { "GameOverCount", gameOverCount},
                        { "GameEndCount", gameOverCount}
                    });
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
#else
                    WriteFile("gameLog.txt", "GAME START");
#endif
                }
			}
            //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
            if (playersTurn || enemiesMoving || doingSetup)
				
				//If any of these are true, return and do not start MoveEnemies.
				return;

            //Start moving enemies.
            StartCoroutine (MoveEnemies ());            
        }
		
		//Call this to add the passed in Enemy to the List of Enemy objects.
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}

    

    //GameOver is called when the player reaches 0 food points
    public void GameOver()
		{
			gameOver = true;
            playersTurn = false;
            gameOverCount++;
        }

		public void EndGame()
		{
			level = 1;
			gameEnd = true;
            playersTurn = false;
            gameEndCount++;
        }
          
        public void ShowEnemies()
        {
            bool hard = false;
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Show(hard);
            }
        }

        //Coroutine to move enemies in sequence.
        IEnumerator MoveEnemies()
		{
            float totalTime = 0.2f;
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;
			
			yield return new WaitForSeconds(0.1f);
			
			//If there are no enemies spawned (IE in first level):
			if (enemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(0.1f);
			}
			
			//Loop through List of Enemy objects.
			for (int i = 0; i < enemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				enemies[i].MoveEnemy ();
				
				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(0.02f);
                totalTime -= 0.02f;

            }

            yield return new WaitForSeconds(totalTime);
            playersTurn = true;
			
			//Enemies are done moving, set enemiesMoving to false.
			enemiesMoving = false;
		}
	}
}

