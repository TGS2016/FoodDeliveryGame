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
    private PhotonView PV;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb2d = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            CommonReferences.Instance.myPlayer = this;
            CommonReferences.Instance.myPV = this.PV;
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
}
