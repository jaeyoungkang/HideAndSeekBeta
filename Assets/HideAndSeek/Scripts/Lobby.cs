using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace HideAndSeek
{
    public class Lobby : MonoBehaviour
    {
        public Text timeText;
        public Text coinText;
        public Button watachAdBtn;
        public Button purchaseBtn;
        public Button tutorialBtn;
        public Button[] dungeonBtns;
        private Dungeon[] dungeons;
        private Dungeon tutorial;

        public Button developerComment;

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

            tutorialBtn.GetComponentInChildren<Text>().text = tutorial.name;
            tutorialBtn.enabled = tutorial.open;            
        }

        void Start()
        {
            dungeons = GameManager.instance.dungeons;
            tutorial = GameManager.instance.tutorial;

            SetupDungeonBtns();
            dungeonBtns[0].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(0); });
            dungeonBtns[1].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(1); });
            dungeonBtns[2].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(2); });
            dungeonBtns[3].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(3); });
            tutorialBtn.onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(100); });

            watachAdBtn.onClick.AddListener(GameManager.instance.GetAShovel);
            purchaseBtn.onClick.AddListener(GameManager.instance.ShowPurchase);
            developerComment.onClick.AddListener(GameManager.instance.ShowDeveloperComment);

            if(GameManager.instance.info.dungeonClearCount.ContainsKey(4))
            {
                developerComment.gameObject.SetActive(true);
            }
            else
            {
                developerComment.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            DateTime now = DateTime.Now.ToLocalTime();
            TimeSpan gen = now - GameManager.instance.preGenTime;

            string remainTimeText = "";
            float remainTime = GameManager.instance.TIME_INTERVAL_GEN - (float)gen.TotalSeconds;
            if (remainTime > 0)
            {                
                int minute = (int)(remainTime / 60);
                float remain = remainTime % 60;
                string time = String.Format("{0:0}:{1:00}", minute, Mathf.Floor(remain));
				remainTimeText = "("+LocalizationManager.instance.GetLocalUIString(UI_STRING.LOBBY_TIME_REMAIN) + time + ")";
            }
            timeText.text = remainTimeText;

            coinText.text = "X " +GameManager.instance.info.enableCount.ToString();

        }
 
    }
}