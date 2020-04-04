using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Route
{
  public int id;
  public Vector2 startPosition;
  public Vector2 endPosition;
  public List<Checkpoint> checkpoints;
  public Route(int _id,Vector2 _startPosition,Vector2 _endPosition,List<Checkpoint> _checkpoints){
    this.id = _id;
    this.startPosition = _startPosition;
    this.endPosition = _endPosition;
    this.checkpoints = _checkpoints;
  }
}
// 一个检查点
public class Checkpoint{
  public float stopTime;
  public Vector2 position;
  public Checkpoint(float _stopTime,Vector2 _position){
    this.stopTime = _stopTime;
    this.position = _position;
  }
}
