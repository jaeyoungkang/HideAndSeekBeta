using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class ItemDescriptionPage : MonoBehaviour {

        public Text title;
        public Image[] itemIcons;
        public Text[] itemDesc;
        public Button closeBtn;

        // Use this for initialization
        void Start() {
            title.text = LocalizationManager.instance.GetLocalUIString(UI_STRING.ITEM_HELP);

            closeBtn.onClick.AddListener(ClosePage);

            for (int i = 0; i < itemIcons.Length; i++)
            {
                if (ItemManager.instance.items.Length<= i) break;
                Item itemInfo = ItemManager.instance.items[i];
                if (itemInfo == null) continue;

                itemDesc[i].text = itemInfo.description;
                itemIcons[i].sprite= itemInfo.tile;
            }
        }

        void ClosePage()
        {
            gameObject.SetActive(false);
        }        
    }
}