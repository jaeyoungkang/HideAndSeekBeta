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

        public Button dungeonABtn;
        public Button dungeonBBtn;
        public Button dungeonCBtn;
        public Button resultButton;
        public Text dungeonText;
        public Text resultText;

        public Button endSelectSkillBtn;

        public Button level1Btn;
        public Button level2Btn;
        public Button level3Btn;
        public Button level4Btn;
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

            shopBtn = GameObject.Find("ShopButton").GetComponent<Button>();
            invenBtn = GameObject.Find("InventoryButton").GetComponent<Button>();

            startButton = GameObject.Find("FrontPageButton").GetComponent<Button>();

            level1Btn = GameObject.Find("level1").GetComponent<Button>();
            level2Btn = GameObject.Find("level2").GetComponent<Button>();
            level3Btn = GameObject.Find("level3").GetComponent<Button>();
            level4Btn = GameObject.Find("level4").GetComponent<Button>();

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
            resultImage.SetActive(bResult);
            controller.SetActive(bPlay);
            dungeonMap.SetActive(bMap);
            inventoryPage.SetActive(bInventory);

            if (bMap || bDungeon || bResult || bPlay)
            {
                HpText.enabled = true;
                GemText.enabled = true;
                TimeText.enabled = true;
                GemImage.enabled = true;
                SetHPText(GameManager.instance.playerHp, Color.white);
                SetGemText(GameManager.instance.dungeonGem, Color.white);
            }
            else
            {
                HpText.enabled = false;
                GemText.enabled = false;
                TimeText.enabled = false;
                GemImage.enabled = false;
            }            
        }

        public void SetupListners()
        {            
            resultButton.onClick.AddListener(GameManager.instance.GotoDungeonMap);
            
            level1Btn.onClick.AddListener(() => { GameManager.instance.EnterLevel(1); });
            level2Btn.onClick.AddListener(() => { GameManager.instance.EnterLevel(2); });
            level3Btn.onClick.AddListener(() => { GameManager.instance.EnterLevel(3); });
            level4Btn.onClick.AddListener(() => { GameManager.instance.EnterLevel(4); });

            dungeonABtn.onClick.AddListener(GameManager.instance.EnterDungeonA);
            dungeonBBtn.onClick.AddListener(GameManager.instance.EnterDungeonB);
            dungeonCBtn.onClick.AddListener(GameManager.instance.EnterDungeonC);

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

        public void SetHPText(int playerHp, Color msgColor)
        {
            string content = "HP:" + playerHp;
            HpText.text = content;
            HpText.color = msgColor;
        }

        public void SetGemText(int playerGem, Color msgColor)
        {
            GemText.text = playerGem.ToString();
            GemText.color = msgColor;
        }

        public void SetTimeTextAndColor(float timeLimit)
        {
            TimeText.text = Mathf.Floor(timeLimit).ToString();
            if (timeLimit <= 10) TimeText.color = Color.red;
            else TimeText.color = Color.white;
        }
    }
}