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
            SHOW_TYPE type = checkPos(0, 0);
            GameManager.instance.ShowMap(transform.position, type);
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
                Notice.instance.Show("시간이 다되어서 피해를 입었다...", 1f, Color.red);
                GameManager.instance.timeLimit = 10;
                LoseHP(1);
                GameManager.instance.dungeonPlayData.damagedByTimeCount++;

                Analytics.CustomEvent("Damaged by Time", new Dictionary<string, object>
                {
                    { "Dungeon id", GameManager.instance.GetDungeonInfo().id},
                    { "Level id", GameManager.instance.GetDungeonInfo().GetCurLevel().id},
                    { "Damage point",  1},
                });
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

        SHOW_TYPE checkPos(int xDir, int yDir)
        {
            float nextPosX = transform.position.x + xDir;
            float nextPosY = transform.position.y + yDir;
            Vector2 pos = new Vector2(nextPosX, nextPosY);

            SHOW_TYPE showType = GameManager.instance.CheckShowTile(pos);

            if(showType != SHOW_TYPE.NONE)
            {
                if(GameManager.instance.GetDungeonInfo().id == 0) ShowExplainTextTile(showType);
                SoundManager.instance.PlaySingle(showSound);

                if (bHideMode) BreakedHiding("특수 바닥을 밟아서 은신이 풀렸다...");
            }
            return showType;
        }

        private void ShowExplainTextTile(SHOW_TYPE showType)
        {
            Color textColor = Color.white;
            string msg = "";
            switch (showType)
            {
                case SHOW_TYPE.ALL: msg = "검은색 바닥을 밟으니 방 전체가 보인다."; textColor = Color.gray; break;
                case SHOW_TYPE.GEM_ITEM: msg = "노란색  닥을 밟으니 아이템들이 보인다."; textColor = Color.yellow; break;
                case SHOW_TYPE.NEAR: msg = "초록색 바닥을 밟으니 주변이 보인다."; textColor = Color.green; break;
                case SHOW_TYPE.MONSTER: msg = "빨간색 바닥을 밟으니 괴물들이 보인다."; textColor = Color.red; break;
                case SHOW_TYPE.TRAP: msg = "파란색 바닥을 밟으니 함정들이 보인다."; textColor = Color.blue; break;
            }

            Notice.instance.Show(msg, 1f, textColor);
        }

        protected override void AttemptMove <T> (int xDir, int yDir)
		{
            SHOW_TYPE st = checkPos(xDir, yDir);
            if(st == SHOW_TYPE.NONE) GameManager.instance.ShowAllMap(false);
            else 
                GameManager.instance.ShowMap(new Vector3(transform.position.x + xDir, transform.position.y + yDir, 0), st);
            
            GameManager.instance.StopTime(false);

            base.AttemptMove <T> (xDir, yDir);
			RaycastHit2D hit;
			if (Move (xDir, yDir, out hit)) 
			{
                if(bHideMode == false) SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
                
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

                if(trap.name.Contains("trapLv2")) LoseHP(2);
                else LoseHP(1);
                SoundManager.instance.RandomizeSfx(attackedSound1, attackedSound2);

                GameManager.instance.dungeonPlayData.damagedBytrapCount++;

                Analytics.CustomEvent("Damaged by Trap", new Dictionary<string, object>
                {
                    { "Dungeon id", GameManager.instance.GetDungeonInfo().id},
                    { "Level id", GameManager.instance.GetDungeonInfo().GetCurLevel().id},
                    { "Damage point",  1},
                });
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

                if (bHideMode) BreakedHiding("무언가에 부딪쳐서 은신이 풀렸다...");
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
                GameManager.instance.ShowAllMap(true);
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

                GameManager.instance.dungeonPlayData.gemCount++;
                if (bHideMode) BreakedHiding("보석을 줍는 소리에 은신이 풀렸다...");

                Analytics.CustomEvent("Discover Gem", new Dictionary<string, object>
                {
                    { "Dungeon id", GameManager.instance.GetDungeonInfo().id},
                    { "Level id", GameManager.instance.GetDungeonInfo().GetCurLevel().id},
                });
            }
            else if (other.tag == "Item")
            {
                ItemObject item = other.gameObject.GetComponent<ItemObject>();

                string itemName = ItemManager.instance.GetNameByItemId(item.itemId);
                if(itemName == "최대체력")
                {
                    if(GameManager.instance.ExtendHp(1)) Notice.instance.Show("최대 체력이 늘어났다.", 1f, Color.yellow);
                    else Notice.instance.Show("이미 최대 체력이다.", 1f, Color.yellow);
                    StartCoroutine(HideAni(other.gameObject));
                }
                else if (itemName == "가방확장")
                {
                    if(GameManager.instance.ExtendBagSize()) Notice.instance.Show("가방 공간이 늘어났다.", 1f, Color.yellow);
                    else Notice.instance.Show("이미 최대치이다...", 1F, Color.white);
                    StartCoroutine(HideAni(other.gameObject));
                }
                else if (GameManager.instance.AddItemInBag(item.itemId))
                {                    
                    SoundManager.instance.RandomizeSfx(goldASound, goldASound);
                    StartCoroutine(HideAni(other.gameObject));
                    Controller controller = FindObjectOfType(typeof(Controller)) as Controller;
                    controller.SetupSlots();

                    GameManager.instance.dungeonPlayData.getItems.Add(itemName);
                    if (bHideMode) BreakedHiding("아이템을 줍는 소리에 은신이 풀렸다...");

                    Analytics.CustomEvent("Discover Item", new Dictionary<string, object>
                    {
                        { "Item id", item.itemId},
                        { "Dungeon id", GameManager.instance.GetDungeonInfo().id},
                        { "Level id", GameManager.instance.GetDungeonInfo().GetCurLevel().id},
                    });
                    if (GameManager.instance.GetDungeonInfo().id == 0) ShowExplainTextItem(item.itemId);
                }
                else
                {
                    Notice.instance.Show("가방에 공간이 부족하다.", 2f, Color.white);
                }
            }
        }

        void ShowExplainTextItem(int itemId)
        {
            string itemName = ItemManager.instance.GetNameByItemId(itemId);
            Notice.instance.Show(itemName + "을 발견했다.", 2f, Color.white);
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

        void BreakedHiding(string reason)
        {            
            Notice.instance.Show(reason, 2f, Color.white);
            SetHideMode(false);
        }

		public void LoseHP (int loss)
		{
            if (bHideMode) BreakedHiding("피해를 입어서 은신이 풀렸다...");
            animator.SetTrigger ("playerHit");
            GameManager.instance.LoseHp(loss);
            
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

        public void UseItem(int index)
        {
            if (index >= GameManager.instance.bag.Count) return;
            if (bHideMode) BreakedHiding("아이템 사용하는 소리에 은신이 풀렸다...");
            SoundManager.instance.PlaySingle(skillSound);
            string name = ItemManager.instance.GetNameByItemId(GameManager.instance.bag[index]);

            GameManager.instance.dungeonPlayData.useItems.Add(name);
            switch (name)
            {
                case "은신": Hide(); break;
                case "회복10": GameManager.instance.RecoverHP(1); break;
                case "회복20": GameManager.instance.RecoverHP(2); break;
                case "회복30": GameManager.instance.RecoverHP(3); break;
                case "근처보기":
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.NEAR);
                    break;
                case "괴물보기":
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.MONSTER);
                    break;
                case "함정보기":
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.TRAP);
                    break;
                case "보물보기":
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.GEM_ITEM);
                    break;
                case "전체보기":
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.ALL);
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

            Analytics.CustomEvent("Use Item", new Dictionary<string, object>
            {
                { "Item id", GameManager.instance.bag[index]},
                { "Dungeon id", GameManager.instance.GetDungeonInfo().id},
                { "Level id", GameManager.instance.GetDungeonInfo().GetCurLevel().id},
            });

            GameManager.instance.bag.RemoveAt(index);            
        }
        
        void GameOver()
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.ShowAllMap(true);            
            GameManager.instance.GameOver();
        }
    }
}

