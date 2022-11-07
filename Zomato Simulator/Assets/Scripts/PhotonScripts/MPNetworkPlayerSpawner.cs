using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPNetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject spawnedPlayerPrefab;
  

    public void GeneratePlayer(int _no = 0, bool _custmer = false)
    {
         
        var randomNo = PhotonNetwork.PlayerList.Length - 1;
        if (randomNo >= CommonReferences.Instance.playerPoz.Length)
        {
            randomNo = Random.Range(0, CommonReferences.Instance.playerPoz.Length);
        }
        Debug.Log("GeneratePlayer " + _no);
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Player", CommonReferences.Instance.playerPoz[randomNo].position, Quaternion.identity /*MetaManager.Instance.playerPoz[randomNo].rotation*/);
       

        // if (_custmer) spawnedPlayerPrefab.GetComponent<NetworkPlayer>().myNoIs = _no;
        //CoreManager.myDefaultStartPoz = _no;
        //PhotonView photonView = GetComponent<PhotonView>();
        //NetworkManager.insta.SendUserRole(PhotonNetwork.LocalPlayer.UserId);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("I LEFT");
        if (spawnedPlayerPrefab)
        {
            PhotonNetwork.Destroy(spawnedPlayerPrefab);
            Debug.Log("Destroyed Prefab");
        }
    }

    private void OnApplicationQuit()
    {
        if (spawnedPlayerPrefab)
        {
            if(spawnedPlayerPrefab.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log("still mine");
                PhotonNetwork.Destroy(spawnedPlayerPrefab);

                Debug.Log("Destroyed Prefab");
            }
        }
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        Debug.Log("QUIT APPLICATION");
    }

}
