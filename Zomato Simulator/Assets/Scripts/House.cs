using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class House : MonoBehaviour
{
    private GameObject icon;
    private Collider2D iconCollider;
    public List<int> PendingFood = new List<int>();

    private void Awake()
    {
        icon = transform.GetChild(0).gameObject;
        iconCollider = icon.GetComponent<Collider2D>();
    }

    #region Trigger Interactions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PhotonView>().IsMine && PendingFood.Count >0)
        {
            iconCollider.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PhotonView>().IsMine)
        {
            iconCollider.enabled = false;
        }
    }
    #endregion

    #region Toggle Icon
    public void ToggleHouseIcon()
    {
        icon.SetActive(PendingFood.Count > 0);
    }
    #endregion

    #region Delivered
    public void HouseDelivered(int foodID)
    {
        if (PendingFood.Contains(foodID))
        {
            PendingFood.Remove(foodID);
        }
        if (PendingFood.Count > 0)
        {
            Debug.Log(PendingFood.Count + "orders left");
        }
        ToggleHouseIcon();
    }
    #endregion
}
