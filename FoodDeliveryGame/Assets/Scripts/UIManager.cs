using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public OrderList orderList;
    public UIPointer pointer;
    public Slider fuelSlider;    

    public string username;
    public int user_char;


    public TMP_Text CoinCountText;
    public bool getStatedeliveredUI => delivered_ui.activeInHierarchy;
    [SerializeField] GameObject carPointerBTN;

    public void ToggleCarPointerBTN(bool enabled)
    {
        carPointerBTN.GetComponent<Button>().interactable = enabled;
    }

    #region Tutorial Section
    [Header("Tutorial")]
    [SerializeField] public List<TutorialSteps> Step = new List<TutorialSteps>();
    [SerializeField] GameObject TutorialPanel;
    //[SerializeField] GameObject TutorialHand;
    [SerializeField] Vector3 Tut_Init_Pos;
    [SerializeField] TMP_Text tutorialText;
    public bool PlayingTutorial { get; set; }
    public bool tutStepInProgress { get; set; }

    public void EnableSteps()
    {
        PlayerPrefs.SetInt("TutDone", 1);
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

        if (Step[TutID].SkipThisStep || Step[TutID].AdminSkip)
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
        AudioManager.Instance.playTutorialSound(Step[TutID].clip);
        LeanTween.move(TutorialPanel.GetComponent<RectTransform>(), Tut_Init_Pos + new Vector3(0, -280, 0), 1.25f).setEaseOutQuad();
       
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
    public IEnumerator tutorialCO(String StepCode)
    {
        int ID = Step.FindIndex(x => x.Code == StepCode);
        while (tutStepInProgress)
        {
            yield return new WaitForEndOfFrame();
        }

        if (PlayingTutorial && !Step[ID].SkipThisStep && !Step[ID].AdminSkip)
        {
            if (StepCode == "find restaurant")
            {
                CommonReferences.Instance.myPlayer.DisableInputs = true;
                CommonReferences.Instance.myCar.DisableInputs = true;
            }
            OpenTutorialPanel(ID);
        }
    }


    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

       
        //LeanTween.alpha(TutorialHand, 0, 0.01f);
    }
    [SerializeField] Button PendingPanelClosebutton;
    [SerializeField] Button BagPanelCloseButton;
    private void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Its over UI elements");
        }
        else
        {
            Debug.Log("Its NOT over UI elements");
            if (Input.GetMouseButtonDown(0))
            {
                CloseFoodPanels();
            }
        }        
    }

    public void CloseFoodPanels()
    {
        if (PendingPanelClosebutton.gameObject.activeSelf)
        {
            PendingPanelClosebutton.onClick.Invoke();
        }
        if (BagPanelCloseButton.gameObject.activeSelf)
        {
            BagPanelCloseButton.onClick.Invoke();
        }
    }

    public void ToggleGameplayUI(bool enabled)
    {
        gameplay_ui.SetActive(enabled);
    }
    public void ToggleStartUI(bool enabled)
    {
        start_ui.SetActive(enabled);
    }
    public void UpdateUserName(string _name, string _ethad = null)
    {
        if (_ethad != null)
        {
            usernameText.text = "Hi, " + _name + "\n  Your crypto address is : " + _ethad;
            username = _name;
        }
        else usernameText.text = _name;
    }
    internal void UpdateStatus(string status)
    {
        Debug.Log(status);
        //throw new NotImplementedException();
    }


    #region VoiceChat
    [Header("VoiceChat")]
    [SerializeField] FrostweepGames.WebGLPUNVoice.Recorder recorder;

  
    [SerializeField] FrostweepGames.WebGLPUNVoice.Listener lister;
    [SerializeField] Image recorderImg;
    [SerializeField] Image listenerImg;
    [SerializeField] Sprite[] recorderSprites; //0 on 1 off
    [SerializeField] Sprite[] listenerSprites; //0 on 1 off
    public void MuteUnmute()
    {
        if (recorder.recording)
        {
            recorder.recording = false;
            recorderImg.sprite = recorderSprites[1];
            recorder.StopRecord();
        }
        else
        {
            recorder.RefreshMicrophones();
            recorder.recording = true;
            recorder.StartRecord();
            recorderImg.sprite = recorderSprites[0];
        }
    }

    public void MuteUnmuteListner()
    {
        if (lister._listening)
        {
            lister._listening = false;
            listenerImg.sprite = listenerSprites[1];
        }
        else
        {
            lister._listening = true;
            listenerImg.sprite = listenerSprites[0];
        }
    }
    #endregion

    #region fuel Area
    [Header("FUEL AREA")]
    [SerializeField] GameObject FuelRechargeBTN;

    public void ToggleRechargeButton(bool enable,int price=0)
    {
        FuelRechargeBTN.SetActive(enable);
        FuelRechargeBTN.transform.GetChild(0).GetComponent<TMP_Text>().text ="Cost : "+ price + "-" + (price + 2).ToString() ;
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


    #region Button Helper

    public void ShowNearestFuel()
    {
        var nearest = Mathf.Infinity;
        var GS = CommonReferences.GasStations;
        var MC = CommonReferences.Instance.myCar;
        Transform nearestStation = null;
        foreach (var item in GS)
        {
            var distance = Vector2.Distance(item.position, MC.transform.position);

            if(distance < nearest)
            {
                nearestStation = item;
            }
        }

        if (nearestStation != null)
        {
            pointer.Target = nearestStation;
        }
    }

    public void ShowMyCar()
    {
        pointer.Target = CommonReferences.Instance.myCar.transform;
    }

    public void EmergencyFuel()
    {
        var mycar = CommonReferences.Instance.myCar;
        var rechargeCost = 50;



        if(DatabaseManager.Instance.GetLocalData().coins >= rechargeCost)
        {
            if (mycar.currentFuel / mycar.maxFuel < 0.15f)
            {
                mycar.currentFuel += mycar.maxFuel * 0.25f;
                LocalData data = DatabaseManager.Instance.GetLocalData();
                data.coins -= rechargeCost;
                DatabaseManager.Instance.UpdateData(data);
            }
            else
            {
                MessageBox.insta.showMsg("You have Enough Fuel!\nCan't user emergency fuel now.", true);
            }
        }
        else
        {
            MessageBox.insta.showMsg("Not Enough Coins! Now go on foot to deliver food.", true);
        }
    }
    #endregion


    #region Data Refresh UI SHOW
    public void UpdatePlayerUIData(bool _show, LocalData data, bool _init = false)
    {
        if (_show)
        {
            //scoreTxt.text = data.coins.ToString();
            // if (PhotonNetwork.LocalPlayer.CustomProperties["health"] != null) healthSlider.value = float.Parse(PhotonNetwork.LocalPlayer.CustomProperties["health"].ToString());
        }
    }
    #endregion

    #region Panel Management
    [Space(20f)]
    [Header("Panels")]
    [SerializeField] GameObject login_ui;
    [SerializeField] GameObject start_ui;
    [SerializeField] GameObject gameplay_ui;
    [SerializeField] GameObject start_ui_btns;
    [SerializeField] GameObject editprofile_ui;

    [Header("Buttons")]
    [SerializeField] GameObject loginui_btns;

    public void StartGame()
    {
        //start_ui.SetActive(false);
        //StartUI.SetActive(false);
       
       
        MPNetworkManager.insta.OnConnectedToServer();
       
        //MPNetworkManager.insta.OnConnectedToServer();
    }
    #endregion

    #region Edit Profile Section
    [SerializeField] Toggle[] char_toggles;
    [SerializeField] TMP_InputField name_input;
    public void OpenEditProfile()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();

        name_input.text = data.name;
        for (int i = 0; i < char_toggles.Length; i++)
        {
            if (i == data.char_id)
            {
                char_toggles[data.char_id].isOn = true;
                break;
            }
        }

        start_ui_btns.SetActive(false);
        editprofile_ui.SetActive(true);
    }
    public void SetProfile()
    {
        if (string.IsNullOrEmpty(name_input.text)) return;

        LocalData data = DatabaseManager.Instance.GetLocalData();

        data.name = name_input.text;
        for (int i = 0; i < char_toggles.Length; i++)
        {
            if (char_toggles[i].isOn)
            {
                data.char_id = i;
                break;
            }
        }
        DatabaseManager.Instance.UpdateData(data);


        start_ui_btns.SetActive(true);
        editprofile_ui.SetActive(false);
        UpdateUserName(data.name, SingletonDataManager.userethAdd);
    }
    #endregion

    [Space(20f)]
    [Header("Informaion (Login)")]
    [SerializeField] TMP_Text usernameText;
    [SerializeField] TMP_Text statusText;

    [Header("Informaion (InGame)")]
    [SerializeField] GameObject information_box;
    [SerializeField] TMP_Text information_text;
    [SerializeField] Image information_image;
    Coroutine info_coroutine;

   

    public void ShowInformationMsg(string msg, float time, Sprite image = null)
    {
        if (image != null)
        {
            information_image.sprite = image;
            information_image.gameObject.SetActive(true);
        }
        else
        {
            information_image.gameObject.SetActive(false);
        }

        information_text.text = msg;

        if (info_coroutine != null)
        {
            StopCoroutine(info_coroutine);
        }
        info_coroutine = StartCoroutine(disableInformationMsg(time));
    }
    IEnumerator disableInformationMsg(float time)
    {
        LeanTween.cancel(information_box);

        information_box.SetActive(true);
        LeanTween.scaleY(information_box, 1, 0.15f).setFrom(0);
       // AudioManager.Instance.playSound(0);

        yield return new WaitForSeconds(time);

        LeanTween.scaleY(information_box, 0, 0.15f).setOnComplete(() => {
            information_box.SetActive(false);
        });


    }

    #region Coin Texts
    [SerializeField] TMP_Text[] coin_texts;
    [SerializeField] TMP_Text[] token_texts;
    public void SetCoinText()
    {
        int coins = DatabaseManager.Instance.GetLocalData().coins;
        for (int i = 0; i < coin_texts.Length; i++)
        {
            coin_texts[i].text = coins.ToString();
        }
    }
    public void SetTokenBalanceText()
    {
        for (int i = 0; i < token_texts.Length; i++)
        {
            token_texts[i].text = EvmosManager.userTokenBalance;
        }
    }
    #endregion

    #region Food Delivered UI
    public GameObject delivered_ui;
    int reward;
    [SerializeField] TMP_Text reward_coins_text;
    public void ShowOrderDeliveredPanel(int coinsReward)
    {
        reward = coinsReward;
        reward_coins_text.text = reward.ToString();
        delivered_ui.SetActive(true);
        ClaimCoins();
        
    }
    public void ClaimCoins()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        data.coins += reward;
        DatabaseManager.Instance.UpdateData(data);
        
        SetCoinText();

    }
    public void ClaimToken()
    {
        delivered_ui.SetActive(false);
        EvmosManager.Instance.getDailyToken();
        
    }
    public void IgnoreToken()
    {
        delivered_ui.SetActive(false);
    }
    public void ClaimExtraCoins()
    {
        Debug.LogWarning("IMPLEMENT TOKEN HERE");

        delivered_ui.SetActive(false);
    }


    #endregion

    #region DAILY SPIN SYSYTEM
    [SerializeField] GameObject fortuneWheelUI;
    public void ToggleSpinUI(bool activate)
    {
        fortuneWheelUI.SetActive(activate);

        if (!activate)
        {
            bool tutorialDone = PlayerPrefs.GetInt("TutDone", 0) == 1;
            if (!tutorialDone)
            {
                UIManager.Instance.EnableSteps();
            }
        }
    }
    #endregion

    [SerializeField] GameObject LoadingPanel;
    public void ToggleLoadingPanel(bool _show)
    {
        LoadingPanel.SetActive(_show);
    }

}
