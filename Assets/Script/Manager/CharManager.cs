using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharManager : MonoBehaviour
{
  [HideInInspector]
  public CharcterData originalData;// 原生数据
  [HideInInspector]
  public CharcterData runningData;// 运行数据

  public bool hpSilderDisplay = true;

  public GameObject hpSilder;// 血条
  public Transform head;
  public Transform firePosition;
  public GameObject bulletPrefab;

  private RangeRunning range;
  private float attackTimer = 0;
  private float moveTimer = 0;
  private AttackStatus attackStatus = AttackStatus.STOP;
  private MoveStatus moveStatus = MoveStatus.WAIT;
  private MoveStatus enemyMoveStatus = MoveStatus.MOVE;

  public GameObject perfab;

  public List<GameObject> blockEnemies = new List<GameObject>();

  // 敌人专用
  [HideInInspector]
  private Route route;
  private Vector3 endPos3d;
  public float distanceToEnd = 0;
  private float reserve = 0.2f;//移动保留量
  private float magnification = 5f;//移动倍率
  private int nextPointIndex = 0;
  private float waitTime;

  void Awake()
  {
    gameObject.GetComponent<SphereCollider>().enabled = false;
  }
  void Update()
  {
    if (originalData != null)
    {
      if (originalData.attributes.canAtk)
        AttackStatusMachine();
      if (originalData.attributes.canMove)
        MoveStatusMachine();
      UpdateBlockEnemies();
    }
  }
  private void AttackStatusMachine()
  {
    range.SetRadius(runningData.attributes.range);
    // 如果范围内第一次出现敌人
    if (range.enemies.Count > 0)
    {
      if (range.enemies.Count > 0 && range.enemies[0].enemy != null)
      {
        Vector3 targetPosition = range.enemies[0].enemy.transform.position;
        targetPosition.y = head.position.y;
        head.LookAt(targetPosition);
      }
      attackTimer += Time.deltaTime;
      switch (attackStatus)
      {
        case AttackStatus.STOP:
          attackStatus = AttackStatus.SEARCH_ENEMY;
          break;
        case AttackStatus.SEARCH_ENEMY:
          if (attackTimer >= runningData.attributes.baseSearchTime)
          {
            attackTimer = 0;
            attackStatus = AttackStatus.BEFORE_ATTACK;
          }
          break;
        case AttackStatus.BEFORE_ATTACK:
          if (attackTimer >= runningData.attributes.baseAttackForwardTime)
          {
            attackTimer = 0;
            Attack();
            attackStatus = AttackStatus.AFTER_ATTACK;
          }
          break;
        case AttackStatus.AFTER_ATTACK:
          if (attackTimer >= runningData.attributes.baseAttackTime - runningData.attributes.baseAttackForwardTime)
          {
            attackTimer = 0;
            attackStatus = AttackStatus.BEFORE_ATTACK;
          }
          break;
      }
    }
    else
    {
      attackTimer = 0;
      attackStatus = AttackStatus.STOP;
    }
  }
  private void MoveStatusMachine()
  {
    moveTimer += Time.deltaTime;
    switch (moveStatus)
    {
      case MoveStatus.WAIT:
        {
          if (route == null) break;
          else if (moveTimer >= waitTime)
          {
            if (originalData.isEnemy)
              perfab.SetActive(true);

            moveTimer = 0;
            moveStatus = MoveStatus.MOVE;
            gameObject.GetComponent<SphereCollider>().enabled = true;
          }
        }
        break;
      case MoveStatus.MOVE:
        {
          if (nextPointIndex != route.checkpoints.Count)
          {
            Vector3 pos3d = Math.Ve2ToVe3(route.checkpoints[nextPointIndex].position * GameManager.space, GameManager.enemyHeight);
            if (Vector3.Distance(pos3d, transform.position) > reserve)
            {
              transform.Translate((pos3d - transform.position).normalized * Time.deltaTime * runningData.attributes.moveSpeed * magnification);
            }
            else if (enemyMoveStatus == MoveStatus.STOP)
            {
              moveStatus = MoveStatus.STOP;
            }
            else
            {
              moveTimer = 0;
              moveStatus = MoveStatus.STOP;
            }
          }
          else
          {
            if (Vector3.Distance(endPos3d, transform.position) > reserve)
            {
              transform.Translate((endPos3d - transform.position).normalized * Time.deltaTime * runningData.attributes.moveSpeed * magnification);
            }
            else
            {
              moveTimer = 0;
              moveStatus = MoveStatus.DEAD;
            }
          }
          break;
        }
      case MoveStatus.STOP:
        enemyMoveStatus = MoveStatus.STOP;
        if (moveTimer >= route.checkpoints[nextPointIndex].stopTime)
        {
          moveTimer = 0;
          nextPointIndex++;
          moveStatus = MoveStatus.MOVE;
          enemyMoveStatus = MoveStatus.MOVE;
        }
        break;
      case MoveStatus.BLOCK:
        if (!originalData.isEnemy)
        {
        }
        else
        {

        }
        break;
      case MoveStatus.DEAD:
        GameManager.options.lifePoint--;
        GameObject.Destroy(gameObject);
        break;
    }
    CountDistanceToEnd();
  }
  private void Attack()
  {
    int remainAtkNum = runningData.attributes.attackNum;
    for (int i = 0; i < blockEnemies.Count; i++)
    {
      if (remainAtkNum > 0)
      {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
        bullet.SetActive(true);
        object[] message = new object[] { runningData, blockEnemies[i] };
        bullet.SendMessage("InitBullet", message);
        remainAtkNum--;
      }
      else break;
    }
    if (remainAtkNum > 0)
    {
      // 攻击先进入攻击范围/攻击先上场 看情况
      int flag = 0;
      if (range.enemies.Count <= remainAtkNum)
        flag = range.enemies.Count;
      else
        flag = remainAtkNum;
      for (int i = 0; i < flag; i++)
      {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
        bullet.SetActive(true);
        object[] message = new object[] { runningData, range.enemies[i].enemy };
        bullet.SendMessage("InitBullet", message);
      }
    }
  }
  public void GetDamage(float damage, DamageType damageType)
  {
    // Debug.Log(1);
    if (runningData.attributes.maxHp <= 0) return;
    CountDamage(damage, damageType);
    SetHpSilder(runningData.attributes.maxHp / originalData.attributes.maxHp, true);
    if (runningData.attributes.maxHp <= 0)
      Die();
  }
  private void Die()
  {
    if (originalData.isEnemy)
    {
      GameManager.options.killEnemy++;
    }
    else
    {
      originalData.attributes.maxDeployCount++;
    }
    Destroy(this.gameObject);
  }
  void OnDestroy()
  {
    if (originalData.isEnemy)
      GameManager.options.countEnemyAlive--;
  }
  void SetOpt(object[] obj)
  {
    perfab = (GameObject)obj[0];
    originalData = (CharcterData)obj[1];
    runningData = Math.DeepCopyByBinary<CharcterData>(originalData);
    InitCharManager();
    foreach (Transform child in perfab.GetComponentsInChildren<Transform>(true))
    {
      if (child.gameObject.name == "FirePosition")
      {
        firePosition = child.gameObject.transform;
      }
      else if (child.gameObject.name == "Head")
      {
        head = child.gameObject.transform;
      }
      else if (child.gameObject.name == "Bullet")
      {
        bulletPrefab = child.gameObject;
      }
    }
    gameObject.GetComponent<SphereCollider>().enabled = true;
    gameObject.GetComponent<SphereCollider>().isTrigger = false;
    bulletPrefab.SetActive(false);
  }
  void CountDamage(float damage, DamageType damageType)
  {
    float finalDamage = 0;
    if (damageType == DamageType.Physical)
      finalDamage = damage - damage - runningData.attributes.def;
    else if (damageType == DamageType.Magic)
      finalDamage = damage * (100 - runningData.attributes.magicResistance) / 100;
    else finalDamage = damage;
    if (finalDamage < 0.05f * damage)
      finalDamage = 0.05f * damage;
    //Debug.Log(finalDamage);
    runningData.attributes.maxHp -= finalDamage;
  }

  // 敌人生命周期开始处
  void SetEnemy(object[] obj)
  {
    perfab = (GameObject)obj[0];
    originalData = (CharcterData)obj[1];
    route = (Route)obj[2];
    waitTime = (float)obj[3];
    InitCharManager();
    // 预先计算终点坐标，方便调用
    endPos3d = Math.Ve2ToVe3(route.endPosition * GameManager.space, GameManager.enemyHeight);
    // 敌人一开始是隐藏状态
    perfab.SetActive(false);
  }


  private void CountDistanceToEnd()
  {
    float temporary = 0;
    if (nextPointIndex <= route.checkpoints.Count - 1)
    {
      temporary += (transform.position
      - Math.Ve2ToVe3(route.checkpoints[nextPointIndex].position * GameManager.space, GameManager.enemyHeight)).sqrMagnitude;
      for (int i = nextPointIndex; i < route.checkpoints.Count - 1; i++)
      {
        temporary += (Math.Ve2ToVe3(route.checkpoints[i].position * GameManager.space, GameManager.enemyHeight)
         - Math.Ve2ToVe3(route.checkpoints[i + 1].position * GameManager.space, GameManager.enemyHeight)).sqrMagnitude;
      }
      temporary += (Math.Ve2ToVe3(route.checkpoints[route.checkpoints.Count - 1].position * GameManager.space, GameManager.enemyHeight)
           - Math.Ve2ToVe3(route.endPosition * GameManager.space, GameManager.enemyHeight)).sqrMagnitude;
    }
    else
      temporary += (transform.position
           - Math.Ve2ToVe3(route.endPosition * GameManager.space, GameManager.enemyHeight)).sqrMagnitude;
    distanceToEnd = temporary;
    //Debug.Log(distanceToEnd);
  }
  private void SetHpSilder(float hp, bool isActive)
  {
    hpSilder.GetComponent<Slider>().value = hp; ;
    hpSilder.SetActive(isActive);
  }
  private void InitCharManager()
  {
    // runningData需要深拷贝
    this.runningData = Math.DeepCopyByBinary<CharcterData>(this.originalData);
    // 获取攻击范围组件
    range = gameObject.GetComponentInChildren<RangeRunning>();
    // 血条组件已经初始化，可以直接调用
    SetHpSilder(runningData.attributes.maxHp / originalData.attributes.maxHp, true);
  }

  void OnTriggerEnter(Collider col)
  {
    if (col.tag == "Char")
    {
      // Debug.Log(originalData.isEnemy);
      if (col.GetComponent<CharManager>().originalData.isEnemy != originalData.isEnemy)
      {
        CharManager other = col.GetComponent<CharManager>();
        // 如果被触发者是敌人
        if (originalData.isEnemy)
        {
          // 敌人如果处于阻挡状态，直接跳过
          if (moveStatus == MoveStatus.BLOCK)
            return;
          // 如果不是阻挡状态，判断能不能阻挡，干员当前阻挡-敌人最大阻挡大于 0
          else if (other.runningData.attributes.supplyBlockCnt - runningData.attributes.maxBlockCnt >= 0)
          {
            moveStatus = MoveStatus.BLOCK;
            blockEnemies.Add(col.GetComponent<Transform>().gameObject);
            //Debug.Log(blockEnemies[0].GetComponent<CharManager>());
            // 敌人阻挡数不用变
          }
        }
        else
        {
          // 判断能不能阻挡，干员当前阻挡-敌人最大阻挡大于 0
          if (runningData.attributes.supplyBlockCnt - other.runningData.attributes.maxBlockCnt >= 0)
          {
            //Debug.Log(other.runningData.attributes.supplyBlockCnt);
            moveStatus = MoveStatus.BLOCK;
            blockEnemies.Add(col.GetComponent<Transform>().gameObject);
          }
          Debug.Log(blockEnemies.Count);
        }
      }
    }
  }
  private void UpdateBlockEnemies()
  {
    // 移除死亡敌人
    List<int> emptyIndex = new List<int>();
    for (int index = 0; index < blockEnemies.Count; index++)
      if (blockEnemies[index] == null)
        emptyIndex.Add(index);
    for (int i = 0; i < emptyIndex.Count; i++)
      blockEnemies.RemoveAt(emptyIndex[i] - i);
    if (!originalData.isEnemy)
    {
      if (blockEnemies.Count == 0)
      {
        moveStatus = MoveStatus.WAIT;
      }
      else
      {
        int blockCnt = 0;
        // 更新当前阻挡数
        for (int i = 0; i < blockEnemies.Count; i++)
        {
          if (runningData.attributes.maxBlockCnt < blockCnt + blockEnemies[i].GetComponent<CharManager>().runningData.attributes.maxBlockCnt)
          {
            Object[] obj = new Object[] { };
            // 发送取消阻挡消息
            blockEnemies[i].SendMessage("BlockCancel", obj);
            blockEnemies.RemoveAt(blockEnemies.Count - 1);// 移除最后一个阻挡的人
            i--;
          }
          else
          {
            blockCnt += blockEnemies[i].GetComponent<CharManager>().runningData.attributes.maxBlockCnt;
          }
        }
      }
    }
  }
  void BlockCancel(Object[] obj)
  {
    blockEnemies = new List<GameObject>();
    // 如果是敌人，阻挡数组没有敌人时移动，干员切换到WAIT;
    if (originalData.isEnemy && moveStatus != MoveStatus.WAIT)
      moveStatus = enemyMoveStatus;
  }
}
public enum AttackStatus
{
  STOP,// 停止攻击
  SEARCH_ENEMY,// 攻击抬手
  BEFORE_ATTACK,// 攻击前摇
  AFTER_ATTACK,// 攻击后摇
}
public enum MoveStatus
{
  WAIT,// 等待时间
  MOVE,// 主动移动
  BLOCK,// 阻挡
  STOP,// 暂停移动
  DEAD// 死亡
}
