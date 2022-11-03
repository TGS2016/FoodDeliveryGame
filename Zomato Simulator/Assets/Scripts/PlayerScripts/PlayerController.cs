using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _input;
   

    public float speed;
    private PhotonView PV;
    public bool canMove;


    //CAR DATA
    public int selected_car;
    public int selected_car_color;

    private Rigidbody2D _rb2d;
    [SerializeField] SpriteRenderer player_sr;
    [SerializeField] Collider2D playerCollider;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb2d = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {

            GameObject car = PhotonNetwork.Instantiate("Car", this.transform.position + new Vector3(3, 0, 0), Quaternion.identity);

            if (CommonReferences.Instance) {
                CommonReferences.Instance.myPlayer = this;
                CommonReferences.Instance.myPV = this.PV;
                CommonReferences.Instance.myCar = car.GetComponent<CarController>();
                CommonReferences.Instance.myCar.SetupCar(selected_car, selected_car_color);
            }            

        }
        canMove = true;
    }

    private void Update()
    {
        if(canEnterCar && _input.GetInteractButton())
        {
            canEnterCar = false;
            //Enter Car
            //Debug.Log(_input.GetInteractButton());
            TogglePlayer(false);
            CommonReferences.Instance.myCar.ToggleCar(true);
        }

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            selected_car++;
            if (selected_car >= AllCarInfo.Instance.allCarInfo.Count)
            {
                selected_car = 0;
            }

            CommonReferences.Instance.myCar.SetupCar(selected_car,selected_car_color);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            selected_car_color++;
            if (selected_car_color >= 8)
            {
                selected_car_color = 0;
            }
            CommonReferences.Instance.myCar.SetupCar(selected_car, selected_car_color);
        }
#endif
    }
    
    public void TogglePlayer(bool enabled)
    {        
        Debug.Log("player toggled" + enabled);

        /*this.gameObject.SetActive(enabled);*/
        player_sr.enabled = enabled;
        _rb2d.isKinematic = !enabled;
        playerCollider.enabled = enabled;

        canMove = enabled;    
    }

    private void FixedUpdate()
    {
        if (PV.IsMine && canMove)
        {
            Move();
        }
    }

    private void Move()
    {
        _rb2d.MovePosition(_rb2d.position + _input.GetPlayerMovement() * Time.fixedDeltaTime * speed);
    }

    [SerializeField] bool canEnterCar=false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("car") && collision.GetComponent<PhotonView>().IsMine)
        {
            canEnterCar = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("car") && collision.GetComponent<PhotonView>().IsMine)
        {
            canEnterCar = false;
        }
    }
}
