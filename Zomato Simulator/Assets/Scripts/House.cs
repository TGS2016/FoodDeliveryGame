using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;


public class House : MonoBehaviour
{
    private GameObject icon;
    private Collider2D iconCollider;
    public List<OrderDetails> PendingFood = new List<OrderDetails>();
    //public List<int> PendingNumOfFood = new List<int>();
    public List<SpriteRenderer> ClientPic = new List<SpriteRenderer>();

    private int HomeID = -1;
    private void Awake()
    {
        icon = transform.GetChild(0).gameObject;
        iconCollider = icon.GetComponent<Collider2D>();
        HomeID = transform.GetSiblingIndex();
    }

    #region Trigger Interactions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("tis got called");
        if (collision.GetComponentInParent<PhotonView>().IsMine && PendingFood.Count >0)
        {
            Debug.Log("tis got called 2");
            iconCollider.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>().IsMine)
        {
            iconCollider.enabled = false;
        }
    }
    #endregion

    #region Toggle Icon
    public void ToggleHouseIcon()
    {
        icon.SetActive(PendingFood.Count > 0);
        if (PendingFood.Count > 0)
        {
            SetClientPic(PendingFood[0].ClientPic);
        }

    }
    #endregion

    #region Delivered
    public void HouseDelivered(OrderDetails food)
    {
        if (PendingFood[0].FoodPicID == (food.FoodPicID))
        {
            PendingFood.Remove(food);
        }
        if (PendingFood.Count > 0)
        {
            Debug.Log(PendingFood.Count + "orders left");
        }
        ToggleHouseIcon();
    }
    #endregion


    public void SetClientPic(List<Sprite> ClientFeatures)
    {
        for (int i = 0; i < ClientFeatures.Count; i++)
        {
            int temp = i;
            ClientPic[temp].sprite = ClientFeatures[temp];
        }
    }
}
