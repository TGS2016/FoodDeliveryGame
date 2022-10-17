using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerZomatoApp : MonoBehaviour
{
    private PhotonView PV;
    private int myID;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            CommonReferences.Instance.myZomatoApp = this;
            myID = PV.ViewID;
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
