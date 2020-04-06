using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * 获取在攻击范围内的敌人
 * 绑定在range上
 */
public class RangeRunning : MonoBehaviour
{
  public List<Enemy> enemies = new List<Enemy>();
  private List<int> enemySortByTimeOfEnter = new List<int>();
  //private List<int> enemySortByTimeOfAppear = new List<int>();
  void Start()
  {
  }
  //敌人进入碰撞体事件
  void OnTriggerEnter(Collider col)
  {
    //Debug.Log(col.tag);
    if (col.tag == "Char" && col.GetComponent<CharManager>().originalData.isEnemy != gameObject.GetComponentInParent<CharManager>().originalData.isEnemy)
    {
      Enemy enemy = new Enemy(col.gameObject, enemies.Count);
      enemies.Add(enemy);
    }
  }
  //敌人离开碰撞体事件
  void OnTriggerExit(Collider col)
  {
    if (col.tag == "Char" && col.GetComponent<CharManager>().originalData.isEnemy != gameObject.GetComponentInParent<CharManager>().originalData.isEnemy)
    {
      int offesst = 0;
      for (int i = 0; i < enemies.Count; i++)
      {
        if (enemies[i].enemy.gameObject == col.gameObject)
        {
          offesst = enemies[i].enterTime;
          enemies.RemoveAt(i);
        }
      }
      for (int i = 0; i < enemies.Count; i++)
      {
        if (enemies[i].enterTime > offesst)
          enemies[i].enterTime -= 1;
      }
    }
  }

  void Update()
  {
    UpdateEnemys();
  }
  // 设置索敌范围
  public void SetRadius(float radius)
  {
    gameObject.GetComponent<SphereCollider>().radius = radius;
  }
  //修正敌人List
  void UpdateEnemys()
  {
    List<int> emptyIndex = new List<int>();
    for (int index = 0; index < enemies.Count; index++)
      if (enemies[index].enemy == null)
        emptyIndex.Add(index);
    for (int i = 0; i < emptyIndex.Count; i++)
      enemies.RemoveAt(emptyIndex[i] - i);
    foreach (Enemy enemy in enemies)
    {
      enemy.distance = enemy.enemy.GetComponentInParent<CharManager>().distanceToEnd;
    }
    SortByDistance();
  }
  // 只对干员攻击距离起作用
  // 对敌人List按离蓝门的距离进行排序
  void SortByDistance()
  {
    Math.SortFromSmallToBig(enemies, 0, enemies.Count - 1);
  }
  public bool SearchEnemy(GameObject target){
    for(int i = 0;i<enemies.Count;i++){
      if(target == enemies[i].enemy) return true;
    }
    return false;
  }
}
public class Enemy
{
  public float distance = 0;
  public GameObject enemy;
  public int enterTime = 0;
  public Enemy(GameObject _enemy, int _enterTime)
  {
    this.enemy = _enemy;
    this.enterTime = _enterTime;
  }
}
