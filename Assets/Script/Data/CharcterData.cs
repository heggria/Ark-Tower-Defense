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
  public bool hasHead = false;
  public bool hasBulletEffect = true;

  public BulletType bulletType = BulletType.BOTH;// 弹道与弹体
  public float ballisticSpeed = 1;// 弹道速度

  public float colliderSize = 1;// 碰撞模型大小，统一球形
  public bool canAtk = false;// 是否可以攻击
  public bool canMove = false;// 是否可以移动
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
