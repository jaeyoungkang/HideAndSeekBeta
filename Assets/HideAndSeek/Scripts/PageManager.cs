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

        public GameObject statusPanel;

        public Button dungeonABtn;
        public Button dungeonBBtn;
        public Button dungeonCBtn;
        public Button resultButton;
        public Text dungeonText;
        public Text resultText;

        public Button endSelectSkillBtn;

        public Button startButton;

        public Button shopBtn;
        public Button invenBtn;

        public Image GemImage;
        public Text HpText;
        public Text GemText;
        public Text TimeText;

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

            statusPanel = GameObject.Find("Status");

            shopBtn = GameObject.Find("ShopButton").GetComponent<Button>();
            invenBtn = GameObject.Find("InventoryButton").GetComponent<Button>();

            startButton = GameObject.Find("FrontPageButton").GetComponent<Button>();

            dungeonText = GameObject.Find("DungeonText").GetComponent<Text>();
            resultText = GameObject.Find("ResultText").GetComponent<Text>();

            HpText = GameObject.Find("HpText").GetComponent<Text>();
            GemText = GameObject.Find("GemText").GetComponent<Text>();
            TimeText = GameObject.Find("TimeText").GetComponent<Text>();
            GemImage = GameObject.Find("GemImage").GetComponent<Image>();

            resultButton = GameObject.Find("ResultButton").GetComponent<Button>();
            dungeonABtn = GameObject.Find("DungeonABtn").GetComponent<Button>();
            dungeonBBtn = GameObject.Find("DungeonBBtn").GetComponent<Button>();
            dungeonCBtn = GameObject.Find("DungeonCBtn").GetComponent<Button>();

            SetupListners();
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

            switch (gameState)
            {
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

        public void SetupListners()
        {            
            resultButton.onClick.AddListener(GameManager.instance.GotoDungeonMap);
            
            dungeonABtn.onClick.AddListener(() => { GameManager.instance.SelectDungeon(0); });
            dungeonBBtn.onClick.AddListener(() => { GameManager.instance.SelectDungeon(1); });
            dungeonCBtn.onClick.AddListener(() => { GameManager.instance.SelectDungeon(2); });

            shopBtn.onClick.AddListener(GameManager.instance.EnterShop);
            invenBtn.onClick.AddListener(GameManager.instance.EnterInven);
            startButton.onClick.AddListener(GameManager.instance.GoToLobby);
        }

        public void SetLevelEnterPageText(string content)
        {
            dungeonText.text = content;
        }

        public void SetResultPageText(int reward, int gem)
        {
            string content = "Gem Clear Reward : " + reward.ToString() + "\n";
            content += "Gem discoverd : " + gem;
            resultText.text = content;
        }        
    }
}