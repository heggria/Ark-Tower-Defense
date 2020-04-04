using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
  [HideInInspector]
  public CharcterData originalData;
  [HideInInspector]
  public CharcterData runningData;

  public Slider hpSilder;//血条
  public Transform head;
  public Transform firePosition;
  public SphereCollider searchRange;
  public GameObject bulletPrefab;

  private RangeRunning range;
  private float attackTimer = 0;
  private float moveTimer = 0;
  private AttackStatus attackStatus = AttackStatus.STOP;
  private MoveStatus moveStatus = MoveStatus.WAIT;

  // 敌人专用
  public GameObject enemyPerfab;

  [HideInInspector]
  public float distanceToEnd = 0;
  private Route route;
  private float reserve = 0.2f;//移动保留量
  private float magnification = 10f;//移动倍率
  private int nextPointIndex = 0;
  private Vector3 endPos3d;
  private float waitTime;
  void Start()
  {
    if (enemyPerfab != null)
      enemyPerfab.SetActive(false);
    else
    {
      range = searchRange.GetComponent<RangeRunning>();
    }
  }
  void Update()
  {
    if (originalData != null)
    {
      if (!originalData.isEnemy)
        AttackStatusMachine();
      else
        MoveStatusMachine();
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
              enemyPerfab.SetActive(true);
            moveTimer = 0;
            moveStatus = MoveStatus.MOVE;
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
        if (moveTimer >= route.checkpoints[nextPointIndex].stopTime)
        {
          moveTimer = 0;
          nextPointIndex++;
          moveStatus = MoveStatus.MOVE;
        }
        break;
      case MoveStatus.BLOCK:
        if (!originalData.isEnemy)
        {
          //todo
        }
        else
        {

        }
        break;
      case MoveStatus.DEAD:
        GameManager.options.lifePoint--;
        GameObject.Destroy(this.gameObject);
        break;
    }
    CountDistanceToEnd();
  }
  private void Attack()
  {
    // 攻击先进入攻击范围/攻击先上场 看情况
    int flag = 0;
    if (range.enemies.Count <= runningData.attributes.attackNum)
      flag = range.enemies.Count;
    else
      flag = runningData.attributes.attackNum;
    for (int i = 0; i < flag; i++)
    {
      GameObject bullet = GameObject.Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
      object[] message = new object[] { runningData, range.enemies[i].enemy.transform };
      bullet.SendMessage("InitBullet", message);
    }
  }
  public void TakeDamage(float damage)
  {
    //Debug.Log(1);
    if (runningData.attributes.maxHp <= 0) return;
    runningData.attributes.maxHp -= damage;
    hpSilder.value = (float)runningData.attributes.maxHp / originalData.attributes.maxHp;
    if (runningData.attributes.maxHp <= 0)
      Die();
  }
  private void Die()
  {
    Destroy(this.gameObject);
  }
  void OnDestroy()
  {
    if (originalData.isEnemy)
      GameManager.options.countEnemyAlive--;
  }
  void SetOpt(object[] obj)
  {
    originalData = (CharcterData)obj[0];
    runningData = Math.DeepCopyByBinary<CharcterData>(originalData);
  }
  void SetEnemy(object[] obj)
  {
    this.originalData = (CharcterData)obj[0];
    this.route = (Route)obj[1];
    this.waitTime = (float)obj[2];
    this.runningData = Math.DeepCopyByBinary<CharcterData>(this.originalData);
    hpSilder.value = (float)runningData.attributes.maxHp / originalData.attributes.maxHp;
    endPos3d = Math.Ve2ToVe3(route.endPosition * GameManager.space, GameManager.enemyHeight);
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
