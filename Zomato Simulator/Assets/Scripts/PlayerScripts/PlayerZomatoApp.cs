using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerZomatoApp : MonoBehaviour
{
    private PhotonView PV;
    private int myID;
    [SerializeField] private float TimeTillNextOrder;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            CommonReferences.Instance.myZomatoApp = this;
            myID = PV.ViewID;
        }
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            StartCoroutine(StartGettingOrders());
        }
    }

    IEnumerator StartGettingOrders()
    {
        float initialDelay = Random.Range(10, 20);
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            while(UIManager.Instance.tutStepInProgress)
            {
                yield return new WaitForEndOfFrame();
            }
            var inventory = CommonReferences.Instance.myInventory;
            if (inventory.MyDispatchedOrders.Count < inventory.MaxDispatchFoodCount)
            {
                GetAnOrder();
            }
            #region tutorialStep
            if (UIManager.Instance.PlayingTutorial)
            {
                int ID = UIManager.Instance.Step.FindIndex(x => x.Code == "found restaurant");
                if (!UIManager.Instance.Step[ID].SkipThisStep)
                {
                    if (CommonReferences.Instance.myInventory.MyDispatchedOrders.Count > 0)
                    {
                        UIManager.Instance.Step[ID].ObjectToPoint = CommonReferences.Instance.myInventory.MyDispatchedOrders[0].transform;
                    }
                    StartCoroutine(UIManager.Instance.tutorialCO("find restaurant"));
                }
            }
            #endregion
            if (inventory.MyDispatchedOrders.Count < inventory.MaxDispatchFoodCount)
            {
                float randomWait = Random.Range(TimeTillNextOrder - 15, TimeTillNextOrder + 15);
                yield return new WaitForSeconds(randomWait);
            }
            else
            {
                float tryAfterX = Random.Range(15, 30);
                yield return new WaitForSeconds(tryAfterX);
            }
        }
    }
    public void GetAnOrder()
    {
        CommonReferences.Instance.DispatchOrder(myID);
    }

    internal void RPC_Order_PickedUP(int orderID, int RestaurantID)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_OrderPickedUp", RpcTarget.Others, orderID, RestaurantID);
        }
    }

    [PunRPC]
    public void RPC_OrderPickedUp(int orderID, int RestaurantID)
    {
        CommonReferences.Restaurants[RestaurantID].OrderPickedUpBySomeoneElse(orderID);
    }


}
