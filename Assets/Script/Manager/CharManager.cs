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

  public BuffManager buffManager = new BuffManager();// buff管理器

  public bool hpSilderDisplay = true;

  public GameObject hpSilder;// 血条
  public Transform head;
  public Transform firePosition;
  public GameObject bulletPrefab;
  public GameObject laserPerfab;
  public GameObject perfab;

  private RangeRunning range;
  private float attackTimer = 0;
  private float moveTimer = 0;
  private AttackStatus attackStatus = AttackStatus.STOP;
  private MoveStatus moveStatus = MoveStatus.WAIT;
  private BlockStatus blockStatus = BlockStatus.NONE;


  public List<GameObject> blockEnemies = new List<GameObject>();
  public List<Enemy> enemies;
  public List<GameObject> attackEnemies = new List<GameObject>();

  private int attackEnemyCount = 0;

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
    // 设置攻击范围
    range.SetRadius(runningData.attributes.rangeRadius);
    if (originalData != null)
    {
      if (originalData.perfabSetting.canAtk)
        AttackStatusMachine();
      if (originalData.perfabSetting.canMove)
        MoveStatusMachine();
      UpdateBlockEnemies();
    }
  }
  // 攻击状态机
  private void AttackStatusMachine()
  {
    // Debug.Log(attackStatus);
    // 计时
    attackTimer += Time.deltaTime;
    if (runningData.attributes.attackNum > 0)
    {
      // 如果范围内有敌人
      if (enemies.Count > 0)
      {
        if (attackStatus != AttackStatus.AFTER_ATTACK)
        {
          // 状态判断
          switch (attackStatus)
          {
            case AttackStatus.STOP:
              // 如果是STOP，进入SEARCH_ENEMY
              attackTimer = 0;
              attackStatus = AttackStatus.SEARCH_ENEMY;
              break;
            case AttackStatus.SEARCH_ENEMY:
              // SEARCH_ENEMY计时结束，进入BEFORE_ATTACK，保存当前敌人数组
              if (attackTimer >= runningData.attributes.baseSearchTime)
              {
                attackTimer = 0;
                attackStatus = AttackStatus.CONFIRM_ENEMY;
              }
              break;
            case AttackStatus.CONFIRM_ENEMY:
              attackTimer = 0;
              // 重置锁定enemy数组
              attackEnemies = new List<GameObject>();
              // 计算应该攻击的敌人数
              CountAttackEnemyNum();
              // 保存抬手结束时的敌人数组（锁定）
              for (int i = 0; i < attackEnemyCount; i++)
              {
                if (blockEnemies.Count > 0)
                {
                  if (i < blockEnemies.Count)
                    attackEnemies.Add(blockEnemies[i]);
                  else
                    for (int j = 0; j < blockEnemies.Count; j++)
                    {
                      if (enemies[i - blockEnemies.Count].enemy == blockEnemies[j])
                        break;
                      else if (j == blockEnemies.Count - 1)
                      {
                        attackEnemies.Add(enemies[i - blockEnemies.Count].enemy);
                      }
                    }

                }
                else
                  attackEnemies.Add(enemies[i - blockEnemies.Count].enemy);
              }
              attackStatus = AttackStatus.BEFORE_ATTACK;
              break;
            case AttackStatus.BEFORE_ATTACK:
              // 持续检测锁定的敌人是否仍然存在（目前只考虑第一个敌人即主目标）
              // 不存在就重置前摇
              if (attackEnemies[0] == null)
                attackStatus = AttackStatus.CONFIRM_ENEMY;

              // BEFORE_ATTACK计时结束，进入AFTER_ATTACK
              if (attackTimer >= runningData.attributes.baseAttackForwardTime)
              {
                attackTimer = 0;
                for (int i = 0; i < attackEnemyCount; i++)
                  Attack(attackEnemies[i]);
                attackStatus = AttackStatus.AFTER_ATTACK;
              }
              break;
            case AttackStatus.AFTER_ATTACK:
              break;
          }
        }
        // 有敌人的后摇
        else
        {
          // Debug.Log(attackStatus);
          if (attackTimer >= runningData.attributes.baseAttackTime - runningData.attributes.baseAttackForwardTime)
          {
            attackTimer = 0;
            attackStatus = AttackStatus.CONFIRM_ENEMY;
          }
        }
        // 物体头部转动，指向敌人数组第一个
        if (originalData.perfabSetting.hasHead && attackEnemies.Count > 0 && attackEnemies[0] != null)
        {
          Vector3 targetPosition = attackEnemies[0].transform.position;
          targetPosition.y = head.position.y;
          head.LookAt(targetPosition);
        }
      }
      // 没有敌人的后摇
      else if (attackStatus == AttackStatus.AFTER_ATTACK)
      {
        if (attackTimer >= runningData.attributes.baseAttackTime - runningData.attributes.baseAttackForwardTime)
        {
          attackTimer = 0;
          attackStatus = AttackStatus.STOP;
        }
      }
    }
  }

  private void MoveStatusMachine()
  {
    moveTimer += Time.deltaTime;
    switch (moveStatus)
    {
      case MoveStatus.WAIT:
        if (route == null) break;
        else if (moveTimer >= waitTime)
        {
          if (originalData.isEnemy)
            perfab.SetActive(true);

          moveTimer = 0;
          moveStatus = MoveStatus.MOVE;
          gameObject.GetComponent<SphereCollider>().enabled = true;
        }
        break;
      case MoveStatus.MOVE:
        {
          if (blockStatus != BlockStatus.BLOCKING)
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
                moveStatus = MoveStatus.AUTOSTOP;
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
                moveStatus = MoveStatus.INTOPOINT;
              }
            }
          break;
        }
      case MoveStatus.AUTOSTOP:
        if (moveTimer >= route.checkpoints[nextPointIndex].stopTime)
        {
          moveTimer = 0;
          nextPointIndex++;
          moveStatus = MoveStatus.MOVE;
        }
        break;
      case MoveStatus.INTOPOINT:
        GameManager.options.lifePoint--;
        GameObject.Destroy(gameObject);
        break;
    }
    if (originalData.isEnemy)
      CountDistanceToEnd();
  }

  // 计算应该打的敌人个数
  private void CountAttackEnemyNum()
  {
    attackEnemyCount = 0;
    // 先计算阻挡
    if (blockEnemies.Count > 0)
    {
      if (blockEnemies.Count < runningData.attributes.attackNum)
      {
        attackEnemyCount = blockEnemies.Count;
        if (enemies.Count < runningData.attributes.attackNum - attackEnemyCount)
          attackEnemyCount += enemies.Count;
        else attackEnemyCount = runningData.attributes.attackNum;// 满了
      }
      else attackEnemyCount = runningData.attributes.attackNum;// 满了
    }
    else
    {
      if (enemies.Count < runningData.attributes.attackNum)
        attackEnemyCount = enemies.Count;
      else attackEnemyCount = runningData.attributes.attackNum;// 满了
    }
  }
  private void Attack(GameObject target)
  {
    switch (runningData.perfabSetting.bullet1.bulletType)
    {
      case BulletType.NONE:
        target.GetComponent<CharManager>().GetDamage(CauseDamage(), runningData.attributes.damageType);
        break;
      case BulletType.TRAJECTORY:
        CreateLaser(firePosition, target);
        target.GetComponent<CharManager>().GetDamage(CauseDamage(), runningData.attributes.damageType);
        break;
      default:
        CreateBullet(target);
        break;
    }
  }
  private void CreateBullet(GameObject target)
  {
    GameObject bullet = GameObject.Instantiate(bulletPrefab, firePosition.position, Quaternion.identity);
    object[] message = new object[] { runningData, target };
    bullet.SetActive(true);
    bullet.SendMessage("InitBullet", message);
  }
  private void CreateLaser(Transform firePosition, GameObject target)
  {
    GameObject laser = GameObject.Instantiate(laserPerfab, firePosition.position, Quaternion.identity);
    Object[] message = new Object[] { firePosition, target, range };
    laser.SetActive(true);
    laser.SendMessage("InitLaser", message);
  }
  private float CauseDamage()
  {
    return runningData.attributes.atk;
  }
  public void GetDamage(float damage, DamageType damageType)
  {
    if (runningData.attributes.nowHp <= 0) return;
    CountDamage(damage, damageType);
    SetHpSilder(runningData.attributes.nowHp / originalData.attributes.maxHp, true);
    if (runningData.attributes.nowHp <= 0)
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
  private void CountDamage(float damage, DamageType damageType)
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
    runningData.attributes.nowHp -= finalDamage;
  }

  void SetOpt(object[] obj)
  {
    perfab = (GameObject)obj[0];
    originalData = (CharcterData)obj[1];
    InitCharManager();
    gameObject.GetComponent<SphereCollider>().enabled = true;
    gameObject.GetComponent<SphereCollider>().isTrigger = false;
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
    gameObject.GetComponent<SphereCollider>().radius = 1;
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
    // 绑定范围敌人数组
    enemies = range.enemies;
    // 血条组件已经初始化，可以直接调用
    SetHpSilder(runningData.attributes.nowHp / originalData.attributes.maxHp, true);
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
      else if (child.gameObject.name == "Bullet1")
      {
        child.gameObject.SetActive(false);
      }
      else if (child.gameObject.name == "Bullet2")
      {
        bulletPrefab = child.gameObject;
        bulletPrefab.SetActive(false);
      }
      else if (child.gameObject.name == "Laser")
      {
        laserPerfab = child.gameObject;
        laserPerfab.SetActive(false);
      }/*
      else if (child.gameObject.name == "Collider")
      {
        if(originalData.isEnemy)
        child.gameObject.GetComponent<Rigidbody>().constraints =
        RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        else
        child.gameObject.GetComponent<Rigidbody>().constraints =
        RigidbodyConstraints.FreezePositionZ;
      }*/
    }
  }

  private void BlockStart()
  {
    if (attackStatus == AttackStatus.AFTER_ATTACK)
    {
      if (originalData.isEnemy)
        attackStatus = AttackStatus.CONFIRM_ENEMY;
      else
      {

      }
    }
    blockStatus = BlockStatus.BLOCKING;
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
          if (blockStatus == BlockStatus.BLOCKING)
            return;
          // 如果不是阻挡状态，判断能不能阻挡，干员当前阻挡-敌人最大阻挡大于 0
          else if (other.runningData.attributes.maxBlockCnt - runningData.attributes.maxBlockCnt >= 0)
          {
            BlockStart();
            blockEnemies.Add(col.GetComponent<Transform>().gameObject);
            //Debug.Log(blockEnemies[0].GetComponent<CharManager>());
            // 敌人阻挡数不用变
          }
        }
        else
        {
          // 判断能不能阻挡，干员当前阻挡-敌人最大阻挡大于 0
          if (runningData.attributes.maxBlockCnt - other.runningData.attributes.maxBlockCnt >= 0)
          {
            //Debug.Log(other.runningData.attributes.supplyBlockCnt);
            BlockStart();
            blockEnemies.Add(col.GetComponent<Transform>().gameObject);
          }
          // Debug.Log(blockEnemies.Count);
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
    blockStatus = BlockStatus.NONE;
  }
}
public enum AttackStatus
{
  STOP,// 停止攻击
  SEARCH_ENEMY,// 攻击抬手
  CONFIRM_ENEMY,// 锁定敌人
  BEFORE_ATTACK,// 攻击前摇
  AFTER_ATTACK,// 攻击后摇
}
public enum MoveStatus
{
  WAIT,// 等待时间
  MOVE,// 主动移动
  AUTOSTOP,// 自动暂停
  ATTACKSTOP,// 攻击暂停
  INTOPOINT// 死亡
}
public enum BlockStatus
{
  NONE,
  BLOCKING,
}
