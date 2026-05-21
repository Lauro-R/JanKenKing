using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum GameChoices { None, Rock, Paper, Scissors }

public class GameController : MonoBehaviourPunCallbacks
{

    public static GameController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {

            Destroy(this.gameObject);
        }
        else
        {

            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    float spawnRate = 2f;
    float spawnTime = 0;

    [SerializeField]
    Text scoreText;

    public string nomeWon;

    int score = 0;
    int scoreMax = 3;

    int rock = 0;
    int paper = 1;
    int scissors = 2;


    int MasterChoice;
    int ChallengerChoice;

    /*[SerializeField]
    GameObject playerPrefab;*/
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("0. Start");
        float posX = Random.Range(-9, 9);
        float posY = Random.Range(-2, 2);
        Vector2 spawnPos = new Vector2(posX, posY);
        PhotonNetwork.Instantiate("Player", spawnPos, this.transform.rotation);
        //Instantiate(playerPrefab);
        AtualizarUI();

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
        Debug.Log("2. UpdateScore");
        score += newScore;
        scoreText.text = "Score: " + score;
        }



    public void DrawChoices() //To Do: Mover esse Draw Choices para um script separado utilizando o enumerator que o ivan fez pros pokemons de referencia na aula 11
    {
        Debug.Log("3. DrawChoices");
        if (MasterChoice == ChallengerChoice)
        {
            Debug.Log("3. No One Wins - MasterChoice == ChallengerChoice");

        }
        else if (
            (MasterChoice == rock && ChallengerChoice == scissors) ||
            (MasterChoice == paper && ChallengerChoice == rock) ||
            (MasterChoice == scissors && ChallengerChoice == paper)
        )
        {
            Debug.Log("3. DrawChoices P1 - MasterChoice != ChallengerChoice");
            UpdateScore(1);//Player 1 Wins
        }
        else if (ChallengerChoice == rock && MasterChoice == scissors ||
            ChallengerChoice == paper && MasterChoice == rock ||
            ChallengerChoice == scissors && MasterChoice == paper)
        {
            Debug.Log("3. DrawChoices P2 - Challenger != MasterChoice");
            UpdateScore(1);//Player 2 Wins
        }
    }

    public void InputFromButton(int choice)
    {
        MasterChoice = choice;

        if (ChallengerChoice != 0)
        {
            DrawChoices();
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("4. OnPlayerPropertiesUpdate");
        AtualizarUI();
        if(PhotonNetwork.IsMasterClient)
        {
            VerificarFimDeJogo(targetPlayer);
        }
    }
    void AtualizarUI()
    {
        Debug.Log("5. AtualizarUI");
        string novoTexto = "";
        int contador = 0;

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            contador++;
            novoTexto += player.CustomProperties["Nick"] + ": " + player.CustomProperties["Score"] + "  ";
        }
        scoreText.text = novoTexto;
    }
    void VerificarFimDeJogo(Player player)
    {
        object scoreAtualizado;
        if (player.CustomProperties.TryGetValue("Score", out scoreAtualizado))
        {
            if ((int)scoreAtualizado >= scoreMax)
            {
                object nome;
                if (player.CustomProperties.TryGetValue("Nick", out nome))
                {
                    nomeWon = (string)nome;
                }
                Debug.Log("Fim de jogo!");
                AtualizarRoomProperties(nomeWon);
                //PhotonNetwork.LoadLevel(2);
            }
        }
    }

    void AtualizarRoomProperties(string nomeWon)
    {
        Hashtable roomProperties = new Hashtable();
        roomProperties.Add("Won", nomeWon);
        roomProperties.Add("Fim", true);

        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            return;
        }
        object fim;
        if (propertiesThatChanged.TryGetValue("Fim", out fim))
        {
            if ((bool)fim == true)
            {
                PhotonNetwork.LoadLevel(2);
            }
        }
    }
    public void ResetSingleton()
    {
        Destroy(gameObject);
    }
}
