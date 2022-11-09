using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPointer : MonoBehaviour
{
    public Image img;
    public Transform Target { get; set; }
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        showDirection();
    }

    public void showDirection()
    {
        if (Target == null)
        {
            if (img.gameObject.activeSelf)
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

        if(!isOffScreen())
        {
            if (UIManager.Instance.PlayingTutorial && !UIManager.Instance.tutStepInProgress)
            {
                if (Target.CompareTag("Restaurant") )
                {
                    int ID = UIManager.Instance.Step.FindIndex(x => x.Code == "found restaurant");
                    if (!UIManager.Instance.Step[ID].SkipThisStep)
                    {
                        UIManager.Instance.Step[ID].ObjectToPoint = Target;
                        StartCoroutine(UIManager.Instance.tutorialCO("found restaurant"));
                    }
                }
                if (Target.CompareTag("House"))
                {
                    int ID = UIManager.Instance.Step.FindIndex(x => x.Code == "found house");
                    if (!UIManager.Instance.Step[ID].SkipThisStep)
                    {
                        UIManager.Instance.Step[ID].ObjectToPoint = Target;
                        StartCoroutine(UIManager.Instance.tutorialCO("found house"));
                    }
                    
                }
            }
        }
    }

    private bool isOffScreen()
    {
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(Target.transform.position);
        float borderSize = 100;
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;
        return isOffScreen;
    }
}
