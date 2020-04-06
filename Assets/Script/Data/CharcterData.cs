using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharcterData
{
  //TODO
  public string key;
  //需要改成 enum
  public int id;
  // 是否为敌人
  public bool isEnemy;
  // 种族
  public string ethnicityId;
  // 阵营
  public string campId;
  // 职业
  public string profession;
  // 部署位
  public DeployPlace deployPlace;
  // 攻击方式
  public AttackSytle attackSytle;
  // 伤害类型
  public DamageType damageType = DamageType.Physical;

  public Attributes attributes;

  public List<int> buffIdSet;// 临时保存拥有的buffId

  public PerfabSetting perfabSetting = new PerfabSetting();

  public CharcterData(int _id ,string _key ,bool _isEnemy,Attributes _attributes){
    this.key = _key;
    this.id = _id;
    this.isEnemy = _isEnemy;
    this.attributes = _attributes;
  }
}
[System.Serializable]
public class PerfabSetting{
  public BulletSetting bullet1 = new BulletSetting();

  public bool hasHead = false;
  public float colliderSize = 1;// 碰撞模型大小，统一球形
  public bool canAtk = false;// 是否可以攻击
  public bool canMove = false;// 是否可以移动
}
[System.Serializable]
public class BulletSetting{
  public BulletType bulletType = BulletType.NONE;// 弹道与弹体
  public bool hasEffect = false;
  public bool lockEnemy = false;// 锁定敌人，即中心到达敌人位置才进行碰撞判定
  public int maxDamageCount = 1;// 区别于AttackNum，是弹体的最大攻击个数，-1为范围内全部伤害
  public int bulletSize = 1;// 弹体的大小
  public float ballisticSpeed = 1;// 弹道速度
  public float delay = 1f;// 伤害判定延迟
  public bool followEnemy = false;// 跟随敌人
}
public enum BulletType{
  NONE,
  TRAJECTORY,
  PROJECTILE,
  BOTH
}
public enum AttackSytle{
  None,
  Melee,
  Remote,
  Special
}

public enum DamageType{
  Physical,
  Magic,
  Real
}

public enum DeployPlace{
  Ground,
  Hill,
  Both,
  Neither,
}
