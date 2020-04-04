using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour{
  public Text lifePoint;
  public Text costText;
  void Update(){
    lifePoint.text = GameManager.options.lifePoint.ToString();
    costText.text = GameManager.options.nowCost.ToString();
  }
}
