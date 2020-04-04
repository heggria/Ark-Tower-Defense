using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectionBar : MonoBehaviour
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
}
