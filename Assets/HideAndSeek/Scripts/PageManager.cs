using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class PageManager : MonoBehaviour {
        public static PageManager instance = null;
        public GameObject titleImage;
        public GameObject lobbyImage;
        public GameObject levelEnterImage;
        public GameObject resultImage;
        public GameObject shopImage;
        public GameObject controller;
        public GameObject dungeonMap;
        public GameObject dungeonMapLarge;
        public GameObject dungeonInfoPage;
        public GameObject levelInfoPage;

        public GameObject statusPanel;

        public Text dungeonTitleText;
        public Text levelTitleText;

        public Button startButton;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public void InitUI()
        {
            titleImage = GameObject.Find("FrontPageImage");
            lobbyImage = GameObject.Find("LobbyImage");
            shopImage = GameObject.Find("ShopImage");
            levelEnterImage = GameObject.Find("LevelEnterImage");
            resultImage = GameObject.Find("ResultImage");
            controller = GameObject.Find("Controller");
            dungeonMap = GameObject.Find("DungeonMap");
            dungeonMapLarge = GameObject.Find("DungeonMapLarge");
            
            dungeonInfoPage = GameObject.Find("DungeonInfo");
            levelInfoPage = GameObject.Find("LevelInfo");

            statusPanel = GameObject.Find("Status");

            startButton = GameObject.Find("FrontPageButton").GetComponent<Button>();

            dungeonTitleText = GameObject.Find("DungeonTitleText").GetComponent<Text>();
            levelTitleText = GameObject.Find("LevelTitleText").GetComponent<Text>();

            startButton.onClick.AddListener(GameManager.instance.GoToLobby);
        }

        public void Setup(GAME_STATE gameState)
        {
            bool bTitle = false;
            bool bLobby = false;
            bool bShop = false;
            bool bDungeon = false;
            bool bDungeonInfo = false;
            bool bLevelInfo = false;
            bool bResult = false;
            bool bPlay = false;
            bool bMap = false;
            bool bMapLarge = false;

            switch (gameState)
            {
                case GAME_STATE.START: bTitle = true; break;
                case GAME_STATE.SHOP: bShop = true; break;
                case GAME_STATE.LOBBY: bLobby = true; break;
                case GAME_STATE.LEVEL: bDungeon = true; break;
                case GAME_STATE.LEVEL_INFO: bLevelInfo = true; break;
                case GAME_STATE.DUNGEON_INFO: bDungeonInfo = true; break;                    
                case GAME_STATE.MAP: bMap = true; break;
                case GAME_STATE.MAP_LARGE: bMapLarge = true; break;                    
                case GAME_STATE.PLAY: bPlay = true; break;
                case GAME_STATE.RESULT: bResult = true; break;
                case GAME_STATE.OVER: bResult = true; break;
            }

            shopImage.SetActive(bShop);
            lobbyImage.SetActive(bLobby);
            titleImage.SetActive(bTitle);
            levelEnterImage.SetActive(bDungeon);
            dungeonInfoPage.SetActive(bDungeonInfo);
            levelInfoPage.SetActive(bLevelInfo);
            resultImage.SetActive(bResult);
            controller.SetActive(bPlay);            
            dungeonMap.SetActive(bMap);
            dungeonMapLarge.SetActive(bMapLarge);



            if (bMap || bDungeon || bResult || bPlay || bShop || bMap || bMapLarge)
            {
                statusPanel.gameObject.SetActive(true);
            }
            else
            {
                statusPanel.gameObject.SetActive(false);
            }            
        }
        
        public void SetLevelEnterPageText(string dungeonTitle, string levelTitle)
        {
            dungeonTitleText.text = dungeonTitle;
            levelTitleText.text = levelTitle;
        }
    }
}