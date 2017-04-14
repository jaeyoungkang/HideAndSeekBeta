﻿using UnityEngine;
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
        
        public int costDestroy = 4;
        public int costShow = 1;
        public int costHP = 2;
        public int costHide = 3;

        public Text foodText;
        public Text ScoreText;
        public Text TimeText;
        public Text gameEndText;        

        public Canvas EndGroup;

        public Button HealBtn;
        public Button ShowBtn;
        public Button HideBtn;
        public Button DestroyBtn;
        

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
            ScoreText.text = gem.ToString();
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
            HideBtn.onClick.AddListener(Hide);

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
                    SetHideMode(false);
                }
            }       

            if (!GameManager.instance.playersTurn) return;

            if (hitPoint <= 0) return;

            int horizontal = 0;  	//Used to store the horizontal move direction.
			int vertical = 0;		//Used to store the vertical move direction.

			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			if(horizontal != 0)
			{
				vertical = 0;
			}
			
			if(horizontal != 0 || vertical != 0)
			{
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

            if(bShow) SetHideMode(false);
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
                if(bHideMode == false) SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);

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
                SetHideMode(false);
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
                SetHideMode(false);
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
                SetHideMode(false);                
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

        bool bHideMode = false;

        void SetHideMode(bool bHide)
        {
            if(bHideMode && bHide == false)
            {
                GameManager.instance.SetSearchEnemies(!bHide);
                SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                if(renderer)
                {
                    Color playerColor = renderer.color;
                    playerColor.a = 1f;
                    renderer.color = playerColor;
                }
            }
            else if (bHide && bHideMode == false)
            {
                GameManager.instance.SetSearchEnemies(!bHide);
                SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                if (renderer)
                {
                    Color playerColor = renderer.color;
                    playerColor.a = 0.5f;                    
                    renderer.color = playerColor;
                }
            }

            bHideMode = bHide;
        }

        void Hide()
        {
            if (bHideMode) return;
            if (gem >= costHide)
            {
                GameManager.instance.gameInfo.skillHide++;
                SetHideMode(true);                
                UseGem(costHide);                
                SoundManager.instance.PlaySingle(skillSound);
            }
        }

        void ShowMap()
        {            
            if (GameManager.instance.IsShowing()) return;
            if (gem >= costShow)
            {
                GameManager.instance.gameInfo.skillShow++;
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
                GameManager.instance.DestoryEnemies(transform.position);
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

