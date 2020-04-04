using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRunning : MonoBehaviour
{
  public GameObject explosionEffectPrefab;
  private Transform target;
  private CharcterData optData;
  private int attackNum = 1;

  void Update()
  {
    if (optData != null)
    {
      if (target != null)
      {
        //Debug.Log(optData.attributes.ballisticSpeed);
        transform.LookAt(target.position);
        transform.Translate(Vector3.forward * optData.attributes.ballisticSpeed * Time.deltaTime);
      }
      else
      {
        Destroy(this.gameObject);
        return;
      }
    }
  }

  // 击中任何敌人都向敌人发送一个damage消息,同时自身销毁
  void OnTriggerEnter(Collider col)
  {
    if (col.tag == "Enemy" && attackNum > 0)
    {
      attackNum--;
      //Debug.Log(1);
      // todo
      col.GetComponentInParent<StatusManager>().TakeDamage(optData.attributes.atk);
    }else{
      Die();
    }
  }
  void Die()
  {
    GameObject effect = GameObject.Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
    Destroy(effect, 0.5f);
    Destroy(this.gameObject);
  }
  void InitBullet(object[] obj)
  {
    this.optData = (CharcterData)obj[0];
    this.target = (Transform)obj[1];
  }/*
  void SynchronizeOptData(object[] obj){
    this.optData = (OptData)obj[0];
  }*/
}
