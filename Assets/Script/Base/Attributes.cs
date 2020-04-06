/**
 * 基本属性
 */
[System.Serializable]
public class Attributes {
  public float maxHp = 0;
  public float nowHp = 0;

  public float maxSp = 0;
  public float nowSp = 0;

  public float atk = 0;
  public float def = 0;
  public float magicResistance = 0;
  public int cost = 0;
  public float moveSpeed = 1;
  public float attackSpeed = 100;
  public float baseAttackTime = 1;
  public float baseAttackForwardTime = 0;// 攻击前摇时间，攻击前摇时间必须小于攻击间隔，且与攻击间隔等比例变化
  public float baseSearchTime = 0;// 第一个敌人进入攻击范围时会有第一次抬手时间，固定，攻击范围内没有敌人时重置
  public float respawnTime = 0;
  public float toughness = 0;// 韧性，对所有负面效果起作用

  public int maxBlockCnt = 1;// 最大阻挡数
  public int maxDeployCount = 1;// 最大部署数量
  public int maxDeckStackCnt = 1;// 最大保存数量

  public float rangeRadius = 20;//攻击距离
  public int attackNum = 1;// 攻击个数

  // 伤害类型
  public DamageType damageType = DamageType.Physical;

  public Attributes(float _maxHp,float _atk,float _rangeRadius){
    maxHp = _maxHp;
    nowHp = maxHp;
    atk = _atk;
    rangeRadius = _rangeRadius;
  }
}
public enum DamageType{
  Physical,
  Magic,
  Real
}
