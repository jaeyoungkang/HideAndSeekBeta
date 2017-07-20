using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class DungeonInfo : MonoBehaviour
    {
        public Text titleText;
        public Text contentText;
        public Button enterBtn;
        public Button returnBtn;

        void Start()
        {
            SetupInfo();

            enterBtn.onClick.AddListener(GameManager.instance.EnterDungeon);
            returnBtn.onClick.AddListener(GameManager.instance.GoToLobby);
        }

        void SetupInfo()
        {
            if (GameManager.instance == null) return;
            Dungeon curDungeon = GameManager.instance.GetDungeonInfo();
            if (curDungeon == null) return;

            int tryCount = 0;
            int clearCount = 0;

            if (GameManager.instance.info.dungeonTryCount.ContainsKey(curDungeon.id))
            {
                tryCount = GameManager.instance.info.dungeonTryCount[curDungeon.id] - 1;
            }

            if (GameManager.instance.info.dungeonClearCount.ContainsKey(curDungeon.id))
            {
                clearCount = GameManager.instance.info.dungeonClearCount[curDungeon.id];
            }

            string needAcoin = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.INFO_NEED_SHOVEL);
            if (curDungeon.id == 0) needAcoin = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.INFO_FREE_ENTER);
            titleText.text = curDungeon.name;
            contentText.text = needAcoin + "\n"
                + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.INFO_TIME_LIMIT) + curDungeon.TimeLimit() + "\n"
                + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.INFO_NUM_CHAMBER) + curDungeon.levels.Length + "\n"
                + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.INFO_NUM_SHOVEL) + GameManager.instance.info.enableCount + "\n"
                + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.INFO_NUM_CHALLANGE) + tryCount + "\n"
                + LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.INFO_NUM_CLEAR) + clearCount + "\n";


        }
    }
}