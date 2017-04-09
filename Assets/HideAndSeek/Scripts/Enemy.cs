using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace HideAndSeek
{
	public class Enemy : MovingObject
	{
		public int rangeOfSense = 1;
		public int playerDamage;
		public AudioClip attackSound1;
		public AudioClip attackSound2;
		
		private Animator animator;
        private Transform target;

        protected override void Start ()
		{
            GameManager.instance.AddEnemyToList (this);
			animator = GetComponent<Animator> ();
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			base.Start ();
		}

		public virtual void MoveEnemy ()
		{
            int xDir = 0;
			int yDir = 0;

			float distanceX = Mathf.Abs (target.position.x - transform.position.x);
			float distanceY = Mathf.Abs (target.position.y - transform.position.y);

            //if(distanceY <= rangeOfSense && distanceX <= rangeOfSense)
            if( distanceX + distanceY <= rangeOfSense )
            {
                print(distanceX.ToString() + " " + distanceY.ToString());
                float value = Random.Range(0f, 1f);
                if (value < 0.5f)
                {
                    if (distanceX < float.Epsilon)
                        yDir = target.position.y > transform.position.y ? 1 : -1;
                    else
                        xDir = target.position.x > transform.position.x ? 1 : -1;
                }                
                else
                {
                    if (distanceY < float.Epsilon)
                        xDir = target.position.x > transform.position.x ? 1 : -1;
                    else
                        yDir = target.position.y > transform.position.y ? 1 : -1;                    
                }
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
			
			hitPlayer.LoseFood (playerDamage);
                        
            animator.SetTrigger ("enemyAttack");
            StartCoroutine(ShowEnemy());
            SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}

        protected IEnumerator ShowEnemy()
        {            
            Show(true);
            yield return new WaitForSeconds(0.1f);
            Show(false); 
        }        
    }
}
