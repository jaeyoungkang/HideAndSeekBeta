using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class DungeonMap : MonoBehaviour {
        public Image[] lineImages;
        public Button[] levelBtns;
        private Level[] levels;

        void Start() {
            Dungeon curDungeon = GameManager.instance.GetDungeonInfo();
            levels = curDungeon.levels;
            SetupBtns();
            SetupLineImages();

            levelBtns[0].onClick.AddListener(() => { GameManager.instance.EnterLevel(1); });
            levelBtns[1].onClick.AddListener(() => { GameManager.instance.EnterLevel(2); });
            levelBtns[2].onClick.AddListener(() => { GameManager.instance.EnterLevel(3); });
            levelBtns[3].onClick.AddListener(() => { GameManager.instance.EnterLevel(4); });
            levelBtns[4].onClick.AddListener(() => { GameManager.instance.EnterLevel(5); });
            levelBtns[5].onClick.AddListener(() => { GameManager.instance.EnterLevel(6); });
            levelBtns[6].onClick.AddListener(() => { GameManager.instance.EnterLevel(7); });
            levelBtns[7].onClick.AddListener(() => { GameManager.instance.EnterLevel(8); });
            levelBtns[8].onClick.AddListener(() => { GameManager.instance.EnterLevel(9); });
        }

        void SetupBtns()
        {
            for (int i = 0; i < levelBtns.Length; i++)
            {
                levelBtns[i].gameObject.SetActive(false);
            }
            
            for (int i = 0; i < levelBtns.Length; i++)
            {
                foreach(Level lv in levels)
                {
                    if (lv.index - 1 == i)
                        levelBtns[i].gameObject.SetActive(true);
                }                
            }
            
            for(int i =0; i< levels.Length; i++)
            {
                int btnIndex = levels[i].index-1;
                if (btnIndex >= levelBtns.Length) continue;
                levelBtns[btnIndex].GetComponentInChildren<Text>().text = levels[i].name;
                levelBtns[btnIndex].enabled = !levels[i].close;
            }           
        }

        void SetupLineImages()
        {
            foreach(Image img in lineImages)
            {
                img.gameObject.SetActive(false);
            }

            foreach (Level lv in levels)
            {
                foreach(int index in lv.nextIndex)
                {
                    string lineName = "line" + lv.index.ToString() + index.ToString();
                    foreach (Image img in lineImages)
                    {
                        if (img.gameObject.name == lineName) img.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
