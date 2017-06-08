using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Status : MonoBehaviour
    {
        public Text gemText;
        public Text hpText;
        public Text timeText;

        void Start()
        {

        }
 
        void Update()
        {
            gemText.text = GameManager.instance.dungeonGem.ToString();

            hpText.text = "HP :" + GameManager.instance.playerHp.ToString();
            if(GameManager.instance.playerHp < 20) hpText.color = Color.red;
            else hpText.color = Color.white;

            timeText.text = Mathf.Floor(GameManager.instance.timeLimit).ToString();
            if (GameManager.instance.timeLimit <= 10) timeText.color = Color.red;
            else timeText.color = Color.white;            
        }
    }
}