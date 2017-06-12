using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace HideAndSeek
{

    [System.Serializable]
    public class Skill
    {
        public string skillname;
        public int price;
    }

    public class Shop : MonoBehaviour
    {
        public Button[] DisplayBtns;
        public int[] display;

        public Button[] InvenBtns;
        public Button ReturnBtn;

        public Text GemText;        

        void EnableInvenSlot()
        {
            foreach (Button invenBtn in InvenBtns)
            {
                invenBtn.gameObject.SetActive(false);
            }

            for (int i = 0; i < GameManager.instance.info.invenSize; i++)
            {
                InvenBtns[i].gameObject.SetActive(true);
            }
        }

        void onEnable()
        {
            if (GameManager.instance == null) return;
            if (!GameManager.instance.CheckState(GAME_STATE.SHOP)) return;
            Start();
        }

        void Start()
        {
            SetupDisplay();
            SetupInventory();
            EnableInvenSlot();

            InvenBtns[0].onClick.AddListener(() => { SellSkill(0); });
            InvenBtns[1].onClick.AddListener(() => { SellSkill(1); });
            InvenBtns[2].onClick.AddListener(() => { SellSkill(2); });
            InvenBtns[3].onClick.AddListener(() => { SellSkill(3); });
            InvenBtns[4].onClick.AddListener(() => { SellSkill(4); });
            InvenBtns[5].onClick.AddListener(() => { SellSkill(5); });
            InvenBtns[6].onClick.AddListener(() => { SellSkill(6); });
            InvenBtns[7].onClick.AddListener(() => { SellSkill(7); });
            InvenBtns[8].onClick.AddListener(() => { SellSkill(8); });
            InvenBtns[9].onClick.AddListener(() => { SellSkill(9); });
            
            DisplayBtns[0].onClick.AddListener(() => { BuySkill(0); });
            DisplayBtns[1].onClick.AddListener(() => { BuySkill(1); });
            DisplayBtns[2].onClick.AddListener(() => { BuySkill(2); });
            DisplayBtns[3].onClick.AddListener(() => { BuySkill(3); });
            DisplayBtns[4].onClick.AddListener(() => { BuySkill(4); });
            DisplayBtns[5].onClick.AddListener(() => { BuySkill(5); });
            DisplayBtns[6].onClick.AddListener(() => { BuySkill(6); });
            DisplayBtns[7].onClick.AddListener(() => { BuySkill(7); });
            DisplayBtns[8].onClick.AddListener(() => { BuySkill(8); });
            DisplayBtns[9].onClick.AddListener(() => { BuySkill(9); });
            DisplayBtns[10].onClick.AddListener(() => { BuySkill(10); });
            DisplayBtns[11].onClick.AddListener(() => { BuySkill(11); });
            DisplayBtns[12].onClick.AddListener(() => { BuySkill(12); });
            DisplayBtns[13].onClick.AddListener(() => { BuySkill(13); });
            DisplayBtns[14].onClick.AddListener(() => { BuySkill(14); });

            ReturnBtn.onClick.AddListener(GameManager.instance.GoToLobby);
        }

        void Update()
        {
            GemText.text = "Gem: " + GameManager.instance.info.invenGem.ToString();
        }

        void BuySkill(int index)
        {            
            if (index >= display.Length) return;

            int price = ItemManager.instance.GetPriceByItemId(display[index]);

            if (GameManager.instance.info.invenGem < price) return;

            string name = ItemManager.instance.GetNameByItemId(display[index]);
            if (name == "인벤추가")
            {
                if (GameManager.instance.ExtendInvenSize(InvenBtns.Length))
                {
                    GameManager.instance.info.invenGem -= price;
                    EnableInvenSlot();
                }
            }
            else
            {
                if (GameManager.instance.info.invenSize == GameManager.instance.info.inven.Count) return;
                GameManager.instance.info.inven.Add(display[index]);
                GameManager.instance.info.invenGem -= price;
                SetupInventory();
            }
        }

        void SellSkill(int index)
        {
            if (index >= GameManager.instance.info.inven.Count) return;
            int price = ItemManager.instance.GetPriceByItemId(GameManager.instance.info.inven[index]);
            GameManager.instance.info.invenGem += price;
            GameManager.instance.info.inven.RemoveAt(index);            
            SetupInventory();            
        }

        private void SetupInventory()
        {
            for (int i = 0; i < InvenBtns.Length; i++)
            {
                InvenBtns[i].GetComponentInChildren<Text>().text = "";
            }

            for (int i = 0; i < InvenBtns.Length; i++)
            {
                if (GameManager.instance.info.inven.Count <= i) break;
                Item item = ItemManager.instance.GetItemByItemId(GameManager.instance.info.inven[i]);
                InvenBtns[i].GetComponentInChildren<Text>().text = item.name;

                Color itemGradeColor = ItemManager.instance.GetColorByItemGrade(item.grade);
                ItemManager.instance.SetItemUIColor(InvenBtns[i], itemGradeColor);
            }            
        }

        private void SetupDisplay()
        {
            display = ItemManager.instance.GetEnableToSellItemIds();
            Array.Sort(display);
            for (int i = 0; i < DisplayBtns.Length; i++)
            {
                if (display.Length <= i) break;                
                Item itemInfo = ItemManager.instance.GetItemByItemId(display[i]);
                if (itemInfo == null) continue;
                DisplayBtns[i].GetComponentInChildren<Text>().text = itemInfo.name + "\n(" + itemInfo.price + ")";

                Color itemGradeColor = ItemManager.instance.GetColorByItemGrade(itemInfo.grade);
                ItemManager.instance.SetItemUIColor(DisplayBtns[i], itemGradeColor);                         
            }
        }
    }
}