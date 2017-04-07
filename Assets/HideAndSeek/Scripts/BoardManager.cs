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
        public GameObject[] wallTiles;
        public GameObject[] foodTiles;
        public GameObject[] sodaTiles;
        public GameObject[] goldATiles;
        public GameObject[] enemyTiles;
        public GameObject[] strongEnemyTiles;
        public GameObject[] outerWallTiles;

        public GameObject[] trapTiles;

        private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
        private List<Vector3> gridPositions = new List<Vector3>();   //A list of possible locations to place tiles.

        //Clears our list gridPositions and prepares it to generate a new board.
        void InitialiseList()
        {
            gridPositions.Clear();

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (x == columns - 1 && y == rows - 1) continue;

                    gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }


        //Sets up the outer walls and floor (background) of the game board.
        void BoardSetup()
        {
            //Instantiate Board and set boardHolder to its transform.
            boardHolder = new GameObject("Board").transform;

            //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
            for (int x = -1; x < columns + 1; x++)
            {
                //Loop along y axis, starting from -1 to place floor or outerwall tiles.
                for (int y = -1; y < rows + 1; y++)
                {
                    //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                    //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                    if (x == -1 || x == columns || y == -1 || y == rows)
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                    //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                    GameObject instance =
                        Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    bool bShow = false;
                    if (x == 0 && y == 0) bShow = true;
                    else if (x == 7 && y == 7) bShow = true;
                    else if (x == 7 && y == 0) bShow = true;
                    else if (x == 0 && y == 7) bShow = true;
                    //else if (x == 1 && y == 4) bShow = true;
                    //else if (x == 2 && y == 1) bShow = true;
                    //else if (x == 2 && y == 6) bShow = true;
                    //else if (x == 4 && y == 7) bShow = true;
                    //else if (x == 5 && y == 0) bShow = true;
                    //else if (x == 5 && y == 5) bShow = true;
                    //else if (x == 6 && y == 3) bShow = true;
                    if (bShow)
                    {
                        Color lerpedColor = instance.GetComponent<Renderer>().material.color;
                        lerpedColor = Color.Lerp(lerpedColor, Color.red, 0.1f);
                        instance.GetComponent<Renderer>().material.color = lerpedColor;
                    }
                    //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                    instance.transform.SetParent(boardHolder);
                }
            }
        }

        //RandomPosition returns a random position from our list gridPositions.
        Vector3 RandomPosition()
        {
            //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
            int randomIndex = Random.Range(0, gridPositions.Count);

            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
            Vector3 randomPosition = gridPositions[randomIndex];

            //Remove the entry at randomIndex from the list so that it can't be re-used.
            gridPositions.RemoveAt(randomIndex);

            //Return the randomly selected Vector3 position.
            return randomPosition;
        }


        void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {            
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
                GameManager.instance.objsOnStage.Add(instance);
            }
        }

        void LayoutTrapsAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {            
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomPosition();
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
                Vector3 randomPosition = RandomPosition();
                while (0 == randomPosition.x || randomPosition.x == 7 || randomPosition.y == 0 || randomPosition.y == 7)
                {
                    randomPosition = RandomPosition();
                }

                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }


        public void SetupScene(int level)
        {
            BoardSetup();

            InitialiseList();

            SetupLevelRandom(level);
            //if (level < 4) SetupBeginnerLevel(level);
            //else SetupLevelRandom(level);
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        }

        public void SetupLevelRandom(int level)
        {
            int foodRate = 0;
            int sodaRate = 0;
            int goldRate = 0;

            int enemyCount = 0;
            int strongEnemyCount = 0;

            int trapCount = 0;
            if (level < 4) // 1,2,3
            {
                if (level == 3) foodRate = 1;
                trapCount = level + 6;
                goldRate = level;
            }
            else if (3 < level && level < 7) // 4,5,6
            {
                if (level == 6) foodRate = 2;
                else foodRate = 1;

                trapCount = level + 1;
                goldRate = 3;
                enemyCount = 1;
            }
            else if (6 < level && level < 10) // 7,8,9
            {
                enemyCount = 2;
                trapCount = level - 2;
                goldRate = 3;

                if (level == 9)
                {
                    foodRate = 1;
                    sodaRate = 1;
                }
                else
                {
                    foodRate = 2;
                }

            }
            else if (9 < level && level < 13) // 10,11,12
            {
                enemyCount = 3;
                trapCount = level - 5;
                goldRate = 3;
                foodRate = 1;
                sodaRate = 1;
            }
            else if (12 < level && level < 16) // 13,14,15
            {
                enemyCount = level - 13;
                strongEnemyCount = 1;
                trapCount = 7;
                goldRate = 3;
                foodRate = 2;
                sodaRate = 1;
            }
            else if (15 < level && level < 19) // 16,17,18
            {
                enemyCount = level - 16;
                strongEnemyCount = 2;
                trapCount = 7;
                goldRate = 3;
                foodRate = 3;
                sodaRate = 1;
            }

            GameManager.instance.objsOnStage.Clear();
            LayoutObjectAtRandom(foodTiles, foodRate, foodRate);
            LayoutObjectAtRandom(sodaTiles, sodaRate, sodaRate);
            LayoutObjectAtRandom(goldATiles, goldRate, goldRate);

            GameManager.instance.trapsOnStage.Clear();
            LayoutTrapsAtRandom(trapTiles, trapCount, trapCount);

            LayoutEnemiesAtRandom(enemyTiles, enemyCount, enemyCount);
            LayoutEnemiesAtRandom(strongEnemyTiles, strongEnemyCount, strongEnemyCount);
        }
    }
}
