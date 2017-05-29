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
        
        public int costDestroy = 2;
        public int costShow = 1;
        public int costHP = 1;
        public int costHide = 1;
        
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

        protected override void Start ()
		{   
            animator = GetComponent<Animator>();

            GameManager.instance.SetHPText(Color.white);
            GameManager.instance.SetGemText(Color.white);

            upBtn.onClick.AddListener(MoveUp);
            downBtn.onClick.AddListener(MoveDown);
            leftBtn.onClick.AddListener(MoveLeft);
            rightBtn.onClick.AddListener(MoveRight);

            HealBtn.onClick.AddListener(UseHPRecover);
            DestroyBtn.onClick.AddListener(DestoryEnemy);
            ShowBtn.onClick.AddListener(ShowMap);
            HideBtn.onClick.AddListener(Hide);

            base.Start ();
            if (checkPos(0, 0)) GameManager.instance.ShowNear(transform.position);
        }

        void MoveUp()
        {
            AttemptMove<Enemy>(0, 1);
        }

        void MoveDown()
        {
            AttemptMove<Enemy>(0, -1);
        }

        void MoveRight()
        {
            AttemptMove<Enemy>(1, 0);
        }

        void MoveLeft()
        {
            AttemptMove<Enemy>(-1, 0);
        }
		
        private void Update ()
		{   
            if (GameManager.instance.timeLimit <= 0)
            {
                GameManager.instance.timeLimit = 10;
                LoseHP(10);
                SoundManager.instance.RandomizeSfx(attackedSound1, attackedSound2);
                SetHideMode(false);
            }      

            if (!GameManager.instance.playersTurn) return;

            if (GameManager.instance.playerHp <= 0) return;

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

            Vector3[] range = GameManager.instance.GetShowPositions();
            foreach(Vector3 pos in range)
            {
                if(nextPosX == pos.x && nextPosY == pos.y) bShow = true;
            }

            if(bShow) SetHideMode(false);
            return bShow;
        }

        protected override void AttemptMove <T> (int xDir, int yDir)
		{
            GameManager.instance.SetHPText(Color.white);
            GameManager.instance.SetGemText(Color.white);            
            GameManager.instance.ShowMap(false);

            base.AttemptMove <T> (xDir, yDir);
			RaycastHit2D hit;
			if (Move (xDir, yDir, out hit)) 
			{
                if(bHideMode == false) SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);

                if(checkPos(xDir, yDir)) GameManager.instance.ShowNear(new Vector3(transform.position.x + xDir, transform.position.y + yDir, 0));
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
                LoseHP(10);
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
                GameManager.instance.ShowMap(true);
                SoundManager.instance.RandomizeSfx(levelClearSound, levelClearSound);
				GameManager.instance.ShowResult();
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
            GameManager.instance.playerGem += count;
            GameManager.instance.SetGemText(Color.green);
        }

        IEnumerator HideAni(GameObject obj)
        {
            yield return new WaitForSeconds(0.1f);

            obj.SetActive(false);
        }

		public void LoseHP (int loss)
		{
            animator.SetTrigger ("playerHit");
            GameManager.instance.playerHp -= loss;

            GameManager.instance.SetHPText(Color.red);

            CheckIfGameOver ();
		}
		
		
		private void CheckIfGameOver ()
		{            
			if (GameManager.instance.playerHp <= 0) 
			{
				GameOver();
			}
		}

        void UseGem(int count)
        {
            GameManager.instance.playerGem -= count;
            GameManager.instance.SetGemText(Color.red);
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
            if (GameManager.instance.playerGem >= costHide)
            {
                SetHideMode(true);                
                UseGem(costHide);                
                SoundManager.instance.PlaySingle(skillSound);
            }
        }

        void ShowMap()
        {            
            if (GameManager.instance.IsShowing()) return;
            if (GameManager.instance.playerGem >= costShow)
            {
                UseGem(costShow);
                GameManager.instance.ShowMap(transform.position);
                SoundManager.instance.PlaySingle(skillSound);
            }
        }

        void DestoryEnemy()
        {
            if(GameManager.instance.playerGem >= costDestroy)
            {
                UseGem(costDestroy);
                GameManager.instance.DestoryEnemies(transform.position);
                SoundManager.instance.PlaySingle(skillSound);
            }
        }        

        void UseHPRecover()
        {
            if (GameManager.instance.playerGem >= costHP)
            {
                RecoverHP(10);
                UseGem(costHP);                
                SoundManager.instance.PlaySingle(skillSound);
            }
        }

        void RecoverHP(int delta)
        {
            GameManager.instance.playerHp += delta;

            GameManager.instance.SetHPText(Color.green);            
        }

        void GameOver()
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.ShowMap(true);            
            GameManager.instance.GameOver();
        }
    }
}

