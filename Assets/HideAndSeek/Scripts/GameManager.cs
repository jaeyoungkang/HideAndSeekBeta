using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 

namespace HideAndSeek
{    
    public enum GAME_STATE { START, LOBBY, SHOP, DUNGEON_INFO, LEVEL, LEVEL_INFO, MAP, PLAY, RESULT, OVER }

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

        public List<int> bag;
        public int maxBagSize;
        public int bagSize;
        public int maxHp;
        public int playerHp;
        public int dungeonGem = 0;
        public float timeLimit;
        public DungeonPlayData dungeonPlayData = new DungeonPlayData();
        
        public void RemoveObj(GameObject obj)
        {
            curObjsOnStage.Remove(obj);
        }

        public void RemoveEnemy(GameObject obj)
        {
            int randomValue = Random.Range(0, 9);
            if (randomValue < 4) DropGem(obj.transform.position);
            else if (randomValue > 7)
            {
                Enemy en = obj.GetComponent<Enemy>();
                if (en.playerDamage == 1)
                {
                    int[] dropItemList = { 101, 104, 114 };
                    DropItem(obj.transform.position, dropItemList);
                }
                else if (en.playerDamage == 2)
                {
                    int[] dropItemList = { 109, 110, 111};
                    DropItem(obj.transform.position, dropItemList);
                }
            }

            obj.SetActive(false);
            curEnemiesOnStage.Remove(obj);

            dungeonPlayData.destroyEnemy++;
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

        public void ExtendBagSize()
        {
            if (bagSize < 6)
            {
                bagSize++;
            }
            else
            {
                Notice.instance.Show("이미 최대치이다...", 1F, Color.white);
            }            
        }

        public bool AddItemInBag(int itemId)
        {
            if (bag.Count == bagSize) return false;
            bag.Add(itemId);
            return true;
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
#if UNITY_EDITOR
                //string culomnName = "";
                //culomnName += "dungeonName \t levelName \t gemCount \t deathCount \t attackedCount \t damagedByTimeCount \t (getItem, useItem, hps)";

                //DateTime dt = DateTime.Now;
                //String strDate = dt.ToString("MMdd_HHmmss");
                //logfileName = "log/" + strDate + "_playData.txt";
                //SaveLoad.WriteFile(logfileName, culomnName);
#endif
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            boardScript = GetComponent<BoardManager>();

            SaveLoad.Load();

            Notice.instance.InitUI();
            PageManager.instance.InitUI();

            ChangeState(GAME_STATE.START);

            DungeonData dungeonData = new DungeonData();
            dungeonData.GenerateShowTileSet();
            dungeons[0] = dungeonData.SetupDungeon1Data();
            dungeons[1] = dungeonData.SetupDungeon2Data();
            dungeons[2] = dungeonData.SetupDungeon3Data();
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
                Notice.instance.InitUI();
                PageManager.instance.InitUI();
                instance.ChangeState(instance.gameState);
            }
        }

        private Dungeon curDungeon;
        public Dungeon GetDungeonInfo() { return curDungeon; }

