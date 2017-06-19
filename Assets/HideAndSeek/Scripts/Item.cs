using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HideAndSeek
{
    [System.Serializable]
    public class Item 
    {
        public string name;
        public int id;
        public int price;
        public int grade;
        public bool enableSell;

        public Sprite tile;
    }
}