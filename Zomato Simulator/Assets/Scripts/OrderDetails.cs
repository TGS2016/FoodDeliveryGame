using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class OrderDetails : MonoBehaviour, IPunObservable
{
    public int DriverID = -1;
    public int FoodPicID = -1;
    public int HomeID = -1;
    public int RestaurantID = -1;

    public float HotPlateTimer;
    public float RatingTimer = 60;

    public bool FreeForAll = false;
    public bool isPickedUp = false;

    public Sprite foodPic;
    public Sprite Client;
    public Transform DeliveryAddress;

    public bool isInitialized = false;
    private bool FoodHasBeenAdded= true;

    public void InitializeOrder(int DriverID, int foodPicIndex, int RestaurantID)
    {
        this.DriverID = DriverID;
        this.FoodPicID = foodPicIndex;
        this.RestaurantID = RestaurantID;

        this.HomeID = Random.Range(0, CommonReferences.Houses.Count);


        this.DeliveryAddress = CommonReferences.Houses[HomeID];
        this.foodPic = CommonReferences.Instance.foodTypes[foodPicIndex];

        this.HotPlateTimer = Vector2.Distance(DeliveryAddress.position, Vector2.zero) * 10;

        //CommonReferences.Restaurants[RestaurantID].Orders.Add(this);
        CommonReferences.HouseNumOfOrders[HomeID]++;
        CommonReferences.Restaurants[RestaurantID].AddThisInList(this);
        CommonReferences.Restaurants[RestaurantID].UpdateRestaurantStatus();
        isInitialized = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(isPickedUp);
            stream.SendNext(DriverID);
            stream.SendNext(FoodPicID);
            stream.SendNext(HomeID);
            stream.SendNext(RestaurantID);

            stream.SendNext(HotPlateTimer);
            stream.SendNext(RatingTimer);

        }
        else
        {
            isPickedUp = (bool)stream.ReceiveNext();
            if (!isInitialized)
            {
                DriverID = (int)stream.ReceiveNext();
                FoodPicID = (int)stream.ReceiveNext();
                HomeID = (int)stream.ReceiveNext();
                RestaurantID = (int)stream.ReceiveNext();
                HotPlateTimer = (float)stream.ReceiveNext();
                RatingTimer = (float)stream.ReceiveNext();

                FoodHasBeenAdded = false;
                if (DriverID != -1)
                {
                    isInitialized = true;
                }
            }
            if (!isPickedUp && !FoodHasBeenAdded)
            {
                if(FoodPicID != -1)
                {
                    Debug.Log("why are you being called twice");
                    FoodHasBeenAdded = true;
                    this.foodPic = CommonReferences.Instance.foodTypes[FoodPicID];
                    CommonReferences.HouseNumOfOrders[HomeID]++;
                    CommonReferences.Restaurants[RestaurantID].AddThisInList(this);
                    CommonReferences.Restaurants[RestaurantID].UpdateRestaurantStatus();
                }
            }
        }
    }

    private void Update()
    {
        if (HotPlateTimer > 0)
        {
            HotPlateTimer -= Time.deltaTime;
        }
    }
}
