using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class OrderDetails : MonoBehaviour, IPunObservable
{
    #region data
    public int Reward = 1;
    public float RewardMultiplier = 0.2f;

    public int DriverID = -1;
    public int FoodPicID = -1;
    public int HomeID = -1;
    public int RestaurantID = -1;

    public float HotPlateTimer= 60;
    public float InitialTimer= 60;

    public float window= 60;
    public float RatingTimer = 60;

    public bool FreeForAll = false;
    private bool _isPickedUp;
    public bool isPickedUp
     {
         get { return _isPickedUp; }
         set
         {
             _isPickedUp = value;
            if (isPickedUp)
            {
                var myInv = CommonReferences.Instance.myInventory;
                Debug.Log(1);
                if (myInv.MyDispatchedOrders.Contains(this) )
                {
                    Debug.Log(2);
                    myInv.MyDispatchedOrders.Remove(this);
                }
                
            }
        }
     }
    public Sprite foodPic;
    public Sprite Client;
    public Transform DeliveryAddress;

    public bool isInitialized = false;
    private bool FoodHasBeenAdded= false;

    public GameObject OrderPrefab;

    public GameObject myUIPrefab;
    public GameObject myOrderListPrefab;
    private PhotonView myPV;

    private Stars _rating = Stars.five;
    public Stars rating { get { return _rating; } set
        {
            _rating = value;
            if(myUIPrefab!=null)
            myUIPrefab.GetComponent<FoodIconDetailsHolder>().CheckState();
            if(myOrderListPrefab!=null)
            myOrderListPrefab.GetComponent<FoodIconDetailsHolder>().CheckState();
        }
    }

    public static System.Action<OrderDetails> onOtherPlayerPickedUP;
    private void OnEnable()
    {
        myPV = GetComponent<PhotonView>();
    }
    private void OnDisable()
    {
        if(myUIPrefab != null)
        {
            Destroy(myUIPrefab);
        }
    }
    public void InitializeOrder(int DriverID, int foodPicIndex, int RestaurantID)
    {
        this.DriverID = DriverID;
        this.FoodPicID = foodPicIndex;
        this.RestaurantID = RestaurantID;

        this.HomeID = Random.Range(0, CommonReferences.Houses.Count);
        if (CommonReferences.Instance.firstOrder)
        {
            while (Vector2.Distance(CommonReferences.Houses[this.HomeID].transform.position, CommonReferences.Restaurants[RestaurantID].transform.position) > 50)
            {
                Debug.Log("tried finding new house");
                this.HomeID = Random.Range(0, CommonReferences.Houses.Count);
            }
        }

        Debug.Log(Vector2.Distance(CommonReferences.Houses[this.HomeID].transform.position, CommonReferences.Restaurants[RestaurantID].transform.position));
        this.DeliveryAddress = CommonReferences.Houses[HomeID].transform;
        this.foodPic = CommonReferences.Instance.foodTypes[foodPicIndex];

        var pickupPos = CommonReferences.Houses[HomeID].transform.position;
        var deliverPos = CommonReferences.Restaurants[RestaurantID].transform.position;

        Reward = (int)(Vector2.Distance(pickupPos, deliverPos) * RewardMultiplier);
        this.gameObject.name = Reward.ToString();

        GetRandomPerson();
        InstantiateInUI();
        CommonReferences.Restaurants[RestaurantID].AddThisInList(this);
        CommonReferences.Restaurants[RestaurantID].UpdateRestaurantStatus();
        isInitialized = true;
        FoodHasBeenAdded = true;
        
    }
    #endregion
    #region sync data
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(isPickedUp);
            stream.SendNext(DriverID);
            stream.SendNext(FoodPicID);
            stream.SendNext(HomeID);
            stream.SendNext(RestaurantID);

            stream.SendNext(HotPlateTimer);
            stream.SendNext(RatingTimer);
            stream.SendNext((int)rating);

        }
        else
        {
            if (!myPV.IsMine)
            {
                isPickedUp = (bool)stream.ReceiveNext();
                if (!isPickedUp)
                {

                    if (!isInitialized)
                    {
                        DriverID = (int)stream.ReceiveNext();
                        FoodPicID = (int)stream.ReceiveNext();
                        HomeID = (int)stream.ReceiveNext();
                        RestaurantID = (int)stream.ReceiveNext();
                        HotPlateTimer = (float)stream.ReceiveNext();
                        RatingTimer = (float)stream.ReceiveNext();
                        rating = (Stars)stream.ReceiveNext();


                        //FoodHasBeenAdded = false;
                        if (DriverID != -1)
                        {
                            isInitialized = true;
                        }
                    }
                    if (!FoodHasBeenAdded)
                    {
                        if (FoodPicID != -1)
                        {
                            Debug.Log("why are you being called twice");
                            FoodHasBeenAdded = true;
                            GetRandomPerson();
                            this.foodPic = CommonReferences.Instance.foodTypes[FoodPicID];
                            //CommonReferences.PendingOrdersForHouse[HomeID]++;
                            CommonReferences.Restaurants[RestaurantID].AddThisInList(this);
                            CommonReferences.Restaurants[RestaurantID].UpdateRestaurantStatus();
                        }
                    }
                }
            }
        }
    }
    #endregion

    private void Update()
    {
        if (rating == Stars.one && isPickedUp) return;
        if (HotPlateTimer > 0)
        {
            HotPlateTimer -= Time.deltaTime;
            HotPlateTimer = Mathf.Clamp(HotPlateTimer, 0, HotPlateTimer);
        }
        else
        {
            rating++;
            if (rating == (Stars.one + 1) && !isPickedUp)
            {
                rating = Stars.five;
                HotPlateTimer += InitialTimer;
                window = InitialTimer;
                return;
            }
            HotPlateTimer += RatingTimer;
            window = RatingTimer;
        }
    }

    public void InstantiateInUI()
    {
        
        myUIPrefab = Instantiate(OrderPrefab, CommonReferences.Instance.OrderUIParent);
        var myIconDetails = myUIPrefab.GetComponent<FoodIconDetailsHolder>();
        var myUIButton = myUIPrefab.GetComponent<Button>();


        myIconDetails.orderDetails = this;
        /*if (!myIconDetails.CircularTimer)
        { myUIButton.enabled = false; }*/
        myUIPrefab.transform.SetParent(CommonReferences.Instance.OrderUIParent);
        myUIPrefab.transform.SetAsLastSibling();

        CommonReferences.Instance.myInventory.MyDispatchedOrders.Add(this);
    }

    public List<Sprite> ClientPic = new List<Sprite>();
    private void GetRandomPerson()
    {
        ClientPic = CommonReferences.ClientGenerator.GetClientPic(); 
    }

    public void TransferDataToNewOrder(OrderDetails NewOrder)
    {

        NewOrder.HomeID = this.HomeID;
        NewOrder.DeliveryAddress = this.DeliveryAddress;
        NewOrder.ClientPic = this.ClientPic;

        int temp = this.Reward;
        this.Reward = NewOrder.Reward;
        NewOrder.Reward = temp;

        NewOrder.myUIPrefab.GetComponent<FoodIconDetailsHolder>().orderDetails = NewOrder;

        int originalHouseID = CommonReferences.Houses[HomeID].PendingFood.IndexOf(this);
        CommonReferences.Houses[HomeID].PendingFood[originalHouseID] = NewOrder;
    }

    public void TransferOwnership()
    {
        myPV.RequestOwnership();
    }    
}

public enum Stars
{
    five,
    four,
    three,
    two,
    one
}

