using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.UI;                   //Allows us to use UI.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
using System.IO;
using System;

namespace HideAndSeek
{    
    public enum GAME_STATE { START, TUTORIAL, LOBBY, SHOP, INVENTORY, DUNGEON_INFO, LEVEL, LEVEL_INFO, MAP, PLAY, RESULT, OVER }

    public class GameManager : MonoBehaviour
    {
        public string logfileName;
        public static GameManager instance = null;
        [HideInInspector]
        public bool playersTurn = true;

        public Dungeon tutorial;
        public Dungeon[] dungeons;
        private BoardManager boardScript;

        private bool enemiesMoving;

        private GAME_STATE gameState;
        private GAME_STATE preGameState;

        public List<GameObject> curTrapsOnStage = new List<GameObject>();
        public List<GameObject> curObjsOnStage = new List<GameObject>();
        public List<GameObject> curTilesOnStage = new List<GameObject>();
        public List<GameObject> curEnemiesOnStage = new List<GameObject>();

        public Dictionary<int, List<GameObject>> enemiesOnStages = new Dictionary<int, List<GameObject>>();

        public Dictionary<int, List<GameObject>> trapsOnStages = new Dictionary<int, List<GameObject>>();
        public Dictionary<int, List<GameObject>> objsOnStages = new Dictionary<int, List<GameObject>>();
        public Dictionary<int, List<GameObject>> tilesOnStages = new Dictionary<int, List<GameObject>>();

        public GameInfo info = new GameInfo();

        public int playerHp = 20;
        public int dungeonGem = 0;
        public float timeLimit;
        public LevelPlayData playData = new LevelPlayData();

        public float TIME_INTERVAL_GEN = 300f;

        public void UpdateCoin()
        {
            DateTime now = DateTime.Now.ToLocalTime();
            TimeSpan gen = now - info.preGenTime;

            if (gen.TotalSeconds > TIME_INTERVAL_GEN)
            {
                print(now + " " + info.preGenTime + " " + gen.TotalSeconds);
                int numOfCoin = (int)(gen.TotalSeconds / TIME_INTERVAL_GEN);
                AddCoinByTime(numOfCoin);
            }
        }

        public int MAX_COIN = 5;
        public void AddCoinByTime(int numOfCoin)
        {
            print("numOfCoin: " + numOfCoin + "my coin: " + info.coin);
            info.preGenTime = DateTime.Now.ToLocalTime();
            AddCoin(numOfCoin);
        }
        public void AddCoin(int numOfCoin)
        {
            info.coin += numOfCoin;
            if (info.coin > MAX_COIN) info.coin = MAX_COIN;
        }

        public void RemoveObj(GameObject obj)
        {
            curObjsOnStage.Remove(obj);
        }

        public void RemoveEnemy(GameObject obj)
        {
            int randomValue = Random.Range(0, 9);
            if (randomValue < 5) DropGem(obj.transform.position);
            else if (randomValue == 9) DropItem(obj.transform.position);

            obj.SetActive(false);
            curEnemiesOnStage.Remove(obj);
        }

        public void AddEnemy(GameObject enemy, int id)
        {
            enemiesOnStages[id].Add(enemy);
        }

        public void AddTrap(GameObject trap, int id)
        {
            trapsOnStages[id].Add(trap);
        }

        public void AddObj(GameObject obj, int id)
        {
            objsOnStages[id].Add(obj);
        }

        public void AddTile(GameObject tile, int id)
        {
            tilesOnStages[id].Add(tile);
        }

        public bool ExtendInvenSize(int limit)
        {
            if (info.invenSize < limit)
            {
                info.invenSize++;
                return true;
            }

            return false;
        }

        public int GetPriceExtendBag(int limit, int curSize)
        {
            int delta = limit - curSize;
            if (delta > 2) return 5;
            else if (delta > 1) return 15;
            else return 30;
        }

        public int GetPriceExtendInven(int limit, int curSize)
        {
            int delta = limit - curSize;
            if (delta > 4) return 10;
            else if (delta > 2) return 20;
            else return 30;
        }

