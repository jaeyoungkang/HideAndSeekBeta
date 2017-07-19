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
        public Button[] DetailDisplayBtns;
        public Button[] DisplayBtns;
        public ITEM_ID[] display;
        public List<ITEM_ID> curDisplay = new List<ITEM_ID>();

        public Button[] BagBtns;
        public Button ReturnBtn;

        public Button AllBtn;
        public Button DetailBtn1;
        public Button DetailBtn2;
        public Button DetailBtn3;

        public Image simpleView;
        public Image detailView1;

        public AudioClip buySellSound;

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
            SetupBag();
            EnableBagSlot();
            SetupView(0);
        }

        void Start()
        {
            if (GameManager.instance == null) return;
            if (!GameManager.instance.CheckState(GAME_STATE.SHOP)) return;

            display = ItemManager.instance.GetEnableToSellItemIds();
            Array.Sort(display);

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

            DetailDisplayBtns[0].onClick.AddListener(() => { BuyItem(0); });
            DetailDisplayBtns[1].onClick.AddListener(() => { BuyItem(1); });
            DetailDisplayBtns[2].onClick.AddListener(() => { BuyItem(2); });
            DetailDisplayBtns[3].onClick.AddListener(() => { BuyItem(3); });
            DetailDisplayBtns[4].onClick.AddListener(() => { BuyItem(4); });

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

            SetupDisplay(type);
        }

        void BuyItem(int index)
        {            
            if (index >= curDisplay.Count) return;

            int price = ItemManager.instance.GetPriceByItemId(curDisplay[index]);

            if (GameManager.instance.dungeonGem < price)
            {
                Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.LACK_GEM), 2f, Color.white);
                return;
            }

            if (curDisplay[index] == ITEM_ID.EXTEND_MAX_HP)
            {
                if (GameManager.instance.ExtendHp(1))
                {
                    Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.INC_MAXHP), 1f, Color.yellow);
                    GameManager.instance.dungeonGem -= price;
                }
                else Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.LIMIT_MAXHP), 1f, Color.yellow);
                
                return;
            }
            else if (curDisplay[index] == ITEM_ID.EXTEND_BAG)
            {
                if (GameManager.instance.ExtendBagSize())
                {
                    Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.INC_BAG), 1f, Color.yellow);
                    GameManager.instance.dungeonGem -= price;
                    EnableBagSlot();
                }
                else Notice.instance.Show(LocalizationManager.instance.GetLocalString(GAME_STRING.LIMIT_BAG), 1F, Color.white);
                
                return;
            }

            if (GameManager.instance.bagSize == GameManager.instance.bag.Count) return;
            GameManager.instance.bag.Add(curDisplay[index]);
            GameManager.instance.dungeonGem -= price;
            SetupBag();

            SoundManager.instance.PlaySingle(buySellSound);

            GameManager.instance.dungeonPlayData.butItems++;
            Analytics.CustomEvent("Buy Item", new Dictionary<string, object>
            {
                { "Item id", curDisplay[index]},
            });

        }

        void SellItem(int index)
        {
            if (index >= GameManager.instance.bag.Count) return;
            Analytics.CustomEvent("Sell Item", new Dictionary<string, object>
            {
                { "Item id", GameManager.instance.bag[index]},
            });

            int price = ItemManager.instance.GetPriceByItemId(GameManager.instance.bag[index]);
            GameManager.instance.dungeonGem += price;
            GameManager.instance.bag.RemoveAt(index);
            SetupBag();
            SoundManager.instance.PlaySingle(buySellSound);

            GameManager.instance.dungeonPlayData.sellItems++;            
        }

        private void SetupBag()
        {
            for (int i = 0; i < BagBtns.Length; i++)
            {
                BagBtns[i].image.color = Color.gray;
                BagBtns[i].image.sprite = null;
                BagBtns[i].GetComponentInChildren<Text>().text = "";
            }

            for (int i = 0; i < BagBtns.Length; i++)
            {
                if (GameManager.instance.bag.Count <= i) break;
                Item item = ItemManager.instance.GetItemByItemId(GameManager.instance.bag[i]);
                BagBtns[i].GetComponentInChildren<Text>().text = item.price.ToString();

                BagBtns[i].image.sprite = item.tile;
                BagBtns[i].image.color = Color.white;                
            }            
        }

        private void SetupDisplay(int type)
        {
            curDisplay.Clear();
            if (type == 0) curDisplay.AddRange(display);
            else if (type == 1)
            {
                for(int i=0; i<5; i++)
                {
                    curDisplay.Add(display[i*3]);
                }                
            }
            else if (type == 2)
            {
                for (int i = 0; i < 5; i++)
                {
                    curDisplay.Add(display[i * 3 + 1]);
                }
            }
            else if (type == 3)
            {
                for (int i = 0; i < 5; i++)
                {
                    curDisplay.Add(display[i * 3 + 2]);
                }
            }

            Button[] curButtons;
            if (type == 0) curButtons = DisplayBtns;
            else curButtons = DetailDisplayBtns;

            for (int i = 0; i < curButtons.Length; i++)
            {
                if (curDisplay.Count <= i) break;                
                Item itemInfo = ItemManager.instance.GetItemByItemId(curDisplay[i]);
                if (itemInfo == null) continue;

                curButtons[i].image.overrideSprite = itemInfo.tile;
                curButtons[i].image.color = Color.white;
                curButtons[i].GetComponentInChildren<Text>().text = itemInfo.price.ToString();
                if (type != 0)
                {
                    curButtons[i].gameObject.transform.Find("discribe").gameObject.GetComponentInChildren<Text>().text = itemInfo.description;
                }
            }
        }
    }
}