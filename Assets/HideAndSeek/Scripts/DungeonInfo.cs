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
            Dungeon curDungeon =  GameManager.instance.GetDungeonInfo();
            if (curDungeon == null) return;

            titleText.text = curDungeon.name;
            contentText.text = "제한시간: " + curDungeon.TimeLimit() + "\n"
                + "레벨 수: " + curDungeon.levels.Length + "\n"
                + "보상: " + curDungeon.gem + "\n"
                + "진입비용: " + curDungeon.cost + "\n";
        }
    }
}