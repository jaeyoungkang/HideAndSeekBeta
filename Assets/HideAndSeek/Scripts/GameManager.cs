using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

namespace HideAndSeek
{
	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;                   //Allows us to use UI.

    public class GameManager : MonoBehaviour
    {
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

        enum GAME_STATE { START, OVER, END };

        //Awake is always called before any Start functions
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //Assign enemies to a new List of Enemy objects.
            enemies = new List<Enemy>();

            //Get a component reference to the attached BoardManager script
            boardScript = GetComponent<BoardManager>();

            //Call the InitGame function to initialize the first level 
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

        //This is called each time a scene is loaded.
        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (instance != null)
            {
                instance.level++;
                instance.InitGame();
            }
        }

        //Initializes the game for each level.
        void InitGame()
        {
            if (level == 11)
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

            Analytics.CustomEvent("InitGame", new Dictionary<string, object>
            {
                { "level", level},
                { "enemies", enemies.Count}
            });
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
				levelText.text = "Level " + level;
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
                if (Input.GetKeyUp (KeyCode.Space))
#endif
                {
                    gameStart = false;
					ChangeTitleText ();
					Invoke("HideLevelImage", levelStartDelay);
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
			level = 0;
			gameOver = true;
            playersTurn = false;
        }

		public void EndGame()
		{
			level = 1;
			gameEnd = true;
            playersTurn = false;
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
			//While enemiesMoving is true player is unable to move.
			enemiesMoving = true;
			
			//Wait for turnDelay seconds, defaults to .1 (100 ms).
			yield return new WaitForSeconds(0.5f);
			
			//If there are no enemies spawned (IE in first level):
			if (enemies.Count == 0) 
			{
				//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
				yield return new WaitForSeconds(0);
			}
			
			//Loop through List of Enemy objects.
			for (int i = 0; i < enemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				enemies[i].MoveEnemy ();
				
				//Wait for Enemy's moveTime before moving next Enemy, 
				yield return new WaitForSeconds(0.01f);
			}
			//Once Enemies are done moving, set playersTurn to true so player can move.
			playersTurn = true;
			
			//Enemies are done moving, set enemiesMoving to false.
			enemiesMoving = false;
		}
	}
}

