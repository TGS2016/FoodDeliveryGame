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
    public bool GetInteractButton()
    {
        return _PlayerInputAction.Player.F.WasPressedThisFrame();
    }
}
