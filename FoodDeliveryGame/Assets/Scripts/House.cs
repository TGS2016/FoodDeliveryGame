using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class House : MonoBehaviour
{
    private GameObject icon;
    private Collider2D iconCollider;
    public List<OrderDetails> PendingFood = new List<OrderDetails>();
    //public List<int> PendingNumOfFood = new List<int>();
    public List<SpriteRenderer> ClientPic = new List<SpriteRenderer>();

    private int HomeID = -1;
    private void Awake()
    {
        icon = transform.GetChild(0).gameObject;
        iconCollider = icon.GetComponent<Collider2D>();
        HomeID = transform.GetSiblingIndex();

        EventTrigger eventTrigger = this.transform.GetChild(0).gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }

    private void OnPointerClickDelegate(PointerEventData data)
    {
        CommonReferences.Instance.myInventory.OpenBag(transform.GetSiblingIndex());
    }

    #region Trigger Interactions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine && PendingFood.Count >0)
        {
            StayingNear = true;
            //iconCollider.enabled = true;

            //
            
            

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PhotonView>() == null) return;

        if (collision.GetComponentInParent<PhotonView>().IsMine)
        {
            StayingNear = false;
            // iconCollider.enabled = false;
        }
    }
    #endregion

    #region Toggle Icon
    public void ToggleHouseIcon()
    {
        icon.SetActive(PendingFood.Count > 0);
        if (PendingFood.Count > 0)
        {
            SetClientPic(PendingFood[0].ClientPic);
        }
        else
        {
            if (UIManager.Instance.pointer.Target == this.transform)
            {
                UIManager.Instance.pointer.Target = null;
            }
        }

    }
    #endregion

    #region Delivered
    public void HouseDelivered(OrderDetails food)
    {
        if (PendingFood[0].FoodPicID == (food.FoodPicID))
        {
            PendingFood.Remove(food);
            StartCoroutine(UIManager.Instance.tutorialCO("gave food"));
        }
        if (PendingFood.Count > 0)
        {
            Debug.Log(PendingFood.Count + "orders left");
        }
        ToggleHouseIcon();
    }
    #endregion

    bool StayingNear = false;
    float DeliveryTimer;
    private void Update()
    {
        if(StayingNear && PendingFood.Count > 0)
        {
            if (UIManager.Instance.getStatedeliveredUI) { DeliveryTimer = 0; return; }
            DeliveryTimer += Time.deltaTime;

            if (DeliveryTimer > 2)
            {
                if (CommonReferences.Instance.myInventory.myPickedUpFood.Contains(PendingFood[0]))
                {
                    DeliveryTimer = 0;
                    CommonReferences.Instance.myInventory.foodButtonOnClickMethod(this.HomeID, PendingFood[0]);
                }
            }
        }
        else
        {
            DeliveryTimer = 0;
        }
    }
    public void SetClientPic(List<Sprite> ClientFeatures)
    {
        for (int i = 0; i < ClientFeatures.Count; i++)
        {
            int temp = i;
            ClientPic[temp].sprite = ClientFeatures[temp];
        }
    }
}
