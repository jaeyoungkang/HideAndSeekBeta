using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using UnityEngine.Analytics;

namespace HideAndSeek
{
    public class Shop : MonoBehaviour
    {
        public Button[] DisplayBtns;
        public int[] display;

        public Button[] BagBtns;
        public Button ReturnBtn;
        public Button ExtendBtn;

        public Text GemText;        

        void EnableBagSlot()
        {
            foreach (Button bagBtn in BagBtns)
            {
                bagBtn.gameObject.SetActive(false);
            }

            for (int i = 0; i < GameManager.instance.bagSize; i++)
            {
                BagBtns[i].gameObject.SetActive(true);
            }

            ExtendBtn.GetComponentInChildren<Text>().text = "가방확장(" + GameManager.instance.GetPriceExtendBag(BagBtns.Length, GameManager.instance.bagSize) + ")";
        }

        void onEnable()
        {
            if (GameManager.instance == null) return;
            if (!GameManager.instance.CheckState(GAME_STATE.SHOP)) return;
            Start();
        }

        void Start()
        {
            if (GameManager.instance == null) return;
            if (!GameManager.instance.CheckState(GAME_STATE.SHOP)) return;
            SetupDisplay();
            SetupBag();
            EnableBagSlot();

            BagBtns[0].onClick.AddListener(() => { SellItem(0); });
            BagBtns[1].onClick.AddListener(() => { SellItem(1); });
            BagBtns[2].onClick.AddListener(() => { SellItem(2); });
            BagBtns[3].onClick.AddListener(() => { SellItem(3); });
            BagBtns[4].onClick.AddListener(() => { SellItem(4); });
            BagBtns[5].onClick.AddListener(() => { SellItem(5); });
            
            DisplayBtns[0].onClick.AddListener(() => { BuyItem(0); });
            DisplayBtns[1].onClick.AddListener(() => { BuyItem(1); });
            DisplayBtns[2].onClick.AddListener(() => { BuyItem(2); });
            DisplayBtns[3].onClick.AddListener(() => { BuyItem(3); });
            DisplayBtns[4].onClick.AddListener(() => { BuyItem(4); });
            DisplayBtns[5].onClick.AddListener(() => { BuyItem(5); });
            DisplayBtns[6].onClick.AddListener(() => { BuyItem(6); });
            DisplayBtns[7].onClick.AddListener(() => { BuyItem(7); });
            DisplayBtns[8].onClick.AddListener(() => { BuyItem(8); });
            DisplayBtns[9].onClick.AddListener(() => { BuyItem(9); });
            DisplayBtns[10].onClick.AddListener(() => { BuyItem(10); });
            DisplayBtns[11].onClick.AddListener(() => { BuyItem(11); });
            DisplayBtns[12].onClick.AddListener(() => { BuyItem(12); });
            DisplayBtns[13].onClick.AddListener(() => { BuyItem(13); });
            DisplayBtns[14].onClick.AddListener(() => { BuyItem(14); });

            ReturnBtn.onClick.AddListener(GameManager.instance.GoToLobby);
            ExtendBtn.onClick.AddListener(ExtendBagSize);
        }

        void Update()
        {
            GemText.text = "Gem: " + GameManager.instance.dungeonGem.ToString();
        }

        void ExtendBagSize()
        {
            int extendPrice = GameManager.instance.GetPriceExtendBag(BagBtns.Length, GameManager.instance.bagSize);
            if (GameManager.instance.dungeonGem < extendPrice)
            {
                Notice.instance.Show("보석이 부족합니다.", 2f, Color.white);
                return;
            }

            if (GameManager.instance.ExtendBagSize(BagBtns.Length))
            {
                GameManager.instance.dungeonGem -= extendPrice;
                EnableBagSlot();
            }
        } 

        void BuyItem(int index)
        {            
            if (index >= display.Length) return;

            int price = ItemManager.instance.GetPriceByItemId(display[index]);

            if (GameManager.instance.dungeonGem < price)
            {
                Notice.instance.Show("보석이 부족합니다.", 2f, Color.white);
                return;
            }

            if (GameManager.instance.bagSize == GameManager.instance.bag.Count) return;
            GameManager.instance.bag.Add(display[index]);
            GameManager.instance.dungeonGem -= price;
            SetupBag();

            Analytics.CustomEvent("Buy Item", new Dictionary<string, object>
            {
                { "Item id", display[index]},
            });

        }

        void SellItem(int index)
        {
            if (index >= GameManager.instance.bag.Count) return;
            int price = ItemManager.instance.GetPriceByItemId(GameManager.instance.bag[index]);
            GameManager.instance.dungeonGem += price;
            GameManager.instance.bag.RemoveAt(index);
            SetupBag();

            Analytics.CustomEvent("Sell Item", new Dictionary<string, object>
            {
                { "Item id", display[index]},
            });
        }

        private void SetupBag()
        {
            for (int i = 0; i < BagBtns.Length; i++)
            {
                BagBtns[i].GetComponentInChildren<Text>().text = "";
            }

            for (int i = 0; i < BagBtns.Length; i++)
            {
                if (GameManager.instance.bag.Count <= i) break;
                Item item = ItemManager.instance.GetItemByItemId(GameManager.instance.bag[i]);
                BagBtns[i].GetComponentInChildren<Text>().text = item.name;

                Color itemGradeColor = ItemManager.instance.GetColorByItemGrade(item.grade);
                ItemManager.instance.SetItemUIColor(BagBtns[i], itemGradeColor);
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