        public bool ExtendBagSize(int limit)
        {
            if (info.bagSize < limit)
            {

                info.bagSize++;
                return true;
            }

            return false;
        }

        public bool AddBag(int itemId)
        {
            if (info.bag.Count == info.bagSize) return false;
            info.bag.Add(itemId);
            return true;
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;

                string culomnName = "";
                culomnName += "dungeonName \t levelName \t gemCount \t deathCount \t attackedCount \t damagedByTimeCount \t (getItem, useItem, hps)";

                DateTime dt = DateTime.Now;
                String strDate = dt.ToString("MMdd_HHmmss");
                logfileName = "log/" + strDate + "_playData.txt";
                SaveLoad.WriteFile(logfileName, culomnName);
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            boardScript = GetComponent<BoardManager>();

            SaveLoad.Load();

            PageManager.instance.InitUI();
            ChangeState(GAME_STATE.START);

            InvokeRepeating("UpdateCoin", 0, 1.0f);
        }

        //this is called only once, and the paramter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static public void CallbackInitialization()
        {
            //register the callback to be called everytime the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (instance != null)
            {
                PageManager.instance.InitUI();
                instance.ChangeState(instance.gameState);
            }
        }

        private Dungeon curDungeon;
        public Dungeon GetDungeonInfo() { return curDungeon; }

        public void GetResult()
        {
            info.invenGem += curDungeon.GetReward();
            info.invenGem += dungeonGem;
            AddCoin(1);
        }

        public void OpenNextDungeon()
        {
            int nextId = curDungeon.nextId;
            foreach (Dungeon dungeon in dungeons)
            {
                if (dungeon.id == nextId) dungeon.open = true;
            }

            info.dungeonIdsOpened.Clear();
            foreach (Dungeon dungeon in dungeons)
            {
                if (dungeon.open) info.dungeonIdsOpened.Add(dungeon.id);
            }
        }

        public void ShowResult()
        {
            curDungeon.clearCurLevel();
            if (curDungeon.IsEnd())
            {
                OpenNextDungeon();
                GetResult();
            }
            GameManager.instance.ChangeState(GAME_STATE.RESULT);
        }

        void SetActiveObjs(List<GameObject> objs, bool active)
        {
            foreach (GameObject obj in objs)
            {
                obj.SetActive(active);
            }
        }

        void SetActiveMap(Dictionary<int, List<GameObject>> map, bool active)
        {
            foreach (KeyValuePair<int, List<GameObject>> pair in map)
            {
                SetActiveObjs(pair.Value, active);
            }
        }

        void setupLevel()
        {
            int levelId = curDungeon.GetCurLevel().id;

            if (!trapsOnStages.ContainsKey(levelId) ||
                !objsOnStages.ContainsKey(levelId) ||
                !tilesOnStages.ContainsKey(levelId) ||
                !enemiesOnStages.ContainsKey(levelId))
            {
                print("Error: wrong level id " + levelId);
                return;
            }

            playData.levelName = curDungeon.GetCurLevel().name;

            SetActiveMap(trapsOnStages, false);
            SetActiveMap(objsOnStages, false);
            SetActiveMap(tilesOnStages, false);
            SetActiveMap(enemiesOnStages, false);

            curTrapsOnStage = trapsOnStages[levelId];
            curObjsOnStage = objsOnStages[levelId];
            curTilesOnStage = tilesOnStages[levelId];
            curEnemiesOnStage = enemiesOnStages[levelId];

            SetActiveObjs(curTrapsOnStage, true);
            SetActiveObjs(curObjsOnStage, true);
            SetActiveObjs(curTilesOnStage, true);
            SetActiveObjs(curEnemiesOnStage, true);

            ChangeState(GAME_STATE.PLAY);
            ShowMap(false);

            Player player = FindObjectOfType(typeof(Player)) as Player;
            player.Init();

            foreach(ShowTile st in curDungeon.GetCurLevel().showTiles)
            {
                foreach(GameObject obj in curTilesOnStage)
                {
                    if (st.pos.x == obj.transform.position.x && st.pos.y == obj.transform.position.y)
                    {        
                        StartCoroutine(BlinkEffect(obj));
                    }
                }                    
            }
        }
        
