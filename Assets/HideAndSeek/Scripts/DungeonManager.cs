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
        public string name;
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

        public Dungeon(Level[] _levels, string _name,  int _cost, int _gem, float _timeLimit)
        {
            levels = _levels;
            name = _name;
            cost = _cost;
            gem = _gem;
            timeLimit = _timeLimit;
        }

        public void init()
        {
            foreach(Level alevel in levels)
            {
                alevel.clear = false;
                alevel.close = true;
            }
            if (levels[0].index == 1) levels[0].close = false;
        }

        public override string ToString() { return GetCurLevel().name; }

        public int Cost() { return cost; }
        public float TimeLimit() { return timeLimit; }

        public Level GetCurLevel()
        {
            foreach (Level lv in levels)
            {
                if (lv.index == curLevel)
                {
                    return lv;
                }
            }

            return null;
        }

        public void clearCurLevel()
        {
            foreach (Level lv in levels)
            {
                if (lv.index == curLevel)
                {
                    lv.clear = true;
                    OpenNextLevel(lv.nextIndex);
                }                    
            }
        }

        public void OpenNextLevel(int[] nextIndexs)
        {            
            foreach (int index in nextIndexs)
            {
                foreach (Level lv in levels)
                {
                    if (lv.index == index) lv.close = false;
                }
            }
        }

        public bool IsEnd() { return levels[levels.Length - 1].clear; }
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
