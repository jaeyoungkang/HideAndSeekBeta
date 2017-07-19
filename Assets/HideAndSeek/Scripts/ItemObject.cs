using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class ItemObject : MonoBehaviour
    {
        public ITEM_ID itemId;

        void Start()
        {
            Item itemInfo = ItemManager.instance.GetItemByItemId(itemId);
            if(itemInfo == null)
            {
                print("Error : wrong id! " + itemId);
                return;
            }
            SpriteRenderer renterer = gameObject.GetComponent<SpriteRenderer>();
            renterer.sprite = itemInfo.tile;
        }
    }
}