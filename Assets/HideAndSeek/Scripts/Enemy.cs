using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.Analytics;

namespace HideAndSeek
{
	public class Enemy : MovingObject
	{
		public int rangeOfSense = 1;
		public int playerDamage;
		public AudioClip attackSound1;
		public AudioClip attackSound2;
        public AudioClip noticeSound;

        private Animator animator;
        protected Transform target;
        private bool skipMove = false;

        private bool bSearch = true;
        private bool bSpottedPlayer = false;

        public void SetSearch ( bool value ) { bSearch = value; }

        protected override void Start ()
		{
			animator = GetComponent<Animator> ();
			target = GameObject.FindGameObjectWithTag ("Player").transform;
            SetSearch(true);
            base.Start ();
            Show(false);
        }

		public virtual void MoveEnemy ()
		{
            if (skipMove)
            {
                skipMove = false;
                return;
            }

            if (bSearch == false)
            {
                bSpottedPlayer = false;
                return;
            }

            int xDir = 0;
            int yDir = 0;

            int distanceX = (int)Mathf.Abs(target.position.x - transform.position.x);
            int distanceY = (int)Mathf.Abs(target.position.y - transform.position.y);
            if (distanceX + distanceY <= rangeOfSense)
            {
                float value = Random.Range(0f, 1f);
                if (value < 0.5f)
                {
                    if (distanceX == 0)
                        yDir = target.position.y > transform.position.y ? 1 : -1;
                    else
                        xDir = target.position.x > transform.position.x ? 1 : -1;
                }
                else
                {
                    if (distanceY == 0)
                        xDir = target.position.x > transform.position.x ? 1 : -1;
                    else
                        yDir = target.position.y > transform.position.y ? 1 : -1;
                }

                if(bSpottedPlayer == false)
                {
                    SoundManager.instance.PlaySingle(noticeSound);
                    bSpottedPlayer = true;
                    Notice.instance.Show("무언가 썩고있는 냄새가 난다...", 2f, Color.red);
                }
            }
            else
            {
                bSpottedPlayer = false;
            }

            AttemptMove <Player> (xDir, yDir);
		}
        
        public void Show(bool bShow)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();

            if (sprite)
            {
                Color color = sprite.material.color;
                if (bShow) color.a = 1.0f;
                else color.a = 0f;

                sprite.material.color = color;
            }
        }	
        
		protected override void OnCantMove <T> (T component)
		{
			Player hitPlayer = component as Player;
			
			hitPlayer.LoseHP(playerDamage);

            Show(true);
            animator.SetTrigger ("enemyAttack");
            SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);

            skipMove = true;

            GameManager.instance.dungeonPlayData.damagedByEnemyCount += playerDamage;

            Analytics.CustomEvent("Damaged by Enemy", new Dictionary<string, object>
            {
                { "Dungeon id", GameManager.instance.GetDungeonInfo().id},
                { "Level id", GameManager.instance.GetDungeonInfo().GetCurLevel().id},
                { "Damage point",  playerDamage},
            });
        }
    }
}
