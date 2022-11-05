using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    internal object usergender;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
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


}
