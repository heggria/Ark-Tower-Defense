using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapData
{
  public string mapId;
  public int width;
  public int height;
  public MapOptions options;
  public List<MapTile> mapTileDatas;//地图数据
  public List<CharcterData> enemyDatas;//所有出现在本关卡的敌人数据
  //todo
  public List<Route> routeDatas;//敌人路径数据
  public List<Wave> waveDatas;//敌人波次数据
  //public List<Buff> globalBuffs;
  public MapData(string _mapId, int _width, int _height, MapOptions _options, List<MapTile> _mapTileDatas, List<CharcterData> _enemyDatas, List<Route> _routeDatas, List<Wave> _waveDatas)
  {
    this.mapId = _mapId;
    this.width = _width;
    this.height = _height;
    this.options = _options;
    this.mapTileDatas = _mapTileDatas;
    this.enemyDatas = _enemyDatas;
    this.routeDatas = _routeDatas;
    this.waveDatas = _waveDatas;
  }
}
public class MapOptions
{
  //public int characterLimit;
  public int lifePoint = 10;
  public int nowCost = 1000;
  public int countEnemyAlive = 0;
  //public int maxCost;
  public float costIncreaseTime = 1;// cost增加1的时间
  public float moveMultiplier = 0.5f;// 控制移动速度
  public MapOptions(int _lifePoint, int _nowCost)
  {
    this.lifePoint = _lifePoint;
    this.nowCost = _nowCost;
  }
}
