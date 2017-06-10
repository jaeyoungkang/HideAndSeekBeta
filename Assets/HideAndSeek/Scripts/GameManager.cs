using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using UnityEngine.UI;                   //Allows us to use UI.

using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
using System.IO;
using System;

namespace HideAndSeek
{    
    public enum GAME_STATE { START, TUTORIAL, LOBBY, SHOP, INVENTORY, DUNGEON_INFO, LEVEL, MAP, PLAY, RESULT, OVER }

    public class GameManager : MonoBehaviour
    {
        public bool isClearTutorial = false;

        public static GameManager instance = null;
        [HideInInspector]
        public bool playersTurn = true;

        public Dungeon tutorial;
        public Dungeon[] dungeons;
        private BoardManager boardScript;
        
        private bool enemiesMoving;
                
        private GAME_STATE gameState;

        public List<GameObject> curTrapsOnStage = new List<GameObject>();
        public List<GameObject> curObjsOnStage = new List<GameObject>();
        public List<GameObject> curTilesOnStage = new List<GameObject>();
        public List<GameObject> curEnemiesOnStage = new List<GameObject>();
        
        public Dictionary<int, List<GameObject>> enemiesOnStages = new Dictionary<int, List<GameObject>>();

        public Dictionary<int, List<GameObject>> trapsOnStages = new Dictionary<int, List<GameObject>>();
        public Dictionary<int, List<GameObject>> objsOnStages = new Dictionary<int, List<GameObject>>();
        public Dictionary<int, List<GameObject>> tilesOnStages = new Dictionary<int, List<GameObject>>();

        public List<Skill> inven = new List<Skill>();
        public List<Skill> bag = new List<Skill>();
        public int bagSize;
        public int invenSize;

        public int playerHp = 20;
        public int invenGem = 0;
        public int dungeonGem = 0;
        public float timeLimit;

        public void RemoveObj(GameObject obj)
        {
            curObjsOnStage.Remove(obj);
        }

        public void RemoveEnemy(GameObject obj)
        {
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
            if (invenSize < limit)
            {
                invenSize++;
                return true;
            }

            return false;
        }
        public bool AddBag(Skill skill)
        {
            if (bag.Count == bagSize) return false;            
            bag.Add(skill);            
            return true;
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            boardScript = GetComponent<BoardManager>();

            PageManager.instance.InitUI();
            ChangeState(GAME_STATE.START);
            
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
            invenGem += curDungeon.GetReward();
            invenGem += dungeonGem;
        }

        public void ShowResult()
        {
            curDungeon.clearCurLevel();
            if(curDungeon.IsEnd())
            {
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
            int levelId =  curDungeon.GetCurLevel().id;
            
            if(!trapsOnStages.ContainsKey(levelId) || !objsOnStages.ContainsKey(levelId) || !tilesOnStages.ContainsKey(levelId) || !enemiesOnStages.ContainsKey(levelId))
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
            ShowMap(false);

            Player player = FindObjectOfType(typeof(Player)) as Player;
            player.Init();
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

        public void EnterDungeon()
        {
            if(curDungeon.cost > invenGem)
            {
                print("Popup: You need more gem to enter the Dungeon");
                return;
            }
            curDungeon.init();
            timeLimit = curDungeon.TimeLimit();
            dungeonGem = 0;
            playerHp = 20;

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

        public void EnterLevel(int level)
        {
            curDungeon.SetLevel(level);
            GameManager.instance.ChangeState(GAME_STATE.LEVEL);
            PageManager.instance.SetLevelEnterPageText(curDungeon.ToString());
            Invoke("setupLevel", 2f);
        }

        public void GoToLobby()
        {               
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

            if(isClearTutorial) ChangeState(GAME_STATE.LOBBY);
            else ChangeState(GAME_STATE.TUTORIAL);
        }
        
        public void ChangeState(GAME_STATE nextState)
        {
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

        public void ShowMap(Vector3 targetPos, int type)
        {
            switch (type)
            {
                default:
                case 0: ShowNear(targetPos);
                    break;

                case 1:
                    ShowAllUnits(true);
                    break;

                case 2:
                    ShowTraps(true);
                    break;

                case 3:
                    ShowGems(true);
                    break;

                case 4:
                    ShowMap(true);
                    break;

            }
            
        }


        public Vector3[] GetShowPositions()
        {
            return new Vector3[]
                    {
                        new Vector3(0, 0, 0 ),
                        new Vector3(2, 2, 0 ),
                        new Vector3(5, 2, 0 ),
                        new Vector3(2, 5, 0 ),
                        new Vector3(5, 5, 0 ),
//                        new Vector3(7, 7, 0 ),
                        new Vector3(0, 7, 0 ),
                        new Vector3(7, 0, 0 )
                    };
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
            foreach (GameObject en in targetEnemies) en.SetActive(false);
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
            aTrap.SetActive(false);
            curTrapsOnStage.Remove(aTrap);
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

        public void WriteFile(string fileName, string info)
        {
            if (File.Exists(fileName))
            {
                File.AppendAllText(fileName, info + Environment.NewLine);
                return;
            }

            var sr = File.CreateText(fileName);
            sr.WriteLine(info);
            sr.Close();
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