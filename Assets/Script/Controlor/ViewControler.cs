using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景漫游摄像机,该脚本需要直接挂在主摄像机下.
/// </summary>
public class ViewControler : MonoBehaviour
{
  private float mSpeed = 20f;
  private float rSpeed = 25f;
  private float sSpeed = 1000f;
  private Vector3 CameraR;

  void Start()
  {
    CameraR = Camera.main.transform.rotation.eulerAngles;
  }

  void Update()
  {
    Vector3 Face = transform.rotation * Vector3.forward;
    Face = Face.normalized;

    Vector3 Left = transform.rotation * Vector3.left;
    Left = Left.normalized;

    Vector3 Right = transform.rotation * Vector3.right;
    Right = Right.normalized;

    if (Input.GetMouseButton(1))
    {
      //官方脚本
      float yRot = Input.GetAxis("Mouse X");
      float xRot = Input.GetAxis("Mouse Y");
      Vector3 R = CameraR + new Vector3(rSpeed * -xRot, rSpeed * yRot, 0f);
      CameraR = Vector3.Slerp(CameraR, R, rSpeed * Time.deltaTime);
      transform.rotation = Quaternion.Euler(CameraR);

      if (Input.GetKey("w"))
        transform.position += Face * mSpeed * Time.deltaTime;
      if (Input.GetKey("s"))
        transform.position -= Face * mSpeed * Time.deltaTime;
      if (Input.GetKey("a"))
        transform.position += Left * mSpeed * Time.deltaTime;
      if (Input.GetKey("d"))
        transform.position += Right * mSpeed * Time.deltaTime;
    }
    if (Input.GetAxis("Mouse ScrollWheel") > 0)
    {
      transform.position += Face * sSpeed * Time.deltaTime;
    }
    else if (Input.GetAxis("Mouse ScrollWheel") < 0)
    {
      transform.position -= Face * sSpeed * Time.deltaTime;
    }
  }
}
