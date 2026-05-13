
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameController : MonoBehaviourPunCallbacks
{
    /*[SerializeField]
    GameObject playerPrefab;*/
    // Start is called before the first frame update
    void Start()
    {
        float posX = Random.Range(-9, 9);
        float posY = Random.Range(-2, 2);
        Vector2 spawnPos = new Vector2(posX, posY);
        PhotonNetwork.Instantiate("Player", spawnPos, this.transform.rotation);
        //Instantiate(playerPrefab);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