        IEnumerator BlinkEffect(GameObject obj)
        {
            Color backUpColor = obj.GetComponent<Renderer>().material.color;            
            obj.GetComponent<Renderer>().material.color = Color.Lerp(backUpColor, Color.white, 0.5f);
            yield return new WaitForSeconds(1.0f);
            obj.GetComponent<Renderer>().material.color = backUpColor;
            yield return new WaitForSeconds(1.0f);
            obj.GetComponent<Renderer>().material.color = Color.Lerp(backUpColor, Color.white, 0.5f);
            yield return new WaitForSeconds(1.0f);
            obj.GetComponent<Renderer>().material.color = backUpColor;
            yield return new WaitForSeconds(1.0f);
            obj.GetComponent<Renderer>().material.color = Color.Lerp(backUpColor, Color.white, 0.5f);
            yield return new WaitForSeconds(1.0f);
            obj.GetComponent<Renderer>().material.color = backUpColor;
            yield return new WaitForSeconds(1.0f);
            obj.GetComponent<Renderer>().material.color = Color.Lerp(backUpColor, Color.white, 0.5f);
            yield return new WaitForSeconds(1.0f);
            obj.GetComponent<Renderer>().material.color = backUpColor;
            yield return new WaitForSeconds(1.0f);
            obj.GetComponent<Renderer>().material.color = Color.Lerp(backUpColor, Color.white, 0.5f);
            yield return new WaitForSeconds(1.0f);
            obj.GetComponent<Renderer>().material.color = backUpColor;
        } 

        public void BacktoPreState()
        {
            ChangeState(preGameState);
        }

        public void ShowDungeonInfo(int index)
        {
            SelectDungeon(dungeons[index]);
            ChangeState(GAME_STATE.DUNGEON_INFO);
        }

        public void SelectDungeon(Dungeon dungeonSelected)
        {
            curDungeon = dungeonSelected;            
        }

        public void SetupPlayerData()
        {
            playData.Init();
            playData.dungeonName = curDungeon.name;
        }

        public void EnterDungeon()
        {
            if(curDungeon.cost > info.coin)
            {
                print("Popup: You need more gem to enter the Dungeon");
                return;
            }
            curDungeon.init();
            timeLimit = curDungeon.TimeLimit();
            dungeonGem = 0;
            playerHp = 20;
            info.coin -= curDungeon.cost;

            SetupPlayerData();

            GameManager.instance.tilesOnStages.Clear();
            GameManager.instance.objsOnStages.Clear();
            GameManager.instance.trapsOnStages.Clear();
            GameManager.instance.enemiesOnStages.Clear();

            foreach(Level lv in curDungeon.levels)
            {
                GameManager.instance.tilesOnStages[lv.id] = new List<GameObject>();
                GameManager.instance.objsOnStages[lv.id] = new List<GameObject>();
                GameManager.instance.trapsOnStages[lv.id] = new List<GameObject>();
                GameManager.instance.enemiesOnStages[lv.id] = new List<GameObject>();
            }
            
            boardScript.SetupScene(curDungeon.levels);

            ChangeState(GAME_STATE.MAP);
        }

        public void EnterInven()
        {
            ChangeState(GAME_STATE.INVENTORY);
        }

        public void EnterShop()
        {
            ChangeState(GAME_STATE.SHOP);
        }

        public void GotoDungeonMap()
        {
            ChangeState(GAME_STATE.MAP);
        }

        public void SelectLevel(int level)
        {
            curDungeon.SetLevel(level);
            GameManager.instance.ChangeState(GAME_STATE.LEVEL_INFO);
        }

        public void EnterLevel()
        {            
            GameManager.instance.ChangeState(GAME_STATE.LEVEL);
            PageManager.instance.SetLevelEnterPageText(curDungeon.name, curDungeon.GetCurLevel().name);
            Invoke("setupLevel", 2f);            
        }

