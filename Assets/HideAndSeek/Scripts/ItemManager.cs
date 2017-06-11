using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager instance = null;
        public Dictionary<int, Item> itemList = new Dictionary<int, Item>();
        public Item[] items;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            itemList.Clear();
            foreach (Item item in items)
            {
                if (!itemList.ContainsKey(item.id)) itemList.Add(item.id, item);
            }
        }

        public int GetPriceByItemId(int id)
        {
            if (itemList.ContainsKey(id)) return itemList[id].price;

            print("Error: id doesnot exist8");
            return -1;
        }

        public Item GetItemByItemId(int id)
        {
            if (itemList.ContainsKey(id)) return itemList[id];

            print("Error: id doesnot exist8");
            return null;
        }

        public string GetNameByItemId(int id)
        {
            if (itemList.ContainsKey(id)) return itemList[id].name;

            print("Error: id doesnot exist8");
            return "";
        }

        public int[] GetEnableToSellItemIds()
        {
            List<int> itemIds = new List<int>();
            foreach(Item item in items)
            {
                if (item.enableSell) itemIds.Add(item.id);
            }
            return itemIds.ToArray();
        }

        public void SetItemUIColor(Button btn, Color itemGradeColor)
        {
            btn.GetComponentInChildren<Text>().color = itemGradeColor;
            //var colors = btn.colors;
            //colors.normalColor = itemGradeColor;
            //btn.colors = colors;
        }

        public Color GetColorByItemGrade(int grade)
        {
            switch(grade)
            {
                case 1: return Color.white;
                case 2: return Color.green;
                case 3: return Color.cyan;
                case 4: return Color.yellow;
                default:
                    return Color.white;
            }
        }
    }
}