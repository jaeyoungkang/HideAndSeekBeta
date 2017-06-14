using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class DungeonInfo : MonoBehaviour
    {
        public Text titleText;
        public Text gemText;
        public Text contentText;
        public Button invenBtn;
        public Button enterBtn;
        public Button returnBtn;

        void Start()
        {
            SetupInfo();

            invenBtn.onClick.AddListener(GameManager.instance.EnterInven);
            enterBtn.onClick.AddListener(GameManager.instance.EnterDungeon);
            returnBtn.onClick.AddListener(GameManager.instance.GoToLobby);
        }

        void SetupInfo()
        {
            if (GameManager.instance == null) return;
            Dungeon curDungeon =  GameManager.instance.GetDungeonInfo();
            if (curDungeon == null) return;

            titleText.text = curDungeon.name;
            gemText.text = "보유 보석: " + GameManager.instance.info.invenGem;
            contentText.text = "제한시간: " + curDungeon.TimeLimit() + "\n"
                + "레벨 수: " + curDungeon.levels.Length + "\n"
                + "보상 보석: " + curDungeon.gem + "\n"
                + "보상 주화: 1\n"
                + "진입 비용: " + curDungeon.cost + "\n";
        }
    }
}