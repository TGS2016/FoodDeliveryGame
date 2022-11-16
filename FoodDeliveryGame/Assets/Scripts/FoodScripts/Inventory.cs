using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class Inventory : MonoBehaviour,IPunOwnershipCallbacks
{
    public Transform ItemDataHolder;
    public Transform BagUIPanel;
    public Transform BagParent;
    public GameObject FoodIcon;
    public int MaxDispatchFoodCount = 5;
    public int BagSize = 3;
    public List<OrderDetails> MyDispatchedOrders = new List<OrderDetails>();
    public List<OrderDetails> myPickedUpFood = new List<OrderDetails>();



    private void Awake()
    {
        if (CommonReferences.Instance != null)
        {
            CommonReferences.Instance.myInventory = this;
        }
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        Debug.Log("transfer requested");
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView.IsMine)
        {
            Debug.Log("transfer successful");
            AddToBag(TempClickedFood);
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {

    }
    OrderDetails TempClickedFood;
    public void PickUpFood(OrderDetails pickedUpFood)
    {
        TempClickedFood = pickedUpFood;
        var foodPV = pickedUpFood.GetComponent<PhotonView>();
        if (!foodPV.IsMine)
        {
            pickedUpFood.TransferOwnership();
            return; 
        }
        AddToBag(pickedUpFood);
    }

    public void AddToBag(OrderDetails pickedUpFood)
    {
        pickedUpFood.isPickedUp = true;
        Debug.Log(3);
        myPickedUpFood.Add(pickedUpFood);

        Debug.Log("CHANGE STATE to " + pickedUpFood.isPickedUp + pickedUpFood.name);
        pickedUpFood.transform.parent = ItemDataHolder.transform;

        CommonReferences.Houses[pickedUpFood.HomeID].PendingFood.Add(pickedUpFood);
        CommonReferences.OnDisplayHouse?.Invoke(pickedUpFood.HomeID);

        pickedUpFood.myUIPrefab.transform.SetParent(BagUIPanel);

        UIManager.Instance.pointer.ChangeIcon(2);
        UIManager.Instance.pointer.Target = CommonReferences.Houses[pickedUpFood.HomeID].transform;
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
           // foodDetails.ClientFeatures[0].transform.parent.gameObject.SetActive(false);

            foodDetails.orderDetails = myPickedUpFood[temp];
            foodDetails.CheckState();
            food.SetParent(BagParent.GetChild(0),false);

            foodButton.onClick.AddListener(() =>
            {
                foodButtonOnClickMethod(HouseID_ofClickedHouse, foodDetails.gameObject);
            });
        }
        StartCoroutine(UIManager.Instance.tutorialCO("giving food"));
    }


    public int coinsReward=0;
    public void foodButtonOnClickMethod(int HouseID_ofClickedHouse, OrderDetails od)
    {
        //int FoodID = foodDetails.transform.GetSiblingIndex();
        OrderDetails ClickedFood = od;
        House House = CommonReferences.Houses[HouseID_ofClickedHouse];

        var foodtobedeleted = ClickedFood;
        var actualFood = myPickedUpFood.Find(x => x.HomeID == HouseID_ofClickedHouse);
        if (DidTheyOrderThisFood(ClickedFood, House))
        {
            int reward = ClickedFood.Reward;
            if (!IsOwnerSame(ClickedFood, House))
            {
                if (actualFood != null)
                {

                    reward = actualFood.Reward;
                    ClickedFood.TransferDataToNewOrder(actualFood);
                    ClickedFood = actualFood;
                }

            }


            coinsReward = reward;

            /*  LocalData data=DatabaseManager.Instance.GetLocalData();
              data.coins += reward;
              DatabaseManager.Instance.UpdateData(data);*/
            UIManager.Instance.ShowOrderDeliveredPanel(coinsReward);
            AudioManager.Instance.playSound(0);


            //DataHolder.Instance.CoinCount += reward;

            CommonReferences.Instance.HouseDelivered(ClickedFood, HouseID_ofClickedHouse);


            myPickedUpFood.Remove(od);

            Destroy(foodtobedeleted.gameObject);
            //Destroy(foodDetails.gameObject);

            CloseBag();
        }
        else
        {
            AudioManager.Instance.playSound(3);
            /*actualFood.Reward -= 5;
            actualFood.Reward = Mathf.Clamp(actualFood.Reward ,0, actualFood.Reward);*/
        }
    }
    public void foodButtonOnClickMethod(int HouseID_ofClickedHouse, GameObject foodDetails)
    {
        int FoodID = foodDetails.transform.GetSiblingIndex();
        OrderDetails ClickedFood = myPickedUpFood[FoodID];
        House House = CommonReferences.Houses[HouseID_ofClickedHouse];

        var foodtobedeleted = ClickedFood;
        var actualFood = myPickedUpFood.Find(x => x.HomeID == HouseID_ofClickedHouse);
        if (DidTheyOrderThisFood(ClickedFood, House))
        {
            int reward = ClickedFood.Reward;
            if (!IsOwnerSame(ClickedFood, House))
            {
                if (actualFood != null)
                {

                    reward = actualFood.Reward;
                    ClickedFood.TransferDataToNewOrder(actualFood);
                    ClickedFood = actualFood;
                }
               
            }


            coinsReward = reward;

          /*  LocalData data=DatabaseManager.Instance.GetLocalData();
            data.coins += reward;
            DatabaseManager.Instance.UpdateData(data);*/
            UIManager.Instance.ShowOrderDeliveredPanel(coinsReward);
            AudioManager.Instance.playSound(0);


            //DataHolder.Instance.CoinCount += reward;

            CommonReferences.Instance.HouseDelivered(ClickedFood,HouseID_ofClickedHouse);

            
            myPickedUpFood.RemoveAt(FoodID);

            Destroy(foodtobedeleted.gameObject);
            Destroy(foodDetails.gameObject);

            CloseBag();
        }
        else
        {
            AudioManager.Instance.playSound(3);
            /*actualFood.Reward -= 5;
            actualFood.Reward = Mathf.Clamp(actualFood.Reward ,0, actualFood.Reward);*/
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

    public void PlayBagTutorial()
    {
        if (myPickedUpFood.Count == 0) return;
        StartCoroutine(UIManager.Instance.tutorialCO("open bag panel"));
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
