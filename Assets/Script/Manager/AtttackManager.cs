/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class AtttackManager
{

  public CharcterData runningData;// 运行数据
  public List<GameObject> attackEnemies = new List<GameObject>();
  public List<Enemy> enemies;

  public AttackStatus attackStatus = AttackStatus.STOP;
  public bool turnHead = false;
  public int attackEnemyCount = 0;

  private float attackTimer = 0;

  public AtttackManager(CharcterData _runningData,List<Enemy> _enemies)
  {
    runningData = _runningData;
    enemies = _enemies;
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
  // 攻击状态机
  public void AttackStatusMachine()
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

              // BEFORE_ATTACK计时结束，进入ATTACK
              if (attackTimer >= runningData.attributes.baseAttackForwardTime)
              {
                attackTimer = 0;
                attackStatus = AttackStatus.ATTACK;
              }
              break;
            case AttackStatus.ATTACK:
              attackStatus = AttackStatus.AFTER_ATTACK;
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
        if (runningData.perfabSetting.hasHead && attackEnemies.Count > 0 && attackEnemies[0] != null)
        {
          turnHead = true;
        }
        else turnHead = false;
      }
      // 没有敌人的后摇
      else if (attackStatus == AttackStatus.AFTER_ATTACK)
      {
        if (attackTimer >= runningData.attributes.baseAttackTime - runningData.attributes.baseAttackForwardTime)
        {
          attackTimer = 0;
          attackStatus = AttackStatus.STOP;
          turnHead = false;
        }
      }
    }
  }
}

public enum AttackStatus
{
  STOP,// 停止攻击
  SEARCH_ENEMY,// 攻击抬手
  CONFIRM_ENEMY,// 锁定敌人
  ATTACK,// 锁定敌人
  BEFORE_ATTACK,// 攻击前摇
  AFTER_ATTACK,// 攻击后摇
}
*/
