using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
  public string name;// 波次名称
  public float preDelay;// 准备延迟
  public float maxTimeWaitingForNextWave;// 长于这个时间自动开始下一大波次
  public List<EnemyFragment> enemyFragments;//敌人单元对象
  public Wave(string _name,float _preDelay,float _maxTimeWaitingForNextWave,List<EnemyFragment> _enemyFragments){
    this.name=_name;
    this.preDelay=_preDelay;
    this.maxTimeWaitingForNextWave=_maxTimeWaitingForNextWave;
    this.enemyFragments=_enemyFragments;
  }
}
public class EnemyFragment{
  public float preDelay;// 准备延迟
  public List<EnemyAction> enemyActions;//敌人单元对象
  public EnemyFragment(float _preDelay,List<EnemyAction> _enemyActions){
    this.preDelay=_preDelay;
    this.enemyActions=_enemyActions;
  }
}
public class EnemyAction{
  public string enemyKey;// 敌人id
  public float preDelay;// 准备延迟
  public int count;// 这种敌人共多少个
  public float interval;// 每一个敌人之间的间隔
  public int routeId;// 路线id
  //public bool autoPreviewRoute;// 自动寻路
  public EnemyAction(string _enemyKey,float _preDelay,int _count,float _interval,int _routeId){
    this.enemyKey=_enemyKey;
    this.preDelay=_preDelay;
    this.count=_count;
    this.interval=_interval;
    this.routeId=_routeId;
  }
}
