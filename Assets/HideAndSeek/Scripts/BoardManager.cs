using UnityEngine;
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

        public GameObject[] trapTiles;

        private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
        private List<Vector3> gridPositions = new List<Vector3>();   //A list of possible locations to place tiles.

        private List<List<Vector3>> grids = new List<List<Vector3>>();
        private List<Vector3> gridPositions1 = new List<Vector3>();
        private List<Vector3> gridPositions2 = new List<Vector3>();
        private List<Vector3> gridPositions3 = new List<Vector3>();
        private List<Vector3> gridPositions4 = new List<Vector3>();

        private List<Vector3> gridPositionsExcept = new List<Vector3>();

        //Clears our list gridPositions and prepares it to generate a new board.
        void InitialiseList()
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

            gridPositionsExcept.Add(new Vector3(0f, 0f, 0f));
            gridPositionsExcept.Add(new Vector3(1f, 0f, 0f));
            gridPositionsExcept.Add(new Vector3(0f, 1f, 0f));

            gridPositionsExcept.Add(new Vector3(columns - 1, 0f, 0f));
            gridPositionsExcept.Add(new Vector3(columns - 2, 0f, 0f));
            gridPositionsExcept.Add(new Vector3(columns - 1, 1f, 0f));

            gridPositionsExcept.Add(new Vector3(0f, rows - 1, 0f));
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


        void BoardSetup()
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
                    bool bShow = false;
                    if (x == 0 && y == 0) bShow = true;
                    else if (x == columns-1 && y == rows-1) bShow = true;
                    else if (x == columns-1 && y == 0) bShow = true;
                    else if (x == 0 && y == rows-1) bShow = true;

                    if (bShow)
                    {
                        Color lerpedColor = instance.GetComponent<Renderer>().material.color;
                        lerpedColor = Color.Lerp(lerpedColor, Color.red, 0.1f);
                        instance.GetComponent<Renderer>().material.color = lerpedColor;
                    }

                    instance.transform.SetParent(boardHolder);

                    GameManager.instance.tilesOnStage.Add(instance);
                }
            }
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


        void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {            
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomGridsPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
                GameManager.instance.objsOnStage.Add(instance);
            }
        }

        void LayoutTrapAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomGridsPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
                GameManager.instance.trapsOnStage.Add(instance);
            }
        }

        void LayoutEnemiesAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomGridsPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }


        public void SetupScene(Level levelInfo)
        {
            GameManager.instance.tilesOnStage.Clear();
            GameManager.instance.objsOnStage.Clear();
            GameManager.instance.trapsOnStage.Clear();

            BoardSetup();

            InitialiseList();

            SetupLevelRandom(levelInfo);

            GameObject instance = Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
            GameManager.instance.tilesOnStage.Add(instance);
        }

        public void SetupLevelRandom(Level levelInfo)
        {
            int gemRate = levelInfo.gem;
            int enemyCount = levelInfo.enemy;
            int strongEnemyCount = levelInfo.strongEnemy;
            int thiefCount = levelInfo.thief;
            int trapCount = levelInfo.trap;
            
            LayoutObjectAtRandom(gemTiles, gemRate, gemRate);

            LayoutTrapAtRandom(trapTiles, trapCount, trapCount);
                        
            LayoutEnemiesAtRandom(enemyTiles, enemyCount, enemyCount);
            LayoutEnemiesAtRandom(strongEnemyTiles, strongEnemyCount, strongEnemyCount);
            LayoutEnemiesAtRandom(thiefTiles, thiefCount, thiefCount);
        }
    }
}
