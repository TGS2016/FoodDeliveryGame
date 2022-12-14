using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnableIcon : MonoBehaviour
{
    private Collider2D iconCollider;

    private void Awake()
    {
        iconCollider = transform.GetChild(0).GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var PV = collision.GetComponentInParent<PhotonView>();
        if ( PV != null)
        {
            if (PV.IsMine)
            {
                iconCollider.enabled = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var PV = collision.GetComponentInParent<PhotonView>();
        if (PV != null)
        {
            if (PV.IsMine)
            {
                iconCollider.enabled = false;
            }
        }
    }
}
