using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace HideAndSeek
{
	//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
	public class Enemy : MovingObject
	{
		public int rangeOfSense = 1;
		public int playerDamage; 							//The amount of food points to subtract from the player when attacking.
		public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.
		
		
		private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.
		private Transform target;							//Transform to attempt to move toward each turn.
		private bool skipMove;								//Boolean to determine whether or not enemy should skip a turn or move this turn.
		
		
		//Start overrides the virtual Start function of the base class.
		protected override void Start ()
		{
			//Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
			//This allows the GameManager to issue movement commands.
			GameManager.instance.AddEnemyToList (this);
			
			//Get and store a reference to the attached Animator component.
			animator = GetComponent<Animator> ();
			
			//Find the Player GameObject using it's tag and store a reference to its transform component.
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			
			//Call the start function of our base class MovingObject.
			base.Start ();
		}
		
		
		//Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
		//See comments in MovingObject for more on how base AttemptMove function works.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//Check if skipMove is true, if so set it to false and skip this turn.
			//if(skipMove)
			//{
			//	skipMove = false;
			//	return;				
			//}
			
			//Call the AttemptMove function from MovingObject.
			base.AttemptMove <T> (xDir, yDir);
			
			//Now that Enemy has moved, set skipMove to true to skip next move.
//			skipMove = true;
		}
		
		
		//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
		public void MoveEnemy ()
		{
			//Declare variables for X and Y axis move directions, these range from -1 to 1.
			//These values allow us to choose between the cardinal directions: up, down, left and right.
			int xDir = 0;
			int yDir = 0;

			float distanceX = Mathf.Abs (target.position.x - transform.position.x);
			float distanceY = Mathf.Abs (target.position.y - transform.position.y);

            //            if(distanceY <= rangeOfSense && distanceX <= rangeOfSense)
            if( (distanceY <= rangeOfSense && distanceX < rangeOfSense) || (distanceY < rangeOfSense && distanceX <= rangeOfSense) )
            {                
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
                print(distanceX.ToString() + " " + distanceY.ToString() + " " + xDir.ToString() + " " + yDir.ToString());
            }


            //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
            AttemptMove <Player> (xDir, yDir);
		}

        public bool bShowing = false;
        public void Show(bool bShow)
        {
            bShowing = bShow;
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();

            if (sprite)
            {
                if(bShow) sprite.sortingLayerName = "Units";
                else sprite.sortingLayerName = "Hide";
            }
        }
		
		//OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
		//and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
		protected override void OnCantMove <T> (T component)
		{
			//Declare hitPlayer and set it to equal the encountered component.
			Player hitPlayer = component as Player;
			
			//Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
			hitPlayer.LoseFood (playerDamage);
                        
            animator.SetTrigger ("enemyAttack");
            if (bShowing == false) StartCoroutine(ShowEnemyAttack());
            
            //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
            SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
		}

        IEnumerator ShowEnemyAttack()
        {            
            Show(true);
            yield return new WaitForSeconds(0.1f);
            Show(false); 
        }
    }
}
