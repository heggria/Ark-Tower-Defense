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
  // 部署位，0地面，1高台，2都可以，3都不可以
  public DeployPlace deployPlace;
  // 攻击方式，3不攻击,0近战，1远程，2特殊
  public AttackSytle damageType;
  public Attributes attributes;
  public List<int> buffIdSet;// 临时保存拥有的buffId

  public CharcterData(int _id ,string _key ,bool _isEnemy,Attributes _attributes){
    this.key = _key;
    this.id = _id;
    this.isEnemy = _isEnemy;
    this.attributes = _attributes;
  }
}
public enum AttackSytle{
  None,
  Melee,
  Remote,
  Special
}
public enum DeployPlace{
  Ground,
  Hill,
  Both,
  Neither,
}
