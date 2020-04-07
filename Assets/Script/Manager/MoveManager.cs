/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class MoveManager
{
  private RangeRunning range;
  private RangeRunning range;
  private float moveTimer = 0;
  private MoveStatus moveStatus = MoveStatus.WAIT;

  // 敌人专用
  [HideInInspector]
  private Route route;
  private Vector3 endPos3d;
  public float distanceToEnd = 0;
  private float reserve = 0.2f;//移动保留量
  private float magnification = 5f;//移动倍率
  private int nextPointIndex = 0;
  private float waitTime;

  public void MoveStatusMachine()
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
*/
