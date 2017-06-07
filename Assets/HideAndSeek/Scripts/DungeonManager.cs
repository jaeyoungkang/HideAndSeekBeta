using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace HideAndSeek
{
    [System.Serializable]
    public class Level
    {
        public int index;
        public int trap;
        public int enemy;
        public int strongEnemy;
        public int thief;
        public int gem;
        public int[] nextIndex;
        public bool clear;
        public bool close;

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

    [System.Serializable]
    public class Dungeon
    {
        public string name;
        public int index;
        public int nextIndex;        
        public Level[] levels;        
        public int cost;
        public int gem;
        public float timeLimit;

        public int curLevel;
        public int lastLevel;

        public Dungeon(Level[] _levels, string _name,  int _cost, int _gem, float _timeLimit)
        {
            levels = _levels;
            name = _name;
            cost = _cost;
            lastLevel = levels.Length;
            gem = _gem;
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
        public int GetReward() { return gem;  }

        public void SetLevel(int _level)
        {
            curLevel = _level;
        }
    }

    public class DungeonManager : MonoBehaviour
    {
        public Dungeon[] dungeon;
    }
}
