using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class Tutorial : MonoBehaviour
    {
        public Text titleText;
        public Text contentText;
        public Button enterBtn;

        void OnEnable()
        {
            if (GameManager.instance == null) return;
            SetupInfo();
            enterBtn.onClick.AddListener(GameManager.instance.EnterDungeon);
        }

        void SetupInfo()
        {
            GameManager.instance.SelectDungeon(GameManager.instance.tutorial);
            Dungeon curDungeon = GameManager.instance.tutorial;
            if (curDungeon == null) return;

            titleText.text = curDungeon.name;
            contentText.text = "제한시간: " + curDungeon.TimeLimit() + "\n"
                + "레벨 수: " + curDungeon.levels.Length + "\n"
                + "보상: " + curDungeon.gem + "\n"
                + "진입비용: " + curDungeon.cost + "\n";
        }
    
    }
}