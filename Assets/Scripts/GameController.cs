using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameController : MonoBehaviourPunCallbacks
{

    public static GameController instance;

    private void Awake()
    {
        if (instance == null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    float spawnRate = 2f;
    float spawnTime = 0;

    [SerializeField]
    Text scoreText;

    int score = 0;

    /*[SerializeField]
    GameObject playerPrefab;*/
    // Start is called before the first frame update
    void Start()
    {
        float posX = Random.Range(-9, 9);
        float posY = Random.Range(-2, 2);
        Vector2 spawnPos = new Vector2(posX, posY);
        PhotonNetwork.Instantiate("Player", spawnPos, this.transform.rotation);
        Debug.Log(PhotonNetwork.PlayerList);
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

    public void UpdateScore(int newScore)
    {
        score += newScore;
        scoreText.text = "Score: " + score;
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        string novoTexto;
        int contador = 0;

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            contador++;
            novoTexto += player.CustomProperties["Nick"] + ": " + player.CustomProperties["Score"] + "  ";
        }
        textoScore.text = novoTexto;
    }
}
