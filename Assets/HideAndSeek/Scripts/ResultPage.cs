using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class ResultPage : MonoBehaviour
    {
        public Text titleText;
        public Text contentsText;

        public Button retunrBtn;

        void OnEnable()
        {            
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
                if(GameManager.instance.isClearTutorial == false)
                {
                    GameManager.instance.isClearTutorial = true;
                    titleText.text = "튜토리얼 클리어!";
                }
                else
                {
                    titleText.text = "던전 클리어!";
                }
                
                string content = "Gem Clear Reward : " + curDungeon.GetReward() + "\n";
                content += "Gem discoverd : " + GameManager.instance.dungeonGem;
                contentsText.text = content;

                retunrBtn.GetComponentInChildren<Text>().text = "GO TO LOBBY";
                retunrBtn.onClick.AddListener(GameManager.instance.GoToLobby);
            }
            else if(GameManager.instance.IsGameOver())
            {
                titleText.text = "실패!";
                retunrBtn.GetComponentInChildren<Text>().text = "GO TO LOBBY";
                retunrBtn.onClick.AddListener(GameManager.instance.GoToLobby);
            }
            else
            {
                titleText.text = "레벨 클리어!";
                retunrBtn.GetComponentInChildren<Text>().text = "RETUN TO MAP";
                retunrBtn.onClick.AddListener(GameManager.instance.GotoDungeonMap);
            }            
        }

    }
}