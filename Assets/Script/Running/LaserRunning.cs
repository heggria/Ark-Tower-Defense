using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRunning : MonoBehaviour
{
  private Transform firePosition;
  private RangeRunning range;
  private GameObject target;

  void InitLaser(Object[] obj)
  {
    firePosition = (Transform)obj[0];
    target = (GameObject)obj[1];
    range = (RangeRunning)obj[2];
  }
  void Update()
  {
    if (firePosition != null)
      if (target != null)
        if (range.SearchEnemy(target))
        {
          LineRenderer laserRenderer = GetComponent<LineRenderer>();
          laserRenderer.SetPositions(new Vector3[] { firePosition.position, target.transform.position });
        }
        else Destroy(this.gameObject);
      else
        Destroy(this.gameObject);
  }
}
