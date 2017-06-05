using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public float price = 1f;
}

public class ShopScrollList : MonoBehaviour
{
    public List<Item> itemList;
    public Transform contentPanel;
    public ShopScrollList otherShop;
    public Text myGemDisplay;
    public SimpleObjectPool buttonObjectPool;


	void Start ()
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        AddButton();
    }
	
    private void AddButton()
    {
        foreach(Item item in itemList)
        {
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            SkillButton skillButton = newButton.GetComponent<SkillButton>();
            skillButton.Setup(item, this);
        }
    }
}
