using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRunning : MonoBehaviour
{
  public GameObject explosionEffectPrefab;
  private GameObject target;
  // private bool targetIsEnemy = true;
  private CharcterData charRunningData;

  private List<GameObject> hitEnemy;

  public float reserve = 0.5f;

  BulletSetting bullet;

  private float timer = 0;

  void InitBullet(object[] obj)
  {
    this.charRunningData = (CharcterData)obj[0];
    this.target = (GameObject)obj[1];
    bullet = charRunningData.perfabSetting.bullet1;
    hitEnemy = new List<GameObject>();
    if (bullet.bulletType == BulletType.PROJECTILE)
      transform.position = target.transform.position;
  }

  void Update()
  {
    if (charRunningData != null)
    {
      switch (bullet.bulletType)
      {
        case BulletType.PROJECTILE:
          if (bullet.followEnemy) transform.position = target.transform.position;
          if ((timer += Time.deltaTime) >= charRunningData.perfabSetting.bullet1.delay)
          {
            damageTarget();
            timer = 0;
          }
          break;
        case BulletType.BOTH:
          if (target != null)
          {
            //Debug.Log(optData.attributes.ballisticSpeed);
            transform.LookAt(target.transform.position);
            transform.Translate(Vector3.forward * bullet.ballisticSpeed * Time.deltaTime);
            if (charRunningData.perfabSetting.bullet1.lockEnemy && Vector3.Distance(gameObject.transform.position, target.transform.position) <= reserve)
              damageTarget();
          }
          else
          {
            hitEnemy.Remove(target);
            damageTarget();
          }
          break;
      }
    }
  }

  // 击中任何敌人都向敌人发送一个damage消息,同时自身销毁
  void OnTriggerEnter(Collider col)
  {
    if (col.tag == "Char" && charRunningData.perfabSetting.bullet1.maxDamageCount > 0)
      if (charRunningData.isEnemy != col.GetComponent<CharManager>().originalData.isEnemy)
      {
        hitEnemy.Add(col.GetComponent<CharManager>().gameObject);
        if (!charRunningData.perfabSetting.bullet1.lockEnemy)
          damageTarget();
      }
  }
  void OnTriggerExit(Collider col)
  {
    if (col.tag == "Char" && bullet.maxDamageCount > 0)
      if (charRunningData.isEnemy != col.GetComponent<CharManager>().originalData.isEnemy)
        hitEnemy.Remove(col.GetComponent<CharManager>().gameObject);
  }
  private void Die()
  {
    if (charRunningData.perfabSetting.bullet1.hasEffect)
    {
      GameObject effect = GameObject.Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
      Destroy(effect, 0.5f);
    }
    Destroy(this.gameObject);
  }
  private void damageTarget()
  {
    int flag = 0;
    if (bullet.maxDamageCount == -1)
      flag = hitEnemy.Count;
    else
    {
      flag = bullet.maxDamageCount;
      if (flag > hitEnemy.Count) flag = hitEnemy.Count;
    }
    Debug.Log(flag);
    for (int i = 0; i < flag; i++)
      hitEnemy[i].GetComponent<CharManager>().GetDamage(CauseDamage(), charRunningData.damageType);
    Die();
  }
  private float CauseDamage()
  {
    return charRunningData.attributes.atk;
  }
}
