using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class LevelInfo : MonoBehaviour
    {
        public Text titleText;
        public Text contentText;
        public Button enterBtn;
        public Button returnBtn;

        void OnEnable()
        {
            if (GameManager.instance == null) return;
            if (!GameManager.instance.CheckState(GAME_STATE.LEVEL_INFO)) return;
            SetupInfo();
            enterBtn.onClick.AddListener(GameManager.instance.EnterLevel);
            returnBtn.onClick.AddListener(GameManager.instance.GotoDungeonMap);
        }

        string getShoTileText(SHOW_TYPE type)
        {
            string showTileText = "";
            if (type == SHOW_TYPE.ALL) showTileText = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_TILE_ALL);
            if (type == SHOW_TYPE.GEM_ITEM) showTileText = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_TILE_ITEM);
            if (type == SHOW_TYPE.MONSTER) showTileText = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_TILE_MONSTER);
            if (type == SHOW_TYPE.TRAP) showTileText = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_TILE_TRAP);;
            return showTileText;
        }

        void SetupInfo()
        {
            Dungeon curDungeon = GameManager.instance.GetDungeonInfo();
            if (curDungeon == null) return;
            Level curLevel = curDungeon.GetCurLevel();
            
            if(curDungeon.id >= 4)
            {
                titleText.text = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER) + " " + curLevel.name;
            }
            else
            {
                titleText.text = curLevel.name;
            }

            int countOfEnemies = curLevel.enemy;
            int countOfStongEnemies = curLevel.strongEnemy;
            string enemyText = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_SKELETON) + countOfEnemies + "\n";
            if(countOfStongEnemies > 0)
            {
                enemyText += LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_SKELETON2) + countOfStongEnemies + "\n";
            }
                        
            string showTileText = "\n" + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_TILE_INFO) + "\n" 
                + getShoTileText(GameManager.instance.curShowTilesOnStage[1].type) + " (1)\n" 
                + getShoTileText(GameManager.instance.curShowTilesOnStage[2].type) + " (1)\n" 
                + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_TILE_NEAR) + " (" + (GameManager.instance.curShowTilesOnStage.Count - 2).ToString() + ")\n";

            contentText.text = enemyText
                + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_TRAP) + curLevel.trap + "\n"
                + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER_INFO_GEM) + curLevel.gem + "\n"
                + showTileText;
        }
    }
}