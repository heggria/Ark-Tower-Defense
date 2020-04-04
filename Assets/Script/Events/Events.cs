using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Events : MonoBehaviour
{
  public void HitMaskLayer()
  {
    OptControl optControl = GameObject.Find("OptControl").transform.Find("OperationCanvas").gameObject.GetComponent<OptControl>();
    optControl.HideOptControlUI();
  }
}
