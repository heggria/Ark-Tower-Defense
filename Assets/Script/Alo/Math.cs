
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
public class Math
{
  //todo
  public static T DeepCopyByBinary<T>(T obj)
  {
    object retval;
    using (MemoryStream ms = new MemoryStream())
    {
      BinaryFormatter bf = new BinaryFormatter();
      bf.Serialize(ms, obj);
      ms.Seek(0, SeekOrigin.Begin);
      retval = bf.Deserialize(ms);
      ms.Close();
    }
    return (T)retval;
  }
  public static Vector3 Ve2ToVe3(Vector2 pos2d, float height)
  {
    return new Vector3(pos2d.x, height, pos2d.y);
  }
  /**
   * 选择排序算法，仅限 Enemy 排序用
   */
  public static void SortFromSmallToBig(List<Enemy> arry, int begin, int end)
  {
    for (int i = 0; i < arry.Count; i++)
    {
      for (int j = i + 1; j < arry.Count; j++)
      {
        if (arry[i].distance > arry[j].distance)
        {
          Enemy temp;
          temp = arry[i];
          arry[i] = arry[j];
          arry[j] = temp;
        }
      }
    }
  }
}
