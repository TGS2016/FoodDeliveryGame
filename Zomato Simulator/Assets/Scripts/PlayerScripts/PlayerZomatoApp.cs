using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerZomatoApp : MonoBehaviour
{
    private PhotonView PV;
    private int myID;
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            myID = PV.ViewID;
        }
    }

    public void GetAnOrder()
    {
        CommonReferences.Instance.DispatchOrder(myID);
    }
}