        public void GoToLobby()
        {
            foreach (int openedId in GameManager.instance.info.dungeonIdsOpened)
            {
                foreach (Dungeon dungeon in dungeons)
                {
                    if (dungeon.id == openedId) dungeon.open = true;
                }
            }

            SaveLoad.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

            if (info.isClearTutorial) ChangeState(GAME_STATE.LOBBY);
            else
            {
                SelectDungeon(tutorial);
                ChangeState(GAME_STATE.TUTORIAL);
            }
        }
        
        public bool CheckState(GAME_STATE state)
        {
            return state == gameState;
        }

        public void ChangeState(GAME_STATE nextState)
        {
            preGameState = gameState;
            gameState = nextState;
            PageManager.instance.Setup(gameState);
        }

        public void StopTime(bool active)
        {
            timeStop = active;
        }

        public void AddTime(float sec)
        {
            timeLimit += sec;
        }

        public bool timeStop = false;
        void UpdateTimeLeft()
        {
            if (timeStop) return;
            timeLimit -= Time.deltaTime;
        }

        public bool IsPlay() { return gameState == GAME_STATE.PLAY; }

        void Update()
        {
            if (gameState == GAME_STATE.PLAY)
            {
                UpdateTimeLeft();

                if (!playersTurn && !enemiesMoving)
                    StartCoroutine(MoveEnemies());
            }
        }

        public bool IsGameOver()
        {
            return gameState == GAME_STATE.OVER;
        }

        public void GameOver()
        {
            playData.deathCount++;
            info.bag.Clear();
            ChangeState(GAME_STATE.OVER);            
        }

        public void ShowMap(bool bShow)
        {
            ShowAllUnits(bShow);
            ShowObjects(bShow);
        }

        public void ShowNear(Vector3 targetPos)
        {
            Vector3[] range = GetShowRange(targetPos);
            for (int i = 0; i < curEnemiesOnStage.Count; i++)
            {
                if (curEnemiesOnStage[i].tag == "Thief" || curEnemiesOnStage[i].tag == "Enemy")
                {
                    foreach (Vector3 pos in range)
                    {
                        if (pos == curEnemiesOnStage[i].transform.position)
                        {
                            ShowObject(curEnemiesOnStage[i], true);
                        }
                    }
                }
            }


            ShowObjects(true, range);
        }        

        public void ShowMap(Vector3 targetPos, SHOW_TYPE type)
        {
            if (type == SHOW_TYPE.NONE) return;
            switch (type)
            {
                default:
                case SHOW_TYPE.NEAR: ShowNear(targetPos);
                    break;

                case SHOW_TYPE.MONSTER:
                    ShowAllUnits(true);
                    break;

                case SHOW_TYPE.TRAP:
                    ShowTraps(true);
                    break;

                case SHOW_TYPE.GEM_ITEM:
                    ShowGems(true);
                    break;

                case SHOW_TYPE.ALL:
                    ShowMap(true);
                    break;
            }            
        }


        public SHOW_TYPE CheckShowTile(Vector2 pos)
        {
            Level curLv = curDungeon.GetCurLevel();

            foreach(ShowTile sTile in curLv.showTiles)
            {
                if (sTile.pos == pos) return sTile.type;

            }
            
            return SHOW_TYPE.NONE;
        }

