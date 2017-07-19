using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager instance = null;
        public Dictionary<ITEM_ID, Item> itemList = new Dictionary<ITEM_ID, Item>();
        public Item[] items;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                MakeItemList();
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            
        }

        public void MakeItemList()
        {
            itemList.Clear();
            foreach (Item item in items)
            {
                if (!itemList.ContainsKey(item.id))
                {
                    item.name = LocalizationManager.instance.GetItemName(item.id);
                    item.description = LocalizationManager.instance.GetItemDiscription(item.id);
                    itemList.Add(item.id, item);
                }
            }
        }

        public int GetPriceByItemId(ITEM_ID id)
        {
            if (itemList.ContainsKey(id)) return itemList[id].price;

            print("Error: id doesnot exist8");
            return -1;
        }

        public Item GetItemByItemId(ITEM_ID id)
        {
            if (itemList.ContainsKey(id)) return itemList[id];

            print("Error: id doesnot exist8");
            return null;
        }

        public string GetNameByItemId(ITEM_ID id)
        {
            if (itemList.ContainsKey(id)) return itemList[id].name;

            print("Error: id doesnot exist8");
            return "";
        }

        public ITEM_ID[] GetEnableToSellItemIds()
        {
            List<ITEM_ID> itemIds = new List<ITEM_ID>();
            foreach(Item item in items)
            {
                if (item.enableSell) itemIds.Add(item.id);
            }
            return itemIds.ToArray();
        }
    }
}