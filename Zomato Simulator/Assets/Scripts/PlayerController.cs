using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _input;
    private Rigidbody2D _rb2d;

    public float speed;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        _rb2d.MovePosition(_rb2d.position + _input.GetPlayerMovement() * Time.fixedDeltaTime * speed);
    }
}
