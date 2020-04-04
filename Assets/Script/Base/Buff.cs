using System.Collections;
using System.Collections.Generic;
public class Buff
{
  // buff流水号
  public int id;
  // buff创建者id
  public int creatorId;
  public OperateStatus status;
  // 改变的数值
  public List<Value> buffValue;
  // 触发器
  public BuffTrigger trigger;
  // buff作用的对象
  public PurposeObj purposeObject;

  public Buff(int id, int creatorId, List<Value> buffValue, BuffTrigger trigger, PurposeObj purposeObject)
  {
    this.id = id;
    this.creatorId = creatorId;
    this.trigger = trigger;
    this.purposeObject = purposeObject;
    this.buffValue = buffValue;
    this.status = 0;
  }
}
// buff状态，0未运行，1开始运行结算，2运行，3结束运行结算
public enum OperateStatus
{
  NotRunning,
  StartRunning,
  Running,
  EndRunning
}
