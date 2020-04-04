/**
 * Trigger 触发器类
 */
[System.Serializable]
public class BuffTrigger {
  /**
   * 时间有关，以帧为单位
   */
  private TriggerType triggerType;
  private TriggerCondition triggerCondition;
  private int effectTime;
  private int duration;
  private int triggerTime;
  private int updataTime;


  public BuffTrigger(TriggerType type, TriggerCondition condition, int effectTime, int duration, int triggerTime, int updataTime){
    this.triggerType = type;
    this.triggerCondition = condition;
    this.effectTime = effectTime;
    this.duration = duration;
    this.triggerTime = triggerTime;
    this.updataTime = updataTime;
  }
}
// 触发类型，1只触发一次，2持续触发不累计，3持续触发累计
public enum TriggerType{
  Once,
  Continuous,
  Cumulative
}
// 触发条件，0无条件，1攻击，2受到攻击
// TODO
public enum TriggerCondition{
  None
}
