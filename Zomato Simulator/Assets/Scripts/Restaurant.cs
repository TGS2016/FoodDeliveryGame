using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restaurant : MonoBehaviour
{
    public List<OrderDetails> FoodServed = new List<OrderDetails>();
    public List<OrderDetails> Orders = new List<OrderDetails>();
    public int AcceptingCapacity = 5;

    private bool _acceptingOrders;
    private GameObject PendingOrdersIcon;

    public static Action<Restaurant> OnOrderRecieved;

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
        Orders.RemoveAt(orderID);
    }

    public void OrderRecieved(int DriverID)
    {
        /* int OrderID = UnityEngine.Random.Range(0, FoodServed.Count);
         Orders.Add(FoodServed[OrderID]);
         OnOrderRecieved?.Invoke(this);*/

        //OrderDetails order = new OrderDetails(DriverID, OrderDispatcher.Instance.gameObject, null);
    }
}
