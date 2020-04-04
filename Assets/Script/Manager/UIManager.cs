using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
  public Text lifePointCount;
  public Text costCount;
  public Text killEnemyCount;
  void Update()
  {
    lifePointCount.text = GameManager.options.lifePoint.ToString();
    costCount.text = GameManager.options.nowCost.ToString();
    killEnemyCount.text = GameManager.options.killEnemy.ToString()+"/"+GameManager.options.totalEnemy.ToString();
  }
}
