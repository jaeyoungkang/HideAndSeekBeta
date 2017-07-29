using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public class PageManager : MonoBehaviour {
        public static PageManager instance = null;
        public GameObject purchaseImage;
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
        public GameObject developerCommentsImage;
        public GameObject escapeImage;

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
            escapeImage = GameObject.Find("EscapeImage");
            developerCommentsImage = GameObject.Find("DeveloperImage");
            purchaseImage = GameObject.Find("PurchaseImage");
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

            startButton.onClick.AddListener(ShowNotice);

            GameObject.Find("FrontPageTitleText").GetComponent<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.FRONT_TITLE);
            GameObject.Find("FrontPageButton").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.FRONT_BUTTON);

            GameObject.Find("LobbyTitleText").GetComponent<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.LOBBY_TITLE);
            GameObject.Find("GetAShovelButton").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.LOBBY_BUTTON_SHOVEL);
            GameObject.Find("PurchaseButton").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.LOBBY_BUTTON_PURCHASE);

            GameObject.Find("PurchaseTitleText").GetComponent<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.PURCHASE_TITLE);
            GameObject.Find("PurchaseNoticeText").GetComponent<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.PURCHASE_NOTICE);
            GameObject.Find("PurchaseButton1").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.PURCHASE_BTN);
            GameObject.Find("PurchaseButton2").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.PURCHASE_BTN);
            GameObject.Find("PurchaseButton3").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.PURCHASE_BTN);
            GameObject.Find("PurchaseReturnButton").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.RETURN_BTN);

            GameObject.Find("DungeonInfoEnterButton").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.ENTER_BTN);
            GameObject.Find("DungeonInfoRetunButton").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.RETURN_BTN);

            GameObject.Find("LevelInfoEnterButton").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.ENTER_BTN);
            GameObject.Find("LevelInfoReturnButton").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.RETURN_BTN);

            GameObject.Find("DungeonMapLargeShop").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.ENTER_SHOP_BTN);            
            GameObject.Find("DungeonMapshop").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.ENTER_SHOP_BTN);

            GameObject.Find("DungeonMapLeave").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.LEAVE_DUNGEON);
            GameObject.Find("DungeonMapLargeLeave").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.LEAVE_DUNGEON);            

            GameObject.Find("ShopTitleText").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.SHOP_TITLE);
            GameObject.Find("ShopDisplayText").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.SHOP_DISPLAY);
            GameObject.Find("ShopBagText").GetComponentInChildren<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.SHOP_BAG);
        }

        public void ShowNotice()
        {
            startButton.gameObject.SetActive(false);
            GameObject.Find("FrontPageNoticeText").GetComponent<Text>().text = LocalizationManager.instance.GetLocalUIString(UI_STRING.FRONT_WARNING);
            Invoke("StartGame", 3f);
        }

        public void StartGame()
        {            
            GameManager.instance.GoToLobby();
        }

        public void Setup(GAME_STATE gameState)
        {
            bool bDeveloper = false;
            bool bTitle = false;
            bool bLobby = false;
            bool bShop = false;
            bool bPurchase = false;
            bool bDungeon = false;
            bool bDungeonInfo = false;
            bool bLevelInfo = false;
            bool bResult = false;
            bool bPlay = false;
            bool bMap = false;
            bool bMapLarge = false;
            bool bEscape = false;

            switch (gameState)
            {
                case GAME_STATE.ESCAPE: bEscape = true; break;
                case GAME_STATE.DEVELOPER_COMMENTS: bDeveloper = true; break;
                case GAME_STATE.PURCHASE: bPurchase = true; break;
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

            escapeImage.SetActive(bEscape);
            developerCommentsImage.SetActive(bDeveloper);
            purchaseImage.SetActive(bPurchase);
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