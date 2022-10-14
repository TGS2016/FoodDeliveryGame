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
    public static List<Transform> Houses = new List<Transform>();
    public static List<Restaurant> Restaurants = new List<Restaurant>();

    public List<Sprite> foodTypes = new List<Sprite>();

    public PhotonView myPV;
    public PlayerController myPlayer;
    public PlayerZomatoApp myZomatoApp;
    public Inventory myInventory;

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
            var h = HouseParent.GetChild(temp);
            Houses.Add(h);
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




    public static Action OnOrderDispatched;
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
}
