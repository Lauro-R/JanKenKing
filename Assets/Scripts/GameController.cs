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

    PhotonView myPhotonView;

    public string nomeWon;

    int score = 0;
    int scoreMax = 3;

    bool player1ready = false;
    bool player2ready = false;

    int results;

    public Mao MasterChoice;
    public Mao ChallengerChoice;

    int p1score;
    int p2score;

    /*[SerializeField]
    GameObject playerPrefab;*/
    // Start is called before the first frame update
    void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        Debug.Log("0. Start");
        float posX = Random.Range(-9, 9);
        float posY = Random.Range(-2, 2);
        Vector2 spawnPos = new Vector2(posX, posY);
        PhotonNetwork.Instantiate("Player", spawnPos, this.transform.rotation);
        //Instantiate(playerPrefab);
        //AtualizarUI();


    }



    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {

        CreateMeteoros(); //spawna uma distração no background
        }
    }

    public void UpdateScore(int player) //Score antigo de properties deprecado
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

            /*if(p1score = 3 && p2score = 3)
            {

            }*/
        }

    public void EscolherMaster(Mao Escolha) //Salva o valor da escolha do Master em um Inteiro utilizando o Enum Mao
    {
        if(player1ready == false)//se o bool estiver marcado com falso você ainda pode escolher pedra papel ou tesoura
        {
        myPhotonView.RPC("EscolherMaster_RPC",RpcTarget.All,(int)Escolha);
        }
        else
        {
            Debug.Log("EscolhaMaster ja Feita");
        }

    }

    public void EscolherClient(Mao Escolha)//Salva o valor da escolha do Client em um Inteiro utilizando o Enum Mao
    {
        if(player2ready == false)//se o bool estiver marcado com falso você ainda pode escolher pedra papel ou tesoura
        {
        myPhotonView.RPC("EscolherClient_RPC",RpcTarget.All,(int)Escolha);
        }
        else
        {
            Debug.Log("EscolhaClient ja Feita");
        }
    }
    [PunRPC]
    public void EscolherMaster_RPC(int Escolha)
    {

        MasterChoice = (Mao)Escolha;
        Debug.Log(MasterChoice + "Master");
        player1ready = true;
        VerifReady();// verifica se as booleanas estão ambas como true
    }
    [PunRPC]
    public void EscolherClient_RPC(int Escolha)
    {

        ChallengerChoice = (Mao)Escolha;
        Debug.Log(ChallengerChoice + "Client");
        player2ready = true;
        VerifReady();
    }

    void VerifReady()
    {
        if(player1ready == true && player2ready == true )
        {
            int Vencedor = VerificarResultado(MasterChoice, ChallengerChoice);
            myPhotonView.RPC("DebugVencedor",RpcTarget.All, Vencedor); //manda um debug mostrando a escolha do vencedor usando o método Verificarresultado()
        }
    }

    [PunRPC]
    public void DebugVencedor(int Vencedor) //metodo que decide qual jogador Venceu e manda para a cena do GameOver
    {
        Debug.Log("Quem venceu é o player  " + Vencedor);
        PhotonNetwork.LoadLevel(2);
        Debug.Log("Deu Load no Gameover pelo DebugVencedor");
    }

    public IEnumerator CountdownDecision()
    {
        yield return new WaitForSeconds(3);
        results = VerificarResultado(MasterChoice, ChallengerChoice);

        Debug.Log("Resultado saiu!! " + results);
        Vencedor(MasterChoice, ChallengerChoice);
        Debug.Log("Vencedor foi chamado");
    }

    public void VencerJogo(int Escolha)
    {
        photonView.RPC("VencerJogo_RPC",RpcTarget.MasterClient,(int)Escolha);
    }

    public void VencerJogo_RPC()
    {

    }

    public int VerificarResultadoDefault() //Passa o Valor do VerificarResultado sem precisar acessar o MasterChoice, ChallengerChoice em outra classe
    {
        return VerificarResultado(MasterChoice, ChallengerChoice);
    }

    public int VerificarResultado(Mao MasterChoice, Mao ChallengerChoice)
    {
        switch ((MasterChoice, ChallengerChoice))
        {
            case (Mao.Rock, Mao.Scissors): //esses cases fazem o Jogador 1 Vencer (MasterChoice)
            case (Mao.Paper, Mao.Rock):
            case (Mao.Scissors, Mao.Paper):
                return 1;
            case (Mao.Rock, Mao.Paper):     //esses cases fazem o Jogador 2 Vencer (ChallengerChoice)
            case (Mao.Paper, Mao.Scissors):
            case (Mao.Scissors, Mao.Rock):
                return 2;

            default:            //se empatar ninguém vence
                return 0;
        }
    }

    public void Vencedor(Mao MasterChoice, Mao ChallengerChoice) //Metodo Deprecado que ajudou a chegar no switch case do metodo acima
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
                Debug.Log("Deu Load no GameOver pelo Properties");
            }
        }
    }
    public void ResetSingleton() //reseta o Singleton se chamado enquanto acessa o valor em outra classe
    {
        Debug.Log("6. Entrou no reset Singleton");
        Destroy(gameObject);
        Debug.Log("7. Destruiu o GameObject");
    }
}
