using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Inventory : MonoBehaviour
    {
        public Button ExtendBagBtn;
        public Button ExtendInvenBtn;
        public Button ReturnBtn;

        public Text GemText;

        public Button[] BagBtns;
        public Button[] InvenBtns;

        void Update()
        {
            GemText.text = "Gem: " + GameManager.instance.info.invenGem.ToString();
        }

        void EnableBagSlot()
        {
            foreach (Button bagBtn in BagBtns)
            {
                bagBtn.gameObject.SetActive(false);
            }

            for (int i = 0; i < GameManager.instance.info.bagSize; i++)
            {
                BagBtns[i].gameObject.SetActive(true);
            }

            ExtendBagBtn.GetComponentInChildren<Text>().text = "가방확장(" + GameManager.instance.GetPriceExtendBag(BagBtns.Length, GameManager.instance.info.bagSize) + ")";
        }

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

            ExtendInvenBtn.GetComponentInChildren<Text>().text = "창고확장(" + GameManager.instance.GetPriceExtendInven(InvenBtns.Length, GameManager.instance.info.invenSize) + ")";
        }

        void Start()
        {
            SetupInventory();
            EnableInvenSlot();
            EnableBagSlot();

            InvenBtns[0].onClick.AddListener(() => { MoveToBag(0); });
            InvenBtns[1].onClick.AddListener(() => { MoveToBag(1); });
            InvenBtns[2].onClick.AddListener(() => { MoveToBag(2); });
            InvenBtns[3].onClick.AddListener(() => { MoveToBag(3); });
            InvenBtns[4].onClick.AddListener(() => { MoveToBag(4); });
            InvenBtns[5].onClick.AddListener(() => { MoveToBag(5); });
            InvenBtns[6].onClick.AddListener(() => { MoveToBag(6); });
            InvenBtns[7].onClick.AddListener(() => { MoveToBag(7); });
            InvenBtns[8].onClick.AddListener(() => { MoveToBag(8); });
            InvenBtns[9].onClick.AddListener(() => { MoveToBag(9); });

            BagBtns[0].onClick.AddListener(() => { MoveToInven(0); });
            BagBtns[1].onClick.AddListener(() => { MoveToInven(1); });
            BagBtns[2].onClick.AddListener(() => { MoveToInven(2); });
            BagBtns[3].onClick.AddListener(() => { MoveToInven(3); });
            BagBtns[4].onClick.AddListener(() => { MoveToInven(4); });
            BagBtns[5].onClick.AddListener(() => { MoveToInven(5); });

            ReturnBtn.onClick.AddListener(GameManager.instance.BacktoPreState);
            ExtendBagBtn.onClick.AddListener(ExtendBagSize);
            ExtendInvenBtn.onClick.AddListener(ExtendInvenSize);            
        }

        void ExtendBagSize()
        {
            int extendPrice = GameManager.instance.GetPriceExtendBag(BagBtns.Length, GameManager.instance.info.bagSize);            
            if (GameManager.instance.info.invenGem < extendPrice)
            {
                print("Popup: Not enough gem.");
                return;
            }

            if (GameManager.instance.ExtendBagSize(BagBtns.Length))
            {
                GameManager.instance.info.invenGem -= extendPrice;
                EnableBagSlot();
            }
                        
        }

        void ExtendInvenSize()
        {
            int extendPrice = GameManager.instance.GetPriceExtendInven(InvenBtns.Length, GameManager.instance.info.invenSize);
            if (GameManager.instance.info.invenGem < extendPrice)
            {
                print("Popup: Not enough gem.");
                return;
            }

            if (GameManager.instance.ExtendInvenSize(InvenBtns.Length))
            {
                GameManager.instance.info.invenGem -= extendPrice;
                EnableInvenSlot();
            }
        }

        void MoveToInven(int index)
        {
            if (index >= GameManager.instance.info.bag.Count) return;
            int itemId = GameManager.instance.info.bag[index];
            GameManager.instance.info.inven.Add(itemId);
            GameManager.instance.info.bag.RemoveAt(index);
            SetupInventory();
        }

        void MoveToBag(int index)
        {
            if (index >= GameManager.instance.info.inven.Count) return;
            if(GameManager.instance.AddBag(GameManager.instance.info.inven[index]))
            {
                GameManager.instance.info.inven.RemoveAt(index);
                SetupInventory();
            }
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

            for (int i = 0; i < BagBtns.Length; i++)
            {
                BagBtns[i].GetComponentInChildren<Text>().text = "";
            }

            for (int i = 0; i < BagBtns.Length; i++)
            {
                if (GameManager.instance.info.bag.Count <= i) break;
                Item item = ItemManager.instance.GetItemByItemId(GameManager.instance.info.bag[i]);
                BagBtns[i].GetComponentInChildren<Text>().text = item.name;

                Color itemGradeColor = ItemManager.instance.GetColorByItemGrade(item.grade);
                ItemManager.instance.SetItemUIColor(BagBtns[i], itemGradeColor);
            }            
        }
    }

}