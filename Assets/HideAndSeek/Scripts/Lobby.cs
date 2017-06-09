using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Lobby : MonoBehaviour
    {
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
        }

        void Start()
        {
            dungeons = GameManager.instance.dungeons;

            SetupDungeonBtns();
            dungeonBtns[0].onClick.AddListener(() => { GameManager.instance.SelectDungeon(0); });
            dungeonBtns[1].onClick.AddListener(() => { GameManager.instance.SelectDungeon(1); });
            dungeonBtns[2].onClick.AddListener(() => { GameManager.instance.SelectDungeon(2); });
            dungeonBtns[3].onClick.AddListener(() => { GameManager.instance.SelectDungeon(3); });

            shopBtn.onClick.AddListener(GameManager.instance.EnterShop);
            invenBtn.onClick.AddListener(GameManager.instance.EnterInven);
        }
 
    }
}