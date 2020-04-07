using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

// 本类负责单char所有buff的管理
public class BuffManager
{
  private List<Buff> buffs = new List<Buff>();

  // 更新buff触发器，去除无用buff
  private void BuffStatusMachine()
  {
    foreach (Buff buff in buffs)
    {
      buff.trigger.timer1 += Time.deltaTime;
      switch (buff.buffStatus)
      {
        case BuffStatus.WAIT:
          if (buff.trigger.timer1 >= buff.trigger.waitTime)
          {
            buff.trigger.timer1 = 0;
            buff.buffStatus = BuffStatus.ACTIVE;
          }
          break;
        case BuffStatus.ACTIVE:
          switch (buff.trigger.triggerType)
          {
            case TriggerType.CONTINUOUS:
              if (buff.trigger.timer1 >= buff.trigger.duration)
                buff.buffStatus = BuffStatus.DEAD;
              break;
            case TriggerType.ONCE:
              buff.buffStatus = BuffStatus.DEAD;
              break;
            case TriggerType.INTERVAL:
              buff.trigger.timer2 += Time.deltaTime;
              if (buff.trigger.timer1 < buff.trigger.duration)
              {
                if (buff.trigger.timer2 >= buff.trigger.updateTime)
                {
                  buff.buffStatus = BuffStatus.SLEEP;
                  buff.trigger.timer2 = 0;
                }
              }
              else buff.buffStatus = BuffStatus.DEAD;
              break;
            case TriggerType.CUMULATIVE:
              buff.trigger.timer2 += Time.deltaTime;
              if (buff.trigger.timer1 < buff.trigger.duration)
              {
                if (buff.trigger.timer2 >= buff.trigger.updateTime && buff.trigger.count < buff.trigger.countTimes)
                {
                  UpdateBuff(buff);
                  buff.trigger.timer2 = 0;
                  buff.trigger.countTimes++;
                }
              }
              else buff.buffStatus = BuffStatus.DEAD;
              break;
          }
          break;
        case BuffStatus.SLEEP:
          if (buff.trigger.timer1 < buff.trigger.duration)
          {
            buff.trigger.timer2 += Time.deltaTime;
            if (buff.trigger.timer2 >= buff.trigger.sleepTime)
            {
              buff.buffStatus = BuffStatus.ACTIVE;
              buff.trigger.timer2 = 0;
            }
          }
          else buff.buffStatus = BuffStatus.DEAD;
          break;
        case BuffStatus.DEAD:
          buffs.Remove(buff);
          break;
      }
    }
  }

