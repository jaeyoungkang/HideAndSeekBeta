using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using UnityEngine.Analytics;

namespace HideAndSeek
{
    public class DetailButton
    {
        public Button btn;
        public Text text;
    }

    public class Shop : MonoBehaviour
    {
        public Button[] DisplayBtns;
        public int[] display;

        public Button[] BagBtns;
        public Button ReturnBtn;
        public Button ExtendBtn;

        public Button AllBtn;
        public Button DetailBtn1;
        public Button DetailBtn2;
        public Button DetailBtn3;

        public Image simpleView;
        public Image detailView1;

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
        }

        void OnEnable()
        {
            if (GameManager.instance == null) return;
            if (!GameManager.instance.CheckState(GAME_STATE.SHOP)) return;
            SetupDisplay();
            SetupBag();
            EnableBagSlot();
            SetupView(0);
        }

        void Start()
        {
            if (GameManager.instance == null) return;
            if (!GameManager.instance.CheckState(GAME_STATE.SHOP)) return;
            SetupDisplay();
            SetupBag();
            EnableBagSlot();
            SetupView(0);

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

            ReturnBtn.onClick.AddListener(GameManager.instance.BacktoPreState);

            AllBtn.onClick.AddListener(() => { SetupView(0); });
            DetailBtn1.onClick.AddListener(() => { SetupView(1); });
            DetailBtn2.onClick.AddListener(() => { SetupView(2); });
            DetailBtn3.onClick.AddListener(() => { SetupView(3); });
        }

        void SetupView(int type)
        {
            if(type == 0)
            {
                simpleView.gameObject.SetActive(true);
                detailView1.gameObject.SetActive(false);
            }
            else
            {
                simpleView.gameObject.SetActive(false);
                detailView1.gameObject.SetActive(true);
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

            GameManager.instance.dungeonPlayData.butItems++;
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

            GameManager.instance.dungeonPlayData.sellItems++;
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