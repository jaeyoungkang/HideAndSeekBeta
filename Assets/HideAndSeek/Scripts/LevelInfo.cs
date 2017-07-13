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
            if (type == SHOW_TYPE.ALL) showTileText = "전체보기";
            if (type == SHOW_TYPE.GEM_ITEM) showTileText = "보물보기";
            if (type == SHOW_TYPE.MONSTER) showTileText = "괴물보기";
            if (type == SHOW_TYPE.TRAP) showTileText = "함정보기";
            return showTileText;
        }

        void SetupInfo()
        {
            Dungeon curDungeon = GameManager.instance.GetDungeonInfo();
            if (curDungeon == null) return;
            Level curLevel = curDungeon.GetCurLevel();

            titleText.text = curLevel.name;
            int countOfEnemies = curLevel.enemy;
            int countOfStongEnemies = curLevel.strongEnemy;
            string enemyText = "해골 : " + countOfEnemies + "\n";
            if(countOfStongEnemies > 0)
            {
                enemyText += "해골 Lv2 : " + countOfStongEnemies + "\n";
            }
                        
            string showTileText = "\n특수 바닥 정보\n" + getShoTileText(GameManager.instance.curShowTilesOnStage[1].type) + ", " +
                getShoTileText(GameManager.instance.curShowTilesOnStage[2].type) + ", " +
                "근처보기" + (GameManager.instance.curShowTilesOnStage.Count - 2).ToString() + "개\n";

            contentText.text = enemyText
                + "함정 수: " + curLevel.trap + "\n"
                + "보석 수: " + curLevel.gem + "\n"
                + showTileText;
        }
    }
}