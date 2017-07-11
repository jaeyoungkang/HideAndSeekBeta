using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class DungeonMapLarge : DungeonMap
    {
        void Start()
        {
            if (GameManager.instance == null) return;
            if (GameManager.instance.GetDungeonInfo() == null) return;

            SetupBtns(GameManager.instance.GetDungeonInfo().levels);
            SetupLineImages(GameManager.instance.GetDungeonInfo().levels);

            levelBtns[0].onClick.AddListener(() => { GameManager.instance.SelectLevel(1); });
            levelBtns[1].onClick.AddListener(() => { GameManager.instance.SelectLevel(2); });
            levelBtns[2].onClick.AddListener(() => { GameManager.instance.SelectLevel(3); });
            levelBtns[3].onClick.AddListener(() => { GameManager.instance.SelectLevel(4); });
            levelBtns[4].onClick.AddListener(() => { GameManager.instance.SelectLevel(5); });
            levelBtns[5].onClick.AddListener(() => { GameManager.instance.SelectLevel(6); });
            levelBtns[6].onClick.AddListener(() => { GameManager.instance.SelectLevel(7); });
            levelBtns[7].onClick.AddListener(() => { GameManager.instance.SelectLevel(8); });
            levelBtns[8].onClick.AddListener(() => { GameManager.instance.SelectLevel(9); });
            levelBtns[9].onClick.AddListener(() => { GameManager.instance.SelectLevel(10); });
            levelBtns[10].onClick.AddListener(() => { GameManager.instance.SelectLevel(11); });
            levelBtns[11].onClick.AddListener(() => { GameManager.instance.SelectLevel(12); });
            levelBtns[12].onClick.AddListener(() => { GameManager.instance.SelectLevel(13); });
            levelBtns[13].onClick.AddListener(() => { GameManager.instance.SelectLevel(14); });
            levelBtns[14].onClick.AddListener(() => { GameManager.instance.SelectLevel(15); });
            levelBtns[15].onClick.AddListener(() => { GameManager.instance.SelectLevel(16); });

            shopBtns.onClick.AddListener(GameManager.instance.EnterShop);
        }


    }
}
