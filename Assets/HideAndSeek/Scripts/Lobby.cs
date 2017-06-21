using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Lobby : MonoBehaviour
    {
        public Text gemText;

        public Button shopBtn;
        public Button invenBtn;

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

			gemText.text = "보유 보석: " + GameManager.instance.info.invenGem + ", 고대주화: " + GameManager.instance.info.coin;

            SetupDungeonBtns();
            dungeonBtns[0].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(0); });
            dungeonBtns[1].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(1); });
            dungeonBtns[2].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(2); });
            dungeonBtns[3].onClick.AddListener(() => { GameManager.instance.ShowDungeonInfo(3); });

            shopBtn.onClick.AddListener(GameManager.instance.EnterShop);
            invenBtn.onClick.AddListener(GameManager.instance.EnterInven);
        }
 
    }
}