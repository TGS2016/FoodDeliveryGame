using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            for (int i = 0; i < ClientFeatures.Count; i++)
            {
                int temp = i;
                ClientFeatures[temp].sprite = orderDetails.ClientPic[temp];
            }

        }
    }


    public Image foodImage;
    public List<Image> ClientFeatures = new List<Image>();
    public Image Timer;
    public bool CircularTimer;
    private Sprite foodSprite;
    

    private void Update()
    {
        if (orderDetails != null)
        {
            if (CircularTimer)
            {
                Timer.fillAmount = orderDetails.HotPlateTimer / 60;
            }
            else
            {
                Timer.rectTransform.localScale = new Vector3(1, orderDetails.HotPlateTimer / 60, 1);
            }
        }
    }



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
