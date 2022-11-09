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
    [SerializeField] public List<TutorialSteps> Step = new List<TutorialSteps>();
    [SerializeField] GameObject TutorialPanel;
    //[SerializeField] GameObject TutorialHand;
    [SerializeField] Vector3 Tut_Init_Pos;
    [SerializeField] TMP_Text tutorialText;
    public bool PlayingTutorial;
    public bool tutStepInProgress;

    public void EnableSteps()
    {
        PlayingTutorial = true;
        for (int i = 0; i < Step.Count; i++)
        {
            Step[i].SkipThisStep = false;
        }
        OpenTutorialPanel(0);
    }
    public void OpenTutorialPanel(int TutID)
    {
        
        if (TutID >= Step.Count) return;

        if (Step[TutID].SkipThisStep)
        {
            /*TutID++;
            OpenTutorialPanel(TutID);*/
            return;
        }
        else
        {
            Step[TutID].SkipThisStep = true;
        }

        tutStepInProgress = true;
        if (closingTweenID != -1) LeanTween.cancel(closingTweenID);
        CancelInvoke(nameof(CloseTutorialPanel));
        
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


    private int closingTweenID = -1;
    private LTDescr closingTween;
    public void CloseTutorialPanel()
    {

        closingTween = LeanTween.move(TutorialPanel.GetComponent<RectTransform>(), Tut_Init_Pos, 1.25f).setEaseOutQuad().setOnComplete(() =>
             {
                 TutorialPanel.SetActive(false);
                 tutStepInProgress = false;
             });
        closingTweenID = closingTween.id;
    }


    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        PlayingTutorial = PlayerPrefs.GetInt("TutDone", 0) == 0;
        if(PlayingTutorial)
        {
            EnableSteps();
        }
        //LeanTween.alpha(TutorialHand, 0, 0.01f);
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
        StartCoroutine(tutorialCO("close pending orders"));
    }

    public void CloseFoodPanel(GameObject panel)
    {
        var rect = panel.GetComponent<RectTransform>();
        LeanTween.move(rect, new Vector3(0, -500), 0.5f).setEaseOutQuad();
    }

    public void OpenBagPanel(GameObject panel)
    {
        var rect = panel.GetComponent<RectTransform>();

        LeanTween.move(rect, Vector3.zero, 0.5f).setEaseOutQuad();
        if (CommonReferences.Instance.myInventory.myPickedUpFood.Count == 0) return;
        int ID = Step.FindIndex(x => x.Code == "find house");
        Step[ID].ObjectToPoint = CommonReferences.Instance.myInventory.myPickedUpFood[0].myUIPrefab.transform;
        StartCoroutine(tutorialCO("find house"));
    }

    public void CloseBagPanel(GameObject panel)
    {
        var rect = panel.GetComponent<RectTransform>();
        LeanTween.move(rect, new Vector3(0, -500), 0.5f).setEaseOutQuad();
    }

    public IEnumerator tutorialCO(String StepCode)
    {
        int ID = Step.FindIndex(x => x.Code == StepCode);
        while (tutStepInProgress)
        {
            yield return new WaitForEndOfFrame();
        }

        if (PlayingTutorial && !Step[ID].SkipThisStep)
        {
            OpenTutorialPanel(ID);
        }
    }


}
