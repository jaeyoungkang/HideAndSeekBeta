using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

namespace HideAndSeek
{    
    public enum SHOW_TYPE { NONE, NEAR, MONSTER, TRAP, GEM_ITEM, ALL };

    [System.Serializable]
    public class ShowTile
    {        
        public float x,y;
        public SHOW_TYPE type;

        public ShowTile() { }
        public ShowTile(Vector2 _pos, SHOW_TYPE _type)
        {
            x = _pos.x;
            y = _pos.y;
            type = _type;
        }
    }

    [System.Serializable]
    public class ItemDropInfo
    {
        public int[] ids;
        public float dropRate;
        public ItemDropInfo() { }
        public ItemDropInfo(int[] _ids, float _dropRate)
        { ids = _ids; dropRate = _dropRate; }
    }

    [System.Serializable]
    public class Level
    {
        public string name;
        public int id;
        public int trap;
        public int trapLv2;
        public int enemy;
        public int strongEnemy;
        public int thief;
        public int gem;
        public ItemDropInfo[] itemDropInfos;        
        public int[] nextIds;
        public bool clear;
        public bool close;
        public int showTileNum = 7;

        public void setup(string _name, int _id, int _trap, int _trapLv2, int _enemy, int _strongEnemy, int _thief, int _gem,
                            ItemDropInfo[] _itemDropInfos, int[] _nextIds, bool _clear, bool _close, int _showTile)
        {
            name = _name;
            id = _id;
            trap = _trap;
            trapLv2 = _trapLv2;
            enemy = _enemy;
            strongEnemy = _strongEnemy;
            thief = _thief ;
            gem = _gem;
            itemDropInfos = _itemDropInfos;
            nextIds = _nextIds;
            clear = _clear;
            close = _close;
            showTileNum = _showTile;
        }
    }

    [System.Serializable]
    public class Dungeon
    {
        public string name;
        public int id;
        public int nextId;        
        public Level[] levels;        
        public float timeLimit;
        public int curLevelId;
        public bool open;
        public bool locked;
        public int gem;

        public Dungeon(string _name, int _id, int _nextId, Level[] _levels, float _timeLimit, bool _open, bool _locked, int _gem)
        {
            name = _name;
            id = _id;
            nextId = _nextId;
            levels = _levels; 
            timeLimit = _timeLimit;
            open = _open;
            locked = _locked;
            gem = _gem;
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

        public void SetLevel(int _level)
        {
            curLevelId = _level;
        }
    }

    public class DungeonPlayData
    {
        public int gemCount = 0;
        public int damagedBytrapCount = 0;
        public int damagedByEnemyCount = 0;
        public int damagedByTimeCount = 0;

        public int destroyEnemy = 0;
        public int destroyTrap = 0;
        public int butItems = 0;
        public int sellItems = 0;

        public List<string> useItems = new List<string>();
        public List<string> getItems = new List<string>();

        public void Init()
        {
            gemCount = 0;
            damagedBytrapCount = 0;
            damagedByEnemyCount = 0;
            damagedByTimeCount = 0;

            destroyEnemy = 0;
            destroyTrap = 0;
            butItems = 0;
            sellItems = 0;

            useItems.Clear();
            getItems.Clear();
        }
    }

    public class DungeonData
    {
        ItemDropInfo dropSetShow10 = new ItemDropInfo(new int[] { 104, 105, 106, 107 }, 0.1f);
        ItemDropInfo dropSetDestroy10 = new ItemDropInfo(new int[] { 109, 110, 111, 114 }, 0.1f);
        ItemDropInfo dropSetHeal10 = new ItemDropInfo(new int[] { 102, 102, 103 }, 0.1f);

