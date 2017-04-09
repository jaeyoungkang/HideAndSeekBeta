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
		public float restartLevelDelay = 1f;
		public int potionA = 10;
        public int potionB = 20;
        
        public int costDestroy = 8;
        public int costShow = 4;
        public int costHP = 2;

        public Text foodText;
        public Text ScoreText;
        public Text TimeText;
        public Text gameEndText;        

        public Canvas EndGroup;

        public Button HealBtn;
        public Button DestroyBtn;
        public Button ShowBtn;

        public Button upBtn;
        public Button downBtn;
        public Button rightBtn;
        public Button leftBtn;
        enum MOVE_DIR { UP, DOWN, RIGHT, LEFT };

        public AudioClip attackedSound1;
        public AudioClip attackedSound2;
        public AudioClip moveSound1;
		public AudioClip moveSound2;
		public AudioClip eatSound1;
		public AudioClip drinkSound1;
		public AudioClip gameOverSound;
        public AudioClip showSound;
        public AudioClip goldASound;
        public AudioClip levelClearSound;
        public AudioClip skillSound;

        private Animator animator;
		private int hitPoint;
        private int gem;
        
        public float prevTime = 0;
        float timeLimit = 60;

        public void InitDatas()
		{
            hitPoint = GameManager.instance.playerHp;
            gem = GameManager.instance.playerGem;
        }

        public void SetScoreText(Color gColor)
        {
            ScoreText.text = "Gem:" + gem.ToString();
            ScoreText.color = gColor;
        }                 
        
        protected override void Start ()
		{
            timeLimit = 60;
            prevTime = Time.time;
            GameManager.instance.waitTimes.Clear();
            animator = GetComponent<Animator>();            

			InitDatas();
			SetHPText(Color.white);
            SetScoreText(Color.white);

            EndGroup.enabled = false;

            upBtn.onClick.AddListener(MoveUp);
            downBtn.onClick.AddListener(MoveDown);
            leftBtn.onClick.AddListener(MoveLeft);
            rightBtn.onClick.AddListener(MoveRight);

            HealBtn.onClick.AddListener(UseHPRecover);
            DestroyBtn.onClick.AddListener(DestoryEnemy);
            ShowBtn.onClick.AddListener(ShowMap);

            base.Start ();
        }

        void MoveUp()
        {
            if (!GameManager.instance.Isplaying()) return;
            AttemptMove<Enemy>(0, 1);
        }

        void MoveDown()
        {
            if (!GameManager.instance.Isplaying()) return;
            AttemptMove<Enemy>(0, -1);
        }

        void MoveRight()
        {
            if (!GameManager.instance.Isplaying()) return;
            AttemptMove<Enemy>(1, 0);
        }

        void MoveLeft()
        {
            if (!GameManager.instance.Isplaying()) return;
            AttemptMove<Enemy>(-1, 0);
        }


        private void SetHPText(Color msgColor)
        {
            foodText.text = "HP:" + hitPoint;
            foodText.color = msgColor;
        }
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			GameManager.instance.playerHp = hitPoint;
            GameManager.instance.playerGem = gem;
        }

        private void Update ()
		{
            if (GameManager.instance.IsGameOver() && GameManager.instance.IsInput())
            {
                GameManager.instance.StartGame();
                Restart();
            }
            
            if (!GameManager.instance.Isplaying())
            {                
                return;
            }
            else
            {
                timeLimit -= Time.deltaTime;
                TimeText.text = Mathf.Floor(timeLimit).ToString();
                if (timeLimit <= 10) TimeText.color = Color.red;
                else TimeText.color = Color.white;

                if (timeLimit <= 0)
                {
                    timeLimit = 10;
                    LoseFood(10);
                    SoundManager.instance.RandomizeSfx(attackedSound1, attackedSound2);
                }
            }       

            if (!GameManager.instance.playersTurn) return;

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
                AttemptMove<Enemy> (horizontal, vertical);
                
            }
		}

        bool checkPos(int xDir, int yDir)
        {
            bool bShow = false;
            float nextPosX = transform.position.x + xDir;
            float nextPosY = transform.position.y + yDir;

            if (nextPosX == 0 && nextPosY == 0) bShow = true;
            else if (nextPosX == 7 && nextPosY == 7) bShow = true;
            else if (nextPosX == 0 && nextPosY == 7) bShow = true;
            else if (nextPosX == 7 && nextPosY == 0) bShow = true;
            return bShow;
        }
                
        
        protected override void AttemptMove <T> (int xDir, int yDir)
		{
            int deltaTime = (int)(Time.time - prevTime);            
            prevTime = Time.time;
            GameManager.instance.waitTimes.Add(deltaTime);

            SetHPText(Color.white);
            SetScoreText(Color.white);            
            GameManager.instance.ShowMap(false);

            base.AttemptMove <T> (xDir, yDir);
			RaycastHit2D hit;
			if (Move (xDir, yDir, out hit)) 
			{
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
				GameManager.instance.gameInfo.moveCount++;
                if(checkPos(xDir, yDir)) GameManager.instance.ShowMap(true);
                CheckTrap(xDir, yDir);               
            }          

			CheckIfGameOver ();
			
			GameManager.instance.playersTurn = false;
        }

        void CheckTrap(int xDir, int yDir)
        {
            float nextPosX = transform.position.x + xDir;
            float nextPosY = transform.position.y + yDir;

            GameObject trap = GameManager.instance.IsTrap(nextPosX, nextPosY);

            if (trap)
            {
                Renderer renderer = trap.GetComponent<SpriteRenderer>();
                if (renderer) renderer.enabled = true;
                LoseFood(10);
                SoundManager.instance.RandomizeSfx(attackedSound1, attackedSound2);
            }
        }


        protected override void OnCantMove <T> (T component)
		{            
			Enemy hitEnemy = component as Enemy;

            SpriteRenderer sprite = hitEnemy.GetComponent<SpriteRenderer>();
            if (sprite)
            {
                Color color = sprite.material.color;
                color.a = 1.0f;
                sprite.material.color = color;
            }

            if (hitEnemy.tag == "Thief")
            {
                Thief hitThief = component as Thief;
                animator.SetTrigger("playerChop");
                Animator thiefAnimator = hitThief.GetComponent<Animator>();
                if (thiefAnimator) thiefAnimator.SetTrigger("playerHit");
                
                SoundManager.instance.RandomizeSfx(attackedSound1, attackedSound2);
                GetGold(2);
                SoundManager.instance.RandomizeSfx(goldASound, goldASound);
                Analytics.CustomEvent("Attack Thief", new Dictionary<string, object>{{ "Attack Thief", 1}});

                hitThief.Hitted();
            }            
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Exit")
            {
                SoundManager.instance.RandomizeSfx(levelClearSound, levelClearSound);
                Invoke("Restart", restartLevelDelay);
                enabled = false;
                GameManager.instance.writeLog();
            }
            else if (other.tag == "Gem")
            {
                Renderer renderer = other.gameObject.GetComponent<SpriteRenderer>();
                if (renderer) renderer.enabled = true;
                SoundManager.instance.RandomizeSfx(goldASound, goldASound);
                GetGold(1);
                StartCoroutine(HideAni(other.gameObject));
            }
        }

        void GetGold(int count)
        {
            gem += count;
            SetScoreText(Color.green);
            GameManager.instance.gameInfo.goldGet += count;
        }

        IEnumerator HideAni(GameObject obj)
        {
            yield return new WaitForSeconds(0.1f);

            obj.SetActive(false);
        }

        private void Restart ()
		{
            GameManager.instance.gameInfo.gold = gem;
            GameManager.instance.gameInfo.playerHp = hitPoint;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		
		
		public void LoseFood (int loss)
		{
            animator.SetTrigger ("playerHit");
			hitPoint -= loss;

            SetHPText(Color.red);
            GameManager.instance.gameInfo.playerHPDecrease += loss;
            
            CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{            
			//Check if food point total is less than or equal to zero.
			if (hitPoint <= 0) 
			{
				GameOver();
			}
		}

        void UseGem(int count)
        {
            gem -= count;
            SetScoreText(Color.red);
        }        

        void ShowMap()
        {
            if (gem >= costShow)
            {
                GameManager.instance.gameInfo.skillHP++;
                UseGem(costShow);
                GameManager.instance.ShowMap(true);
                SoundManager.instance.PlaySingle(skillSound);
            }
        }

        void DestoryEnemy()
        {
            if(gem >= costDestroy)
            {
                GameManager.instance.gameInfo.skillDestroy++;
                UseGem(costDestroy);
                GameManager.instance.DestoryEnemies();
                SoundManager.instance.PlaySingle(skillSound);
            }
        }        

        void UseHPRecover()
        {
            if (gem >= costHP)
            {
                GameManager.instance.gameInfo.skillHP++;
                RecoverHP(10);
                UseGem(costHP);                
                SoundManager.instance.PlaySingle(skillSound);
            }
        }

        void RecoverHP(int delta)
        {
            hitPoint += delta;

            SetHPText(Color.green);            
            GameManager.instance.gameInfo.playerHPIncrease += delta;            
        }

        void GameOver()
        {
            EndGroup.enabled = true;
            gameEndText.text = "Game Over!";

            SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.ShowMap(true);

            hitPoint = 20;
            gem = 0;

            GameManager.instance.GameOver();
        }
    }
}

