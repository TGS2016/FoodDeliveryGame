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

        
        CommonReferences.Houses[pickedUpFood.HomeID].PendingFood.Add(pickedUpFood);
        //CommonReferences.Houses[pickedUpFood.HomeID].PendingNumOfFood.Add(pickedUpFood.FoodPicID);
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
            var food = Instantiate(this.FoodIcon).transform;
            var foodButton = food.GetComponent<Button>();
            var foodDetails = food.GetComponent<FoodIconDetailsHolder>();
            foodDetails.ClientFeatures[0].transform.parent.gameObject.SetActive(false);

            foodDetails.orderDetails = myPickedUpFood[temp];
            food.SetParent(BagParent.GetChild(0));

            foodButton.onClick.AddListener(() =>
            {
                foodButtonOnClickMethod(HouseID_ofClickedHouse, foodDetails.gameObject);
            });

        }
    }

    public void foodButtonOnClickMethod(int HouseID_ofClickedHouse, GameObject foodDetails)
    {
        int FoodID = foodDetails.transform.GetSiblingIndex();
        OrderDetails ClickedFood = myPickedUpFood[FoodID];
        House House = CommonReferences.Houses[HouseID_ofClickedHouse];
        if (DidTheyOrderThisFood(ClickedFood, House))
        {
            if (!IsOwnerSame(ClickedFood, House))
            {
                var actualFood = myPickedUpFood.Find(x => x.HomeID == HouseID_ofClickedHouse);
                ClickedFood.TransferDataToNewOrder(actualFood);
            }
            Debug.Log("Item being delivered");


            CommonReferences.Instance.HouseDelivered(ClickedFood,HouseID_ofClickedHouse);

            Debug.Log("removed now");
            myPickedUpFood.RemoveAt(FoodID);

            DestroyImmediate(ClickedFood.gameObject);
            DestroyImmediate(foodDetails.gameObject);

        }
    }

    private bool DidTheyOrderThisFood(OrderDetails Order ,House House)
    {
        if (House.PendingFood.Count == 0) return false;
        else
        return House.PendingFood[0].FoodPicID == Order.FoodPicID;//.Contains(Order.FoodPicID);
    }

    private bool IsOwnerSame(OrderDetails Food, House House)
    {
        if (House.PendingFood.Count == 0) return false;
        else
        return Food.HomeID == CommonReferences.Houses.IndexOf(House);
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
