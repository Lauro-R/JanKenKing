using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum Mao { Rock = 0, Paper = 1, Scissors = 2 }

public class GameController : MonoBehaviourPunCallbacks
{

    public static GameController _GameSingleton;

    private void Awake()
    {
        if (_GameSingleton != null && _GameSingleton != this)
        {

            Destroy(this.gameObject);
        }
        else
        {

            _GameSingleton = this;
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

    public Mao MasterChoice;
    public Mao ChallengerChoice;

    int p1score;
    int p2score;

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

    public void UpdateScore(int player)
        {
        /*Debug.Log("2. UpdateScore");
        score += newScore;
        scoreText.text = "Score: " + score;*/

            if(player == 1)
            {
            p1score++;
            }
            else
            {
            p2score++;
            }
        }

    public int VerificarResultado(Mao MasterChoice, Mao ChallengerChoice)
    {
        switch ((MasterChoice, ChallengerChoice))
        {
            case (Mao.Rock, Mao.Scissors):
            case (Mao.Paper, Mao.Rock):
            case (Mao.Scissors, Mao.Paper):
                return 1;
            case (Mao.Rock, Mao.Paper):
            case (Mao.Paper, Mao.Scissors):
            case (Mao.Scissors, Mao.Rock):
                return 2;

            default:
                return 0;
        }
    }

    public void Vencedor(Mao MasterChoice, Mao ChallengerChoice)
    {
        int resultado = VerificarResultado(MasterChoice, ChallengerChoice);
        UpdateScore(resultado);


        /* má pratica de check
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
            }*/
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
