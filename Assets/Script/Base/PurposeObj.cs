using System.Collections;
using System.Collections.Generic;
/**
 * PurposeObj buff对象类
 */
[System.Serializable]
public class PurposeObj {
  /**
   * buff作用的对象
   */
  // buff目标，0无意义，1友方，2敌方，3敌友方
  private Aim aim;
  // buff作用范围，index对应，0自身，1指定作用范围，2全场范围，3指定阵营，4指定职业，5指定部署位，6指定伤害类型，7指定伤害方式，8指定干员
  // 实现：获取对应的对象集合id，同时生效会取并集
  private List<List<int>> buffTo;
  // 作用数量，0为作用所有符合条件的对象
  private int effectNum;
  // 是否遵循优先级给buff，否的时候会随机给
  private bool priority;
  // 临时保存buff作用者id,每次update从对象池获取需要更新此buff的对象id，将buffId存入对象buff数组中
  public List<int> recipientId;
  /**
   * 设置buff对象
   */
  public PurposeObj(Aim aim, List<List<int>> buffTo,int effectNum,bool priority) {
      this.aim = aim;
      this.buffTo = buffTo;
      this.effectNum = effectNum;
      this.priority = priority;
  }
}
public enum Aim{
  None,
  Friends,
  Enemies,
  All
}
