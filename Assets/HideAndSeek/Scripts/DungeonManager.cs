using UnityEngine;
using System.Collections.Generic;
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

        public void setup(string _name, int _id, int _trap, int _enemy, int _strongEnemy, int _thief, int _gem,
                            ItemDropInfo[] _itemDropInfos, int[] _nextIds, bool _clear, bool _close, ShowTile[] _showTiles)
        {
            name = _name;
            id = _id;
            trap = _trap;
            enemy = _enemy;
            strongEnemy = _strongEnemy;
            thief = _thief ;
            gem = _gem;
            itemDropInfos = _itemDropInfos;
            nextIds = _nextIds;
            clear = _clear;
            close = _close;
            showTiles = _showTiles;
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

    public class DungeonData
    {
        ShowTile[] tileN3T1;
        ShowTile[] tileN3A1;
        ShowTile[] tileN3E1;
        ShowTile[] tileN3T1E1;        
        ShowTile[] tileN4T1E1A;
        ShowTile[] tileN4T1E1B;
        ShowTile[] tileN5T1E1;

        ItemDropInfo dropSetShow10 = new ItemDropInfo(new int[] { 104, 105, 106, 107 }, 0.1f);
        ItemDropInfo dropSetDestroy10 = new ItemDropInfo(new int[] { 109, 110, 111, 114 }, 0.1f);
        ItemDropInfo dropSetHeal10 = new ItemDropInfo(new int[] { 102, 102, 103 }, 0.1f);

        ItemDropInfo dropSetShow50 = new ItemDropInfo(new int[] { 104, 105, 106 }, 0.5f);
        ItemDropInfo dropSetDestroy50 = new ItemDropInfo(new int[] { 109, 110, 111, 114 }, 0.5f);
        ItemDropInfo dropSetHeal50 = new ItemDropInfo(new int[] { 101, 101, 101, 101, 102 }, 0.5f);
        ItemDropInfo dropSetTime20 = new ItemDropInfo(new int[] { 113, 112, 112,}, 0.2f);
        ItemDropInfo dropSetRare10 = new ItemDropInfo(new int[] { 103, 107 }, 0.1f);
        ItemDropInfo dropSetExtend20 = new ItemDropInfo(new int[] { 115, 116 }, 0.2f);

        public Dungeon SetupDungeon1Data()
        {
            Level level1 = new Level();
            Level level2 = new Level();
            Level level4 = new Level();
            Level level5 = new Level();
            //            이름,번호, 함정, 괴물, 괴물, 도둑, 보석, 아이템 드랍, 다음레벨, 클리어, 오픈, 쇼타일
            level1.setup("시작방", 1, 10, 1, 0, 0, 1, new ItemDropInfo[] { dropSetRare10 }, new int[] { 2, 4 }, false, false, tileN5T1E1);
            level2.setup("방1", 2, 10, 2, 0, 0, 2, new ItemDropInfo[] { dropSetShow50 }, new int[] { 5 }, false, true, tileN4T1E1A);
            level4.setup("방2", 4, 10, 2, 0, 0, 2, new ItemDropInfo[] { dropSetDestroy50 }, new int[] { 5 }, false, true, tileN4T1E1A);
            level5.setup("최종방", 5, 12, 3, 0, 0, 0, new ItemDropInfo[] { dropSetHeal50 }, new int[] {}, false, true, tileN4T1E1B);

            Level[] levels = { level1, level2, level4, level5 };

            Dungeon dungeonInfo = new Dungeon("고대 유적지", 1, 2, levels, 60, false, false, 1);

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

            //            이름,번호, 함정, 괴물, 괴물, 도둑, 보석, 아이템 드랍, 다음레벨, 클리어, 오픈, 쇼타일
            level1.setup( "시작방", 1, 12, 1, 0, 0, 2,  new ItemDropInfo[] { dropSetHeal50,  }, new int[] { 2, 4 }, false, false, tileN4T1E1A);
            level2.setup( "방1", 2, 14, 2, 0, 0, 3,     new ItemDropInfo[] { dropSetShow50, dropSetExtend20 }, new int[] { 3,5 }, false, true, tileN4T1E1A );
            level4.setup( "방2", 4, 14, 2, 0, 0, 3,     new ItemDropInfo[] { dropSetDestroy50, dropSetExtend20 }, new int[] { 5 }, false, true, tileN4T1E1A );
            level3.setup( "방3", 3, 14, 2, 1, 0, 3,      new ItemDropInfo[] { dropSetShow50, dropSetRare10 }, new int[] { 6 }, false, true, tileN4T1E1B);
            level5.setup( "방4", 5, 14, 3, 0, 0, 3,      new ItemDropInfo[] { dropSetDestroy50, dropSetRare10 }, new int[] { 6 }, false, true, tileN4T1E1B);
            level6.setup( "최종방", 6, 16, 3, 1, 0, 0,   new ItemDropInfo[] { dropSetHeal50 }, new int[] { }, false, true, tileN3T1E1);

            Level[] levels = { level1, level2, level4, level3, level5, level6 };

            Dungeon dungeonInfo = new Dungeon("왕의 무덤", 2, 3, levels, 90, false, false, 1);

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
            //            이름,번호, 함정, 괴물, 괴물, 도둑, 보석, 아이템 드랍, 다음레벨, 클리어, 오픈, 쇼타일
            level1.setup("시작방", 1, 14, 2, 0, 0, 2, new ItemDropInfo[] { dropSetHeal50, dropSetShow50, dropSetExtend20 }, new int[] { 2, 4 }, false, false, tileN4T1E1A);
            level2.setup("방1", 2, 15, 3, 0, 0, 3, new ItemDropInfo[] { dropSetShow50, dropSetRare10, dropSetExtend20 }, new int[] { 5 }, false, true, tileN4T1E1A);
            level4.setup("방2", 4, 15, 3, 0, 0, 3, new ItemDropInfo[] { dropSetDestroy50, dropSetRare10, dropSetExtend20 }, new int[] { 5 }, false, true, tileN4T1E1A);
            level5.setup("중간방", 5, 15, 2, 1, 0, 4, new ItemDropInfo[] { dropSetDestroy50, dropSetHeal10}, new int[] { 6,8 }, false, true, tileN3T1);
            level6.setup("방4", 6, 15, 3, 1, 0, 4, new ItemDropInfo[] { dropSetHeal50, dropSetRare10 }, new int[] { 9 }, false, true, tileN3T1);
            level8.setup("방3", 8, 15, 2, 2, 0, 4, new ItemDropInfo[] { dropSetShow50, dropSetRare10 }, new int[] { 9 }, false, true, tileN3A1);
            level9.setup("최종방", 9, 16, 3, 2, 0, 0, new ItemDropInfo[] { dropSetShow50, dropSetHeal10 }, new int[] { }, false, true, tileN3E1);

            Level[] levels = { level1, level2, level4, level5, level6, level8, level9 };

            Dungeon dungeonInfo = new Dungeon("신비한 숲", 3, 4, levels, 120, false, false, 2);

            return dungeonInfo;
        }        
        
        public void GenerateShowTileSet()
        {
            tileN3T1 = new ShowTile[]{
                    new ShowTile(new Vector3(0, 0, 0), SHOW_TYPE.NEAR),
                    new ShowTile(new Vector3(5, 3, 0), SHOW_TYPE.NEAR),
                    new ShowTile(new Vector3(2, 4, 0), SHOW_TYPE.NEAR),
                    new ShowTile(new Vector3(0, 7, 0), SHOW_TYPE.TRAP)
            };

            tileN3A1 = new ShowTile[]{
                    new ShowTile(new Vector3(0, 0, 0), SHOW_TYPE.NEAR),
                    new ShowTile(new Vector3(5, 3, 0), SHOW_TYPE.NEAR),
                    new ShowTile(new Vector3(2, 4, 0), SHOW_TYPE.NEAR),
                    new ShowTile(new Vector3(0, 7, 0), SHOW_TYPE.ALL)
            };

            tileN3E1 = new ShowTile[]{
                    new ShowTile(new Vector3(0, 0, 0), SHOW_TYPE.NEAR),
                    new ShowTile(new Vector3(5, 3, 0), SHOW_TYPE.NEAR),
                    new ShowTile(new Vector3(2, 4, 0), SHOW_TYPE.NEAR),
                    new ShowTile(new Vector3(0, 7, 0), SHOW_TYPE.MONSTER)
            };

            tileN3T1E1 = new ShowTile[]{
                        new ShowTile(new Vector3(0, 0, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(5, 3, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(2, 4, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(7, 0, 0), SHOW_TYPE.MONSTER),
                        new ShowTile(new Vector3(0, 7, 0), SHOW_TYPE.TRAP)
                    };


            tileN4T1E1A = new ShowTile[]{
                        new ShowTile(new Vector3(0, 0, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(2, 2, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(5, 2, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(2, 5, 0), SHOW_TYPE.NEAR),                        
                        new ShowTile(new Vector3(7, 0, 0), SHOW_TYPE.MONSTER),
                        new ShowTile(new Vector3(0, 7, 0), SHOW_TYPE.TRAP)
                };

            tileN4T1E1B = new ShowTile[]{
                        new ShowTile(new Vector3(0, 0, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(5, 5, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(5, 2, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(2, 5, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(7, 0, 0), SHOW_TYPE.MONSTER),
                        new ShowTile(new Vector3(0, 7, 0), SHOW_TYPE.TRAP)
                };

            tileN5T1E1 = new ShowTile[]{
                        new ShowTile(new Vector3(0, 0, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(2, 2, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(5, 2, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(2, 5, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(5, 5, 0), SHOW_TYPE.NEAR),
                        new ShowTile(new Vector3(7, 0, 0), SHOW_TYPE.MONSTER),
                        new ShowTile(new Vector3(0, 7, 0), SHOW_TYPE.TRAP)
                    };

        }
    }
}
