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
    private Sprite foodSprite;
    

    private void Update()
    {
        if (orderDetails != null)
        {
            Timer.rectTransform.localScale = new Vector3(1, orderDetails.HotPlateTimer / 60, 1);
        }
    }
}
