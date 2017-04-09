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
//            GameObject[] Golds = GameObject.FindGameObjectsWithTag("Gold");
            GameObject[] Gems = GameObject.FindGameObjectsWithTag("Gem");
//            Objs.AddRange(Golds);
            Objs.AddRange(Gems);

            SetTarget();
            base.Start();
        }

        protected override void OnCantMove<T>(T component)
        {
            // do nothing
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

            int xDir = 0;
            int yDir = 0;

            float distanceX = Mathf.Abs(targetObj.transform.position.x - transform.position.x);
            float distanceY = Mathf.Abs(targetObj.transform.position.y - transform.position.y);

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

            AttemptMove<Player>(xDir, yDir);
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
