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
  public void OptSet(GameObject charConstructor, GameObject optPerfab, CharcterData optData)
  {
    deployedOptData = optData;
    GameObject charC = GameObject.Instantiate(charConstructor, new Vector3(transform.position.x, GameManager.optHeight, transform.position.z), Quaternion.identity);
    GameObject optI = GameObject.Instantiate(optPerfab, charC.transform.position, Quaternion.identity);
    optI.transform.parent = charC.transform;
    object[] message = new object[] { optI, optData };
    charC.SendMessage("SetOpt", message);
    GameObject effect = GameObject.Instantiate(BuildEffect, transform.position, Quaternion.identity);
    Destroy(effect, 1);
  }
  void OnMouseEnter()
  {
    if (!EventSystem.current.IsPointerOverGameObject())
    {
      if (deployedOptData == null)
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
      else
      {
        GetComponentInChildren<Renderer>().material.color = Color.green;
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
