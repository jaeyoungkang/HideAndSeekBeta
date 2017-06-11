﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Controller : MonoBehaviour
    {
        public Button[] dirBtns;
        public Button[] slotBtns;
        private Player player;

        void OnEnable()
        {
            if (GameManager.instance == null) return;
            SetupSlots();
        }

        void Start()
        {
            if (GameManager.instance == null) return;
            player = FindObjectOfType(typeof(Player)) as Player;
            SetupSlots();

            dirBtns[0].onClick.AddListener(()=> { MovePlayer(MOVE_DIR.UP); });
            dirBtns[1].onClick.AddListener(() => { MovePlayer(MOVE_DIR.DOWN); });
            dirBtns[2].onClick.AddListener(() => { MovePlayer(MOVE_DIR.LEFT); });
            dirBtns[3].onClick.AddListener(() => { MovePlayer(MOVE_DIR.RIGHT); });

            slotBtns[0].onClick.AddListener(() => { UseItem(0); });
            slotBtns[1].onClick.AddListener(() => { UseItem(1); });
            slotBtns[2].onClick.AddListener(() => { UseItem(2); });
            slotBtns[3].onClick.AddListener(() => { UseItem(3); });
            slotBtns[4].onClick.AddListener(() => { UseItem(4); });
            slotBtns[5].onClick.AddListener(() => { UseItem(5); });
        }

        public void SetupSlots()
        {
            for (int i = 0; i < slotBtns.Length; i++)
            {
                slotBtns[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < slotBtns.Length; i++)
            {
                if (GameManager.instance.info.bagSize <= i) break;
                slotBtns[i].gameObject.SetActive(true);
            }

            for (int i = 0; i < slotBtns.Length; i++)
            {
                slotBtns[i].GetComponentInChildren<Text>().text = "";
            }

            for (int i = 0; i < slotBtns.Length; i++)
            {
                if (GameManager.instance.bag.Count <= i) break;
                Item item = ItemManager.instance.GetItemByItemId(GameManager.instance.bag[i]);
                slotBtns[i].GetComponentInChildren<Text>().text = item.name;

                Color itemGradeColor = ItemManager.instance.GetColorByItemGrade(item.grade);
                ItemManager.instance.SetItemUIColor(slotBtns[i], itemGradeColor);
            }
        }

        void MovePlayer(MOVE_DIR dir)
        {
            player.MoveByControlPanel(dir);
        }

        void UseItem(int slotNum)
        {
            player.UseSkill(slotNum);
            SetupSlots();
        }        
    }
}