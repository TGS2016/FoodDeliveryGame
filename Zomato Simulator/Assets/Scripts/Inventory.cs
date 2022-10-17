using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class Inventory : MonoBehaviour
{
    public Transform ItemDataHolder;
    public Transform BagParent;
    public GameObject FoodIcon;
    public int MaxFoodCapacity = 3;
    public List<OrderDetails> myPickedUpFood = new List<OrderDetails>();

    private void Awake()
    {
        if (CommonReferences.Instance != null)
        {
            CommonReferences.Instance.myInventory = this;
        }
    }

    public void PickUpFood(OrderDetails pickedUpFood)
    {
        pickedUpFood.GetComponent<PhotonView>().RequestOwnership();
        myPickedUpFood.Add(pickedUpFood);
        pickedUpFood.isPickedUp = true;
        pickedUpFood.transform.parent = ItemDataHolder.transform;

        //CommonReferences.Houses[pickedUpFood.HomeID].PendingOrders++;
        CommonReferences.Houses[pickedUpFood.HomeID].PendingFood.Add(pickedUpFood.FoodPicID);
        //CommonReferences.PendingOrdersForHouse[pickedUpFood.HomeID]++;
        CommonReferences.OnDisplayHouse?.Invoke(pickedUpFood.HomeID);
    }

    public void OpenBag(int HouseID_ofClickedHouse)
    {
        BagParent.gameObject.SetActive(true);
        DisplayInventory(HouseID_ofClickedHouse);
        CommonReferences.Instance.myPlayer.canMove = false;
    }

    public void DisplayInventory(int HouseID_ofClickedHouse)
    {
        int Items = myPickedUpFood.Count;
        for (int i = 0; i < Items; i++)
        {
            int temp = i;
            Transform food = Instantiate(FoodIcon).transform;
            Button foodButton = food.GetComponent<Button>();
            food.SetParent(BagParent.GetChild(0));
            food.GetComponent<Image>().sprite = myPickedUpFood[temp].foodPic;

            foodButton.onClick.AddListener(() =>
            {
                foodButtonOnClickMethod(HouseID_ofClickedHouse, foodButton);
            });

        }
    }

    public void foodButtonOnClickMethod(int HouseID_ofClickedHouse, Button foodButton)
    {
        int FoodID = foodButton.transform.GetSiblingIndex();
        if (CompareHouses(FoodID, HouseID_ofClickedHouse))
        {
            Debug.Log("Item Delivered");
            OrderDetails DeliveredFood = myPickedUpFood[FoodID];
            CommonReferences.Instance.HouseDelivered(HouseID_ofClickedHouse, DeliveredFood.FoodPicID);


            myPickedUpFood.RemoveAt(FoodID);
            DestroyImmediate(DeliveredFood.gameObject);
            DestroyImmediate(foodButton.gameObject);

        }
    }

    private bool CompareHouses(int FoodID ,int HouseID_ofClickedHouse)
    {
        OrderDetails Order = myPickedUpFood[FoodID];
        House House = CommonReferences.Houses[HouseID_ofClickedHouse];

        return House.PendingFood.Contains(Order.FoodPicID);
    }

    public void CloseBag()
    {
        BagParent.gameObject.SetActive(false);
        Transform ItemHolder = BagParent.GetChild(0);
        int orderCount = ItemHolder.childCount;
        for (int i = 0; i < orderCount; i++)
        {
            DestroyImmediate(ItemHolder.GetChild(0).gameObject);
        }
        CommonReferences.Instance.myPlayer.canMove = true;
    }

    #region misc not implemented
    /*public override void OnLeftRoom()
    {
        base.OnLeftRoom();
       *//* for (int i = 0; i < myPickedUpFood.Count; i++)
        {
            int temp = i;
            if (myPickedUpFood[temp].gameObject != null && myPickedUpFood[temp].gameObject.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(myPickedUpFood[temp].gameObject);
                Debug.Log(myPickedUpFood[temp].gameObject);
            }
        }*//*
    }*/
    #endregion


}
