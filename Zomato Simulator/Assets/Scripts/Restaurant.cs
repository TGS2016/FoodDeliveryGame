using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class Restaurant : MonoBehaviour
{
    public List<Sprite> FoodServed = new List<Sprite>();
    public List<OrderDetails> Orders = new List<OrderDetails>();

    public int AcceptingCapacity = 5;

    private bool _acceptingOrders = true;
    private GameObject PendingOrdersIcon;

    public static Action<int, int> OnOrderPickedUp;
    public static Action<int, int> OnOrderReceived;
    public bool AcceptingOrders
    {
        get { return _acceptingOrders; }
        set { _acceptingOrders = value; }
    }

    public bool isInitialized = false;
    private void Awake()
    {
        PendingOrdersIcon = transform.GetChild(0).gameObject;
        EventTrigger eventTrigger = this.transform.GetChild(0).gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }

    private void OnPointerClickDelegate(PointerEventData data)
    {
        UIManager.Instance.orderList.ShowOrders(this);
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

    #region Order Recieved
    public void OrderRecieved(int DriverID)
    {   
        OrderDetails order =  PhotonNetwork.Instantiate("OrderDetailsPrefab", this.transform.position, Quaternion.identity).GetComponent<OrderDetails>();

        int localfoodID = UnityEngine.Random.Range(0, FoodServed.Count);
        int foodID = CommonReferences.Instance.foodTypes.IndexOf(FoodServed[localfoodID]);
        int RestaurantID = CommonReferences.Restaurants.IndexOf(this);

        Debug.Log("spawning a : " + FoodServed[localfoodID].name);
        order.InitializeOrder(DriverID, foodID, RestaurantID);
        
    }
    internal void AddThisInList(OrderDetails orderDetails)
    {
        Orders.Add(orderDetails);
        int orderID = Orders.IndexOf(orderDetails);
        int RestaurantID = CommonReferences.Restaurants.IndexOf(this);
        OnOrderReceived?.Invoke(orderID, RestaurantID);
    }

    #endregion

    #region Order Picked Up
    internal void OrderPickedUpByMyself(int orderID)
    {
        int RestaurantID = CommonReferences.Restaurants.IndexOf(this);
        CommonReferences.Instance.myZomatoApp.RPC_Order_PickedUP(orderID,RestaurantID);
        CommonReferences.Instance.myInventory.PickUpFood(Orders[orderID]);
        //OrderDetails.onOtherPlayerPickedUP?.Invoke(Orders[orderID]);
        Orders.RemoveAt(orderID);
        OnOrderPickedUp?.Invoke(orderID, RestaurantID);
        UpdateRestaurantStatus();
    }
    internal void OrderPickedUpBySomeoneElse(int orderID)
    {
        if (orderID < Orders.Count)
        {
            if (Orders[orderID].DriverID == CommonReferences.Instance.myPV.ViewID)
            {
                Debug.Log("SOMEONE STOLE MY ORDER");
            }

            Orders.RemoveAt(orderID);
            int RestaurantID = CommonReferences.Restaurants.IndexOf(this);
            OnOrderPickedUp?.Invoke(orderID, RestaurantID);
            UpdateRestaurantStatus();
        }
    }

    #endregion

    #region Misc
    public void UpdateRestaurantStatus()
    {
        AcceptingOrders = Orders.Count < AcceptingCapacity;
        PendingOrdersIcon.SetActive(Orders.Count > 0);
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

    #endregion
    //rpc goes here
}
