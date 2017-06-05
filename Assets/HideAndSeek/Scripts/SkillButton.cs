using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        priceLabel.text = item.price.ToString();
        nameLabel.text = item.itemName;
        iconImage.sprite = item.icon;  

    }
}
