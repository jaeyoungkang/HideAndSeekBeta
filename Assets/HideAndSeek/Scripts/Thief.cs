using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

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
        Vector2 DeadPos = new Vector2(-1, -1);
        void MakePath()
        {
            path.Clear();
            Vector2 startPos = transform.position;
            Vector2 destPos = targetObj.transform.position;

            List<Vector2> startPath = new List<Vector2>();
            startPath.Add(startPos);
            path = Search4Way(startPath, destPos);

            //print(startPos.ToString());
            //print(destPos.ToString());
            //foreach (Vector2 pos in path)
            //{
            //    print(pos.ToString());
            //}
            path.RemoveAt(0);
        }

        int CompareWeight(Vector2 A, Vector2 B, Vector2 target)
        {            
            float weightAX = Mathf.Abs(target.x - A.x);
            float weightAY = Mathf.Abs(target.y - A.y);
            float weightA = weightAX + weightAY;

            float weightBX = Mathf.Abs(target.x - B.x);
            float weightBY = Mathf.Abs(target.y - B.y);
            float weightB = weightBX + weightBY;


            if (weightA > weightB) return 1;
            else if (weightA < weightB) return -1;
            return 0;
        }

        bool IsCollision(Vector2 start, Vector2 end)
        {
            RaycastHit2D hit;
            BoxCollider2D boxCollider = gameObject.GetComponent<BoxCollider2D>();
            boxCollider.enabled = false;
            hit = Physics2D.Linecast(start, end, blockingLayer);            
            boxCollider.enabled = true;
            if (hit.transform == null) return false;

            return true;
        }
        
        List<Vector2> Search4Way(List<Vector2> prevs, Vector2 destPos)
        {
            Vector2 curPos = prevs[prevs.Count - 1];    
            Vector2[] nexts = GetNextPos(curPos);            
            List<Vector2> orderedNexts = new List<Vector2>(nexts);
            orderedNexts.Sort((Vector2 x, Vector2 y) => CompareWeight(x, y, destPos));

            foreach (Vector2 nPos in orderedNexts)
            {
                bool bOk = GameManager.instance.IsAvailablePos(nPos);
                if (bOk && !existPos(prevs, nPos))
                {
                    List<Vector2> temp = new List<Vector2>();
                    temp.AddRange(prevs);
                    temp.Add(nPos);
                    if (nPos == destPos) return temp;
                    else return Search4Way(temp, destPos);
                }
            }

            prevs.Add(DeadPos);
            return prevs;
        }

        bool existPos(List<Vector2> prevs, Vector2 pos)
        {
            foreach (Vector2 prev in prevs)
            {
                if (pos == prev) return true;
            }

            return false;
        }


        Vector2[] GetNextPos(Vector2 curPos)
        {
            Vector2[] nexts = { new Vector2(curPos.x + 1, curPos.y),
                            new Vector2( curPos.x - 1, curPos.y ),
                            new Vector2(curPos.x, curPos.y + 1 ),
                            new Vector2(curPos.x, curPos.y - 1 ) };
            return nexts;            
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
                path.RemoveAt(0);
                
                xDir = (int)(nextPos.x - transform.position.x);
                yDir = (int)(nextPos.y - transform.position.y);
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
