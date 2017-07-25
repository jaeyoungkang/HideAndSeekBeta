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
		public AudioClip gameOverSound;
        public AudioClip gemSound;
        public AudioClip itemSound;
        public AudioClip levelClearSound;
        public AudioClip skillSound;

        private Animator animator;

        public void Init()
        {
            enabled = true;
            transform.position = new Vector3(0, 0, 0);            
        }

        public void CheckStartPos()
        {
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
                Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.DAMAGE_TIME), 1f, Color.red);
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

                if (bHideMode) BreakedHiding(LocalizationManager.instance.GetLocalString(GAME_STRING.HIDE_BROKEN_FLOOR));
            }
            return showType;
        }

        private void ShowExplainTextTile(SHOW_TYPE showType)
        {
            Color textColor = Color.white;
            string msg = "";
            switch (showType)
            {
                case SHOW_TYPE.ALL: msg = LocalizationManager.instance.GetLocalString(GAME_STRING.FLOOR_SHOW_ALL); textColor = Color.gray; break;
                case SHOW_TYPE.GEM_ITEM: msg = LocalizationManager.instance.GetLocalString(GAME_STRING.FLOOR_SHOW_ITEM); textColor = Color.yellow; break;
                case SHOW_TYPE.NEAR: msg = LocalizationManager.instance.GetLocalString(GAME_STRING.FLOOR_SHOW_NEAR); textColor = Color.green; break;
                case SHOW_TYPE.MONSTER: msg = LocalizationManager.instance.GetLocalString(GAME_STRING.FLOOR_SHOW_MONSTER); textColor = Color.red; break;
                case SHOW_TYPE.TRAP: msg = LocalizationManager.instance.GetLocalString(GAME_STRING.FLOOR_SHOW_TRAP); textColor = Color.blue; break;
            }

            Notice.instance.Show(msg, 1f, textColor);
        }

        protected override void AttemptMove <T> (int xDir, int yDir)
		{
            GameManager.instance.ShowAllMap(false);
            GameManager.instance.StopTime(false);

            base.AttemptMove <T> (xDir, yDir);
			RaycastHit2D hit;
			if (Move (xDir, yDir, out hit)) 
			{
                if(bHideMode == false) SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
                GameManager.instance.ShowMap(new Vector3(transform.position.x + xDir, transform.position.y + yDir, 0), checkPos(xDir, yDir));
                CheckTrap(xDir, yDir);
            }
            else
            {
                GameManager.instance.ShowMap(new Vector3(transform.position.x, transform.position.y, 0), checkPos(0, 0));
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

                if (bHideMode) BreakedHiding(LocalizationManager.instance.GetLocalString(GAME_STRING.HIDE_BROKEN_BUMP));
            }

            if (hitEnemy.tag == "Thief")
            {
                Thief hitThief = component as Thief;
                animator.SetTrigger("playerChop");
                Animator thiefAnimator = hitThief.GetComponent<Animator>();
                if (thiefAnimator) thiefAnimator.SetTrigger("playerHit");
                
                SoundManager.instance.RandomizeSfx(attackedSound1, attackedSound2);
                GetGem(2);
                SoundManager.instance.PlaySingle(gemSound);
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
                SoundManager.instance.PlaySingle(gemSound);
                GetGem(1);
                StartCoroutine(HideAni(other.gameObject));

                GameManager.instance.dungeonPlayData.gemCount++;
                if (bHideMode) BreakedHiding(LocalizationManager.instance.GetLocalString(GAME_STRING.HIDE_BROKEN_GEM));

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
                if(item.itemId == ITEM_ID.EXTEND_MAX_HP)
                {
                    if(GameManager.instance.ExtendHp(1)) Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.INC_MAXHP), 1f, Color.yellow);
                    else Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.LIMIT_MAXHP), 1f, Color.yellow);
                    StartCoroutine(HideAni(other.gameObject));
                    SoundManager.instance.PlaySingle(skillSound);
                }
                else if (item.itemId == ITEM_ID.EXTEND_BAG)
                {
                    if(GameManager.instance.ExtendBagSize()) Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.INC_BAG), 1f, Color.yellow);
                    else Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.LIMIT_BAG), 1F, Color.white);
                    StartCoroutine(HideAni(other.gameObject));
                    SoundManager.instance.PlaySingle(skillSound);
                }
                else if (GameManager.instance.AddItemInBag(item.itemId))
                {                    
                    SoundManager.instance.PlaySingle(itemSound);
                    StartCoroutine(HideAni(other.gameObject));
                    Controller controller = FindObjectOfType(typeof(Controller)) as Controller;
                    controller.SetupSlots();

                    GameManager.instance.dungeonPlayData.getItems.Add(itemName);
                    if (bHideMode) BreakedHiding(LocalizationManager.instance.GetLocalString(GAME_STRING.HIDE_BROKEN_PICK_ITEM));

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
                    Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.NO_SPACE_BAG), 2f, Color.white);
                }
            }
        }

        void ShowExplainTextItem(ITEM_ID itemId)
        {
            string itemName = ItemManager.instance.GetNameByItemId(itemId);
            string explain = string.Format(LocalizationManager.instance.GetLocalString(GAME_STRING.FOUND_ITEM), itemName);
            Notice.instance.Show(explain, 2f, Color.white);
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
            if (bHideMode) BreakedHiding(LocalizationManager.instance.GetLocalString(GAME_STRING.HIDE_BROKEN_DAMAGE));
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
            if (bHideMode) BreakedHiding(LocalizationManager.instance.GetLocalString(GAME_STRING.HIDE_BROKEN_USE));
            SoundManager.instance.PlaySingle(skillSound);
            string name = ItemManager.instance.GetNameByItemId(GameManager.instance.bag[index]);
            ITEM_ID itemId = GameManager.instance.bag[index];

            GameManager.instance.dungeonPlayData.useItems.Add(name);
            switch (itemId)
            {
                case ITEM_ID.HIDE: Hide(); break;
                case ITEM_ID.HEAL1: GameManager.instance.RecoverHP(1); break;
                case ITEM_ID.HEAL2: GameManager.instance.RecoverHP(2); break;
                case ITEM_ID.HEAL3: GameManager.instance.RecoverHP(3); break;
                case ITEM_ID.FRAGMENT_NEAR:
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.NEAR);                    
                    break;
                case ITEM_ID.FRAGMENT_MONSTER:
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.MONSTER);
                    break;
                case ITEM_ID.FRAGMENT_TRAP:
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.TRAP);
                    break;
                case ITEM_ID.FRAGMENT_ITEM:
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.GEM_ITEM);
                    break;
                case ITEM_ID.FRAGMENT_ALL:
                    GameManager.instance.ShowMap(transform.position, SHOW_TYPE.ALL);
                    break;
                case ITEM_ID.DESTROY_4D:
                    GameManager.instance.DestoryEnemies(transform.position, ITEM_ID.DESTROY_4D);
                    break;
                case ITEM_ID.DESTROY_LR:
                    GameManager.instance.DestoryEnemies(transform.position, ITEM_ID.DESTROY_LR);
                    break;
                case ITEM_ID.DESTROY_UD:
                    GameManager.instance.DestoryEnemies(transform.position, ITEM_ID.DESTROY_UD);
                    break;
                case ITEM_ID.ADD_TIME:
                    GameManager.instance.AddTime(30);
                    break;

                case ITEM_ID.STOP_TIME:
                    GameManager.instance.StopTime(true);
                    break;

                case ITEM_ID.ESCAPE:
                    GameManager.instance.GotoDungeonMap();
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

