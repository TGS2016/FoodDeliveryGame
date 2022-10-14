using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OrderDetails : MonoBehaviour
{
    public int DriverID;
    public float HotPlateTimer;
    public float RatingTimer;
    public bool FreeForAll = false;

    public Sprite foodPic;
    public Sprite Client;
    public GameObject DeliveryAddress;

    /*public OrderDetails(int DriverID, GameObject DeliveryAddress , Sprite foodPic )
    {
        this.DriverID = DriverID;
        this.DeliveryAddress = DeliveryAddress;
        this.foodPic = foodPic;

        HotPlateTimer = Vector2.Distance(DeliveryAddress.transform.position, Vector2.zero) * 10;
    }*/
}
