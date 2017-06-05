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
    public enum GAME_STATE { START, LOBBY, SHOP, LEVEL, MAP, PLAY, RESULT, OVER }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;
        [HideInInspector]
        public bool playersTurn = true;

        private DungeonManager dungeonManager;
        private BoardManager boardScript;
        private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
        private bool enemiesMoving;                             //Boolean to check if enemies are moving.
                
        private GAME_STATE gameState;
                
        public List<GameObject> trapsOnStage = new List<GameObject>();
        public List<GameObject> objsOnStage = new List<GameObject>();
        public List<GameObject> tilesOnStage = new List<GameObject>();

        public List<Skill> inven = new List<Skill>();

        public int playerHp = 20;
        public int invenGem = 0;
        public int dungeonGem = 0;
        public float timeLimit;
       
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            enemies = new List<Enemy>();
            boardScript = GetComponent<BoardManager>();
            dungeonManager = GetComponent<DungeonManager>();            
            dungeonManager.InitDungeons();

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
                instance.setupLevel();
            }
        }

        private Dungeon curDungeon;

        public void ShowResult()
        {
            curDungeon.clearCurLevel();
            if(curDungeon.IsEnd())
            {                
                PageManager.instance.SetResultPageText(curDungeon.GetReward(), dungeonGem);
                invenGem += curDungeon.GetReward();
                invenGem += dungeonGem;
            }
            GameManager.instance.ChangeState(GAME_STATE.RESULT);
        }
        
        void setupLevel()
        {
            enemies.Clear();
            boardScript.SetupScene(curDungeon.GetCurLevel());
            ChangeState(GAME_STATE.PLAY);
            ShowMap(false);            
        }

        public void EnterDungeon(Dungeon dungeon)
        {
            curDungeon = dungeon;
            timeLimit = curDungeon.TimeLimit();
            dungeonGem = 0;
            playerHp = 20;
            ChangeState(GAME_STATE.MAP);
        }

        public void EnterDungeonA()
        {
            EnterDungeon(dungeonManager.DungeonA());
        }

        public void EnterDungeonB()
        {
            EnterDungeon(dungeonManager.DungeonB());
        }

        public void EnterDungeonC()
        {
            EnterDungeon(dungeonManager.DungeonC());
        }

        public void EnterShop()
        {
            ChangeState(GAME_STATE.SHOP);
        }

        public void GotoDungeonMap()
        {
            if (curDungeon.IsEnd()) ChangeState(GAME_STATE.LOBBY);
            else if (playerHp == 0) ChangeState(GAME_STATE.LOBBY);
            else ChangeState(GAME_STATE.MAP);
        }

        public void EnterLevel(int level)
        {
            curDungeon.SetLevel(level);
            GameManager.instance.ChangeState(GAME_STATE.LEVEL);
            PageManager.instance.SetLevelEnterPageText(curDungeon.ToString());
            Invoke("InitiateLevel", 2f);
        }

        void InitiateLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public void GoToLobby()
        {
            ChangeState(GAME_STATE.LOBBY);
        }

        public void ChangeState(GAME_STATE nextState)
        {
            gameState = nextState;
            PageManager.instance.Setup(gameState);
        }

        void Update()
        {
            if (gameState == GAME_STATE.PLAY)
            {
                timeLimit -= Time.deltaTime;
                PageManager.instance.SetTimeTextAndColor(timeLimit);                                

                if (!playersTurn && !enemiesMoving)
                    StartCoroutine(MoveEnemies());
            }
        }

        public void AddEnemyToList(Enemy script)
        {
            enemies.Add(script);
            ShowNear(GameObject.FindGameObjectWithTag("Player").transform.position);
        }

        public void GameOver()
        {
            ChangeState(GAME_STATE.OVER);            
        }

        bool bShowing = false;
        public bool IsShowing() { return bShowing; }

        public void ShowMap(bool bShow)
        {
            bShowing = bShow;
            ShowAllUnits(bShow);
            ShowObjects(bShow);
        }

        public void ShowNear(Vector3 targetPos)
        {
            Vector3[] range = GetShowRange(targetPos);
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "Thief" || enemies[i].tag == "Enemy")
                {
                    foreach (Vector3 pos in range)
                    {
                        if (pos == enemies[i].transform.position)
                            enemies[i].Show(true);
                    }
                }
            }


            ShowObjects(true, range);
        }
        

        public void ShowMap(Vector3 targetPos, int type)
        {
            bShowing = true;
                        
            switch (type)
            {
                default:
                case 0: ShowNear(targetPos);
                    break;

                case 1:
                    ShowAllUnits(true);
                    break;

                case 2:
                    ShowGems(true);
                    break;

                case 3:
                    ShowTraps(true);
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

        public void ShowAllUnits(bool bShow)
        {
            ShowEnemies(bShow);
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "Thief") enemies[i].Show(bShow);
            }
        }

        public void ShowEnemies(bool bShow)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].tag == "Enemy") enemies[i].Show(bShow);
            }
        }

        public List<Enemy> SearchEnemies(Vector3[] range)
        {
            List<Enemy> result = new List<Enemy>();

            foreach (Vector3 v in range)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].tag == "Thief") continue;
                    if (enemies[i].transform.position == v) result.Add(enemies[i]);
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
                        new Vector3(targetPos.x+1, targetPos.y+1, 0  ),
                        new Vector3(targetPos.x+1, targetPos.y-1, 0  ),
                        new Vector3(targetPos.x-1, targetPos.y+1, 0  ),
                        new Vector3(targetPos.x-1, targetPos.y-1, 0  )
                    };
                case 2:
                    return new Vector3[]
                    {
                        new Vector3(targetPos.x+1, targetPos.y, 0  ),
                        new Vector3(targetPos.x+2, targetPos.y, 0  ),
                        new Vector3(targetPos.x-1, targetPos.y, 0  ),
                        new Vector3(targetPos.x-2, targetPos.y, 0  )
                    };
                case 3:
                    return new Vector3[]
                    {
                        new Vector3(targetPos.x, targetPos.y-1, 0  ),
                        new Vector3(targetPos.x, targetPos.y-2, 0  ),
                        new Vector3(targetPos.x, targetPos.y+1, 0  ),
                        new Vector3(targetPos.x, targetPos.y+2, 0  )
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

        public void DestoryEnemies(Vector3 targetPos, int type)
        {
            Vector3[] range = GetDestroyRange(targetPos, type);

            List<GameObject> targetTiles = new List<GameObject>();

            foreach (GameObject obj in tilesOnStage)
            {
                foreach (Vector3 v in range)
                {
                    if (obj.tag == "Wall") continue;
                    if (obj.transform.position == v) targetTiles.Add(obj);
                }
            }

            foreach (GameObject obj in trapsOnStage)
            {
                foreach (Vector3 v in range)
                {
                    if (obj.transform.position == v) targetTiles.Add(obj);
                }
            }

            List<Enemy> targetEnemies = SearchEnemies(range);
            StartCoroutine(DestroyEffectFloor(targetTiles));
            StartCoroutine(DestroyEffect(targetEnemies));
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

        IEnumerator DestroyEffect(List<Enemy> targetEnemies)
        {
            foreach (Enemy en in targetEnemies) en.Show(true);
            yield return new WaitForSeconds(0.1f);
            foreach (Enemy en in targetEnemies) en.Show(false);
            yield return new WaitForSeconds(0.1f);
            foreach (Enemy en in targetEnemies) en.Show(true);
            yield return new WaitForSeconds(0.05f);
            foreach (Enemy en in targetEnemies) en.Show(false);
            yield return new WaitForSeconds(0.05f);
            foreach (Enemy en in targetEnemies) en.Show(true);
            yield return new WaitForSeconds(0.1f);
            foreach (Enemy en in targetEnemies) en.gameObject.SetActive(false);
        }

        public void SetSearchEnemies(bool value)
        {
            foreach (Enemy en in enemies)
            {
                en.SetSearch(value);
            }
        }

        IEnumerator MoveEnemies()
        {
            float totalTime = 0.24f;
            enemiesMoving = true;

            yield return new WaitForSeconds(0.08f);

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].gameObject.activeSelf) enemies[i].MoveEnemy();

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

            foreach (Enemy en in enemies)
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
            foreach (Enemy en in enemies)
            {
                Vector2 pos = en.transform.position;
                if (pos == dest) return false;
            }
            return true;
        }

        public void RemoveTrap(GameObject aTrap)
        {
            aTrap.SetActive(false);
            trapsOnStage.Remove(aTrap);

        }

        public GameObject IsTrap(float x, float y)
        {
            foreach (GameObject obj in trapsOnStage)
            {
                if (obj.transform.position.x == x && obj.transform.position.y == y) return obj;
            }

            return null;
        }

        public void ShowTraps(bool bShow, Vector3[] range = null)
        {
            foreach (GameObject obj in trapsOnStage)
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
            foreach (GameObject obj in objsOnStage)
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

            foreach (GameObject obj in tilesOnStage)
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

    }
}