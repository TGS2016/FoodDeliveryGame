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

    private bool _acceptingOrders = true;
    private GameObject PendingOrdersIcon;

    public static Action<int, int> OnOrderPickedUp;
    public static Action<int, int> OnOrderReceived;

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

    private void Start()
    {
        UpdateRestaurantStatus();
    }

    private void Update()
    {
        // AcceptingOrders = Orders.Count < AcceptingCapacity;
        //PendingOrdersIcon.SetActive(Orders.Count > 0);


    }


    public void UpdateRestaurantStatus()
    {
        AcceptingOrders = Orders.Count < AcceptingCapacity;
        PendingOrdersIcon.SetActive(Orders.Count > 0);
    }
    //rpc goes here
    internal void OrderPickedUp(int orderID)
    {
        int RestaurantID = CommonReferences.Restaurants.IndexOf(this);
        CommonReferences.Instance.myZomatoApp.RPC_Order_PickedUP(orderID,RestaurantID);
        CommonReferences.Instance.myInventory.PickUpFood(Orders[orderID]);
        Orders.RemoveAt(orderID);
        UpdateRestaurantStatus();
    }

    internal void RemoveThisFromList(int orderID)
    {
        if(orderID < Orders.Count)
        {
            Orders.RemoveAt(orderID);
            int RestaurantID = CommonReferences.Restaurants.IndexOf(this);
            OnOrderPickedUp?.Invoke(orderID, RestaurantID);
            UpdateRestaurantStatus();
        }
    }

    internal void AddThisInList(OrderDetails orderDetails)
    {
        Orders.Add(orderDetails);
        int orderID = Orders.IndexOf(orderDetails);
        int RestaurantID = CommonReferences.Restaurants.IndexOf(this);
        OnOrderReceived?.Invoke(orderID, RestaurantID);
    }

    public void OrderRecieved(int DriverID)
    {   
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
