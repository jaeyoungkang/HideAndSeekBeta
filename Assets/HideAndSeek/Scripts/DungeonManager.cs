using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace HideAndSeek
{
    public enum SHOW_TYPE { NONE, NEAR, MONSTER, TRAP, GEM_ITEM, ALL };

    [System.Serializable]
    public class ShowTile
    {
        public Vector2 pos;
        public SHOW_TYPE type;

        public ShowTile(Vector2 _pos, SHOW_TYPE _type)
        {
            pos = _pos;
            type = _type;
        }
    }

    [System.Serializable]
    public class ItemDropInfo
    {
        public int[] ids;
        public float dropRate;
    }

    [System.Serializable]
    public class Level
    {
        public string name;
        public int id;
        public int trap;
        public int enemy;
        public int strongEnemy;
        public int thief;
        public int gem;
        public ItemDropInfo[] itemDropInfos;        
        public int[] nextIds;
        public bool clear;
        public bool close;
        public ShowTile[] showTiles = 
                    {
                        new ShowTile(new Vector3(0, 0, 0 ), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(2, 2, 0 ), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(5, 2, 0 ), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(2, 5, 0 ), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(5, 5, 0 ), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(7, 0, 0 ), SHOW_TYPE.MONSTER),
                        new ShowTile(new Vector3(0, 7, 0 ), SHOW_TYPE.TRAP)
                    };

        [HideInInspector]
        public int[] itemsDropped;
    }

    [System.Serializable]
    public class Dungeon
    {
        public string name;
        public int id;
        public int nextId;        
        public Level[] levels;        
        public int gem;
        public float timeLimit;
        public int curLevelId;
        public bool open;    

        public Dungeon(Level[] _levels, string _name,  int _cost, int _gem, float _timeLimit)
        {
            levels = _levels;
            name = _name;
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
            if (levels[0].id == 1) levels[0].close = false;
        }

        public override string ToString() { return GetCurLevel().name; }
        public float TimeLimit() { return timeLimit; }

        public Level GetCurLevel()
        {
            foreach (Level lv in levels)
            {
                if (lv.id == curLevelId)
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
                if (lv.id == curLevelId)
                {
                    lv.clear = true;
                    OpenNextLevel(lv.nextIds);
                }                    
            }
        }

        public void OpenNextLevel(int[] nextIds)
        {            
            foreach (int id in nextIds)
            {
                foreach (Level lv in levels)
                {
                    if (lv.id == id) lv.close = false;
                }
            }
        }

        public bool IsEnd() { return levels[levels.Length - 1].clear; }
        public int GetReward() { return gem;  }

        public void SetLevel(int _level)
        {
            curLevelId = _level;
        }
    }

    public class LevelPlayData
    {
        public string dungeonName = "";
        public string levelName = "";
        public int gemCount = 0;
        public int deathCount = 0;
        public int trappedCount = 0;
        public int attackedCount = 0;
        public int damagedByTimeCount = 0;
        
        public List<int> hps = new List<int>();
        public List<string> useItems = new List<string>();
        public List<string> getItems = new List<string>();

        public void Init()
        {
            dungeonName = "";
            InitLevelPlayData();
        }

        public void InitLevelPlayData()
        {
            levelName = "";
            gemCount = 0;
            deathCount = 0;
            trappedCount = 0;
            attackedCount = 0;
            damagedByTimeCount = 0;
            hps.Clear();
            useItems.Clear();
            getItems.Clear();
        }

    }
}
