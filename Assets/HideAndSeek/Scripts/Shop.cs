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

        void Start()
        {
            SetupDisplay();
            SetupInventory();

            InvenBtns[0].onClick.AddListener(() => { SellSkill(0); });
            InvenBtns[1].onClick.AddListener(() => { SellSkill(1); });
            InvenBtns[2].onClick.AddListener(() => { SellSkill(2); });
            InvenBtns[3].onClick.AddListener(() => { SellSkill(3); });
            DisplayBtns[0].onClick.AddListener(() => { BuySkill(0); });
            DisplayBtns[1].onClick.AddListener(() => { BuySkill(1); });
            DisplayBtns[2].onClick.AddListener(() => { BuySkill(2); });
            DisplayBtns[3].onClick.AddListener(() => { BuySkill(3); });
            DisplayBtns[4].onClick.AddListener(() => { BuySkill(4); });

            ReturnBtn.onClick.AddListener(GameManager.instance.GoToLobby);

        }

        void BuySkill(int index)
        {
            if (index >= display.Length) return;
            if (GameManager.instance.invenGem < display[index].price) return;

            GameManager.instance.invenGem -= display[index].price;
            GameManager.instance.inven.Add(display[index]);
            
            SetupInventory();            
        }

        void SellSkill(int index)
        {
            if (index >= GameManager.instance.inven.Count) return;
            GameManager.instance.inven.RemoveAt(index);
            GameManager.instance.invenGem += display[index].price;
            SetupInventory();
            
        }

        private void SetupInventory()
        {
            for (int i = 0; i < 4; i++)
            {
                InvenBtns[i].GetComponentInChildren<Text>().text = "";
            }

            for (int i = 0; i < 4; i++)
            {
                if (GameManager.instance.inven.Count <= i) break;
                InvenBtns[i].GetComponentInChildren<Text>().text = GameManager.instance.inven[i].skillname;
            }

            GemText.text = "Gem: " + GameManager.instance.invenGem.ToString();
        }

        private void SetupDisplay()
        {
            for (int i = 0; i < 5; i++)
            {
                DisplayBtns[i].GetComponentInChildren<Text>().text = display[i].skillname + "\n(" + display[i].price + ")";
            }
        }
    }
}