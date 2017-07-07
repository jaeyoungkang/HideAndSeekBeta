using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using UnityEngine.Analytics;

namespace HideAndSeek
{
    public class ResultPage : MonoBehaviour
    {
        public Text titleText;
        public Text contentsText;

        public Button retunrBtn;

        void OnEnable()
        {
            if (GameManager.instance == null) return;
            if (!GameManager.instance.CheckState(GAME_STATE.RESULT) &&
                !GameManager.instance.CheckState(GAME_STATE.OVER)) return;
            Start();
        }

        void Start()
        {
            if (GameManager.instance == null) return;
            Dungeon curDungeon = GameManager.instance.GetDungeonInfo();
            if (curDungeon == null) return;

            contentsText.text = "";
            if (curDungeon.IsEnd())
            {
                titleText.text = curDungeon.name +" 클리어!";

                contentsText.text = "획득한 보석 : " + GameManager.instance.dungeonPlayData.gemCount + "\n\n"
                    + "획득한 아이템 수 : " + GameManager.instance.dungeonPlayData.getItems.Count + "\n"
                    + "사용한 아이템 수 : " + GameManager.instance.dungeonPlayData.useItems.Count + "\n"
                    + "구입한 아이템 수 : " + GameManager.instance.dungeonPlayData.butItems + "\n"
                    + "판매한 아이템 수 : " + GameManager.instance.dungeonPlayData.sellItems + "\n\n"

                    + "괴물에 의한 피해 : " + GameManager.instance.dungeonPlayData.damagedByEnemyCount + "\n"
                    + "함정에 의한 피해 : " + GameManager.instance.dungeonPlayData.damagedBytrapCount + "\n"
                    + "시간에 의한 피해 : " + GameManager.instance.dungeonPlayData.damagedByTimeCount + "\n\n"

                    + "제거한 괴물 수 : " + GameManager.instance.dungeonPlayData.destroyEnemy + "\n"
                    + "제거한 함정 수 : " + GameManager.instance.dungeonPlayData.destroyTrap + "\n";

                retunrBtn.GetComponentInChildren<Text>().text = "GO TO LOBBY";
                retunrBtn.onClick.AddListener(GameManager.instance.GoToLobby);                
            }
            else if(GameManager.instance.IsGameOver())
            {
                titleText.text = "실패!";
                contentsText.text = "획득한 보석 : " + GameManager.instance.dungeonPlayData.gemCount + "\n\n"
                    + "획득한 아이템 수 : " + GameManager.instance.dungeonPlayData.getItems.Count + "\n"
                    + "사용한 아이템 수 : " + GameManager.instance.dungeonPlayData.useItems.Count + "\n"
                    + "구입한 아이템 수 : " + GameManager.instance.dungeonPlayData.butItems + "\n"
                    + "판매한 아이템 수 : " + GameManager.instance.dungeonPlayData.sellItems + "\n\n"

                    + "괴물에 의한 피해 : " + GameManager.instance.dungeonPlayData.damagedByEnemyCount + "\n"
                    + "함정에 의한 피해 : " + GameManager.instance.dungeonPlayData.damagedBytrapCount + "\n"
                    + "시간에 의한 피해 : " + GameManager.instance.dungeonPlayData.damagedByTimeCount + "\n\n"

                    + "제거한 괴물 수 : " + GameManager.instance.dungeonPlayData.destroyEnemy + "\n"
                    + "제거한 함정 수 : " + GameManager.instance.dungeonPlayData.destroyTrap + "\n";
                retunrBtn.GetComponentInChildren<Text>().text = "GO TO LOBBY";
                retunrBtn.onClick.AddListener(GameManager.instance.GoToLobby);
            }
            else
            {
                titleText.text = "레벨 클리어!";
                retunrBtn.GetComponentInChildren<Text>().text = "RETUN TO MAP";
                retunrBtn.onClick.AddListener(GameManager.instance.GotoDungeonMap);
            }            
        }

        //public void WritelevelData()
        //{
        //    DungeonPlayData data = GameManager.instance.playData;
        //    string info = "";
        //    info += data.dungeonName + "\t" + data.levelName + "\t" + data.gemCount + "\t" + data.deathCount + "\t" + data.attackedCount + "\t" + data.damagedByTimeCount + "\t";
        //    foreach (string name in data.getItems)
        //    {
        //        info += name + ", ";
        //    }
        //    info += "\t";

        //    foreach (string name in data.useItems)
        //    {
        //        info += name + ", ";
        //    }
        //    info += "\t";

        //    foreach (int hp in data.hps)
        //    {
        //        info += hp + ", ";
        //    }
        //    info += "\t";


        //    if (data.levelName != "") SaveLoad.WriteFile(GameManager.instance.logfileName, info);            
        //}

    }
}