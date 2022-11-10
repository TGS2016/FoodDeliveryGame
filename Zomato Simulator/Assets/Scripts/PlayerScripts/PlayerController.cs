using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour,IPunObservable
{
    private PlayerInput _input;
    private Rigidbody2D _rb2d;
    private Animator _animator;
    private PhotonView PV;
    private CarController myCar;
    private Direction _direction;
   

    public float speed;
    public bool canMove;


    //CAR DATA
    public int selected_car;
    public int selected_car_color;

    [SerializeField] Collider2D playerCollider;
    [SerializeField] SpriteRenderer player_sr;

    public PlayerState _pState = PlayerState.WORLD;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        

        if (PV.IsMine)
        {

            GameObject car = PhotonNetwork.Instantiate("Car", this.transform.position + new Vector3(3, 0, 0), Quaternion.identity);

            if (CommonReferences.Instance) {
                
                CommonReferences.Instance.myPlayer = this;
                CommonReferences.Instance.myPV = this.PV;
                CommonReferences.Instance.myCar= myCar = car.GetComponent<CarController>();
                CommonReferences.Instance.myCar.SetupCar(selected_car, selected_car_color);

                CommonReferences.Instance.SetupCameras();
            }

            _animator.SetFloat("GenderID", DatabaseManager.Instance.GetLocalData().char_id);
            UIManager.Instance.SetCoinText();

        }
        canMove = true;
    }

    private void Update()
    {
        if (!PV.IsMine) return;
        if (canEnterCar && _input.GetInteractButton())
        {
            canEnterCar = false;
            //Enter Car
            //Debug.Log(_input.GetInteractButton());
            TogglePlayer(false);
            myCar.ToggleCar(true);
            
            CommonReferences.Instance.SwitchCamera(CAMERA_TYPE.CAR);
        }


        if (_input.GetMapKey())
        {
            CommonReferences.Instance.ToggleMap();
        }
        Vector2 tempMov = _input.GetPlayerMovement();
        int animatorStateValue = tempMov.magnitude > 0 ? 4 : 0;
       
        if(tempMov.y != 0)
        {
            _direction = tempMov.y > 0 ? Direction.back : Direction.front;
        }
        else if (tempMov.x != 0)
        {
            _direction = tempMov.x > 0 ? Direction.right : Direction.left;
        }

        animatorStateValue += (int)_direction;
        _animator.SetFloat("State", animatorStateValue);

        

#if UNITY_EDITOR
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                selected_car++;
                if (selected_car >= AllCarInfo.Instance.allCarInfo.Count)
                {
                    selected_car = 0;
                }

                myCar.SetupCar(selected_car, selected_car_color);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                selected_car_color++;
                if (selected_car_color >= 8)
                {
                    selected_car_color = 0;
                }
                myCar.SetupCar(selected_car, selected_car_color);
            }
        }
#endif
    }

    
    public bool isPlayerenabled = true;
    public void TogglePlayer(bool enabled)
    {        
        
        //Debug.Log("player toggled" + enabled);

        /*this.gameObject.SetActive(enabled);*/
        player_sr.enabled = enabled;
        _rb2d.isKinematic = !enabled;
        playerCollider.enabled = enabled;

        if (enabled)
        {
            _pState = PlayerState.WORLD;
        }
        else{
            _pState = PlayerState.DRIVE;
        }

        canMove = enabled;
        isPlayerenabled = enabled;
       
        if (!enabled)
        {
            if(PV.IsMine)
            this.transform.parent = myCar.transform;
            //CHANGE PLAYER POSITION
        }
        else
        {
            if(PV.IsMine)
            this.transform.parent = null;
        }
        
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isPlayerenabled);
        }
        else if (stream.IsReading)
        {
            bool playerEnabled = (bool)stream.ReceiveNext();
            if(playerEnabled != isPlayerenabled)
            {
                Debug.Log("TOGGLEED");
                TogglePlayer(playerEnabled);
            }
        }
    }

    public enum Direction
    {
        back,
        front,
        left,
        right
    }
}
public enum PlayerState
{
    WORLD,DRIVE
}

