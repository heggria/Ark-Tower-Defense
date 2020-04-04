
using UnityEngine;
public class MapTile
{
  public TileType tileType;//类型
  public int heightType;//高度类型
  public bool isBuildable;//可建造
  public int passableMask;//是否可穿过
  public Vector2 position;
  public MapTile(int _x, int _y, TileType _tileType)
  {
    this.position.x = _x;
    this.position.y = _y;
    this.tileType = _tileType;
    switch (_tileType)
    {
      case TileType.Start:
      case TileType.End:
        this.heightType = 1;
        this.isBuildable = false;
        this.passableMask = 3;
        break;
      case TileType.Forbidden:
        this.heightType = 1;
        this.isBuildable = false;
        this.passableMask = 2;
        break;
      case TileType.Floor:
        this.heightType = 0;
        this.isBuildable = true;
        this.passableMask = 3;
        break;
      case TileType.Road:
        this.heightType = 0;
        this.isBuildable = false;
        this.passableMask = 3;
        break;
      case TileType.Hill:
        this.heightType = 1;
        this.isBuildable = true;
        this.passableMask = 2;
        break;
    }
  }
}
public enum TileType
{
  Forbidden,//格子禁用
  Start,//红门
  Floor,//能部署的地面
  Road,//不能部署的地面
  Hill,//高台
  End,
  //Flystart,
  //TODO
}
