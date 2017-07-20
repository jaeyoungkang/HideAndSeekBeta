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
                titleText.text = curDungeon.name + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CLEAR);

                contentsText.text = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_GET_GEM) + GameManager.instance.dungeonPlayData.gemCount + "\n\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_GET_ITEM) + GameManager.instance.dungeonPlayData.getItems.Count + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_USE_ITEM) + GameManager.instance.dungeonPlayData.useItems.Count + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_BUY_ITEM) + GameManager.instance.dungeonPlayData.butItems + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_USE_ITEM) + GameManager.instance.dungeonPlayData.sellItems + "\n\n"

                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DESTROY_MONSTER) + GameManager.instance.dungeonPlayData.damagedByEnemyCount + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DAMAGED_TRAP) + GameManager.instance.dungeonPlayData.damagedBytrapCount + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DAMAGED_TIME) + GameManager.instance.dungeonPlayData.damagedByTimeCount + "\n\n"

                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DESTROY_MONSTER) + GameManager.instance.dungeonPlayData.destroyEnemy + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DESTROY_TRAP) + GameManager.instance.dungeonPlayData.destroyTrap + "\n";

                retunrBtn.GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.GOTO_LOBBY_BTN);
                retunrBtn.onClick.AddListener(GameManager.instance.GoToLobby);                
            }
            else if(GameManager.instance.IsGameOver())
            {
                titleText.text = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.FAIL);
                contentsText.text = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_GET_GEM) + GameManager.instance.dungeonPlayData.gemCount + "\n\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_GET_ITEM) + GameManager.instance.dungeonPlayData.getItems.Count + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_USE_ITEM) + GameManager.instance.dungeonPlayData.useItems.Count + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_BUY_ITEM) + GameManager.instance.dungeonPlayData.butItems + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_USE_ITEM) + GameManager.instance.dungeonPlayData.sellItems + "\n\n"

                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DESTROY_MONSTER) + GameManager.instance.dungeonPlayData.damagedByEnemyCount + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DAMAGED_TRAP) + GameManager.instance.dungeonPlayData.damagedBytrapCount + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DAMAGED_TIME) + GameManager.instance.dungeonPlayData.damagedByTimeCount + "\n\n"

                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DESTROY_MONSTER) + GameManager.instance.dungeonPlayData.destroyEnemy + "\n"
                    + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RESULT_DESTROY_TRAP) + GameManager.instance.dungeonPlayData.destroyTrap + "\n";
                retunrBtn.GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.GOTO_LOBBY_BTN);
                retunrBtn.onClick.AddListener(GameManager.instance.GoToLobby);
            }
            else
            {
                titleText.text = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CLEAR_CHAMBER);
                retunrBtn.GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.GOTO_MAP_BTN);
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