        ItemDropInfo dropSetShow50 = new ItemDropInfo(new int[] { 104, 105, 106 }, 0.5f);
        ItemDropInfo dropSetDestroy50 = new ItemDropInfo(new int[] { 109, 110, 111, 114 }, 0.5f);
        ItemDropInfo dropSetHeal50 = new ItemDropInfo(new int[] { 101, 101, 101, 101, 102 }, 0.5f);
        ItemDropInfo dropSetTime20 = new ItemDropInfo(new int[] { 113, 112, 112,}, 0.2f);
        ItemDropInfo dropSetRare10 = new ItemDropInfo(new int[] { 103, 107 }, 0.1f);
        ItemDropInfo dropSetExtend20 = new ItemDropInfo(new int[] { 115, 116 }, 0.2f);

        public Dungeon SetupTutorialData()
        {
            Level level1 = new Level();
            Level level2 = new Level();
            Level level3 = new Level();

            //            이름,번호, 함정, 괴물, 괴물, 도둑, 보석, 아이템 드랍, 다음레벨, 클리어, 오픈, 쇼타일
            string chamberString = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER);
            level1.setup(chamberString + 1, 1, 27, 0, 0, 0, 0, 2, new ItemDropInfo[] { dropSetRare10 }, new int[] { 2 }, false, false, 7); // boardmanager에 하드코딩되어 있음
            level2.setup(chamberString + 2, 2, 9, 0, 1, 0, 0, 2, new ItemDropInfo[] { dropSetDestroy50, dropSetShow50 }, new int[] { 3 }, false, true, 7); 
            level3.setup(chamberString + 3, 3, 9, 0, 2, 0, 0, 0, new ItemDropInfo[] { dropSetHeal50 }, new int[] { }, false, true, 7);

            Level[] levels = { level1, level2, level3};

            string dName = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.TUTORIAL);
            Dungeon dungeonInfo = new Dungeon(dName, 0, 1, levels, 300, true, false, 0);

            return dungeonInfo;
        }

        public Dungeon SetupDungeon1Data()
        {
            Level level1 = new Level();
            Level level2 = new Level();
            Level level4 = new Level();
            Level level5 = new Level();

            string chamberString = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER);
            //            이름,번호, 함정, 괴물, 괴물, 도둑, 보석, 아이템 드랍, 다음레벨, 클리어, 오픈, 쇼타일
            level1.setup(chamberString+1, 1, 12, 0, 0, 0, 0, 1, new ItemDropInfo[] { dropSetHeal50 }, new int[] { 2, 4 }, false, false, 6);
            level2.setup(chamberString+2, 2, 14, 0, 0, 0, 0, 2, new ItemDropInfo[] { dropSetShow50 }, new int[] { 5 }, false, true, 5);
            level4.setup(chamberString+3, 4, 14, 0, 1, 0, 0, 3, new ItemDropInfo[] { dropSetDestroy50 }, new int[] { 5 }, false, true, 5);
            level5.setup(chamberString+4, 5, 14, 0, 2, 0, 0, 0, new ItemDropInfo[] { }, new int[] {}, false, true, 5);

            Level[] levels = { level1, level2, level4, level5 };

