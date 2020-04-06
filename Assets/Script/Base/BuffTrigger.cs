/**
 * Trigger 触发器类
 */
[System.Serializable]
public class BuffTrigger {
  /**
   * 时间有关
   */
  public TriggerType triggerType;
  public TriggerCondition triggerCondition;
  public float timer1 = 0;
  public float timer2 = 0;// 小循环使用
  public float waitTime = 0;// 等待时间
  public float sleepTime = 0;// 休眠时间
  public float duration = 10;// 持续时间
  public float updateTime = 0;// 激活时间

  public float triggerTime = 0;// 累计时间
  public int countTimes = 0;// 累计次数
  public int count = 0;// 累计次数

  /*
  public BuffTrigger(TriggerType type, TriggerCondition condition, int effectTime, int duration, int triggerTime, int updateTime){
    this.triggerType = type;
    this.triggerCondition = condition;
    this.effectTime = effectTime;
    this.duration = duration;
    this.triggerTime = triggerTime;
    this.updateTime = updateTime;
  }*/
}
// 触发类型，只触发一次，间隔触发，持续触发不累计，持续触发累计
public enum TriggerType{
  ONCE,
  INTERVAL,
  CONTINUOUS,
  CUMULATIVE
}
// 触发条件，0无条件，1攻击，2受到攻击
// TODO
public enum TriggerCondition{
  NONE
}
