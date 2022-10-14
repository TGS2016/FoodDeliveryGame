using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderDispatcher : MonoBehaviour
{
    public static OrderDispatcher Instance;

    public List<Restaurant> RestaurantList = new List<Restaurant>();
    public static Action OnOrderDispatched;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        var restaurants = FindObjectsOfType<Restaurant>();
        if (restaurants.Length > 0)
        {
            for (int i = 0; i < restaurants.Length; i++)
            {
                int temp = i;
                if (!RestaurantList.Contains(restaurants[temp]))
                {
                    RestaurantList.Add(restaurants[temp]);
                }
            }
        }
    }


    //this method in rpc
    [ContextMenu("Dispatch Order")]
    public void DispatchOrder(int DriverID)
    {
        List <Restaurant> AcceptingRestaurants = new List<Restaurant>();
        foreach (var item in RestaurantList)
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
