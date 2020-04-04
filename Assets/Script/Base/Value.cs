[System.Serializable]
public class Value
{
  public string name;
  public bool enable;
  public NumType numType;
  public NumOperator numOperator;
  public float value;
  public Value(string _name, bool _enable, NumType _numType, NumOperator _numOperator, float _value)
  {
    this.name = _name;
    this.enable = _enable;
    this.numType = _numType;
    this.numOperator = _numOperator;
    this.value = _value;

  }
}
public enum NumType
{
  Percentage,
  Number
}
public enum NumOperator
{
  Plus,
  Multiply
}
