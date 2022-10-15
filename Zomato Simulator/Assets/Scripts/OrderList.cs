using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderList : MonoBehaviour
{
    public GameObject FoodIconPrefab;
    public GameObject EmptyPanel;
    private Transform MenuCard;
    private int RestaurantID;

    private void Awake()
    {
        MenuCard = transform.GetChild(0);
    }

    private void OnEnable()
    {
        CommonReferences.Instance.myPlayer.canMove = false;
        Restaurant.OnOrderPickedUp += OnRemoveItem;
        Restaurant.OnOrderReceived += OnAddItem;
    }
    private void OnDisable()
    {
        DestroyOrderList();
        CommonReferences.Instance.myPlayer.canMove = true;
        Restaurant.OnOrderPickedUp -= OnRemoveItem;
        Restaurant.OnOrderReceived -= OnAddItem;
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
            OnAddItem(temp, RestaurantID);
        }
        #endregion
    }
    private void OnClickMethod(Button button, Restaurant RS)
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
    }
    private void OnAddItem(int OrderID, int RestaurantID)
    {
        
        if (this.RestaurantID == RestaurantID)
        {
            if (MenuCard.gameObject.activeSelf)
            {
                Restaurant RS = CommonReferences.Restaurants[RestaurantID];
                Transform food = Instantiate(FoodIconPrefab).transform;
                Button button = food.GetComponent<Button>();
                
                food.SetParent(MenuCard);
                food.GetComponent<Image>().sprite = RS.Orders[OrderID].foodPic;
                Debug.Log("sprite should be assigned here : " + RS.Orders[OrderID].foodPic.name);
                Debug.Log("food should be assigned here : " + food.name);


                button.onClick.AddListener(() =>
                {
                    OnClickMethod(button, RS);
                });
                CheckAndOpenEmptyPanel();
            }
        }
    }
    private void OnRemoveItem(int OrderID, int RestaurantID)
    {
        if (this.RestaurantID == RestaurantID)
        {
            if (MenuCard.childCount >= OrderID && MenuCard.gameObject.activeSelf)
            {
                DestroyImmediate(MenuCard.GetChild(OrderID).gameObject);
                CheckAndOpenEmptyPanel();
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
        Debug.Log("Trying to Pick up order num : " + orderID);
        if (CommonReferences.Instance.myInventory.myPickedUpFood.Count < CommonReferences.Instance.myInventory.MaxFoodCapacity)
        {
            RS.OrderPickedUp(orderID);
        }
    }
    public void OnOthersOrderClicked(Restaurant RS, GameObject button)
    {
        Debug.Log("Not My order");
        OnTryPickOrder(RS, button.gameObject);
    }
    public void CheckAndOpenEmptyPanel()
    {
        EmptyPanel.SetActive(MenuCard.childCount == 0);
    }
}
