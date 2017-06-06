using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        public Skill[] display;

        public Button[] InvenBtns;
        public Button ReturnBtn;

        public Text GemText;

        public int invenSize;

        void EnableInvenSlot()
        {
            foreach (Button invenBtn in InvenBtns)
            {
                invenBtn.gameObject.SetActive(false);
            }

            for (int i = 0; i < invenSize; i++)
            {
                InvenBtns[i].gameObject.SetActive(true);
            }
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

        void BuySkill(int index)
        {            
            if (index >= display.Length) return;
            if (GameManager.instance.invenGem < display[index].price) return;

            if(display[index].skillname == "인벤추가")
            {
                GameManager.instance.invenGem -= display[index].price;
                
                if (invenSize < InvenBtns.Length) invenSize++;
                EnableInvenSlot();
            }
            else
            {
                if (invenSize == GameManager.instance.inven.Count) return;
                GameManager.instance.inven.Add(display[index]);
                GameManager.instance.invenGem -= display[index].price;
                SetupInventory();
            }

            GemText.text = "Gem: " + GameManager.instance.invenGem.ToString();
        }

        void SellSkill(int index)
        {
            if (index >= GameManager.instance.inven.Count) return;
            GameManager.instance.invenGem += GameManager.instance.inven[index].price;
            GameManager.instance.inven.RemoveAt(index);            
            SetupInventory();

            GemText.text = "Gem: " + GameManager.instance.invenGem.ToString();
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
                InvenBtns[i].GetComponentInChildren<Text>().text = GameManager.instance.inven[i].skillname;
            }            
        }

        private void SetupDisplay()
        {
            for (int i = 0; i < DisplayBtns.Length; i++)
            {
                DisplayBtns[i].GetComponentInChildren<Text>().text = display[i].skillname + "\n(" + display[i].price + ")";
            }
        }
    }
}