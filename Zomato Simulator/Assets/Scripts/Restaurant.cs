using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Restaurant : MonoBehaviour
{
    public List<Sprite> FoodServed = new List<Sprite>();
    public List<OrderDetails> Orders = new List<OrderDetails>();

    public int AcceptingCapacity = 5;

    private bool _acceptingOrders;
    private GameObject PendingOrdersIcon;

    public static Action<Restaurant> OnOrderRecieved;

    public bool isInitialized = false;
    private void Awake()
    {
        PendingOrdersIcon = transform.GetChild(0).gameObject;
        
    }
    public bool AcceptingOrders
    {
        get { return _acceptingOrders; }
        set { _acceptingOrders = value; }
    }

    private void Update()
    {
        AcceptingOrders = Orders.Count < AcceptingCapacity;
        PendingOrdersIcon.SetActive(Orders.Count > 0);
    }

    //rpc goes here
    internal void OrderPickedUp(GameObject order)
    {
        int orderID = order.transform.GetSiblingIndex();
        int RestaurantID = CommonReferences.Restaurants.IndexOf(this);
        CommonReferences.Instance.myPlayer.RPC_Order_PickedUP(orderID,RestaurantID);
        Orders.RemoveAt(orderID);
    }

    internal void RemoveThisFromList(int orderID)
    {
        if(orderID < Orders.Count)
        {
            Orders.RemoveAt(orderID);
        }
    }

    public void OrderRecieved(int DriverID)
    {
       
        /*OnOrderRecieved?.Invoke(this);*/



        OrderDetails order =  PhotonNetwork.Instantiate("OrderPrefab", this.transform.position, Quaternion.identity).GetComponent<OrderDetails>();

        int localfoodID = UnityEngine.Random.Range(0, FoodServed.Count);
        int foodID = CommonReferences.Instance.foodTypes.IndexOf(FoodServed[localfoodID]);
        int RestaurantID = CommonReferences.Restaurants.IndexOf(this);

        Debug.Log("spawning a : " + FoodServed[localfoodID].name);
        order.InitializeOrder(DriverID, foodID, RestaurantID);
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
           /* stream.SendNext(DriverID);
            stream.SendNext(FoodPicID);
            stream.SendNext(HomeID);
            stream.SendNext(HotPlateTimer);
            stream.SendNext(RatingTimer);*/
        }
        else
        {
            if (!isInitialized)
            {
                /*DriverID = (int)stream.ReceiveNext();
                FoodPicID = (int)stream.ReceiveNext();
                HomeID = (int)stream.ReceiveNext();
                HotPlateTimer = (float)stream.ReceiveNext();
                RatingTimer = (float)stream.ReceiveNext();

                if (DriverID != -1)
                {
                    isInitialized = true;
                }*/
            }
        }
    }
}
