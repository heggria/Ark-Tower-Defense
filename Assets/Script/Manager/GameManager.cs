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
  public static CharcterData selectOptData;//当前选择的未部署的
  private CharcterData selectDeployedTurret;//当前选择的已部署的
  [HideInInspector]
  public static int nowLifePoint;
  [HideInInspector]
  private static GameData gameData;
  public static MapOptions options;
  public static float space = 5.1f;//间隔
  public static int enemyHeight = 2;
  public static int optHeight = 2;

  private float waitingForNextWaveTime;
  private GameObject roadTile;
  private GameObject hillTile;
  private GameObject startTile;
  private GameObject endTile;


  private GameObject charConstructor;
  private List<GameObject> enemy = new List<GameObject>();
  private List<GameObject> opt = new List<GameObject>();

  //public ToggleGroup selectTurret;
  private OptControl optControl;
  private float costIncreaseTimer = 0;
  void ChangeCost(int change = 0)
  {
    options.nowCost += change;
  }
  void Start()
  {
    // 初始化游戏数据类
    gameData = new GameData(GameDataType.TEST);
    LoadResources();
    BuildMap();
    // 提出 options 指针
    options = gameData.mapData.options;
    // 游戏计时开始
    gameStartTime = Time.time;
    StartCoroutine(SpawnEnemy());
    // optControl = GameObject.Find("OptControl").transform.Find("OperationCanvas").gameObject.GetComponent<OptControl>();
  }
  // 加载 Resources 资源，临时
  void LoadResources()
  {
    roadTile = Resources.Load<GameObject>("Perfab/Tile/Road/RoadCube");
    hillTile = Resources.Load<GameObject>("Perfab/Tile/Hill/HillCube");
    endTile = Resources.Load<GameObject>("Perfab/Tile/EndPoint/EndPoint");
    startTile = Resources.Load<GameObject>("Perfab/Tile/StartPoint/StartPoint");

    charConstructor = Resources.Load<GameObject>("Perfab/CharConstructor/CharConstructor");

    enemy.Add(Resources.Load<GameObject>("Perfab/Enemy1/Enemy1"));
    opt.Add(Resources.Load<GameObject>("Perfab/StandardTurret/StandardTurret"));
    opt.Add(Resources.Load<GameObject>("Perfab/char_box_000/char_box_000"));

  }
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
      if (!EventSystem.current.IsPointerOverGameObject())
        BuildOpt();
    UpdateCost();
  }
  private void UpdateCost()
  {
    if ((costIncreaseTimer += Time.deltaTime) >= options.costIncreaseTime)
    {
      IncreaseCost(1);
      costIncreaseTimer = 0;
    }
  }
  public static void IncreaseCost(int cost)
  {
    if (options.nowCost + cost <= options.maxCost)
      options.nowCost += cost;
    else
      options.nowCost = options.maxCost;
  }
  private void BuildMap()
  {
    foreach (MapTile mapTileData in gameData.mapData.mapTileDatas)
    {
      Vector3 position = new Vector3(mapTileData.position.x * space, 0, mapTileData.position.y * space);
      if (mapTileData.tileType == TileType.Road)
        GameObject.Instantiate(roadTile, position, Quaternion.identity);
      else if (mapTileData.tileType == TileType.Hill)
        GameObject.Instantiate(hillTile, position, Quaternion.identity);
      else if (mapTileData.tileType == TileType.Start)
        GameObject.Instantiate(startTile, position, Quaternion.identity);
      else if (mapTileData.tileType == TileType.End)
        GameObject.Instantiate(endTile, position, Quaternion.identity);
    }
  }
  IEnumerator SpawnEnemy()
  {
    if (Time.time - gameStartTime >= options.spawnBeginTime)
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
                GameObject charC = GameObject.Instantiate(charConstructor, new Vector3(route.startPosition.x * space, enemyHeight, route.startPosition.y * space), Quaternion.identity);
                GameObject enemyI = GameObject.Instantiate(enemy[0], charC.transform.position, Quaternion.identity);
                enemyI.transform.parent = charC.transform;
                object[] message = new object[]{
                  enemyI,
                  enemyData,
                  route,
                  waitTime
                };
                charC.BroadcastMessage("SetEnemy", message);
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
  private void BuildOpt()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    bool isCollider = Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("MapCube"));
    if (isCollider)
    {
      MapCubeControl mapCubeControl = hit.collider.GetComponent<MapCubeControl>();
      if (selectOptData != null && mapCubeControl.deployedOptData == null)
      {
        // Debug.Log(selectOptData);
        //判断cost够不够
        if (GameManager.options.nowCost >= selectOptData.attributes.cost && selectOptData.attributes.maxDeployCount > 0)
        {
          // Debug.Log(selectOptData.attributes.cost);
          ChangeCost(-selectOptData.attributes.cost);
          selectOptData.attributes.maxDeployCount--;
          // 放置
          mapCubeControl.OptSet(charConstructor, opt[0], selectOptData);
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
  public void OnBoxSelected(bool isOn)
  {
    if (isOn)
      selectOptData = gameData.optDatas[1];
    else
      selectOptData = null;
  }
}
