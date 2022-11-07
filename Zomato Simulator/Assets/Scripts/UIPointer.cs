using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPointer : MonoBehaviour
{
    public Image img;
    public Transform Target;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if(Target == null)
        {
            if(img.gameObject.activeSelf)
                img.gameObject.SetActive(false);
            return;
        }
        img.gameObject.SetActive(isOffScreen());
        float minx = img.GetPixelAdjustedRect().width / 2;
        float maxx = Screen.width - minx; 
        
        float miny = img.GetPixelAdjustedRect().height / 2;
        float maxy = Screen.height - miny;


        Vector2 pos = mainCam.WorldToScreenPoint(Target.position);

        pos.x = Mathf.Clamp(pos.x, minx, maxx);
        pos.y = Mathf.Clamp(pos.y, miny, maxy);

        img.transform.position = pos;

    }

    private bool isOffScreen()
    {
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(Target.transform.position);
        float borderSize = 100;
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;
        return isOffScreen;
    }

    public void ShowDirection(Transform Target)
    {
        this.Target = Target;
    }
}
