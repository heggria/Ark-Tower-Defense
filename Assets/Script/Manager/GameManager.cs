using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  [HideInInspector]
  public static float gameStartTime;
  [HideInInspector]
  public static int nowLifePoint;
  [HideInInspector]
  private static GameData gameData;
  public static MapOptions options;
  public static float space = 5.1f;//间隔
  public static int enemyHeight = 1;
  public static int optHeight = 2;

  private float waitingForNextWaveTime;
  public GameObject roadTile;
  public GameObject wallTile;
  public GameObject startTile;
  public GameObject endTile;
  public GameObject enemy1;
  public GameObject standardOptPrefab;

  private CharcterData selectOptData;//当前选择的未部署的
  private CharcterData selectDeployedTurret;//当前选择的已部署的
  //public ToggleGroup selectTurret;
  private OptControl optControl;
  void ChangeCost(int change = 0)
  {
    options.nowCost += change;
  }
  void Start()
  {
    // 初始化游戏数据类
    gameData = new GameData(GameDataType.TEST);
    BuildMap();
    gameStartTime = Time.time;
    StartCoroutine(SpawnEnemy());
    options = gameData.mapData.options;
    optControl = GameObject.Find("OptControl").transform.Find("OperationCanvas").gameObject.GetComponent<OptControl>();
  }
  void Update()
  {
    BuildOpt();
  }
  private void BuildMap()
  {
    foreach (MapTile mapTileData in gameData.mapData.mapTileDatas)
    {
      Vector3 position = new Vector3(mapTileData.position.x * space, 0, mapTileData.position.y * space);
      if (mapTileData.tileType == TileType.Road)
        GameObject.Instantiate(roadTile, position, Quaternion.identity);
      else if (mapTileData.tileType == TileType.Wall)
        GameObject.Instantiate(wallTile, position, Quaternion.identity);
      else if (mapTileData.tileType == TileType.Start)
        GameObject.Instantiate(startTile, position, Quaternion.identity);
      else if (mapTileData.tileType == TileType.End)
        GameObject.Instantiate(endTile, position, Quaternion.identity);
    }
  }
  IEnumerator SpawnEnemy()
  {
    foreach (Wave wave in gameData.mapData.waveDatas)
    {
      waitingForNextWaveTime = wave.maxTimeWaitingForNextWave;
      yield return new WaitForSeconds(wave.preDelay);
      foreach (EnemyFragment enemyFragment in wave.enemyFragments)
      {
        yield return new WaitForSeconds(enemyFragment.preDelay);
        foreach (EnemyAction enemyAction in enemyFragment.enemyActions)
        {
          yield return new WaitForSeconds(enemyAction.preDelay);
          Route route = gameData.findRouteById(enemyAction.routeId);
          CharcterData enemyData = gameData.findEnemyByKey(enemyAction.enemyKey);
          //Debug.Log(enemyFragment.enemyActions.Count);
          for (int i = 0; i < enemyAction.count; i++)
          {
            if (route != null && enemyData != null)
            {
              float waitTime = i * enemyAction.interval;
              GameObject enemy = GameObject.Instantiate(enemy1, new Vector3(route.startPosition.x * space, 1, route.startPosition.y * space), Quaternion.identity);
              object[] message = new object[]{
                enemyData,
                route,
                waitTime
              };
              enemy.BroadcastMessage("SetEnemy", message);
              options.countEnemyAlive++;

            }
          }
        }
      }
      while (options.countEnemyAlive > 0)
      {
        if ((waitingForNextWaveTime -= Time.deltaTime) < 0)
        {
          waitingForNextWaveTime = 0;
          break;
        }
        else
        {
          yield return 0;
        }
      }
    }
  }
  void BuildOpt()
  {
    if (Input.GetMouseButtonDown(0))
    {
      if (!EventSystem.current.IsPointerOverGameObject())
      {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isCollider = Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("MapCube"));
        //Debug.Log(isCollider);
        if (isCollider)
        {
          MapCubeControl mapCubeControl = hit.collider.GetComponent<MapCubeControl>();
          if (selectOptData != null && mapCubeControl.deployedOptData == null)
          {
            //判断cost够不够
            if (GameManager.options.nowCost >= selectOptData.attributes.cost)
            {
              Debug.Log(selectOptData.attributes.cost);
              ChangeCost(-selectOptData.attributes.cost);
              mapCubeControl.OptSet(standardOptPrefab, selectOptData);
            }
          }
          else
          {//upgrade

          }
        }
      }
    }
  }
  public void OnStandardOptSelected(bool isOn)
  {
    if (isOn)
      selectOptData = gameData.optDatas[0];
    else
      selectOptData = null;
  }
}