  // 累加buff专用
  private void UpdateBuff(Buff buff)
  {
    for(int i =0;i<buff.buffValue.Count;i++){
      buff.buffValue[i].value += buff.buffValuePlus[i].value;
    }
  }
  // 将所有buff进行计算，统计出总buff集合
  private List<ValueFormula> BuffSettlement()
  {
    List<ValueFormula> effects = new List<ValueFormula>();
    foreach (Buff buff in buffs)
    {
      foreach (Value value in buff.buffValue)
      {
        for (int i = 0; i < effects.Count; i++)
        {
          if (effects[i].buffValueType == value.buffValueType)
          {
            ValueJudgment(effects[i], value);
            break;
          }
          else if (i == effects.Count)
          {
            effects.Add(new ValueFormula(value.buffValueType));
            ValueJudgment(effects[effects.Count], value);
          }
        }
      }
    }
    return effects;
  }
  private void ValueJudgment(ValueFormula save, Value val)
  {
    if (val.numType == NumType.NUMBER)
    {
      if (val.numOperator == NumOperator.PLUS)
        save.plusNum += val.value;
      else if (val.numOperator == NumOperator.MULTIPLY)
        save.multipleNum += val.value;
      else
        save.equalNum = val.value;
    }
    else if (val.numType == NumType.PERCENTAGE)
    {
      if (val.numOperator == NumOperator.PLUS)
        save.plusPer += val.value;
      else if (val.numOperator == NumOperator.MULTIPLY)
        save.multiplePer.Add(val.value);
      else
        save.equalPer = val.value;
    }
    else
    {
      save.equalNum = val.value;
    }
  }
  public Attributes CountBuffEffect(Attributes attributes)
  {
    BuffStatusMachine();
    List<ValueFormula> effects = BuffSettlement();
    foreach (ValueFormula value in effects)
    {
      switch (value.buffValueType)
      {
        case BuffValueType.MAX_BLOOD:
          attributes.maxHp = value.Calculation(attributes.maxHp);
          break;
        case BuffValueType.ATK:
          attributes.atk = value.Calculation(attributes.atk);
          break;
        case BuffValueType.DEF:
          attributes.def = value.Calculation(attributes.def);
          break;
        case BuffValueType.MAGIC_RESISTANCE:
          attributes.magicResistance = value.Calculation(attributes.magicResistance);
          break;
        case BuffValueType.COST:
          attributes.cost = (int)value.Calculation(attributes.cost);
          break;
        case BuffValueType.ATTACK_SPEED:
          attributes.attackSpeed = value.Calculation(attributes.attackSpeed);
          break;
        case BuffValueType.BASE_ATTACK_TIME:
          attributes.baseAttackTime = value.Calculation(attributes.baseAttackTime);
          break;
        case BuffValueType.BASE_SEARCH_TIME:
          attributes.baseSearchTime = value.Calculation(attributes.baseSearchTime);
          break;
        case BuffValueType.BASE_ATTACK_FORWARD_TIME:
          attributes.baseAttackForwardTime = value.Calculation(attributes.baseAttackForwardTime);
          break;
        case BuffValueType.RESPAWN_TIME:
          attributes.respawnTime = value.Calculation(attributes.respawnTime);
          break;
        case BuffValueType.MAX_DEPLOY_CNT:
          attributes.maxDeployCount = (int)value.Calculation(attributes.maxDeployCount);
          break;
        case BuffValueType.MAX_DECKSTACK_CNT:
          attributes.maxDeckStackCnt = (int)value.Calculation(attributes.maxDeckStackCnt);
          break;
        case BuffValueType.TOUGHNESS:
          attributes.toughness = value.Calculation(attributes.toughness);
          break;
        case BuffValueType.MAX_BLOCK_CNT:
          attributes.maxBlockCnt = (int)value.Calculation(attributes.maxBlockCnt);
          break;
        case BuffValueType.RANGE_RADIUS:
          attributes.rangeRadius = value.Calculation(attributes.rangeRadius);
          break;
        case BuffValueType.ATTACK_NUM:
          attributes.attackNum = (int)value.Calculation(attributes.attackNum);
          break;
        case BuffValueType.DAMAGE_TYPE:
          attributes.damageType = (DamageType)value.Calculation((float)attributes.damageType);
          break;

        // TODO
        case BuffValueType.HP_CHANGE:
          // value.Calculation(attributes.HP_CHANGE);
          break;
        case BuffValueType.SP_CHANGE:
          // value.Calculation(attributes.maxHp);
          break;
      }
    }
    return attributes;
  }
  public void AddBuff(Buff buff)
  {
    buffs.Add(buff);
  }
}
public class ValueFormula
{
  public BuffValueType buffValueType;
  public float equalNum = -1;// 强制相等
  public float equalPer = -1;
  public float plusNum = 0;
  public float plusPer = 1;
  public float multipleNum = 0;
  public List<float> multiplePer = new List<float>(new float[] { });

  public ValueFormula(BuffValueType _buffValueType)
  {
    this.buffValueType = _buffValueType;
  }

  public float Calculation(float n)
  {
    if (equalNum == -1 && equalPer == -1)
    {
      n = (n + multipleNum) * plusPer + plusNum;
      foreach (float f in multiplePer)
        n *= f;
    }
    else if (equalNum == -1)
      n = n * equalPer;
    else
      n = equalNum;
    return n;
  }
}
