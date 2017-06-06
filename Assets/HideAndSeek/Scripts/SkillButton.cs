using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class SkillButton : MonoBehaviour
    {
        public Button button;
        public Text nameLabel;
        public Text priceLabel;
        public Image iconImage;

        private Item item;
        private ShopScrollList scrollList;

        public void Setup(Item currentItem, ShopScrollList currentScrollList)
        {
            item = currentItem;
            scrollList = currentScrollList;

            priceLabel.text = item.info.price.ToString();
            nameLabel.text = item.info.skillname;
//            iconImage.sprite = item.icon;

        }
    }
}