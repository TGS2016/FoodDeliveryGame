using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Cinemachine;
using Random = UnityEngine.Random;

public class CommonReferences : MonoBehaviour
{

    
    public static CommonReferences Instance;
    public Transform HouseParent;
    public Transform RestaurantParent;
    public Transform GasStationparent;
    public List<Sprite> foodTypes = new List<Sprite>();

    public static List<House> Houses = new List<House>();
    public static List<Restaurant> Restaurants = new List<Restaurant>();
    public static List<Transform> GasStations = new List<Transform>();
    public static RandomSample ClientGenerator;

    public Transform OrderUIParent;

    public PhotonView myPV;
    public PlayerController myPlayer;
    public CarController myCar;

    public PlayerZomatoApp myZomatoApp;
    public Inventory myInventory;

    public static Action OnOrderDispatched;
    public static Action<int> OnDisplayHouse;

    public CinemachineVirtualCamera camera_player;
    public CinemachineVirtualCamera camera_car;
    public CinemachineVirtualCamera camera_map;

    [SerializeField] List<Transform> toScaleObjectsOn = new List<Transform>();
    [SerializeField] List<Transform> toScaleObjectsOn_RestarentIcons = new List<Transform>();

    public Transform[] playerPoz;

    [SerializeField] CAMERA_TYPE lastCamera;
    public void SwitchCamera(CAMERA_TYPE whichCamera)
    {
        switch (whichCamera)
        {
            case CAMERA_TYPE.CAR:
                {
                    lastCamera = whichCamera;
                    camera_car.Priority = 10;
                    camera_player.Priority = 0;
                    camera_map.Priority = 0;
                    break;
                }
               
            case CAMERA_TYPE.PLAYER:
                {
                    lastCamera = whichCamera;
                    camera_car.Priority = 0;
                    camera_player.Priority = 10;
                    camera_map.Priority = 0;
                    break;
                }
            case CAMERA_TYPE.MAP:
                {
                    camera_car.Priority = 0;
                    camera_player.Priority = 0;
                    camera_map.Priority = 10;                  
                     
                    break;
                }
        }
    }

    bool isMapOpen = false;
    public void ToggleMap()
    {
        if (!isMapOpen)
        {
            if (myPlayer.DisableInputs) myPlayer.DisableInputs = false;
            if (myCar.DisableInputs) myCar.DisableInputs = false;


            isMapOpen = true;
            StartCoroutine(UIManager.Instance.tutorialCO("map in"));
            if (myPlayer._pState == PlayerState.WORLD)
            {
                myPlayer.canMove = false;
            }
            else
            {
                myCar.canDrive=false;
            }

            MouseMover.drag = true;
            SwitchCamera(CAMERA_TYPE.MAP);
            for (int i = 0; i < toScaleObjectsOn.Count; i++)
            {
                LeanTween.cancel(toScaleObjectsOn[i].gameObject);
                LeanTween.scale(toScaleObjectsOn[i].gameObject, Vector3.one *2, 0.8f);
            }
            for (int i = 0; i < toScaleObjectsOn_RestarentIcons.Count; i++)
            {
                LeanTween.cancel(toScaleObjectsOn_RestarentIcons[i].gameObject);
                LeanTween.scale(toScaleObjectsOn_RestarentIcons[i].gameObject, Vector3.one * 6, 0.8f);
            }
        }
        else
        {

            isMapOpen = false;

            if (myPlayer._pState == PlayerState.WORLD)
            {
                myPlayer.canMove = true;
            }
            else
            {
                myCar.canDrive = true;
            }


            MouseMover.drag = false;
            myPlayer.canMove = true;
            SwitchCamera(lastCamera);

            for (int i = 0; i < toScaleObjectsOn.Count; i++)
            {
                LeanTween.cancel(toScaleObjectsOn[i].gameObject);
                LeanTween.scale(toScaleObjectsOn[i].gameObject, Vector3.one,0.8f);
            }
            for (int i = 0; i < toScaleObjectsOn_RestarentIcons.Count; i++)
            {
                LeanTween.cancel(toScaleObjectsOn_RestarentIcons[i].gameObject);
                LeanTween.scale(toScaleObjectsOn_RestarentIcons[i].gameObject, Vector3.one * 4, 0.8f);
            }
        }
    }   

    internal void SetupCameras()
    {
        camera_car.Follow = myCar.transform;
        camera_car.LookAt = myCar.transform;


        camera_player.Follow = myPlayer.transform;
        camera_player.LookAt = myPlayer.transform;

    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        PhotonNetwork.SendRate = 10;

        #region populate houses
        int HouseCount = HouseParent.childCount;
        for (int i = 0; i < HouseCount; i++)
        {
            int temp = i;
            var h = HouseParent.GetChild(temp).GetComponent<House>(); 
            Houses.Add(h);
            //PendingOrdersForHouse.Add(0);
        }
        #endregion
        #region populate Restaurants
        int RestaurantCount = RestaurantParent.childCount;
        if (RestaurantCount > 0)
        {
            for (int i = 0; i < RestaurantCount; i++)
            {
                int temp = i;
                var r = RestaurantParent.GetChild(temp).GetComponent<Restaurant>();
                Restaurants.Add(r);
            }
        }
        #endregion
        #region populate GasStations
        int GasStationCount = GasStationparent.childCount;
        if (GasStationCount > 0)
        {
            for (int i = 0; i < GasStationCount; i++)
            {
                int temp = i;
                var G = GasStationparent.GetChild(temp);
                GasStations.Add(G);
            }
        }
        #endregion
        #region ClientGenerator
        ClientGenerator = GetComponent<RandomSample>();
        #endregion
    }

    #region Map Management
  
    #endregion

    private void OnEnable()
    {
        OnDisplayHouse += DisplayHouseIcon;
    }
    private void OnDisable()
    {
        OnDisplayHouse -= DisplayHouseIcon;
    }

    #region DispatchOrder
    public bool firstOrder = true;
    public void DispatchOrder(int DriverID)
    {
        List<Restaurant> AcceptingRestaurants = new List<Restaurant>();


        foreach (var item in Restaurants)
        {
            if (item.AcceptingOrders)
            {
                if (firstOrder)
                {
                    if (Vector2.Distance(item.transform.position, myPlayer.transform.position) < 60)
                    {
                        AcceptingRestaurants.Add(item);
                    }
                }
                else
                {
                    
                    AcceptingRestaurants.Add(item);
                }
            }
        }

        if (AcceptingRestaurants.Count > 0)
        {
            int RestaurantID = Random.Range(0, AcceptingRestaurants.Count);
            Restaurant RS = AcceptingRestaurants[RestaurantID];
            RS.OrderRecieved(DriverID);
            OnOrderDispatched?.Invoke();
            firstOrder = false;
        }
        else
        {
            Debug.Log("All Restaurants are full");
        }
    }
    #endregion

    #region DisplayHouse
    public void DisplayHouseIcon(int HouseID)
    {
        if (HouseID != -1)
        {
            Houses[HouseID].ToggleHouseIcon();
            //UIManager.Instance.pointer.Target = (Houses[HouseID].transform);
        }
    }
    #endregion

    #region HouseDelivered
    public void HouseDelivered(OrderDetails foodID ,int HouseID)
    {
        Houses[HouseID].HouseDelivered(foodID);
    }
    #endregion
}

public enum CAMERA_TYPE
{
    CAR,PLAYER,MAP
}