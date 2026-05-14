
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameController : MonoBehaviourPunCallbacks
{
    float spawnRate = 2f;
    float spawnTime = 0;


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
        if(PhotonNetwork.IsMasterClient)
        {
        CreateMeteoros();
        }
    }

    public void CreateMeteoros()
    {
        spawnTime += Time.deltaTime;
        if (spawnTime >= spawnRate)
        {
            float posX = Random.Range(-6, 6);
            float posY = 6f;
            Vector2 spawnPos = new Vector2(posX, posY);

            GameObject meteoroAtual = PhotonNetwork.Instantiate("Meteoro", spawnPos, Quaternion.identity);


            float speedMeteoro = Random.Range(-30f, -5f);

            meteoroAtual.GetComponent<Meteoro>().SetDirection(speedMeteoro);


            spawnTime = 0;
            spawnRate = Random.Range(0.2f, 1.5f);
        }
    }
}
