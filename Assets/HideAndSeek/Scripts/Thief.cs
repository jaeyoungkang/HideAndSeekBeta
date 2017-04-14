using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using HideAndSeek;



namespace HideAndSeek
{
    //Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
    public class Thief : Enemy
    {
        private int skipCount = 0;
        public GameObject targetObj;
        public List<GameObject> Objs = new List<GameObject>();
        public int hitPoint = 5;

        protected override void Start()
        {
            hitPoint = 5;
            Objs.Clear();
            GameObject[] Gems = GameObject.FindGameObjectsWithTag("Gem");
            Objs.AddRange(Gems);

            SetTarget();
            base.Start();
        }


        List<Vector2> path = new List<Vector2>();
        bool[,] map = new bool[8, 8];

        void MakePath()
        {
            if (targetObj == false || targetObj.activeSelf == false) return;
            path.Clear();
            map = GameManager.instance.UpdateMap(target.position);
            Vector2 startPos = transform.position;
            Vector2 destPos = targetObj.transform.position;                      

            SearchParameters searchParameters = new SearchParameters(startPos, destPos, map);

            PathFinder pathFinder = new PathFinder(searchParameters);
            
            path = pathFinder.FindPath();
        }
        
        
        protected override void OnCantMove<T>(T component)
        {
            // do nothing
        }

        void ChangeTarget()
        {
            bool bChange = false;        
            float prevDistance = 100;
            foreach (GameObject obj in Objs)
            {
                if (obj.activeSelf == false) continue;
                float distanceX = Mathf.Abs(obj.transform.position.x - transform.position.x);
                float distanceY = Mathf.Abs(obj.transform.position.y - transform.position.y);

                if (prevDistance > distanceX + distanceY)
                {
                    prevDistance = distanceX + distanceY;
                    if(targetObj != obj && GameManager.instance.IsAvailablePos(obj.transform.position)) targetObj = obj;
                }
            }            

            if (bChange == false) targetObj = GameObject.FindGameObjectWithTag("Exit");
        }

        void SetTarget()
        {
            skipCount = 1;
            targetObj = null;
            float prevDistance = 100;
            foreach (GameObject obj in Objs)
            {
                if (obj.activeSelf == false) continue;
                float distanceX = Mathf.Abs(obj.transform.position.x - transform.position.x);
                float distanceY = Mathf.Abs(obj.transform.position.y - transform.position.y);

                if (prevDistance > distanceX + distanceY)
                {
                    prevDistance = distanceX + distanceY;
                    if(GameManager.instance.IsAvailablePos(obj.transform.position))
                        targetObj = obj;
                }
            }

            if (targetObj == null) targetObj = GameObject.FindGameObjectWithTag("Exit");
        }

        public override void MoveEnemy()
        {
            if (skipCount > 0)
            {
                skipCount--;
                return;
            }
            if (enabled == false) return;
            if (targetObj.activeSelf == false)
            {
                SetTarget();
                return;
            }

            if (GameManager.instance.IsAvailablePos(targetObj.transform.position) == false)
            {
                ChangeTarget();
            }


            int xDir = 0;
            int yDir = 0;

            if(targetObj) MakePath();
            if (path.Count>0)
            {
                Vector2 nextPos = path[0];
                int x = (int)nextPos.x;
                int y = (int)nextPos.y;
                
                xDir = x - (int)transform.position.x;
                yDir = y - (int)transform.position.y;
                path.RemoveAt(0);
            }
            else
            {
                GetDirection(targetObj.transform.position, transform.position, out xDir, out yDir);
            }
                        
            AttemptMove<Player>(xDir, yDir);
        }

        void GetDirection(Vector3 destPos, Vector3 curPos, out int xDir, out int yDir)
        {
            float distanceX = Mathf.Abs(destPos.x - curPos.x);
            float distanceY = Mathf.Abs(destPos.y - curPos.y);

            xDir = 0;
            yDir = 0;
            float value = Random.Range(0f, 1f);
            if (value < 0.5f)
            {
                if (distanceX < float.Epsilon)
                    yDir = targetObj.transform.position.y > transform.position.y ? 1 : -1;
                else
                    xDir = targetObj.transform.position.x > transform.position.x ? 1 : -1;
            }
            else
            {
                if (distanceY < float.Epsilon)
                    xDir = targetObj.transform.position.x > transform.position.x ? 1 : -1;
                else
                    yDir = targetObj.transform.position.y > transform.position.y ? 1 : -1;
            }
        }

        private bool CheckCollision(Vector2 start, Vector2 end)
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.enabled = false;
            RaycastHit2D  hit = Physics2D.Linecast (start, end, blockingLayer);
            collider.enabled = true;

            if (hit.transform == null) return false;

            return true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Exit")
            {
                enabled = false;
                StartCoroutine(ShowThief());                             
            }
            else if (other.tag == "Gold")
            {
                other.gameObject.SetActive(false);
            }
            else if (other.tag == "Gem")
            {
                other.gameObject.SetActive(false);
            }
        }

        protected IEnumerator ShowThief()
        {
            Show(true);
            yield return new WaitForSeconds(0.5f);            
            gameObject.SetActive(false);
        }

        public void Hitted()
        {
            if (hitPoint <= 0) return;
            hitPoint--;
            if(hitPoint <= 0) gameObject.SetActive(false);
        }
    }
}
