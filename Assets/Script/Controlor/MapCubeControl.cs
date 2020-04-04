using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCubeControl : MonoBehaviour
{
  [HideInInspector]
  public CharcterData deployedOptData;
  public GameObject BuildEffect;
  public int type = 0;

  void Start()
  {
    deployedOptData = null;
  }
  public void OptSet(GameObject optPerfab, CharcterData deployedOptData)
  {
    GameObject deployedOpt = GameObject.Instantiate(optPerfab, new Vector3(transform.position.x, GameManager.optHeight, transform.position.z), Quaternion.identity);
    object[] message = new object[] { deployedOptData };
    deployedOpt.SendMessage("SetOpt", message);
    GameObject effect = GameObject.Instantiate(BuildEffect, transform.position, Quaternion.identity);
    Destroy(effect, 1);
  }
  void OnMouseEnter()
  {
    if (!EventSystem.current.IsPointerOverGameObject())
    {
      if (deployedOptData != null)
      {
        switch (type)
        {
          case 0:
            GetComponentInChildren<Renderer>().material.color = Color.yellow;
            break;
          case 1:
            GetComponentInChildren<Renderer>().material.color = Color.gray;
            break;
        }
      }
    }
  }
  void OnMouseExit()
  {
    switch (type)
    {
      case 0:
        GetComponentInChildren<Renderer>().material.color = Color.white;
        break;
      case 1:
        GetComponentInChildren<Renderer>().material.color = new Color(100f / 255f, 127f / 255f, 151f / 255f, 1);
        break;
    }
  }
}
