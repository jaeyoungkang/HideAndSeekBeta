using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace HideAndSeek
{
    public class Level
    {
        public int index;
        public int trap;
        public int enemy;
        public int strongEnemy;
        public int thief;
        public int gem;
        public bool clear;

        public Level(int _index, int _trap, int _enemy, int _strongEnemy, int _thief, int _gem)
        {
            index = _index;
            trap = _trap;
            enemy = _enemy;
            strongEnemy = _strongEnemy;
            thief = _thief;
            gem = _gem;
            clear = false;
        }
        public void Init() { clear = false; }
    }

    public class Dungeon
    {
        private Level[] levels;
        public int curLevel;
        public int lastLevel;
        private int cost;
        private int gold;
        private float timeLimit;

        public Dungeon(Level[] _levels, int _cost, int _gold, float _timeLimit)
        {
            levels = _levels;
            cost = _cost;
            lastLevel = levels.Length;
            gold = _gold;
            timeLimit = _timeLimit;
        }

        public void init()
        {
            foreach(Level alevel in levels)
            {
                alevel.Init();
            }
        }

        public override string ToString() { return curLevel + "/" + lastLevel; }

        public int Cost() { return cost; }
        public float TimeLimit() { return timeLimit; }

        public Level GetCurLevel() { return levels[curLevel - 1]; }
        public Level[] GetLevels() { return levels; }

        public void clearCurLevel() { levels[curLevel - 1].clear = true; }
        public bool IsEnd() { return levels[lastLevel - 1].clear; }
        public int GetReward() { return gold;  }

        public void SetLevel(int _level)
        {
            curLevel = _level;
        }
    }

    public class DungeonManager : MonoBehaviour
    {        
        private Dungeon dungeonA;
        private Dungeon dungeonB;
        private Dungeon dungeonC;

        public void InitDungeons()
        {
            // TAKE ABOUT 1 MINUTE
            Level[] dungeonAInfo = new Level[] {
                                   new Level(0, 8, 0, 0, 0, 1),
                                   new Level(1, 9, 0, 0, 0, 1),
                                    new Level(2, 10, 1, 0, 0, 2),
                                    new Level(3, 12, 3, 0, 0, 0)
                                };
            dungeonA = new Dungeon(dungeonAInfo, 0, 3, 60);

            // TAKE ABOUT 3 MINUTES
            Level[] dungeonBInfo = new Level[] {
                                   new Level(0, 6, 1, 0, 0, 1),
                                   new Level(1, 7, 1, 0, 0, 1),
                                    new Level(2, 8, 1, 0, 0, 1),
                                    new Level(3, 8, 1, 0, 0, 1),
                                    new Level(4, 8, 1, 0, 0, 1),
                                    new Level(5, 8, 1, 0, 0, 1),
                                    new Level(6, 8, 1, 0, 0, 1),
                                    new Level(7, 8, 1, 0, 0, 1),
                                    new Level(8, 9, 1, 0, 0, 0)
            };
            dungeonB = new Dungeon(dungeonBInfo, 2, 10, 180);

            // TAKE ABOUT 5 MINUTES
            Level[] dungeonCInfo = new Level[] {
                                   new Level(0, 6, 1, 0, 0, 1),
                                   new Level(1, 7, 1, 0, 0, 1),
                                    new Level(2, 8, 1, 0, 0, 1),
                                    new Level(3, 8, 1, 0, 0, 1),
                                    new Level(4, 8, 1, 0, 0, 1),
                                    new Level(5, 8, 1, 0, 0, 1),
                                    new Level(6, 8, 1, 0, 0, 1),
                                    new Level(7, 8, 1, 0, 0, 1),
                                    new Level(8, 8, 1, 0, 0, 1),
                                    new Level(9, 8, 1, 0, 0, 1),
                                    new Level(10, 8, 1, 0, 0, 1),
                                    new Level(11, 8, 1, 0, 0, 1),
                                    new Level(12, 8, 1, 0, 0, 1),
                                    new Level(13, 8, 1, 0, 0, 1),
                                    new Level(14, 8, 1, 0, 0, 1),
                                    new Level(15, 8, 1, 0, 0, 1)
            };
            dungeonC = new Dungeon(dungeonCInfo, 3, 20, 300);
        }

        public Dungeon DungeonA()
        {
            return dungeonA;
        }

        public Dungeon DungeonB()
        {
            return dungeonB;
        }

        public Dungeon DungeonC()
        {
            return dungeonC;
        }
    }
}
