using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Analytics;

namespace HideAndSeek
{
    public enum MOVE_DIR { UP = 0, DOWN, RIGHT, LEFT };

    public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;
		public int potionA = 10;
        public int potionB = 20;

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

        public void Init()
        {
            enabled = true;
            transform.position = new Vector3(0, 0, 0);
            if (checkPos(0, 0)) GameManager.instance.ShowNear(transform.position);
        }

        protected override void Start ()
		{
            animator = GetComponent<Animator>();

            base.Start ();            
        }

        
        public void MoveByControlPanel(MOVE_DIR dir)
        {
            switch (dir)
            {
                case MOVE_DIR.UP: AttemptMove<Enemy>(0, 1); break;
                case MOVE_DIR.DOWN: AttemptMove<Enemy>(0, -1); break;
                case MOVE_DIR.LEFT: AttemptMove<Enemy>(-1, 0); break;
                case MOVE_DIR.RIGHT: AttemptMove<Enemy>(1, 0); break;
            }
        }
		
        private void Update ()
		{
            if (GameManager.instance == null) return;
            if (GameManager.instance.IsPlay() == false) return;
            if (GameManager.instance.playerHp <= 0) return;

            if (GameManager.instance.timeLimit <= 0)
            {
                GameManager.instance.timeLimit = 10;
                LoseHP(10);
                GameManager.instance.playData.damagedByTimeCount++;
            }

            if (!GameManager.instance.playersTurn) return;            

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

            return bShow;
        }

        protected override void AttemptMove <T> (int xDir, int yDir)
		{
            GameManager.instance.ShowMap(false);
            GameManager.instance.StopTime(false);

            base.AttemptMove <T> (xDir, yDir);
			RaycastHit2D hit;
			if (Move (xDir, yDir, out hit)) 
			{
                if(bHideMode == false) SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);

                if(checkPos(xDir, yDir)) GameManager.instance.ShowNear(new Vector3(transform.position.x + xDir, transform.position.y + yDir, 0));
                CheckTrap(xDir, yDir);

                GameManager.instance.playData.hps.Add(GameManager.instance.playerHp);
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

                GameManager.instance.playData.trappedCount++;
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
                GetGem(2);
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
            }
            else if (other.tag == "Gem")
            {
                Renderer renderer = other.gameObject.GetComponent<SpriteRenderer>();
                if (renderer) renderer.enabled = true;
                SoundManager.instance.RandomizeSfx(goldASound, goldASound);
                GetGem(1);
                StartCoroutine(HideAni(other.gameObject));

                GameManager.instance.playData.gemCount++;
            }
            else if (other.tag == "Item")
            {
                ItemObject item = other.gameObject.GetComponent<ItemObject>();
                if (GameManager.instance.AddBag(item.itemId))
                {                    
                    SoundManager.instance.RandomizeSfx(goldASound, goldASound);
                    StartCoroutine(HideAni(other.gameObject));
                    Controller controller = FindObjectOfType(typeof(Controller)) as Controller;
                    controller.SetupSlots();

                    string itemName = ItemManager.instance.GetNameByItemId(item.itemId);
                    GameManager.instance.playData.getItems.Add(itemName);
                }
            }
        }

        void GetGem(int count)
        {
            GameManager.instance.dungeonGem += count;
        }

        IEnumerator HideAni(GameObject obj)
        {
            yield return new WaitForSeconds(0.1f);

            obj.SetActive(false);
            GameManager.instance.RemoveObj(obj);
        }

		public void LoseHP (int loss)
		{
            SetHideMode(false);
            animator.SetTrigger ("playerHit");
            GameManager.instance.playerHp -= loss;

            CheckIfGameOver ();
		}
		
		
		private void CheckIfGameOver ()
		{            
			if (GameManager.instance.playerHp <= 0) 
			{
				GameOver();
			}
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
            SetHideMode(true);
        }

        void RecoverHP(int delta)
        {
            GameManager.instance.playerHp += delta;
        }

        public void UseSkill(int index)
        {
            if (index >= GameManager.instance.info.bag.Count) return;
            
            SoundManager.instance.PlaySingle(skillSound);
            string name = ItemManager.instance.GetNameByItemId(GameManager.instance.info.bag[index]);

            GameManager.instance.playData.useItems.Add(name);
            switch (name)
            {
                case "은신": Hide(); break;
                case "회복10": RecoverHP(10); break;
                case "회복20": RecoverHP(20); break;
                case "회복30": RecoverHP(30); break;
                case "근처보기":
                    GameManager.instance.ShowMap(transform.position, 0);
                    break;
                case "괴물보기":
                    GameManager.instance.ShowMap(transform.position, 1);
                    break;
                case "함정보기":
                    GameManager.instance.ShowMap(transform.position, 2);
                    break;
                case "보물보기":
                    GameManager.instance.ShowMap(transform.position, 3);
                    break;
                case "전체보기":
                    GameManager.instance.ShowMap(transform.position, 4);
                    break;
                case "사방공격":
                    GameManager.instance.DestoryEnemies(transform.position, 0);
                    break;
                case "좌우공격":
                    GameManager.instance.DestoryEnemies(transform.position, 1);
                    break;
                case "상하공격":
                    GameManager.instance.DestoryEnemies(transform.position, 2);
                    break;
                case "30초추가":
                    GameManager.instance.AddTime(30);
                    break;

                case "시간멈춤":
                    GameManager.instance.StopTime(true);
                    break;
            }
            GameManager.instance.info.bag.RemoveAt(index);
        }

        

        void GameOver()
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.ShowMap(true);            
            GameManager.instance.GameOver();
        }
    }
}

