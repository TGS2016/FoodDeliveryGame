using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FoodIconDetailsHolder : MonoBehaviour
{

    private OrderDetails _OD;
    public OrderDetails orderDetails
    {
        get
        { return _OD; }
        set
        {
            _OD = value;
            foodSprite = orderDetails.foodPic;
            foodImage.sprite = foodSprite;
            /*DeliveryAddress = CommonReferences.Houses[_OD.HomeID];
            PickupAddress = CommonReferences.Restaurants[_OD.RestaurantID];*/
            for (int i = 0; i < ClientFeatures.Count; i++)
            {
                int temp = i;
                ClientFeatures[temp].sprite = orderDetails.ClientPic[temp];
            }
            CheckState();

        }
    }

    [SerializeField] TMP_Text Rating;
    [SerializeField] List<Image> Timer = new List<Image>();
    public List<Image> ClientFeatures = new List<Image>();

    public Image foodImage;
    public Image currentTimer;
    public bool CircularTimer;
    private Sprite foodSprite;


    private void Update()
    {
        if (orderDetails != null)
        {
            currentTimer.fillAmount = orderDetails.HotPlateTimer / orderDetails.window;

            if (!CircularTimer)
                CheckState();
        }
    }

    internal void CheckState()
    {
        int rating = (int)orderDetails.rating;

        switch (rating)
        {
            case 0:
                currentTimer = Timer[0];
                break;
            case 1:
                Timer[0].fillAmount = 0;
                currentTimer = Timer[1];
                break;
            case 2:
                Timer[0].fillAmount = 0;
                Timer[1].fillAmount = 0;
                currentTimer = Timer[2];
                break;
            case 3:
                Timer[0].fillAmount = 0;
                Timer[1].fillAmount = 0;
                Timer[2].fillAmount = 0;
                currentTimer = Timer[3];
                break;
            case 4:
                Timer[0].fillAmount = 0;
                Timer[1].fillAmount = 0;
                Timer[2].fillAmount = 0;
                Timer[3].fillAmount = 0;
                break;
        }

        Rating.text = (5-rating).ToString();

    }

   /* private void ShowDistance()
    {
        float DistanceToReach = 0;
        if (orderDetails.isPickedUp)
        {
            DistanceToReach = ((Vector2.Distance(CommonReferences.Instance.myPlayer.transform.position, DeliveryAddress.transform.position) - 5) * DistanceConstant);
        }
        else
        {
            DistanceToReach = ((Vector2.Distance(CommonReferences.Instance.myPlayer.transform.position, PickupAddress.transform.position) - 5) * DistanceConstant);
        }
        DistanceToReach = Mathf.Clamp(DistanceToReach, 0, DistanceToReach);
        distance.text = DistanceToReach.ToString("F1") + "m";
    }*/


    public void ShowLocation()
    {
        if (orderDetails == null)
            return;
        Transform locationToShow = null;
        if(orderDetails.isPickedUp)
        {
            locationToShow = CommonReferences.Houses[orderDetails.HomeID].transform;
        }
        else
        {
            locationToShow = CommonReferences.Restaurants[orderDetails.RestaurantID].transform;
        }

        UIManager.Instance.pointer.ShowDirection(locationToShow);
    }
}
