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
        StartCoroutine(StartGettingOrders());
    }

    IEnumerator StartGettingOrders()
    {
        float initialDelay = Random.Range(10, 20);
        yield return new WaitForSeconds(initialDelay);
        while(true)
        {
            var inventory = CommonReferences.Instance.myInventory;
            if (inventory.MyDispatchedOrders.Count < inventory.MaxDispatchFoodCount)
            {
                GetAnOrder();

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
