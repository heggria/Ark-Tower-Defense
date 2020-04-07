using System.Collections;
using System.Collections.Generic;
public class Buff
{
  // buff流水号
  // public int id;
  // buff创建者id
  // public int creatorId;
  public BuffStatus buffStatus = BuffStatus.WAIT;
  // 改变的数值
  public List<Value> buffValue;
  // 累加的数值，定义时需要和buffValue一一对应
  public List<Value> buffValuePlus;
  // 触发器
  public BuffTrigger trigger;
  // buff作用的对象
  // public PurposeObj purposeObject;

  public Buff(/*int id, int creatorId,*/ List<Value> buffValue, BuffTrigger trigger)
  {
    this.trigger = trigger;
    this.buffValue = buffValue;
  }
}

public enum BuffStatus
{
  WAIT,
  ACTIVE,
  SLEEP,
  DEAD
}
