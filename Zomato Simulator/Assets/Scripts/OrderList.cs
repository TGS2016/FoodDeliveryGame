using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderList : MonoBehaviour
{
    public GameObject FoodIconPrefab;
    private Transform MenuCard;
    private int RestaurantID;

    private void Awake()
    {
        MenuCard = transform.GetChild(0);
    }

    private void OnEnable()
    {
        Restaurant.OnOrderPickedUp += RefreshList;
    }
    private void OnDisable()
    {
        DestroyOrderList();
        Restaurant.OnOrderPickedUp -= RefreshList;
    }



    public void ShowOrders(Restaurant RS)
    {
        this.gameObject.SetActive(true);
        RestaurantID = CommonReferences.Restaurants.IndexOf(RS);

        #region Populate Menu
        int Items = RS.Orders.Count;
        for (int i = 0; i < Items; i++)
        {
            int temp = i;

            Transform food = Instantiate(FoodIconPrefab).transform;
            Button button = food.GetComponent<Button>();

            food.parent = MenuCard;
            food.GetComponent<Image>().sprite = RS.Orders[temp].foodPic;


            button.onClick.AddListener(() =>
            {
                int orderID = button.transform.GetSiblingIndex();
                if (RS.Orders[orderID].DriverID != CommonReferences.Instance.myPV.ViewID && !RS.Orders[orderID].FreeForAll)
                {
                    OnOthersOrderClicked(RS, button.gameObject);
                }
                else
                {
                    OnTryPickOrder(RS, button.gameObject);
                }
            });
        }
        #endregion
    }
    private void RefreshList(int OrderID, int RestaurantID)
    {
        if (this.RestaurantID == RestaurantID)
        {
            if (MenuCard.childCount >= OrderID && MenuCard.gameObject.activeSelf)
            {
                DestroyImmediate(MenuCard.GetChild(OrderID).gameObject);
            }
        }
    }
    private void DestroyOrderList()
    {
        int orderCount = MenuCard.childCount;
        for (int i = 0; i < orderCount; i++)
        {
            DestroyImmediate(MenuCard.GetChild(0).gameObject);
        }
    }

    private void OnTryPickOrder(Restaurant RS, GameObject button)
    {
        int orderID = button.transform.GetSiblingIndex();
        if (CommonReferences.Instance.myInventory.myPickedUpFood.Count < CommonReferences.Instance.myInventory.MaxFoodCapacity)
        {
            RS.OrderPickedUp(orderID);
            DestroyImmediate(button);
            if (MenuCard.childCount == 0)
            {
                Debug.Log("nothing to show here");
            }
        }
    }

    public void OnOthersOrderClicked(Restaurant RS, GameObject button)
    {
        Debug.Log("Not My order");
        OnTryPickOrder(RS, button.gameObject);
    }
}