        public void GetResult()
        {

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
            if(playerHp <= 0)
            {
                GameOver();
                return;
            }

            curDungeon.clearCurLevel();
            Analytics.CustomEvent("Level Clear", new Dictionary<string, object>
            {
                { "Dungeon id", curDungeon.id},
                { "Level id", curDungeon.GetCurLevel().id},
            });

            if (curDungeon.IsEnd())
            {
                OpenNextDungeon();
                GetResult();

                if (info.dungeonClearCount.ContainsKey(curDungeon.id))
                {
                    info.dungeonClearCount[curDungeon.id]++;
                }
                else
                {
                    info.dungeonClearCount.Add(curDungeon.id, 1);
                }

                Analytics.CustomEvent("Dungeon Clear", new Dictionary<string, object>
                {
                    { "id", curDungeon.id},
                });
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
            ShowAllMap(false);

            Player player = FindObjectOfType(typeof(Player)) as Player;
            player.Init();            
        }

        GameObject highlightedTileobj = null;
        Color backUpColor = Color.white;
        public void HighlightTile(Vector2 pos)
        {
            if(highlightedTileobj)
            {
                highlightedTileobj.GetComponent<Renderer>().material.color = backUpColor;
                highlightedTileobj = null;
            }

            foreach (GameObject tile in curTilesOnStage)
            {
                if (tile.transform.position.x == pos.x && tile.transform.position.y == pos.y)
                {
                    highlightedTileobj = tile;
                    break;
                }
            }
            if (highlightedTileobj)
            {
                backUpColor = highlightedTileobj.GetComponent<Renderer>().material.color;
                highlightedTileobj.GetComponent<Renderer>().material.color = Color.yellow;
            }

        }
        
        public void BacktoPreState()
        {
            SoundManager.instance.PlaySingle(btnClick);
            ChangeState(preGameState);
        }

        public AudioClip btnClick;
        public void ShowDungeonInfo(int index)
        {            
            SoundManager.instance.PlaySingle(btnClick);

            if (index == 100) SelectDungeon(tutorial);
            else SelectDungeon(dungeons[index]);
            
            ChangeState(GAME_STATE.DUNGEON_INFO);
        }

        public void SelectDungeon(Dungeon dungeonSelected)
        {
            SoundManager.instance.PlaySingle(btnClick);
            curDungeon = dungeonSelected;            
        }

        public void EnterDungeon()
        {
            if (curDungeon.locked)
            {
                Notice.instance.Show("유료 던전 이다... 유료 결재를 해야한다!", 2f, Color.yellow);
                return;
            }

            if(curDungeon.id != 0)
            {
                if (info.enableCount <= 0)
                {
                    Notice.instance.Show("고대주화가 없어서 입장 할 수 없다...", 2f, Color.yellow);
                    return;
                }
                info.enableCount--;
            }
            SoundManager.instance.PlaySingle(btnClick);

            curDungeon.init();
            timeLimit = curDungeon.TimeLimit();
            dungeonGem = curDungeon.gem;
            playerHp = maxHp;
            bagSize = maxBagSize;
            bag.Clear();
            dungeonPlayData.Init();

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

            bool bTutorial = curDungeon.id == 0;
            boardScript.SetupScene(curDungeon.levels, bTutorial);

            ChangeState(GAME_STATE.MAP);

            if(info.dungeonTryCount.ContainsKey(curDungeon.id))
            {
                info.dungeonTryCount[curDungeon.id]++;
            }
            else
            {
                info.dungeonTryCount.Add(curDungeon.id, 1);
            }

            Analytics.CustomEvent("Dungeon Try", new Dictionary<string, object>
            {
                { "id", curDungeon.id},
            });

        }

        public void EnterShop()
        {
            SoundManager.instance.PlaySingle(btnClick);
            ChangeState(GAME_STATE.SHOP);

            Analytics.CustomEvent("Enter Shop", new Dictionary<string, object>{});
        }

        public void GotoDungeonMap()
        {
            ChangeState(GAME_STATE.MAP);
        }

        public void SelectLevel(int level)
        {
            SoundManager.instance.PlaySingle(btnClick);
            curDungeon.SetLevel(level);
            GameManager.instance.ChangeState(GAME_STATE.LEVEL_INFO);
        }

        public void EnterLevel()
        {
            SoundManager.instance.PlaySingle(btnClick);
            GameManager.instance.ChangeState(GAME_STATE.LEVEL);
            PageManager.instance.SetLevelEnterPageText(curDungeon.name, curDungeon.GetCurLevel().name);
            Invoke("setupLevel", 2f);

            Analytics.CustomEvent("Level Enter", new Dictionary<string, object>
            {
                { "Dungeon id", curDungeon.id},
                { "Level id", curDungeon.GetCurLevel().id},
            });
        }

        
        public void GoToLobby()
        {
            SoundManager.instance.PlaySingle(btnClick);
            foreach (int openedId in info.dungeonIdsOpened)
            {
                foreach (Dungeon dungeon in dungeons)
                {
                    if (dungeon.id == openedId) dungeon.open = true;
                }
            }

            foreach (Dungeon dungeon in dungeons)
            {
                if (dungeon.locked && info.purchaseUser)
                    dungeon.locked = false;
            }

            SaveLoad.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

            ChangeState(GAME_STATE.LOBBY);            
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
            bag.Clear();
            ChangeState(GAME_STATE.OVER);

            Analytics.CustomEvent("Dead", new Dictionary<string, object>
            {
                { "Dungeon id", curDungeon.id},
                { "Level id", curDungeon.GetCurLevel().id},
            });
        }

        public void ShowAllMap(bool bShow)
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

        public void ExtendHp(int delta)
        {
            playerHp += delta;
        }

        public void RecoverHP(int delta)
        {
            playerHp += delta;
            if (playerHp > maxHp)
            {
                playerHp = maxHp;
            }
        }

        public void LoseHp(int delta)
        {
            playerHp -= delta;
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
                    ShowAllMap(true);
                    break;
            }            
        }


        public SHOW_TYPE CheckShowTile(Vector2 pos)
        {
            Level curLv = curDungeon.GetCurLevel();

            foreach(ShowTile sTile in curLv.showTiles)
            {
                if (sTile.x == pos.x && sTile.y == pos.y) return sTile.type;

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
                script.SetSearch(value);
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
            dungeonPlayData.destroyTrap++;
        }

        public void DropItem(Vector3 dropPos, int[] dropItemList)
        {            
            int randomIndex = Random.Range(0, dropItemList.Length);

            GameObject instance = null;
            foreach (GameObject obj in boardScript.itemTiles)
            {
                ItemObject itemObj = obj.GetComponent<ItemObject>();
                if(itemObj.itemId == dropItemList[randomIndex])
                {
                    instance = Instantiate(obj, dropPos, Quaternion.identity);
                    break;
                }
            }

            if(instance)
            {
                GameManager.instance.AddObj(instance, curDungeon.GetCurLevel().id);
                StartCoroutine(DropObjectEffect(instance));
            }            
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