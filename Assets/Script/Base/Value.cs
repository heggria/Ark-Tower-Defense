[System.Serializable]
public class Value
{
  public bool enable;
  public BuffValueType buffValueType;
  public NumType numType;
  public NumOperator numOperator;
  public float value;
  public Value(BuffValueType _buffValueType, bool _enable, NumType _numType, NumOperator _numOperator, float _value)
  {
    this.buffValueType = _buffValueType;
    this.enable = _enable;
    this.numType = _numType;
    this.numOperator = _numOperator;
    this.value = _value;
  }
}
public enum BuffValueType{
  MAX_BLOOD,
  ATK,
  DEF,
  MAGIC_RESISTANCE,
  COST,
  ATTACK_SPEED,
  BASE_ATTACK_TIME,
  BASE_SEARCH_TIME,
  BASE_ATTACK_FORWARD_TIME,
  RESPAWN_TIME,
  MAX_DEPLOY_CNT,
  MAX_DECKSTACK_CNT,
  TOUGHNESS,
  MAX_BLOCK_CNT,
  RANGE_RADIUS,
  ATTACK_NUM,
  DAMAGE_TYPE,
  HP_CHANGE,
  SP_CHANGE,
}
public enum NumType
{
  NONE,
  PERCENTAGE,
  NUMBER
}
public enum NumOperator
{
  NONE,
  PLUS,
  MULTIPLY
}