            string dName = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.RUIN);
            Dungeon dungeonInfo = new Dungeon(dName, 1, 2, levels, 60, false, false, 1);

            return dungeonInfo;           
        }

        public Dungeon SetupDungeon2Data()
        {
            Level level1 = new Level();
            Level level2 = new Level();
            Level level3 = new Level();
            Level level4 = new Level();
            Level level5 = new Level();
            Level level6 = new Level();

            string chamberString = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER);
            //            이름,번호, 함정, 괴물, 괴물, 도둑, 보석, 아이템 드랍, 다음레벨, 클리어, 오픈, 쇼타일
            level1.setup(chamberString+1, 1, 14, 0, 0, 0, 0, 2,  new ItemDropInfo[] { dropSetHeal50,  }, new int[] { 2, 4 }, false, false, 6);
            level2.setup(chamberString+2, 2, 14, 2, 0, 0, 0, 2,     new ItemDropInfo[] { dropSetShow50,  }, new int[] { 3,5 }, false, true, 6);
            level4.setup(chamberString+3, 4, 14, 2, 1, 0, 0, 2,     new ItemDropInfo[] { dropSetDestroy50,  }, new int[] { 5 }, false, true, 6);
            level3.setup(chamberString+4, 3, 16, 2, 1, 1, 0, 3,      new ItemDropInfo[] { dropSetShow50, dropSetRare10, dropSetDestroy10 }, new int[] { 6 }, false, true, 5);
            level5.setup(chamberString+5, 5, 16, 2, 2, 0, 0, 3,      new ItemDropInfo[] { dropSetDestroy50, dropSetRare10, dropSetShow10 }, new int[] { 6 }, false, true, 5);
            level6.setup(chamberString+6, 6, 16, 4, 2, 1, 0, 0,   new ItemDropInfo[] { dropSetHeal50, dropSetTime20 }, new int[] { }, false, true, 5);

            Level[] levels = { level1, level2, level4, level3, level5, level6 };

            string dName = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.TOMB);
            Dungeon dungeonInfo = new Dungeon(dName, 2, 3, levels, 90, false, false, 1);

            return dungeonInfo;
        }

        public Dungeon SetupDungeon3Data()
        {
            Level level1 = new Level();
            Level level2 = new Level();
            Level level4 = new Level();
            Level level5 = new Level();
            Level level6 = new Level();
            Level level8 = new Level();
            Level level9 = new Level();

            string chamberString = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.CHAMBER);
            //            이름,번호, 함정, 괴물, 괴물, 도둑, 보석, 아이템 드랍, 다음레벨, 클리어, 오픈, 쇼타일
            level1.setup(chamberString+1, 1, 16, 0, 0, 0, 0, 2, new ItemDropInfo[] { dropSetHeal50,  }, new int[] { 2, 4 }, false, false, 5);
            level2.setup(chamberString+2, 2, 16, 2, 0, 0, 0, 2, new ItemDropInfo[] { dropSetDestroy10,  }, new int[] { 5 }, false, true, 5);
            level4.setup(chamberString+3, 4, 16, 2, 1, 0, 0, 2, new ItemDropInfo[] { dropSetDestroy10,  }, new int[] { 5 }, false, true, 5);
            level5.setup(chamberString+4, 5, 16, 4, 1, 1, 0, 3, new ItemDropInfo[] { dropSetDestroy50, dropSetHeal10, dropSetShow10, dropSetTime20 }, new int[] { 6,8 }, false, true, 5);
            level6.setup(chamberString+5, 6, 16, 4, 1, 1, 0, 2, new ItemDropInfo[] { dropSetRare10, dropSetTime20 }, new int[] { 9 }, false, true, 5);
            level8.setup(chamberString+6, 8, 16, 4, 2, 0, 0, 2, new ItemDropInfo[] { dropSetRare10, dropSetTime20 }, new int[] { 9 }, false, true, 5);
            level9.setup(chamberString+7, 9, 12, 6, 2, 2, 0, 0, new ItemDropInfo[] { dropSetShow50, dropSetHeal50 }, new int[] { }, false, true, 5);

            Level[] levels = { level1, level2, level4, level5, level6, level8, level9 };

            string dName = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.MAZE);
            Dungeon dungeonInfo = new Dungeon(dName, 3, 4, levels, 120, false, false, 2);

            return dungeonInfo;
        }

        public Dungeon SetupDungeonFinalData()
        {
            Level level1 = new Level();
            Level level2 = new Level();
            Level level3 = new Level();
            Level level4 = new Level();
            Level level5 = new Level();
            Level level6 = new Level();
            Level level7 = new Level();
            Level level8 = new Level();
            Level level9 = new Level();

            Level level10 = new Level();
            Level level11 = new Level();
            Level level12 = new Level();
            Level level13 = new Level();
            Level level14 = new Level();
            Level level15 = new Level();
            Level level16 = new Level();
            
            // 13 14 15 16
            // 9  10 11 12
            // 5  6  7  8 
            // 1  2  3  4

            //            이름,번호, 함정, 괴물, 괴물, 도둑, 보석, 아이템 드랍, 다음레벨, 클리어, 오픈, 쇼타일
            level1.setup("1", 1, 16, 0, 0, 0, 0, 2, new ItemDropInfo[] { dropSetExtend20 }, new int[] { 2, 5 }, false, false, 5);
            level2.setup("2", 2, 16, 2, 0, 0, 0, 2, new ItemDropInfo[] { dropSetExtend20 }, new int[] { 3, 6 }, false, true, 5);
            level5.setup("3", 5, 16, 2, 1, 0, 0, 2, new ItemDropInfo[] { dropSetExtend20 }, new int[] { 6, 9 }, false, true, 5);

            level3.setup("4", 3, 16, 2, 1, 1, 0, 2, new ItemDropInfo[] { dropSetHeal50, dropSetDestroy10 }, new int[] { 7, 4 }, false, true, 5);
            level6.setup("5", 6, 16, 2, 1, 1, 0, 2, new ItemDropInfo[] { dropSetHeal50, dropSetDestroy10 }, new int[] { 7, 10 }, false, true, 5);
            level9.setup("6", 9, 16, 2, 1, 1, 0, 2, new ItemDropInfo[] { dropSetHeal50, dropSetDestroy10 }, new int[] { 10, 13 }, false, true, 5);

            level4.setup("7", 4, 18, 2, 1, 1, 0, 2, new ItemDropInfo[] { dropSetRare10, dropSetShow50, dropSetDestroy10}, new int[] { 3,8 }, false, true, 5);
            level7.setup("8", 7, 16, 4, 2, 0, 0, 2, new ItemDropInfo[] { dropSetRare10, dropSetShow10, dropSetDestroy50 }, new int[] { 3,8,11 }, false, true, 5);
            level10.setup("9", 10, 18, 2, 1, 1, 0, 2, new ItemDropInfo[] { dropSetRare10, dropSetShow50, dropSetDestroy10 }, new int[] { 9,14,11}, false, true, 5);
            level13.setup("10", 13, 16, 4, 2, 0, 0, 2, new ItemDropInfo[] { dropSetRare10, dropSetShow10, dropSetDestroy50 }, new int[] { 9,14}, false, true, 5);

            level8.setup("11", 8, 18, 2, 1, 2, 0, 4, new ItemDropInfo[] { dropSetTime20, dropSetExtend20 }, new int[] { 4,7,12 }, false, false, 6);
            level11.setup("12", 11, 16, 4, 2, 1, 0, 4, new ItemDropInfo[] { dropSetTime20, dropSetExtend20 }, new int[] { 7,10,12,15 }, false, true, 6);
            level14.setup("13", 14, 14, 6, 3, 0, 0, 4, new ItemDropInfo[] { dropSetTime20, dropSetExtend20 }, new int[] { 10,13,15 }, false, true, 6);

            level12.setup("14", 12, 14, 6, 2, 2, 0, 4, new ItemDropInfo[] { dropSetRare10 }, new int[] { 8,11,16 }, false, true, 5);
            level15.setup("15", 15, 14, 6, 2, 2, 0, 4, new ItemDropInfo[] { dropSetRare10 }, new int[] { 11,14,16 }, false, true, 5);
            level16.setup("16", 16, 10, 10, 2, 3, 0, 0, new ItemDropInfo[] { }, new int[] {  }, false, true, 5);

            Level[] levels = { level1, level2, level3, level4, level5, level6, level7, level8, level9, level10, level11, level12, level13, level14, level15, level16 };

            string dName = LocalizationManager.instance.GetDungeonString(DUNGEON_STRING.HELLGATE);
            Dungeon dungeonInfo = new Dungeon(dName, 4, 5, levels, 150, false, true, 2);

            return dungeonInfo;
        }
    }
}
