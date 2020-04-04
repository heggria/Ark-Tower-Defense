using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRunning : MonoBehaviour
{
  public GameObject explosionEffectPrefab;
  private GameObject target;
  // private bool targetIsEnemy = true;
  private CharcterData charRunningData;
  private int attackNum = 1;

  void Update()
  {
    if (charRunningData != null)
    {
      if (target != null)
      {
        //Debug.Log(optData.attributes.ballisticSpeed);
        transform.LookAt(target.transform.position);
        transform.Translate(Vector3.forward * charRunningData.attributes.ballisticSpeed * Time.deltaTime);
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
    if (col.tag == "Char" && attackNum > 0)
      if (charRunningData.isEnemy != col.GetComponent<CharManager>().originalData.isEnemy)
      {
        attackNum--;
        col.GetComponent<CharManager>().GetDamage(CauseDamage(),charRunningData.damageType);
        Die();
      }
  }
  void InitBullet(object[] obj)
  {
    this.charRunningData = (CharcterData)obj[0];
    this.target = (GameObject)obj[1];
  }
  private void Die()
  {
    GameObject effect = GameObject.Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
    Destroy(effect, 0.5f);
    Destroy(this.gameObject);
  }
  private float CauseDamage(){
    return charRunningData.attributes.atk;
  }
}
