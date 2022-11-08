using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public OrderList orderList;
    public UIPointer pointer;
    internal object usergender;


    #region Tutorial Section
    [Header("Tutorial")]
    [SerializeField] float ReadDuration = 10;
    [SerializeField] List<TutorialSteps> Step = new List<TutorialSteps>();
    [SerializeField] GameObject TutorialPanel;
    [SerializeField] GameObject TutorialHand;
    [SerializeField] Vector3 Tut_Init_Pos;
    [SerializeField] TMP_Text tutorialText;
    private bool _enableTutorial;
    public bool EnableTutorial { get {return _enableTutorial; } set
        {
            _enableTutorial = value;
            OpenTutorialPanel(0);
        }
    }
    public void OpenTutorialPanel(int TutID)
    {
        
        if (TutID >= Step.Count) return;

        if (Step[TutID].SkipThisStep)
        {
            TutID++;
            OpenTutorialPanel(TutID);
            return;
        }
        tutorialText.text = Step[TutID].TutorialText;
        TutorialPanel.SetActive(true);
        LeanTween.move(TutorialPanel.GetComponent<RectTransform>(), Tut_Init_Pos + new Vector3(0, -180, 0), 1.25f).setEaseOutQuad();
       
        if (Step[TutID].ObjectToPoint != null)
        {
            //TutorialHand.transform.position = Step[TutID].ObjectToPoint.position;
            var Object = Step[TutID].ObjectToPoint;
            if (Object.TryGetComponent<CanvasGroup>(out CanvasGroup CG))
            {
                LeanTween.alphaCanvas(CG, 0, 0.5f).setLoopPingPong(3);
            }
            else
            {
                LeanTween.alpha(Object.gameObject, 0, 0.5f).setLoopPingPong(3);
            }
        }
        if (Step[TutID].HasFollowUpPopUp)
        {
            
            LeanTween.value(0, 1, Step[TutID].PopUpDuration).setOnComplete(() =>
            {
                Step[TutID].SkipThisStep = true;
                TutID++;
                OpenTutorialPanel(TutID);
            });
            return;
        }
        Invoke(nameof(CloseTutorialPanel), Step[TutID].PopUpDuration);
    }



    public void CloseTutorialPanel()
    {

        LeanTween.move(TutorialPanel.GetComponent<RectTransform>(), Tut_Init_Pos, 1.25f).setEaseOutQuad().setOnComplete(()=> TutorialPanel.SetActive(false));
    }


    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        EnableTutorial = PlayerPrefs.GetInt("TutDone", 0) == 0;
        LeanTween.alpha(TutorialHand, 0, 0.01f);
    }

    internal void UpdateUserName(string name)
    {
        Debug.Log(name);
        //throw new NotImplementedException();
    }

    internal void ToggleGameplayUI(bool state)
    {
        
        //throw new NotImplementedException();
    }

    internal void UpdateStatus(string status)
    {
        Debug.Log(status);
        //throw new NotImplementedException();
    }


    [SerializeField] Transform PendingOrdersParent;
    [SerializeField] GameObject FoodUIPrefab;

    #region fuel Area
    [Header("FUEL AREA")]
    [SerializeField] GameObject FuelRechargeBTN;

    public void ToggleRechargeButton(bool enable)
    {
        FuelRechargeBTN.SetActive(enable);
    }
    public void RechargeFuel()
    {
        //COINS DEDUCT

        CommonReferences.Instance.myCar.RechargeFuel();
    }
    #endregion
    public void OpenFoodPanel(GameObject panel)
    {
        var rect = panel.GetComponent<RectTransform>();
        
        LeanTween.move(rect, Vector3.zero, 0.5f).setEaseOutQuad();
        //LeanTween.moveLocalY(panel, 250, 0.5f).setEaseOutQuad();
    }

    public void CloseFoodPanel(GameObject panel)
    {
        var rect = panel.GetComponent<RectTransform>();
        LeanTween.move(rect, new Vector3(0, -500), 0.5f).setEaseOutQuad();
        //LeanTween.moveLocalY(panel, -250, 0.5f).setEaseOutQuad();
    }

   


}
