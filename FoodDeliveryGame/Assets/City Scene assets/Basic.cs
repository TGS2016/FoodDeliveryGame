using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Basic : MonoBehaviour
{
    BaseInput input;
    Rigidbody2D rb;
    [SerializeField] float speed = 8;
    private void Start()
    {
        input = GetComponent<BaseInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.MovePosition(rb.position + new Vector2(input.GetAxisRaw("Horizontal"), input.GetAxisRaw("Vertical")) * speed * Time.deltaTime);
    }
}
