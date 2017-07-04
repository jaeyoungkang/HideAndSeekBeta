using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace HideAndSeek
{
    public class Lobby : MonoBehaviour
    {
        public Button[] dungeonBtns;
        private Dungeon[] dungeons;

        void SetupDungeonBtns()
        {
            for (int i = 0; i < dungeons.Length; i++)
            {
                if (dungeonBtns.Length <= i) break;
                dungeonBtns[i].GetComponentInChildren<Text>().text = dungeons[i].name;
            }

            for (int i = 0; i < dungeonBtns.Length; i++)
            {
                dungeonBtns[i].enabled = false;
            }

            for (int i = 0; i < dungeons.Length; i++)
            {
                if (dungeonBtns.Length <= i) break;
                dungeonBtns[i].enabled = dungeons[i].open;
            }
        }

        void Start()
        {
            dungeons = GameManager.instance.dungeons;

            SetupDungeonBtns();
            dungeonBtns[0].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(0); });
            dungeonBtns[1].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(1); });
            dungeonBtns[2].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(2); });
            dungeonBtns[3].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(3); });
            dungeonBtns[4].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(4); });
        }

        void Update()
        {
            //DateTime now = DateTime.Now.ToLocalTime();
            //TimeSpan gen = now - GameManager.instance.info.preGenTime;

            //string coinText = "";
            //if (gen.TotalSeconds <= GameManager.instance.TIME_INTERVAL_GEN && GameManager.instance.info.coin < GameManager.instance.MAX_COIN)
            //{
            //    float remainTime = GameManager.instance.TIME_INTERVAL_GEN - (float)gen.TotalSeconds;
            //    int minute = (int)(remainTime / 60);
            //    float remain = remainTime % 60;
            //    string time = String.Format("{0:0}:{1:00}", minute, Mathf.Floor(remain));
            //    coinText = "\n다음 주화 받기까지 남은시간 : " + time;
            //}

            //gemText.text = "보유 보석: " + GameManager.instance.info.invenGem + ", 고대주화: " + GameManager.instance.info.coin + coinText;
        }
 
    }
}