        public Vector3[] GetShowRange(Vector3 targetPos)
        {
            return new Vector3[]
            {
                new Vector3(targetPos.x+1, targetPos.y, 0  ),
                new Vector3(targetPos.x+2, targetPos.y, 0  ),
                new Vector3(targetPos.x+3, targetPos.y, 0  ),
                        
                new Vector3(targetPos.x-1, targetPos.y, 0  ),
                new Vector3(targetPos.x-2, targetPos.y, 0  ),
                new Vector3(targetPos.x-3, targetPos.y, 0  ),

                new Vector3(targetPos.x, targetPos.y+1, 0  ),
                new Vector3(targetPos.x, targetPos.y+2, 0  ),
                new Vector3(targetPos.x, targetPos.y+3, 0  ),

                new Vector3(targetPos.x, targetPos.y-1, 0  ),
                new Vector3(targetPos.x, targetPos.y-2, 0  ),
                new Vector3(targetPos.x, targetPos.y-3, 0  ),

                new Vector3(targetPos.x+2, targetPos.y+1, 0  ),
                new Vector3(targetPos.x+1, targetPos.y+1, 0  ),
                new Vector3(targetPos.x+1, targetPos.y+2, 0  ),

                new Vector3(targetPos.x+2, targetPos.y-1, 0  ),
                new Vector3(targetPos.x+1, targetPos.y-1, 0  ),
                new Vector3(targetPos.x+1, targetPos.y-2, 0  ),

                new Vector3(targetPos.x-2, targetPos.y+1, 0  ),
                new Vector3(targetPos.x-1, targetPos.y+1, 0  ),
                new Vector3(targetPos.x-1, targetPos.y+2, 0  ),

                new Vector3(targetPos.x-2, targetPos.y-1, 0  ),
                new Vector3(targetPos.x-1, targetPos.y-1, 0  ),
                new Vector3(targetPos.x-1, targetPos.y-2, 0  ),
            };
        }

        public void ShowObject(GameObject obj, bool bShow)
        {
            SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();

            if (sprite)
            {
                Color color = sprite.material.color;
                if (bShow) color.a = 1.0f;
                else color.a = 0f;

                sprite.material.color = color;
            }
        }

        public void ShowAllUnits(bool bShow)
        {
            ShowEnemies(bShow);
            for (int i = 0; i < curEnemiesOnStage.Count; i++)
            {
                if (curEnemiesOnStage[i].tag == "Thief") ShowObject(curEnemiesOnStage[i], bShow);
            }
        }

        public void ShowEnemies(bool bShow)
        {
            for (int i = 0; i < curEnemiesOnStage.Count; i++)
            {
                if (curEnemiesOnStage[i].tag == "Enemy") ShowObject(curEnemiesOnStage[i], bShow);
            }
        }

        public List<GameObject> SearchEnemies(Vector3[] range)
        {
            List<GameObject> result = new List<GameObject>();

            foreach (Vector3 v in range)
            {
                for (int i = 0; i < curEnemiesOnStage.Count; i++)
                {
                    if (curEnemiesOnStage[i].tag == "Thief") continue;
                    if (curEnemiesOnStage[i].transform.position == v) result.Add(curEnemiesOnStage[i]);
                }
            }

            return result;
        }


        public Vector3[] GetDestroyRange(Vector3 targetPos, int type)
        {            
            switch (type)
            {
                case 0:
                    return new Vector3[]
                    {
                        new Vector3(targetPos.x+1, targetPos.y, 0  ),
                        new Vector3(targetPos.x-1, targetPos.y, 0  ),
                        new Vector3(targetPos.x, targetPos.y+1, 0  ),
                        new Vector3(targetPos.x, targetPos.y-1, 0  )
                    };

                case 1:
                    return new Vector3[]
                    {
                        new Vector3(targetPos.x+1, targetPos.y, 0  ),
                        new Vector3(targetPos.x+2, targetPos.y, 0  ),
                        new Vector3(targetPos.x-1, targetPos.y, 0  ),
                        new Vector3(targetPos.x-2, targetPos.y, 0  )
                    };
                case 2:
                    return new Vector3[]
                    {
                        new Vector3(targetPos.x, targetPos.y-1, 0  ),
                        new Vector3(targetPos.x, targetPos.y-2, 0  ),
                        new Vector3(targetPos.x, targetPos.y+1, 0  ),
                        new Vector3(targetPos.x, targetPos.y+2, 0  )
                    };

                case 3:
                    return new Vector3[]
                    {
                        new Vector3(targetPos.x+1, targetPos.y+1, 0  ),
                        new Vector3(targetPos.x+1, targetPos.y-1, 0  ),
                        new Vector3(targetPos.x-1, targetPos.y+1, 0  ),
                        new Vector3(targetPos.x-1, targetPos.y-1, 0  )
                    };

                default:
                    return new Vector3[]
                    {
                        new Vector3(targetPos.x, targetPos.y, 0  ),
                        new Vector3(targetPos.x, targetPos.y, 0  ),
                        new Vector3(targetPos.x, targetPos.y, 0  ),
                        new Vector3(targetPos.x, targetPos.y, 0  )
                    };
            }

        }

