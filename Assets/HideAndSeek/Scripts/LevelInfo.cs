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

        void SetupInfo()
        {
            Dungeon curDungeon = GameManager.instance.GetDungeonInfo();
            if (curDungeon == null) return;
            Level curLevel = curDungeon.GetCurLevel();

            titleText.text = curLevel.name;
            string existEnemy = curLevel.enemy > 0 ? "있음" : "없음";            
            string existItem = curLevel.itemTileNumbers.Length > 0 ? "있음" : "없음";

            contentText.text = "해골: " + existEnemy + "\n"
                + "함정 수: " + curLevel.trap + "\n"
                + "보석 수: " + curLevel.gem + "\n"
                + "아이템: " +existItem+ "\n";
        }
    }
}