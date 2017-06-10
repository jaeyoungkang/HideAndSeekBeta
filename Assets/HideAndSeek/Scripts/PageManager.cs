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
        public GameObject dungeonImage;
        public GameObject resultImage;
        public GameObject shopImage;
        public GameObject controller;
        public GameObject dungeonMap;
        public GameObject inventoryPage;
        public GameObject dungeonInfoPage;
        public GameObject tutorialPage;

        public GameObject statusPanel;

        public Text dungeonText;

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
            dungeonImage = GameObject.Find("DungeonImage");
            resultImage = GameObject.Find("ResultImage");
            controller = GameObject.Find("Controller");
            dungeonMap = GameObject.Find("DungeonMap");
            inventoryPage = GameObject.Find("InventoryPage");
            dungeonInfoPage = GameObject.Find("DungeonInfo");
            tutorialPage = GameObject.Find("TutorialInfo");

            statusPanel = GameObject.Find("Status");

            startButton = GameObject.Find("FrontPageButton").GetComponent<Button>();

            dungeonText = GameObject.Find("DungeonText").GetComponent<Text>();

            startButton.onClick.AddListener(GameManager.instance.GoToLobby);
        }


        public void Setup(GAME_STATE gameState)
        {
            bool bTitle = false;
            bool bLobby = false;
            bool bShop = false;
            bool bDungeon = false;
            bool bDungeonInfo = false;
            bool bResult = false;
            bool bPlay = false;
            bool bMap = false;
            bool bInventory = false;
            bool bTutorial = false;

            switch (gameState)
            {
                case GAME_STATE.TUTORIAL: bTutorial = true; break;

                case GAME_STATE.START: bTitle = true; break;
                case GAME_STATE.SHOP: bShop = true; break;
                case GAME_STATE.LOBBY: bLobby = true; break;
                case GAME_STATE.LEVEL: bDungeon = true; break;
                case GAME_STATE.DUNGEON_INFO: bDungeonInfo = true; break;                    
                case GAME_STATE.MAP: bMap = true; break;
                case GAME_STATE.PLAY: bPlay = true; break;
                case GAME_STATE.RESULT: bResult = true; break;
                case GAME_STATE.OVER: bResult = true; break;
                case GAME_STATE.INVENTORY: bInventory = true; break;
                    
            }

            tutorialPage.SetActive(bTutorial);

            shopImage.SetActive(bShop);
            lobbyImage.SetActive(bLobby);
            titleImage.SetActive(bTitle);
            dungeonImage.SetActive(bDungeon);
            dungeonInfoPage.SetActive(bDungeonInfo);
            resultImage.SetActive(bResult);
            controller.SetActive(bPlay);
            dungeonMap.SetActive(bMap);
            inventoryPage.SetActive(bInventory);            

            if (bMap || bDungeon || bResult || bPlay)
            {
                statusPanel.gameObject.SetActive(true);
            }
            else
            {
                statusPanel.gameObject.SetActive(false);
            }            
        }
        
        public void SetLevelEnterPageText(string content)
        {
            dungeonText.text = content;
        }
    }
}