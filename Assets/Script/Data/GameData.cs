using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
  public MapData mapData;
  public List<CharcterData> optDatas;
  public List<CharcterData> enemyDatas;
  public List<CharcterData> optRuningDatas;
  public List<CharcterData> enemyRuningDatas;
  public GameData(GameDataType type)
  {
    if (type == GameDataType.TEST)
    {
      this.optDatas = TestOpt();

      mapData = new MapData("test", 8, 8, new MapOptions(10, 50), TestMapTile(), TestEnemy(), TestRoute(), TestWave());

    }
  }
  private bool IsRoad(Vector2 pos)
  {
    List<Vector2> road = new List<Vector2>(new Vector2[] {
        new Vector2(1,2),
        new Vector2(1,3),
        new Vector2(1,4),
        new Vector2(2,4),
        new Vector2(3,4),
        new Vector2(3,3),
        new Vector2(3,2),
        new Vector2(3,1),
        new Vector2(4,1),
        new Vector2(5,1),
        new Vector2(6,1),
        new Vector2(6,2),
        new Vector2(6,3),
        new Vector2(6,4),
        new Vector2(6,5),
        new Vector2(6,6),
        new Vector2(5,6),
        new Vector2(4,6),
        new Vector2(3,6),
        new Vector2(2,6),
      });
    foreach (Vector2 roadPos in road)
      if (pos == roadPos)
        return true;
    return false;
  }
  private List<MapTile> TestMapTile()
  {
    List<MapTile> mT = new List<MapTile>();
    for (int x = 0; x < 8; x++)
    {
      for (int y = 0; y < 8; y++)
      {
        //Debug.Log(this.IsRoad(new Vector2(x, y)));
        if (x == 1 && y == 1)
          mT.Add(new MapTile(x, y, TileType.Start));
        else if (x == 1 && y == 6)
          mT.Add(new MapTile(x, y, TileType.End));
        else if (this.IsRoad(new Vector2(x, y)))
          mT.Add(new MapTile(x, y, TileType.Road));
        else
          mT.Add(new MapTile(x, y, TileType.Hill));
      }
    }
    return mT;
  }
  private List<CharcterData> TestEnemy()
  {
    List<CharcterData> eD = new List<CharcterData>(new CharcterData[]{
      new CharcterData(0,"Enemy1",true,new Attributes(10000,200,0))
    });

    eD[0].attributes.moveSpeed = 0.5f;
    eD[0].attributes.def = 600;
    eD[0].attributes.range = 10;

    eD[0].perfabSetting.canMove = true;
    eD[0].perfabSetting.hasBulletEffect = false;
    eD[0].perfabSetting.canAtk = false;
    eD[0].perfabSetting.ballisticSpeed = 10;
    return eD;
  }
  private List<CharcterData> TestOpt()
  {
    List<CharcterData> oD = new List<CharcterData>(new CharcterData[]{
      new CharcterData(0,"StandardTurret",false,new Attributes(2000,500,20)),
      new CharcterData(1,"Box",false,new Attributes(2000,0,0))
    });
    oD[0].attributes.cost = 8;
    oD[0].attributes.baseSearchTime = 0;
    oD[0].attributes.baseAttackTime = 0.5f;
    oD[0].attributes.baseAttackForwardTime = 0.5f;
    oD[0].damageType = DamageType.Magic;
    oD[0].attributes.maxBlockCnt = 2;
    oD[0].attributes.attackNum = 1;

    oD[0].perfabSetting.canAtk = true;
    oD[0].perfabSetting.ballisticSpeed = 30;
    oD[0].perfabSetting.hasHead = true;
    return oD;

  }
  private List<Route> TestRoute()
  {
    List<Route> routeDatas = new List<Route>(new Route[]{
        new Route(0,new Vector2(1,1),new Vector2(1,6),new List<Checkpoint>(new Checkpoint[]{
          new Checkpoint(0,new Vector2(1,4)),
          new Checkpoint(0,new Vector2(3,4)),
          new Checkpoint(0,new Vector2(3,1)),
          new Checkpoint(0,new Vector2(6,1)),
          new Checkpoint(0,new Vector2(6,6)),
        }))
      });
    return routeDatas;
  }
  private List<Wave> TestWave()
  {
    List<Wave> waves = new List<Wave>(new Wave[]{
      new Wave("FIRST",0,5,new List<EnemyFragment>(new EnemyFragment[]{
        new EnemyFragment(0,new List<EnemyAction>(new EnemyAction[]{
          new EnemyAction("Enemy1",0,20,2.5f,0),
          //new EnemyAction("Enemy1",1.5f,20,4,0),
          //new EnemyAction("Enemy1",3f,20,2.5f,0),
          //new EnemyAction("Enemy1",2.5f,20,3.5f,0),
        })),
      })),/*
      new Wave("FIRST",0,30,new List<EnemyFragment>(new EnemyFragment[]{
        new EnemyFragment(0,new List<EnemyAction>(new EnemyAction[]{
          new EnemyAction("Enemy1",0,100,2,0),
        })),
      }))*/
    });
    return waves;
  }
  public Route findRouteById(int id)
  {
    foreach (Route route in mapData.routeDatas)
    {
      if (route.id == id) return route;
    }
    return null;
  }
  public CharcterData findEnemyByKey(string key)
  {
    foreach (CharcterData enemyData in mapData.enemyDatas)
    {
      if (enemyData.key == key) return enemyData;
    }
    return null;
  }
}
public enum GameDataType
{
  TEST,
  NORMAL
}
