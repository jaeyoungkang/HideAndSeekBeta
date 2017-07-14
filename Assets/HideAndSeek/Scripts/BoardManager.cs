﻿using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace HideAndSeek	
{
    public class BoardManager : MonoBehaviour
    {
        // Using Serializable allows us to embed a class with sub properties in the inspector.
        [Serializable]
        public class Count
        {
            public int minimum;
            public int maximum;

            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }
        }

        public int columns = 8;                                         //Number of columns in our game board.
        public int rows = 8;											//Number of rows in our game board.
        public GameObject exit;
        public GameObject[] floorTiles;
    	public GameObject[] gemTiles;
        public GameObject[] thiefTiles;
        public GameObject[] enemyTiles;
        public GameObject[] strongEnemyTiles;
        public GameObject[] outerWallTiles;

        public GameObject[] itemTiles;
        public GameObject[] trapTiles;

        private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
        private List<Vector3> gridPositions = new List<Vector3>();   //A list of possible locations to place tiles.

        private List<List<Vector3>> grids = new List<List<Vector3>>();
        private List<Vector3> gridPositions1 = new List<Vector3>();
        private List<Vector3> gridPositions2 = new List<Vector3>();
        private List<Vector3> gridPositions3 = new List<Vector3>();
        private List<Vector3> gridPositions4 = new List<Vector3>();

        private List<Vector3> gridPositionsExcept = new List<Vector3>();

        void InitialiseList(Level lv)
        {
            gridPositions.Clear();

            gridPositions1.Clear();
            gridPositions2.Clear();
            gridPositions3.Clear();
            gridPositions4.Clear();

            grids.Clear();

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (x == columns - 1 && y == rows - 1) continue;

                    gridPositions.Add(new Vector3(x, y, 0f));
                }
            }

            int halfOfCol = columns / 2;
            int halfOfRow = rows / 2;

            for (int x = 0; x < halfOfCol; x++)
            {
                for (int y = 0; y < halfOfRow; y++)
                {
                    gridPositions1.Add(new Vector3(x, y, 0f));
                }
            }

            for (int x = 0; x < halfOfCol; x++)
            {
                for (int y = halfOfRow; y < rows; y++)
                {
                    gridPositions2.Add(new Vector3(x, y, 0f));
                }
            }

            for (int x = halfOfCol; x < columns; x++)
            {
                for (int y = 0; y < halfOfRow; y++)
                {
                    gridPositions3.Add(new Vector3(x, y, 0f));
                }
            }

            for (int x = halfOfCol; x < columns; x++)
            {
                for (int y = halfOfRow; y < rows; y++)
                {
                    gridPositions4.Add(new Vector3(x, y, 0f));
                }
            }

            foreach(ShowTile sTile in GameManager.instance.showTilesOnStage[lv.id])
            {
                Vector3 pos = new Vector3(sTile.x, sTile.y, 0f);
                gridPositionsExcept.Add(pos);
            }
            //gridPositionsExcept.Add(new Vector3(0f, 0f, 0f));
            gridPositionsExcept.Add(new Vector3(1f, 0f, 0f));
            gridPositionsExcept.Add(new Vector3(0f, 1f, 0f));

            //gridPositionsExcept.Add(new Vector3(columns - 1, 0f, 0f));
            gridPositionsExcept.Add(new Vector3(columns - 2, 0f, 0f));
            gridPositionsExcept.Add(new Vector3(columns - 1, 1f, 0f));

            //gridPositionsExcept.Add(new Vector3(0f, rows - 1, 0f));
            gridPositionsExcept.Add(new Vector3(1f, rows - 1, 0f));
            gridPositionsExcept.Add(new Vector3(0f, rows - 2, 0f));

            gridPositionsExcept.Add(new Vector3(columns-1, rows-1, 0f));
            gridPositionsExcept.Add(new Vector3(columns-2, rows-1, 0f));
            gridPositionsExcept.Add(new Vector3(columns-1, rows-2, 0f));

            grids.Add(gridPositions1);
            grids.Add(gridPositions2);
            grids.Add(gridPositions3);
            grids.Add(gridPositions4);
        }

        void BoardSetup(Level lv)
        {
            boardHolder = new GameObject("Board").transform;            

            for (int x = -1; x < columns + 1; x++)
            {
                for (int y = -1; y < rows + 1; y++)
                {
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                    if (x == -1 || x == columns || y == -1 || y == rows)
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;                    
                    instance.transform.SetParent(boardHolder);

                    GameManager.instance.AddTile(instance, lv.id);
                    SetShowTile(instance, GameManager.instance.showTilesOnStage[lv.id].ToArray(), x, y);
                }                
            }
        }

        Vector3[] postionA = {
                                    new Vector3(0, 0, 0),
                                    new Vector3(2, 2, 0),
                                    new Vector3(5, 2, 0),
                                    new Vector3(2, 5, 0),
                                    new Vector3(5, 5, 0),
                                    new Vector3(7, 0, 0),
                                    new Vector3(0, 7, 0),
                                };

        SHOW_TYPE[] sTypeA = {
                                    SHOW_TYPE.NEAR,
                                    SHOW_TYPE.NEAR,
                                    SHOW_TYPE.NEAR,
                                    SHOW_TYPE.NEAR,
                                    SHOW_TYPE.NEAR,
                                    SHOW_TYPE.MONSTER,
                                    SHOW_TYPE.TRAP
                                };

        public Vector3 GetRandomPos(List<Vector3> pos)
        {
            int randomIndex = UnityEngine.Random.Range(0, pos.Count);
            Vector3 randomPosition = pos[randomIndex];
            pos.RemoveAt(randomIndex);
            return randomPosition;
        }

        void SetupTutorialShowTile()
        {
            GameManager.instance.AddShowTile(new ShowTile(new Vector3(0, 0, 0), SHOW_TYPE.NEAR), 1);
            GameManager.instance.AddShowTile(new ShowTile(new Vector3(0, 7, 0), SHOW_TYPE.GEM_ITEM), 1);
            GameManager.instance.AddShowTile(new ShowTile(new Vector3(7, 0, 0), SHOW_TYPE.TRAP), 1);            
            GameManager.instance.AddShowTile(new ShowTile(new Vector3(2, 2, 0), SHOW_TYPE.NEAR), 1);
            GameManager.instance.AddShowTile(new ShowTile(new Vector3(5, 5, 0), SHOW_TYPE.NEAR), 1);
            GameManager.instance.AddShowTile(new ShowTile(new Vector3(5, 2, 0), SHOW_TYPE.NEAR), 1);
            GameManager.instance.AddShowTile(new ShowTile(new Vector3(2, 5, 0), SHOW_TYPE.NEAR), 1);            
        }

        void GenerateShowTileRandom(int id, int count)
        {
            List<Vector3> postionA = new List<Vector3>(){
                                    new Vector3(2, 2, 0),
                                    new Vector3(5, 2, 0),
                                    new Vector3(2, 5, 0),
                                    new Vector3(5, 5, 0),
                                    new Vector3(7, 0, 0),
                                    new Vector3(0, 7, 0),
                                };

            List<SHOW_TYPE> sTypeA = new List<SHOW_TYPE>();
            List<SHOW_TYPE> specailTiles = new List<SHOW_TYPE>() { SHOW_TYPE.MONSTER, SHOW_TYPE.TRAP, SHOW_TYPE.GEM_ITEM, SHOW_TYPE.ALL };
            int idx = Random.Range(0, specailTiles.Count);
            sTypeA.Add(specailTiles[idx]);
            specailTiles.RemoveAt(idx);
            idx = Random.Range(0, specailTiles.Count);
            sTypeA.Add(specailTiles[idx]);
            sTypeA.Add(SHOW_TYPE.NEAR);
            sTypeA.Add(SHOW_TYPE.NEAR);
            sTypeA.Add(SHOW_TYPE.NEAR);
            sTypeA.Add(SHOW_TYPE.NEAR);

            GameManager.instance.AddShowTile(new ShowTile(new Vector3(0, 0, 0), SHOW_TYPE.NEAR), id);

            for (int i = 0; i < count-1; i++)
            {
                Vector3 rPos = GetRandomPos(postionA);
                GameManager.instance.AddShowTile(new ShowTile(rPos, sTypeA[i]), id);
            }
        }

        void SetShowTile(GameObject instance, ShowTile[] showTiles, float x, float y)
        {
            foreach (ShowTile tile in showTiles)
            {
                if (x == tile.x && y == tile.y)
                {
                    switch (tile.type)
                    {
                        case SHOW_TYPE.NEAR: SetTileColor(instance, Color.green); break;
                        case SHOW_TYPE.MONSTER: SetTileColor(instance, Color.red); break;
                        case SHOW_TYPE.TRAP: SetTileColor(instance, Color.blue); break;
                        case SHOW_TYPE.GEM_ITEM: SetTileColor(instance, Color.yellow); break;
                        case SHOW_TYPE.ALL: SetTileColor(instance, Color.black); break;
                    }
                }
            }
        }

        void SetTileColor(GameObject tile, Color color)
        {
            Color lerpedColor = tile.GetComponent<Renderer>().material.color;
            lerpedColor = Color.Lerp(lerpedColor, color, 0.3f);
            tile.GetComponent<Renderer>().material.color = lerpedColor;
        }

        Vector3 RandomPosition()
        {
            int randomIndex = Random.Range(0, gridPositions.Count);
            Vector3 randomPosition = gridPositions[randomIndex];
            gridPositions.RemoveAt(randomIndex);
            return randomPosition;
        }
        
        Vector3 RandomGridsPosition()
        {
            if(grids.Count == 0)
            {
                grids.Add(gridPositions1);
                grids.Add(gridPositions2);
                grids.Add(gridPositions3);
                grids.Add(gridPositions4);
            }

            int randomGridsIndex = Random.Range(0, grids.Count);
            List<Vector3> agrid = grids[randomGridsIndex];
            grids.RemoveAt(randomGridsIndex);

            int randomIndex = Random.Range(0, agrid.Count);
            Vector3 randomPosition = agrid[randomIndex];
            agrid.RemoveAt(randomIndex);

            while (gridPositionsExcept.Contains(randomPosition))
            {
                randomIndex = Random.Range(0, agrid.Count);                
                randomPosition = agrid[randomIndex];
                agrid.RemoveAt(randomIndex);
            }
            
            return randomPosition;
        }

        void LayoutAnObjectAtRandom(int id, GameObject tileChoice)
        {
            Vector3 randomPosition = RandomGridsPosition();
            GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
            GameManager.instance.AddObj(instance, id);
        }

        void LayoutObjectAtRandom(int id, GameObject[] tileArray, int minimum, int maximum)
        {            
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                LayoutAnObjectAtRandom(id, tileChoice);
            }
        }

        void LayoutTrapAtRandom(int id, GameObject trap, int trapNum)
        {
            for (int i = 0; i < trapNum; i++)
            {
                Vector3 randomPosition = RandomGridsPosition();
                GameObject instance = Instantiate(trap, randomPosition, Quaternion.identity);
                GameManager.instance.AddTrap(instance, id);
            }
        }

        void LayoutTrapLv2AtRandom(int id, GameObject trapLv2, int trapLv3Num)
        {
            for (int i = 0; i < trapLv3Num; i++)
            {
                Vector3 randomPosition = RandomGridsPosition();
                GameObject instance = Instantiate(trapLv2, randomPosition, Quaternion.identity);
                GameManager.instance.AddTrap(instance, id);
            }
        }

        void LayoutEnemiesAtRandom(int id, GameObject[] tileArray, int minimum, int maximum)
        {
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomGridsPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
                GameManager.instance.AddEnemy(instance, id);
            }
        }

        public void SetupScene(Level[] levelInfos, bool tutorial = false)
        {
            foreach(Level lv in levelInfos)
            {
                if (tutorial && lv.id == 1) SetupTutorialShowTile();
                else GenerateShowTileRandom(lv.id, lv.showTileNum);
                BoardSetup(lv);
                InitialiseList(lv);
                if(tutorial && lv.id == 1) SetupTutorialLevel(lv);
                else SetupLevelRandom(lv);

                GameObject instance = Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
                GameManager.instance.AddTile(instance, lv.id);                
            }           
        }

        public void SetupItemDrops(ItemDropInfo[] itemDropInfos, Level lv)
        {
            List<int> dropItems = new List<int>();
            foreach (ItemDropInfo dropInfo in itemDropInfos)
            {
                if(Random.Range(0f, 1f) <= dropInfo.dropRate)
                {
                    int randomIndex = Random.Range(0, dropInfo.ids.Length - 1);
                    dropItems.Add(dropInfo.ids[randomIndex]);
                }                                
            }

            foreach (int id in dropItems)
            {
                foreach (GameObject itemTile in itemTiles)
                {
                    ItemObject itemObj = itemTile.GetComponent<ItemObject>();
                    if (itemObj.itemId == id)
                    {
                        LayoutAnObjectAtRandom(lv.id, itemTile);                        
                    }
                }
            }
        }

        public void SetupLevelRandom(Level levelInfo)
        {
            int gemRate = levelInfo.gem;
            int enemyCount = levelInfo.enemy;
            int strongEnemyCount = levelInfo.strongEnemy;
            int thiefCount = levelInfo.thief;
            int trapCount = levelInfo.trap;
            int trapLv2Count = levelInfo.trapLv2;

            ItemDropInfo[] itemDropInfos = levelInfo.itemDropInfos;

            SetupItemDrops(itemDropInfos, levelInfo);

            LayoutObjectAtRandom(levelInfo.id, gemTiles, gemRate, gemRate);            

            LayoutTrapAtRandom(levelInfo.id, trapTiles[0], trapCount);
            LayoutTrapLv2AtRandom(levelInfo.id, trapTiles[1], trapLv2Count);
            
            LayoutEnemiesAtRandom(levelInfo.id, enemyTiles, enemyCount, enemyCount);
            LayoutEnemiesAtRandom(levelInfo.id, strongEnemyTiles, strongEnemyCount, strongEnemyCount);
            LayoutEnemiesAtRandom(levelInfo.id, thiefTiles, thiefCount, thiefCount);            
        }

        public void SetupTutorialLevel(Level levelInfo)
        {
            GameObject showItemTile = null;
            GameObject healItemTile = null;

            foreach (GameObject itemTile in itemTiles)
            {
                ItemObject itemObj = itemTile.GetComponent<ItemObject>();
                if (itemObj.itemId == 104) showItemTile = itemTile;
                if (itemObj.itemId == 101) healItemTile = itemTile;
            }

            Vector3 pos = new Vector3(2f, 7f, 0f);
            GameObject instance = Instantiate(showItemTile, pos, Quaternion.identity);
            GameManager.instance.AddObj(instance, levelInfo.id);

            pos = new Vector3(7f, 3f, 0f);
            instance = Instantiate(healItemTile, pos, Quaternion.identity);
            GameManager.instance.AddObj(instance, levelInfo.id);

            pos = new Vector3(1f, 3f, 0f);
            instance = Instantiate(gemTiles[0], pos, Quaternion.identity);
            GameManager.instance.AddObj(instance, levelInfo.id);

            pos = new Vector3(4f, 3f, 0f);
            instance = Instantiate(gemTiles[0], pos, Quaternion.identity);
            GameManager.instance.AddObj(instance, levelInfo.id);

            Vector3[] tutorialTrapPositions = 
                {
                    new Vector3(0, 2, 0), new Vector3(0, 3, 0), new Vector3(0, 4, 0),
                    new Vector3(1, 6, 0),
                    new Vector3(2, 0, 0), new Vector3(2, 1, 0),new Vector3(2, 3, 0), new Vector3(2, 6, 0),
                    new Vector3(3, 0, 0), new Vector3(3, 1, 0),new Vector3(3, 2, 0), new Vector3(3, 3, 0),new Vector3(3, 4, 0),new Vector3(3, 5, 0),
                    new Vector3(4, 0, 0), new Vector3(4, 1, 0),new Vector3(4, 2, 0),
                    new Vector3(5, 4, 0), new Vector3(5, 6, 0),new Vector3(5, 7, 0),
                    new Vector3(6, 1, 0), new Vector3(6, 2, 0),new Vector3(6, 3, 0),new Vector3(6, 4, 0),new Vector3(6, 5, 0),new Vector3(6, 6, 0),new Vector3(6, 7, 0)
            };

            for (int i = 0; i < tutorialTrapPositions.Length; i++)
            {
                pos = tutorialTrapPositions[i];
                instance = Instantiate(trapTiles[0], pos, Quaternion.identity);
                GameManager.instance.AddTrap(instance, levelInfo.id);
            }            
        }
    }
}
