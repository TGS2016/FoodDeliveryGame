using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Random = UnityEngine.Random;

public class CommonReferences : MonoBehaviour
{
    public static CommonReferences Instance;
    public Transform HouseParent;
    public Transform RestaurantParent;
    public List<Sprite> foodTypes = new List<Sprite>();

    public static List<House> Houses = new List<House>();
    public static List<Restaurant> Restaurants = new List<Restaurant>();

    public Transform OrderUIParent;

    public PhotonView myPV;
    public PlayerController myPlayer;
    public PlayerZomatoApp myZomatoApp;
    public Inventory myInventory;

    public static Action OnOrderDispatched;
    public static Action<int> OnDisplayHouse;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        #region populate houses
        int HouseCount = HouseParent.childCount;
        for (int i = 0; i < HouseCount; i++)
        {
            int temp = i;
            var h = HouseParent.GetChild(temp).GetComponent<House>(); 
            Houses.Add(h);
            //PendingOrdersForHouse.Add(0);
        }
        #endregion
        #region populate Restaurants
        int RestaurantCount = RestaurantParent.childCount;
        if (RestaurantCount > 0)
        {
            for (int i = 0; i < RestaurantCount; i++)
            {
                int temp = i;
                var r = RestaurantParent.GetChild(temp).GetComponent<Restaurant>();
                Restaurants.Add(r);
            }
        }
        #endregion
    }

    private void OnEnable()
    {
        OnDisplayHouse += DisplayHouseIcon;
    }
    private void OnDisable()
    {
        OnDisplayHouse -= DisplayHouseIcon;
    }

    #region DispatchOrder
    public void DispatchOrder(int DriverID)
    {
        List<Restaurant> AcceptingRestaurants = new List<Restaurant>();
        foreach (var item in Restaurants)
        {
            if (item.AcceptingOrders)
                AcceptingRestaurants.Add(item);
        }

        if (AcceptingRestaurants.Count > 0)
        {
            int RestaurantID = Random.Range(0, AcceptingRestaurants.Count);
            Restaurant RS = AcceptingRestaurants[RestaurantID];
            RS.OrderRecieved(DriverID);
            OnOrderDispatched?.Invoke();
        }
        else
        {
            Debug.Log("All Restaurants are full");
        }
    }
    #endregion

    #region DisplayHouse
    public void DisplayHouseIcon(int HouseID)
    {
        if (HouseID != -1)
        {
            Houses[HouseID].ToggleHouseIcon();
        }
    }
    #endregion

    #region HouseDelivered
    public void HouseDelivered(int HouseID, int foodID)
    {
        Houses[HouseID].HouseDelivered(foodID);
    }
    #endregion
}
