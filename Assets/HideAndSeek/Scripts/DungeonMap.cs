﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class DungeonMap : MonoBehaviour {
        public Image[] lineImages;
        public Button[] levelBtns;
        public Button shopBtns;
        public Button leavBtns;

        void OnEnable()
        {
            if (GameManager.instance == null) return;
            if (GameManager.instance.GetDungeonInfo() == null) return;

            SetupBtns(GameManager.instance.GetDungeonInfo().levels);
            SetupLineImages(GameManager.instance.GetDungeonInfo().levels);            
        }

        void Start() {
            if (GameManager.instance == null) return;
            if (GameManager.instance.GetDungeonInfo() == null) return;

            SetupBtns(GameManager.instance.GetDungeonInfo().levels);
            SetupLineImages(GameManager.instance.GetDungeonInfo().levels);

            levelBtns[0].onClick.AddListener(() => { GameManager.instance.SelectLevel(1); });
            levelBtns[1].onClick.AddListener(() => { GameManager.instance.SelectLevel(2); });
            levelBtns[2].onClick.AddListener(() => { GameManager.instance.SelectLevel(3); });
            levelBtns[3].onClick.AddListener(() => { GameManager.instance.SelectLevel(4); });
            levelBtns[4].onClick.AddListener(() => { GameManager.instance.SelectLevel(5); });
            levelBtns[5].onClick.AddListener(() => { GameManager.instance.SelectLevel(6); });
            levelBtns[6].onClick.AddListener(() => { GameManager.instance.SelectLevel(7); });
            levelBtns[7].onClick.AddListener(() => { GameManager.instance.SelectLevel(8); });
            levelBtns[8].onClick.AddListener(() => { GameManager.instance.SelectLevel(9); });

            shopBtns.onClick.AddListener(GameManager.instance.EnterShop);
            leavBtns.onClick.AddListener(GameManager.instance.GoToLobby);
        }

        protected void SetupBtns(Level[] levels)
        {
            for (int i = 0; i < levelBtns.Length; i++)
            {
                levelBtns[i].GetComponent<Image>().color = new Color32(54,54,54,255);
                levelBtns[i].gameObject.SetActive(false);
            }
            
            for (int i = 0; i < levelBtns.Length; i++)
            {
                foreach(Level lv in levels)
                {
                    if (lv.id - 1 == i)
                    {
                        levelBtns[i].gameObject.SetActive(true);
                    }                        
                }                
            }
            
            for(int i =0; i< levels.Length; i++)
            {
                int btnIndex = levels[i].id-1;
                if (btnIndex >= levelBtns.Length) continue;
                levelBtns[btnIndex].GetComponentInChildren<Text>().text = levels[i].name;
                levelBtns[btnIndex].GetComponentInChildren<Text>().color = Color.white;
                levelBtns[btnIndex].enabled = !levels[i].close;
                if (!levels[i].close)
                {
                    if (levels[i].clear) levelBtns[btnIndex].GetComponent<Image>().color = Color.yellow;
                    else levelBtns[btnIndex].GetComponent<Image>().color = Color.green;

                    levelBtns[btnIndex].GetComponentInChildren<Text>().color = Color.black;
                }
            }           
        }

        protected void SetupLineImages(Level[] levels)
        {
            foreach(Image img in lineImages)
            {
                img.color = Color.gray;
                img.gameObject.SetActive(false);
            }

            foreach (Level lv in levels)
            {
                foreach(int id in lv.nextIds)
                {
                    string lineName = "line" + lv.id.ToString() + id.ToString();
                    foreach (Image img in lineImages)
                    {
                        if (img.gameObject.name == lineName)
                        {
                            img.gameObject.SetActive(true);
                            if(lv.clear && IsEnableLevel(levels, id))
                            {
                                img.color = Color.white;
                            }
                        }
                    }
                }
            }
        }

        bool IsEnableLevel(Level[] levels, int levelId)
        {
            foreach (Level lv in levels)
            {
                if (lv.id == levelId && lv.close == false) return true;
            }

            return false;
        }
    }
}
