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

    #region open Menu
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
        if (MenuCard.childCount == 0) return;

        if (UIManager.Instance.PlayingTutorial)
        {
            var target = RS.Orders.FindIndex(x => x.DriverID == CommonReferences.Instance.myPV.ViewID);
            if(target != -1)
            {
                int ID = UIManager.Instance.Step.FindIndex(x => x.Code == "pick up my order");
                UIManager.Instance.Step[ID].ObjectToPoint = MenuCard.GetChild(target);
                StartCoroutine(UIManager.Instance.tutorialCO("pick up my order"));
            }
        }

        if (UIManager.Instance.PlayingTutorial)
        {
            var target = RS.Orders.FindIndex(x => x.DriverID != CommonReferences.Instance.myPV.ViewID);
            if (target != -1)
            {
                int ID = UIManager.Instance.Step.FindIndex(x => x.Code == "pick up other order");
                UIManager.Instance.Step[ID].ObjectToPoint = MenuCard.GetChild(target);
                StartCoroutine(UIManager.Instance.tutorialCO("pick up other order"));
            }
        }


    }
    private void OnAddItem(int OrderID, int RestaurantID)
    {

        if (this.RestaurantID == RestaurantID)
        {
            if (MenuCard.gameObject.activeSelf)
            {
                var RS = CommonReferences.Restaurants[RestaurantID];
                var food = Instantiate(FoodIconPrefab).transform;
                var FoodDetails = food.GetComponent<FoodIconDetailsHolder>();
                var button = food.GetComponent<Button>();

                FoodDetails.orderDetails = CommonReferences.Restaurants[RestaurantID].Orders[OrderID];
                FoodDetails.orderDetails.myOrderListPrefab = food.gameObject;
                //FoodDetails.foodSprite = RS.Orders[OrderID].foodPic;
                food.SetParent(MenuCard,false);
               /* Debug.Log("sprite should be assigned here : " + RS.Orders[OrderID].foodPic.name);
                Debug.Log("food should be assigned here : " + food.name);*/


                button.onClick.AddListener(() =>
                {
                    OnClickMethod(FoodDetails, RS);
                });
                CheckAndOpenEmptyPanel();
            }
        }
    }
    #endregion

    #region order Select
    private void OnClickMethod(FoodIconDetailsHolder FoodDetails, Restaurant RS)
    {
        int orderID = FoodDetails.transform.GetSiblingIndex();
        if (RS.Orders[orderID].DriverID != CommonReferences.Instance.myPV.ViewID && !RS.Orders[orderID].FreeForAll)
        {
            /*OnOthersOrderClicked(RS, FoodDetails.gameObject);*/

            //picking someone else's order
            OnTryPickOrder(RS, FoodDetails.gameObject, false);
        }
        else
        {
            //picking our own order
            OnTryPickOrder(RS, FoodDetails.gameObject,true);
        }
    }

    private void OnTryPickOrder(Restaurant RS, GameObject FoodDetails, bool isMyOrder = true)
    {
        if (CommonReferences.Instance.myInventory.myPickedUpFood.Count < CommonReferences.Instance.myInventory.BagSize)
        {
            int orderID = FoodDetails.transform.GetSiblingIndex();
            if (!isMyOrder)
            {
                Debug.Log("Not My order");
                RS.Orders[orderID].InstantiateInUI();
            }
            Debug.Log("Trying to Pick up order num : " + orderID);


            RS.OrderPickedUpByMyself(orderID);

            //UIManager.Instance.pointer.Target = null;


        }
        else
        {
            UIManager.Instance.ShowInformationMsg("Bag Is Full! Deliver some food items",2f);
        }

    }

    /*public void OnOthersOrderClicked(Restaurant RS, GameObject FoodDetails)
    {
        Debug.Log("Not My order");
        OnTryPickOrder(RS, FoodDetails.gameObject, false);
    }*/

    #endregion

    #region Order Remove
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
    #endregion

    #region Misc
    public void CheckAndOpenEmptyPanel()
    {
        //EmptyPanel.SetActive(MenuCard.childCount == 0);
        if (MenuCard.childCount == 0)
        {
            this.gameObject.SetActive(false);
            CommonReferences.Instance.myInventory.PlayBagTutorial();
        }
    }
    #endregion
}
