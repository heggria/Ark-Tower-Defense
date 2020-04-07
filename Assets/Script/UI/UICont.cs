
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICont : MonoBehaviour {
    public RectTransform mainUI;
    public Button showHideBtn;
    public RepeatButton dragBtn;

    private bool m_isShow = true;
   // public RectTransform
	// Use this for initialization
	void Start () {
        dragBtn.onPress.AddListener(DragBtn);
        showHideBtn.onClick.AddListener(ShowHide);
	}
    void ShowHide() {
        Vector3 vec3Tmp = mainUI.position;
        if(m_isShow) {
            vec3Tmp.x = -mainUI.sizeDelta.x;
            iTween.MoveTo(mainUI.gameObject, vec3Tmp, 1f);
            m_isShow = false;
        } else {
            vec3Tmp.x = 0;
            iTween.MoveTo(mainUI.gameObject, vec3Tmp, 1f);
            m_isShow = true;
        }
    }
    void DragBtn() {
        Vector2 vec2Tmp = mainUI.sizeDelta;
        vec2Tmp.x = Mathf.Clamp(Input.mousePosition.x, 100, 500);
        mainUI.sizeDelta = vec2Tmp;
    }

	// Update is called once per frame
    //void Update () {

    //}
}
