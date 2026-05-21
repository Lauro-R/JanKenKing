using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Meteoro : MonoBehaviour
{


    public void SetDirection(float velY)
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, velY);
    }



    public void Destruir()
    {
        GetComponent<PhotonView>().RPC("DestruirRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void DestruirRPC()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