        IEnumerator DestroyEffectFloor(List<GameObject> targetTiles)
        {
            foreach (GameObject obj in targetTiles)
            {
                SpriteRenderer spRenderer = obj.GetComponent<SpriteRenderer>();
                if (spRenderer)
                {
                    Color color = spRenderer.color;
                    color = new Vector4(1, 0.5f, 0.5F, 1);
                    spRenderer.color = color;
                }
                if (obj.tag == "Trap")
                {
                    spRenderer.enabled = true;
                }
            }
            yield return new WaitForSeconds(0.5f);

            foreach (GameObject obj in targetTiles)
            {
                SpriteRenderer spRenderer = obj.GetComponent<SpriteRenderer>();
                if (spRenderer)
                {
                    Color color = spRenderer.color;
                    color = new Vector4(1, 1, 1, 1);
                    spRenderer.color = color;
                }                
            }

            foreach (GameObject obj in targetTiles)
            {
               if(obj.tag == "Trap")
                {
                    RemoveTrap(obj);                    
                }
            }
        }

        IEnumerator DestroyEffect(List<GameObject> targetEnemies)
        {
            foreach (GameObject en in targetEnemies) ShowObject(en, true);
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject en in targetEnemies) ShowObject(en, false);
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject en in targetEnemies) ShowObject(en, true);
            yield return new WaitForSeconds(0.05f);
            foreach (GameObject en in targetEnemies) ShowObject(en, false);
            yield return new WaitForSeconds(0.05f);
            foreach (GameObject en in targetEnemies) ShowObject(en, true);
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject en in targetEnemies)
            {
                RemoveEnemy(en);
            }
        }

        public void SetSearchEnemies(bool value)
        {
            foreach (GameObject en in curEnemiesOnStage)
            {
                Enemy script = en.GetComponent<Enemy>();
            }
        }

        IEnumerator MoveEnemies()
        {
            float totalTime = 0.24f;
            enemiesMoving = true;

            yield return new WaitForSeconds(0.08f);

            for (int i = 0; i < curEnemiesOnStage.Count; i++)
            {
                if (curEnemiesOnStage[i].gameObject.activeSelf)
                {
                    Enemy anEnemy = curEnemiesOnStage[i].GetComponent<Enemy>();
                    anEnemy.MoveEnemy();
                }


                yield return new WaitForSeconds(0.03f);
                totalTime -= 0.03f;
            }

            if (totalTime > 0) yield return new WaitForSeconds(totalTime);
            playersTurn = true;

            enemiesMoving = false;
        }

        public bool[,] UpdateMap(Vector3 playerPos)
        {
            bool[,] map = new bool[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    map[i, j] = true;
                }
            }

            foreach (GameObject en in curEnemiesOnStage)
            {
                Vector2 pos = en.transform.position;
                int x = (int)pos.x;
                int y = (int)pos.y;
                map[x, y] = false;
            }

            map[(int)playerPos.x, (int)playerPos.y] = false;

            return map;
        }

        public bool IsAvailablePos(Vector2 dest)
        {
            foreach (GameObject en in curEnemiesOnStage)
            {
                Vector2 pos = en.transform.position;
                if (pos == dest) return false;
            }
            return true;
        }

        public void RemoveTrap(GameObject aTrap)
        {
            if(Random.Range(0,4) == 1) DropGem(aTrap.transform.position);

            aTrap.SetActive(false);
            curTrapsOnStage.Remove(aTrap);
        }

        public void DropItem(Vector3 dropPos)
        {
            int randomIndex = Random.Range(0, 2);
            GameObject instance = Instantiate(boardScript.itemTiles[randomIndex], dropPos, Quaternion.identity);
            GameManager.instance.AddObj(instance, curDungeon.GetCurLevel().id);

            StartCoroutine(DropObjectEffect(instance));
        }

        public void DropGem(Vector3 dropPos)
        {
            GameObject instance = Instantiate(boardScript.gemTiles[0], dropPos, Quaternion.identity);
            GameManager.instance.AddObj(instance, curDungeon.GetCurLevel().id);

            StartCoroutine(DropObjectEffect(instance));
        }

        IEnumerator DropObjectEffect(GameObject dropOjb)
        {
            ShowObject(dropOjb, true);
            yield return new WaitForSeconds(0.1f);
            ShowObject(dropOjb, false);
            yield return new WaitForSeconds(0.1f);
            ShowObject(dropOjb, true);
            yield return new WaitForSeconds(0.1f);
            ShowObject(dropOjb, false);
            yield return new WaitForSeconds(0.1f);
            ShowObject(dropOjb, true);
        }

        public GameObject IsTrap(float x, float y)
        {
            foreach (GameObject obj in curTrapsOnStage)
            {
                if (obj.transform.position.x == x && obj.transform.position.y == y) return obj;
            }

            return null;
        }

        public void ShowTraps(bool bShow, Vector3[] range = null)
        {
            foreach (GameObject obj in curTrapsOnStage)
            {
                if (obj == null) continue;
                Renderer renderer = obj.GetComponent<SpriteRenderer>();

                if (range != null)
                {
                    foreach (Vector3 pos in range)
                    {
                        if (obj.transform.position == pos)
                        {
                            if (renderer) renderer.enabled = bShow;
                        }
                    }
                }
                else
                {
                    if (renderer) renderer.enabled = bShow;
                }
            }
        }

        public void ShowGems(bool bShow, Vector3[] range = null)
        {
            foreach (GameObject obj in curObjsOnStage)
            {
                if (obj == null) continue;
                Renderer renderer = obj.GetComponent<SpriteRenderer>();
                if (range != null)
                {
                    foreach (Vector3 pos in range)
                    {
                        if (obj.transform.position == pos)
                        {
                            if (renderer) renderer.enabled = bShow;
                        }
                    }
                }
                else
                {
                    if (renderer) renderer.enabled = bShow;
                }
            }
        }

        public void ShowObjects(bool bShow, Vector3[] range = null)
        {
            ShowGems(bShow, range);
            ShowTraps(bShow, range);

            foreach (GameObject obj in curTilesOnStage)
            {
                if (obj == null) continue;
                Renderer renderer = obj.GetComponent<SpriteRenderer>();
                if (range != null)
                {
                    foreach (Vector3 pos in range)
                    {
                        if (obj.transform.position == pos)
                        {
                            if (renderer)
                            {
                                if (bShow)
                                {
                                    Color color = renderer.material.color;
                                    color.a = 1f;
                                    renderer.material.color = color;
                                }
                                else
                                {
                                    Color color = renderer.material.color;
                                    color.a = 0.6f;
                                    renderer.material.color = color;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (renderer)
                    {
                        if (bShow)
                        {
                            Color color = renderer.material.color;
                            color.a = 1f;
                            renderer.material.color = color;
                        }
                        else
                        {
                            Color color = renderer.material.color;
                            color.a = 0.6f;
                            renderer.material.color = color;
                        }
                    }
                }                
            }
        }

        public void DestoryEnemies(Vector3 targetPos, int type)
        {
            Vector3[] range = GetDestroyRange(targetPos, type);

            List<GameObject> targetTiles = new List<GameObject>();

            foreach (GameObject obj in curTilesOnStage)
            {
                foreach (Vector3 v in range)
                {
                    if (obj.tag == "Wall") continue;
                    if (obj.transform.position == v) targetTiles.Add(obj);
                }
            }

            foreach (GameObject obj in curTrapsOnStage)
            {
                foreach (Vector3 v in range)
                {
                    if (obj.transform.position == v) targetTiles.Add(obj);
                }
            }

            List<GameObject> targetEnemies = SearchEnemies(range);
            StartCoroutine(DestroyEffectFloor(targetTiles));
            StartCoroutine(DestroyEffect(targetEnemies));
        }
    }
}