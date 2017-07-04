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
            Dungeon curDungeon =  GameManager.instance.GetDungeonInfo();
            if (curDungeon == null) return;

            titleText.text = curDungeon.name;
            if (curDungeon.locked) titleText.text += " (유료)";
            contentText.text = "제한시간: " + curDungeon.TimeLimit() + "\n"
                + "레벨 수: " + curDungeon.levels.Length + "\n"
                + " 입장하는데 1개의 고대 주화가 필요\n"
                + " 현재 보유한 고대 주화 : " + GameManager.instance.info.enableCount + "\n";

            
        }
    }
}