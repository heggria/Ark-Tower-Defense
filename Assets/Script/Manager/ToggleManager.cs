using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.UI;
using UnityEngine;

public class ToggleManager : MonoBehaviour
{
  public TurretToggle[] turretToggles;
  void Update()
  {
    for(int i = 0;i<turretToggles.Length;i++){
      if(turretToggles[i].turretCost<=GameManager.options.nowCost){
        turretToggles[i].toggle.interactable = true;
      }else
        turretToggles[i].toggle.interactable = false;
      Text text = turretToggles[i].toggle.GetComponentInChildren<Text>();
      text.text = turretToggles[i].turretCost+"C";
    }
  }
}
public class TurretToggle
{
  public Toggle toggle;
  public int turretCost;
}
