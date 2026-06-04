using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;

public class HandButtonManager : MonoBehaviour
{
    [SerializeField]
    Mao valorBotao;
    bool Countdown;

    [SerializeField]
    GameController _GameSingleton; //Singleton que gerencia o main menu e quando ele é clicado

    public void saveChoice()
    {
        //GetComponent<PhotonView>().RPC("saveChoice_RPC", RpcTarget.All);
        if (PhotonNetwork.IsMasterClient)
        {
            _GameSingleton.EscolherMaster(valorBotao);


        }
        else
        {
            _GameSingleton.EscolherClient(valorBotao);

        }
    }

    [PunRPC]
    public void saveChoice_RPC()
    {

    }


}
