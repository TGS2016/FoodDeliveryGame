using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderList : MonoBehaviour
{
    public GameObject Food;
    public void ShowOrders(Restaurant RS)
    {
        this.gameObject.SetActive(true);
        int Items = RS.Orders.Count;
        for (int i = 0; i < Items; i++)
        {
            int temp = i;
            GameObject food = Instantiate(Food);
            food.transform.parent = transform.GetChild(0);
            food.GetComponent<Image>().sprite = RS.Orders[temp].foodPic;

            Button button = food.GetComponent<Button>();

            button.onClick.AddListener(() =>
            {
                RS.OrderPickedUp(button.gameObject);
                DestroyImmediate(button.gameObject);
                if(transform.GetChild(0).childCount == 0 )
                {
                    Debug.Log("nothing to show here");
                }
            });
        }
    }

    private void OnDisable()
    {
        DestroyOrderList();
    }

    private void DestroyOrderList()
    {
        int orderCount = transform.GetChild(0).childCount;
        for (int i = 0; i < orderCount; i++)
        {
            DestroyImmediate(transform.GetChild(0).GetChild(0).gameObject);
        }
    }
}
