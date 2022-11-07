using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class OrderDetails : MonoBehaviour, IPunObservable
{
    #region data
    public int DriverID = -1;
    public int FoodPicID = -1;
    public int HomeID = -1;
    public int RestaurantID = -1;

    public float HotPlateTimer= 60;
    public float window= 60;
    public float RatingTimer = 60;

    public bool FreeForAll = false;
    [SerializeField]private bool _isPickedUp;
     public bool isPickedUp
     {
         get { return _isPickedUp; }
         set
         {
             _isPickedUp = value;
            if (isPickedUp)
            {
                var myInv = CommonReferences.Instance.myInventory;
                if (myInv.MyDispatchedOrders.Contains(this) && !myInv.myPickedUpFood.Contains(this))
                {
                    myInv.MyDispatchedOrders.Remove(this);
                    Destroy(this.gameObject);
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
    private PhotonView myPV;

    private Stars _rating = Stars.five;
    public Stars rating { get { return _rating; } set
        {
            _rating = value;
            myUIPrefab.GetComponent<FoodIconDetailsHolder>().CheckState();
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


        this.DeliveryAddress = CommonReferences.Houses[HomeID].transform;
        this.foodPic = CommonReferences.Instance.foodTypes[foodPicIndex];

        //this.HotPlateTimer = Vector2.Distance(DeliveryAddress.position, Vector2.zero) * 10;

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

        }
        else
        {
            if (!myPV.IsMine)
            {
                isPickedUp = (bool)stream.ReceiveNext();
                Debug.Log(isPickedUp);
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
        if (rating == Stars.one) return;
        if (HotPlateTimer > 0)
        {
            HotPlateTimer -= Time.deltaTime;
        }
        else
        {
            rating++;
            Debug.Log(rating);
            if (rating == Stars.one) return;
            HotPlateTimer += RatingTimer;
            window = RatingTimer;
        }
    }

    public void InstantiateInUI()
    {
        
        myUIPrefab = Instantiate(OrderPrefab);
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
        Debug.LogWarning(NewOrder == null) ;
        NewOrder.HomeID = this.HomeID;
        NewOrder.DeliveryAddress = this.DeliveryAddress;
        NewOrder.ClientPic = this.ClientPic;

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

