using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Inventory : MonoBehaviour
    {
        public Button ReturnBtn;        

        public Button[] BagBtns;
        public Button[] InvenBtns;

        void EnableBagSlot()
        {
            foreach (Button invenBtn in InvenBtns)
            {
                invenBtn.gameObject.SetActive(false);
            }

            for (int i = 0; i < GameManager.instance.info.invenSize; i++)
            {
                InvenBtns[i].gameObject.SetActive(true);
            }

            foreach (Button bagBtn in BagBtns)
            {
                bagBtn.gameObject.SetActive(false);
            }

            for (int i = 0; i < GameManager.instance.info.bagSize; i++)
            {
                BagBtns[i].gameObject.SetActive(true);
            }
        }

        void Start()
        {
            SetupInventory();
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

            ReturnBtn.onClick.AddListener(GameManager.instance.GoToLobby);
        }

        void MoveToInven(int index)
        {
            if (index >= GameManager.instance.bag.Count) return;
            int itemId = GameManager.instance.bag[index];
            GameManager.instance.inven.Add(itemId);
            GameManager.instance.bag.RemoveAt(index);
            SetupInventory();
        }

        void MoveToBag(int index)
        {
            if (index >= GameManager.instance.inven.Count) return;
            if(GameManager.instance.AddBag(GameManager.instance.inven[index]))
            {
                GameManager.instance.inven.RemoveAt(index);
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
                if (GameManager.instance.inven.Count <= i) break;
                Item item = ItemManager.instance.GetItemByItemId(GameManager.instance.inven[i]);
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
                if (GameManager.instance.bag.Count <= i) break;
                Item item = ItemManager.instance.GetItemByItemId(GameManager.instance.bag[i]);
                BagBtns[i].GetComponentInChildren<Text>().text = item.name;

                Color itemGradeColor = ItemManager.instance.GetColorByItemGrade(item.grade);
                ItemManager.instance.SetItemUIColor(BagBtns[i], itemGradeColor);
            }
        }
    }

}