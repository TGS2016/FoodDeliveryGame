using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _input;
    private Rigidbody2D _rb2d;

    public float speed;
    [SerializeField] PhotonView PV;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb2d = GetComponent<Rigidbody2D>();
        if (PV.IsMine)
        {
            CommonReferences.Instance.myPlayer = this;
        }
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
        {
            Move();
        }
    }

    private void Move()
    {
        _rb2d.MovePosition(_rb2d.position + _input.GetPlayerMovement() * Time.fixedDeltaTime * speed);
    }

    internal void RPC_Order_PickedUP(int orderID, int RestaurantID)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_OrderPickedUp", RpcTarget.Others, orderID, RestaurantID);
        }
    }

    [PunRPC]
    public void RPC_OrderPickedUp(int orderID, int RestaurantID)
    {
        CommonReferences.Restaurants[RestaurantID].RemoveThisFromList(orderID);
    }


}
