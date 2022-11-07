using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _PlayerInputAction;
   

    private void Awake()
    {
        _PlayerInputAction = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _PlayerInputAction.Enable();
    }
    private void OnDisable()
    {
        _PlayerInputAction.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return _PlayerInputAction.Player.Move.ReadValue<Vector2>();
    }
    public float GetVerticalInput()
    {
        return _PlayerInputAction.Player.Move.ReadValue<Vector2>().y;
    }
    public float GetHorizontalInput()
    {
        return _PlayerInputAction.Player.Move.ReadValue<Vector2>().x;
    }

    internal Vector2 GetMouseDelta()
    {
        return _PlayerInputAction.Player.MouseDelta.ReadValue<Vector2>();
    }

    public bool GetInteractButton()
    {
        return _PlayerInputAction.Player.F.WasPressedThisFrame();
    }

    public bool GetMapKey()
    {
        return _PlayerInputAction.Player.MapKey.WasPressedThisFrame();
    }
}
