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

            string needAcoin = " 입장하는데 1개의 도굴삽이 필요\n";
            if (curDungeon.id == 0) needAcoin = "무료 입장\n"; 
            titleText.text = curDungeon.name;
            if (curDungeon.locked) titleText.text += " (유료)";
            contentText.text = "제한시간: " + curDungeon.TimeLimit() + "\n"
                + "레벨 수: " + curDungeon.levels.Length + "\n"
                + needAcoin
                + " 현재 보유한 도굴삽 : " + GameManager.instance.info.enableCount + "\n"
                + " 도  전 회수 : " + tryCount + "\n"
                + " 클리어 회수 : " + clearCount + "\n";


        }
    }
}