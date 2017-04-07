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

			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}

        public int columns = 8; 										//Number of columns in our game board.
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

        private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();   //A list of possible locations to place tiles.

        //Clears our list gridPositions and prepares it to generate a new board.
        void InitialiseList ()
		{ 
            gridPositions.Clear ();
			
			for(int x = 0; x < columns; x++)
			{
				for(int y = 0; y < rows; y++)
				{
                    if (x == 0 && y == 0) continue;
                    if (x == columns-1 && y == rows-1) continue;

                    gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
		
		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
			boardHolder = new GameObject ("Board").transform;

            //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
            for (int x = -1; x < columns + 1; x++)
			{
				//Loop along y axis, starting from -1 to place floor or outerwall tiles.
				for(int y = -1; y < rows + 1; y++)
				{
					//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					
					//Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
					if(x == -1 || x == columns || y == -1 || y == rows)
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
					//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
					GameObject instance =
						Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
                    bool bShow = false;
                    if (x == 0 && y == 0) bShow = true;
                    else if (x == 7 && y == 7) bShow = true;
                    else if (x == 1 && y == 4) bShow = true;
                    else if (x == 2 && y == 1) bShow = true;
                    else if (x == 2 && y == 6) bShow = true;
                    else if (x == 4 && y == 7) bShow = true;
                    else if (x == 5 && y == 0) bShow = true;
                    else if (x == 5 && y == 5) bShow = true;
                    else if (x == 6 && y == 3) bShow = true;
                    if (bShow)
                    {
                        Color lerpedColor = instance.GetComponent<Renderer>().material.color;
                        lerpedColor = Color.Lerp(lerpedColor, Color.red, 0.1f);
                        instance.GetComponent<Renderer>().material.color = lerpedColor;
                    }
                    //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                    instance.transform.SetParent (boardHolder);
                }
			}
        }

		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);
			
			//Return the randomly selected Vector3 position.
			return randomPosition;
		}
		
		
		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];

                //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}

        void LayoutTrapsAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            GameManager.instance.trapsOnStage.Clear();
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
                GameManager.instance.trapsOnStage.Add(instance);
            }
        }

        void LayoutGoldsAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            GameManager.instance.goldsOnStage.Clear();
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomPosition();         
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];         
                GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
                GameManager.instance.goldsOnStage.Add(instance);
            }
        }

        void LayoutEnemiesAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            int objectCount = Random.Range(minimum, maximum + 1);
            float value = Random.Range(0f, 1f);

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


        public void SetupScene (int level)
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
            int foodRate = 2;
            int strongFoodRate = 0;
            int sodaRate = 0;
            int strongSodaRate = 0;
            int goldRate = level;
            int strongGoldRate = 0;
            if (goldRate > 5) goldRate = 5;
                        
            int enemyCount = 0;
            int strrongEnemyCount = 0;
            if (3 < level && level < 7) // 4,5,6
            {
                enemyCount = level + 2; // 6,7,8
                strrongEnemyCount = 0;
            }
            else if (6 < level && level < 10) // 7,8,9
            {
                enemyCount = level - 3; // 4,5,6
                strrongEnemyCount = 1;
            }
            else if (9 < level && level < 13) // 10,11,12
            {
                enemyCount = level - 7; // 3,4,5
                strrongEnemyCount = 2;
            }
            else if (12 < level && level < 16) // 13,14,15
            {
                enemyCount = level - 11; // 2,3,4
                strrongEnemyCount = 3;
            }
            else if (15 < level && level < 19) // 16,17,18
            {
                enemyCount = 4; // 3,3,3
                strrongEnemyCount = 4;
            }
                        
            LayoutObjectAtRandom(foodTiles, foodRate, foodRate);
            //            LayoutObjectAtRandom(sodaTiles, sodaRate, sodaRate);
            LayoutGoldsAtRandom(goldATiles, goldRate, goldRate);
            LayoutTrapsAtRandom(trapTiles, 3, 3);
            LayoutEnemiesAtRandom(enemyTiles, enemyCount, enemyCount);
            LayoutEnemiesAtRandom(strongEnemyTiles, strrongEnemyCount, strrongEnemyCount);            
        }

        enum TILE_TYPE { FOOD, SODA, ENEMY_A, ENEMY_B, GOLD_A, GOLD_B, GOLD_C }

        struct TILE_INFO
        {
            public TILE_TYPE type;
            public int x;
            public int y;
        }

        GameObject GetTile(TILE_TYPE tileType)
        {
            switch(tileType)
            {
                case TILE_TYPE.FOOD:
                    return foodTiles[Random.Range(0, foodTiles.Length)];

                case TILE_TYPE.ENEMY_A:
                    return enemyTiles[Random.Range(0, enemyTiles.Length)];

                case TILE_TYPE.SODA:
                    return sodaTiles[Random.Range(0, sodaTiles.Length)];

                case TILE_TYPE.GOLD_A:
                    return goldATiles[Random.Range(0, goldATiles.Length)];
            }

            return null;
        }        

        void LayoutTiles(List<TILE_INFO> tiles)
        {
            foreach(TILE_INFO tile in tiles)
            {
                Vector3 tilePosition = new Vector3(tile.x, tile.y, 0);
                
                GameObject tileChoice = GetTile(tile.type);
                GameObject instance = Instantiate(tileChoice, tilePosition, Quaternion.identity);

                if (tile.type == TILE_TYPE.GOLD_A)
                {
                    GameManager.instance.goldsOnStage.Add(instance);
                }
            }            
        }       

        public void SetupBeginnerLevel(int level)
        {               
            List<TILE_INFO> tiles = new List<TILE_INFO>();
            // level, type, posx, posy &
            String[] infos1 = { "food,4,4", "food,4,3",
                                "EnemyA,1,1", "EnemyA,6,6", "EnemyA,3,3",
                                "goldA,7,0"};

            String[] infos2 = { "food,4,4", "food,4,3",
                                "EnemyA,1,1", "EnemyA,6,6", "EnemyA,3,3", "EnemyA,4,1",
                                "goldA,7,0", "goldA,2,7"};

            String[] infos3 = { "food,4,4", "food,4,3",
                                "EnemyA,2,1", "EnemyA,6,7", "EnemyA,3,4", "EnemyA,6,2", "EnemyA,0,7",
                                "goldA,7,0", "goldA,2,7", "goldA,6,6"};
            String[] infos;

            if (level == 1) infos = infos1;
            else if (level == 2) infos = infos2;
            else if (level == 3) infos = infos3;
            else infos = infos1;            

            for (int i=0; i< infos.Length; i++)
            {
                String[] info = infos[i].Split(',');
                TILE_INFO tileInfo = new TILE_INFO();
                tileInfo.x = Int32.Parse(info[1]);
                tileInfo.y = Int32.Parse(info[2]);
                if (info[0] == "food")
                    tileInfo.type = TILE_TYPE.FOOD;
                else if (info[0] == "EnemyA")
                    tileInfo.type = TILE_TYPE.ENEMY_A;
                else if (info[0] == "soda")
                    tileInfo.type = TILE_TYPE.SODA;
                else if (info[0] == "goldA")
                    tileInfo.type = TILE_TYPE.GOLD_A;

                tiles.Add(tileInfo);
            }

            LayoutTiles(tiles);
        }
    }    
}
