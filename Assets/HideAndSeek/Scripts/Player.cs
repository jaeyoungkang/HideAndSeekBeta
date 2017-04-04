using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Analytics;

namespace HideAndSeek
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int pointsPerFood = 10;				//Number of points to add to player food points when picking up a food object.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
        
		public Text foodText;
        public Text ScoreText;
        public Text messageText;
        public Text gameEndText;

        public Canvas EndGroup;
        public Button yesBtn;
        public Button noBtn;

        public Button showBtn;
        public Button upBtn;
        public Button downBtn;
        public Button rightBtn;
        public Button leftBtn;
        enum MOVE_DIR { UP, DOWN, RIGHT, LEFT };

        public AudioClip moveSound1;
		public AudioClip moveSound2;
		public AudioClip eatSound1;
		public AudioClip eatSound2;
		public AudioClip drinkSound1;
		public AudioClip drinkSound2;
		public AudioClip gameOverSound;
        public AudioClip showSound;
        public AudioClip goldASound;
        public AudioClip levelClearSound;

        private Animator animator;
		private int hitPoint;
        private int Gold;
        private int Coin;        

		public void InitDatas()
		{
            hitPoint = GameManager.instance.playerHp;
            Gold = GameManager.instance.playerGold;
            Coin = GameManager.instance.playerCoin;

        }

        public void SetGoldText(Color gColor)
        {
            ScoreText.text = "Gold: " + Gold.ToString();
            ScoreText.color = gColor;
        }


        public void SetMessageText(string msg)
		{
			messageText.text = msg;
			messageText.enabled = true;
		}

		//Start overrides the Start function of MovingObject
		protected override void Start ()
		{
            animator = GetComponent<Animator>();            

			InitDatas();
			SetHPText(Color.white);
            SetGoldText(Color.white);
            SetMessageText ("");

            EndGroup.enabled = false;

//            showBtn.onClick.AddListener(UseSoda);
            upBtn.onClick.AddListener(MoveUp);
            downBtn.onClick.AddListener(MoveDown);
            leftBtn.onClick.AddListener(MoveLeft);
            rightBtn.onClick.AddListener(MoveRight);

            yesBtn.onClick.AddListener(Continue);
            noBtn.onClick.AddListener(Restart);

            base.Start ();
        }

        void MoveUp()
        {
            if (!GameManager.instance.Isplaying()) return;
            AttemptMove<Wall>(0, 1);
        }

        void MoveDown()
        {
            if (!GameManager.instance.Isplaying()) return;
            AttemptMove<Wall>(0, -1);
        }

        void MoveRight()
        {
            if (!GameManager.instance.Isplaying()) return;
            AttemptMove<Wall>(1, 0);
        }

        void MoveLeft()
        {
            if (!GameManager.instance.Isplaying()) return;
            AttemptMove<Wall>(-1, 0);
        }


        private void SetHPText(Color msgColor)
        {
            foodText.text = "HP: " + hitPoint;
            foodText.color = msgColor;
        }
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			GameManager.instance.playerHp = hitPoint;
            GameManager.instance.playerGold = Gold;
            GameManager.instance.playerCoin = Coin;
        }
		
		
		private void Update ()
		{
			//If it's not the player's turn, exit the function.
			if(!GameManager.instance.playersTurn) return;
            if (!GameManager.instance.Isplaying()) return;
            if (hitPoint <= 0) return;

            int horizontal = 0;  	//Used to store the horizontal move direction.
			int vertical = 0;		//Used to store the vertical move direction.
			
			//Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			
			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//Check if moving horizontally, if so set vertical to zero.
			if(horizontal != 0)
			{
				vertical = 0;
			}
			//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			////Check if Input has registered more than zero touches
			//if (Input.touchCount > 0)
			//{
			//	//Store the first touch detected.
			//	Touch myTouch = Input.touches[0];
				
			//	//Check if the phase of that touch equals Began
			//	if (myTouch.phase == TouchPhase.Began)
			//	{
			//		//If so, set touchOrigin to the position of that touch
			//		touchOrigin = myTouch.position;
			//	}
				
			//	//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
			//	else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
			//	{
			//		//Set touchEnd to equal the position of this touch
			//		Vector2 touchEnd = myTouch.position;
					
			//		//Calculate the difference between the beginning and end of the touch on the x axis.
			//		float x = touchEnd.x - touchOrigin.x;
					
			//		//Calculate the difference between the beginning and end of the touch on the y axis.
			//		float y = touchEnd.y - touchOrigin.y;
					
			//		//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
			//		touchOrigin.x = -1;
					
			//		//Check if the difference along the x axis is greater than the difference along the y axis.
			//		if (Mathf.Abs(x) > Mathf.Abs(y))
			//			//If x is greater than zero, set horizontal to 1, otherwise set it to -1
			//			horizontal = x > 0 ? 1 : -1;
			//		else
			//			//If y is greater than zero, set horizontal to 1, otherwise set it to -1
			//			vertical = y > 0 ? 1 : -1;
			//	}
			//}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
			//Check if we have a non-zero value for horizontal or vertical
			if(horizontal != 0 || vertical != 0)
			{
                //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
                //Pass in horizontal and vertical as parameters to specify the direction to move Player in.                
                AttemptMove<Wall> (horizontal, vertical);
                
            }
		}
		
		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
            SetMessageText("");
            GameManager.instance.gameInfo.moveTryCount++;
            SetHPText(Color.white);
            SetGoldText(Color.white);            

            base.AttemptMove <T> (xDir, yDir);
			RaycastHit2D hit;
			if (Move (xDir, yDir, out hit)) 
			{
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
				GameManager.instance.gameInfo.moveCount++;
			}
            else
            {
                if (hit.collider.tag == "Enemy")
                {
                    SetMessageText("무언가에 막혀서 갈 수 없다...");
                }
            }

			CheckIfGameOver ();
			
			GameManager.instance.playersTurn = false;
        }

		void SenseEnemy(int xDir, int yDir)
		{
			GameObject[] targets = GameObject.FindGameObjectsWithTag ("Enemy");
			bool bEnemy = false;
			foreach (GameObject target in targets) {
				float x = transform.position.x + xDir;
				float y = transform.position.y + yDir;
				float distanceX = Mathf.Abs (target.transform.position.x - x);
				float distanceY = Mathf.Abs (target.transform.position.y - y);

                if (distanceX <= 1 && distanceY <= 1)
						bEnemy = true;
				
			}

			if(bEnemy)
                SetMessageText("썩은 냄새가 나는 것 같다...");
			else
				SetMessageText ("");
        }

        protected override void OnCantMove <T> (T component)
		{
			Wall hitWall = component as Wall;
            hitWall.DamageWall (wallDamage);			
			animator.SetTrigger ("playerChop");          
        }            


        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D (Collider2D other)
		{
            if (other.tag == "Exit")
            {
                SoundManager.instance.RandomizeSfx(levelClearSound, levelClearSound);                
                GameManager.instance.ShowEnemies(true);
                Invoke("Restart", restartLevelDelay);
                enabled = false;
            }
            else if (other.tag == "Food")
            {
                hitPoint += pointsPerFood;

                SetHPText(Color.green);
                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
                SetMessageText("체력을 회복했다!");
                other.gameObject.SetActive(false);

                GameManager.instance.gameInfo.playerHPIncrease++;
            }
            else if (other.tag == "Soda")
            {
                GameManager.instance.ShowEnemies(true);
                SoundManager.instance.RandomizeSfx(showSound, showSound);
                SetMessageText("괴물들이 보인다!");
                other.gameObject.SetActive(false);
            }
            else if (other.tag == "Gold")
            {
                SoundManager.instance.RandomizeSfx(goldASound, goldASound);
                other.gameObject.SetActive(false);
                SetMessageText("골드를 획득했다!");
                Gold += 10;

                SetGoldText(Color.yellow);
            }
        }		
		
		private void Restart ()
		{
			GameManager.instance.gameInfo.playerHp = hitPoint;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		
		
		public void LoseFood (int loss)
		{
			animator.SetTrigger ("playerHit");			
			hitPoint -= loss;

            SetHPText(Color.red);
            SetMessageText("으악!");
            GameManager.instance.gameInfo.playerHPDecrease++;
			
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{            
			//Check if food point total is less than or equal to zero.
			if (hitPoint <= 0) 
			{
				ShowContiue();
			}
		}

        void Continue()
        {
            if (Coin == 0) return;
            hitPoint = 40;
            Coin--;
            GameManager.instance.GameContinue();
            Restart();
        }

        void GameOver()
        {
            hitPoint = 40;
            Gold = 0;

            GameManager.instance.GameOver();
            Restart();
        }

		void ShowContiue()
		{
            EndGroup.enabled = true;
            gameEndText.text = "Game Over!\n\ncontinue?";

            SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.ShowEnemies(true);  
        }

    }
}

