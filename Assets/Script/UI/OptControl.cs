using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 过时
public class OptControl : MonoBehaviour
{
  void Start()
  {
    gameObject.SetActive(true);
  }
  void OnMouseDownInOtherPlace()
  {
    //Debug.Log(1);
    if (!EventSystem.current.IsPointerOverGameObject())
      HideOptControlUI();
  }
  //todo：重复选择技能
  public void ShowOptControlUI(Vector3 pos)
  {
    //Debug.Log(pos);
    gameObject.SetActive(true);
    gameObject.transform.position = pos;
  }
  public void HideOptControlUI()
  {
    gameObject.SetActive(false);
  }
  void OnMouseDown()
  {
    if (!EventSystem.current.IsPointerOverGameObject())
    {
      Vector3 pos = transform.position + new Vector3(0, 5, 0);
      ShowOptControlUI(pos);
    }
  }
}
