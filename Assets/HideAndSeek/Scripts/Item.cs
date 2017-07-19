using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HideAndSeek
{
    public enum ITEM_ID
    {
        HEAL1 = 101,
        HEAL2,
        HEAL3,
        FRAGMENT_NEAR = 104,
        FRAGMENT_MONSTER,
        FRAGMENT_TRAP,
        FRAGMENT_ALL,
        FRAGMENT_ITEM,
        DESTROY_4D,
        DESTROY_LR,
        DESTROY_UD,
        ADD_TIME,
        STOP_TIME,
        HIDE,
        EXTEND_MAX_HP,
        EXTEND_BAG,
        ESCAPE
    }

    [System.Serializable]
    public class Item 
    {
        public string name;
        public ITEM_ID id;
        public int price;
        public int grade;
        public bool enableSell;

        public Sprite tile;

        public string description;
    }
}