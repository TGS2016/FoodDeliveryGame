using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviourPunCallbacks
{
    public int MaxFoodCapacity = 3;
    public List<OrderDetails> myPickedUpFood = new List<OrderDetails>();
    private PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
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
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
       /* for (int i = 0; i < myPickedUpFood.Count; i++)
        {
            int temp = i;
            if (myPickedUpFood[temp].gameObject != null && myPickedUpFood[temp].gameObject.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(myPickedUpFood[temp].gameObject);
                Debug.Log(myPickedUpFood[temp].gameObject);
            }
        }*/
    }


}
