using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(1)]
public class Inventory : MonoBehaviour
{
    public GameObject BagGameObject;
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
        myPickedUpFood.Add(pickedUpFood);
        pickedUpFood.isPickedUp = true;
        pickedUpFood.GetComponent<PhotonView>().RequestOwnership();
        pickedUpFood.transform.parent = this.transform;

        CommonReferences.OnDisplayHouse?.Invoke(pickedUpFood.HomeID);
    }

    public void OpenBag()
    {
        BagGameObject.SetActive(true);
    }

   

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


}
