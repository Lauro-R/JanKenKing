using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Library da Photon
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; //Está delineando que Hashtable significa utilizar a Hashtable da Photon

public class MenuController : MonoBehaviourPunCallbacks  //Utilizando um callback, Retorno de uma Função, colocando MonoBehaviourPunCallbacks contem tudo que um monobehavior tem e também possui os callbacks da Photon
{

    [SerializeField]
    GameObject inicio, lobby, sala, engajamento, usuario;

    [SerializeField]
    TMP_InputField inputName, inputRoom;

    [SerializeField]
    TMP_Text mensagemRoom, joinRandomfailedMessage;

    [SerializeField]
    GameObject btnStartGame;


    // Start is called before the first frame update
    void Start()
    {
        AbrirTela(0);


    }

    public override void OnConnected() //conecta com o servidor da photon
    {
        Debug.Log("1- Me conectei");
    }

    public void Conectando()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster() //permite o jogo dar poder de criar salas com o multiplayer
    {
        Debug.Log("2- Me conectei na Master");
        StartCoroutine("ReturnPing", 1f); //mostra o ping do jogador quando conectado ao servidor
        usuario.SetActive(true);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    IEnumerator ReturnPing(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.LogWarning("3- Server Region : " + PhotonNetwork.CloudRegion);
        Debug.LogWarning("3- Ping: " + PhotonNetwork.GetPing());
        Debug.LogWarning("3- _______________");
        //StartCoroutine("ReturnPing", 1f);
    }

    public void EntrarLobby()
    {
        Debug.Log("4- Tentando entrar em um Lobby");
        PhotonNetwork.JoinLobby();
        inicio.SetActive(false);
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("5- Entrei em um Lobby");
        AbrirTela(Telas.Lobby);
    }

    public void EntrarRoom()
    {
        Debug.Log("6- Bora numa room");
        PhotonNetwork.JoinRandomRoom();
        lobby.SetActive(false);
        sala.SetActive(true);

        /*string roomName = "Room_" + Random.Range(1, 999); //gera a sala para entrar no range de 1 a 999
        Debug.Log("Criando Room " + roomName);

        PhotonNetwork.CreateRoom(roomName);*/

    }
    public override void OnJoinedRoom()
    {
        AbrirTela(Telas.Salas);
        Debug.Log("7- danrandanROOM");
        mensagemRoom.text = "Entrei no " + PhotonNetwork.CurrentRoom.Name + " com " +
            PhotonNetwork.CurrentRoom.PlayerCount + " players";

            SetPlayerProperties();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("8- Falhou entrar num Room");
        joinRandomfailedMessage.text = "Não tem room disponível";

    }

    public override void OnCreatedRoom()
    {
        Debug.Log("10- Sala Criada com Sucesso");
        Hashtable roomProperties = new Hashtable();
        roomProperties.Add("Room Name", inputRoom.text);
        roomProperties.Add("PlayersReady", 0);

        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("9- Entraste");
        mensagemRoom.text = "Entrei no " + PhotonNetwork.CurrentRoom.Name + " com " +
            PhotonNetwork.CurrentRoom.PlayerCount + " players";
    }
    public void AbrirTela(Telas tela)
    {
        AbrirTela((int) tela);
    }

    public void AbrirTela(int tela)
    {
        FecharTelas();

        switch ((Telas)tela)
        {
            case Telas.Engajamento:
                engajamento.SetActive(true);
                break;

            case Telas.Salas:
                sala.SetActive(true);
                btnStartGame.SetActive(false);
                    //btnStartGame.SetActive(PhotonNetwork.IsMasterClient); //é o mesmo que o um if else para checar se é o master client, com o photon dentro do set active voce já está perguntando o true or false dentro do set active.
                break;
            case Telas.Usuario:
                PhotonNetwork.ConnectUsingSettings();
                //usuario.SetActive(true);
                break;
            case Telas.Lobby:
                lobby.SetActive(true);
                break;
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        object playersReady;
        if (PhotonNetwork.IsMasterClient && propertiesThatChanged.TryGetValue("PlayersReady", out playersReady))
        {
            if ((int)playersReady == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                btnStartGame.SetActive(true);
            }
            else
            {
                btnStartGame.SetActive(false);
            }
        }
    }

    void FecharTelas()
    {
        inicio.SetActive(false);
        lobby.SetActive(false);
        sala.SetActive(false);
        usuario.SetActive(false);
        engajamento.SetActive(false);
    }

    public void Conectar()
    {
        SalvarNick();
        PhotonNetwork.JoinLobby();
        AbrirTela(Telas.Salas);
    }

    void SalvarNick()
    {
        string defname = inputName.text;
        if (defname == "")
        {
            defname = "player_" + Random.Range(0, 99);
        }

        PhotonNetwork.NickName = defname;
        Debug.Log("Nome do player é " + defname);
    }

    public void CriarRoom()
    {
        string roomName = inputRoom.text;
        if (inputRoom.text == "")
            {
            inputRoom.text = "Room_" + Random.Range(10, 999);
            }
        else
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;

            PhotonNetwork.CreateRoom(inputRoom.text, roomOptions);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        joinRandomfailedMessage.text = "Nome já em uso";
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    void SetPlayerProperties()
    {
        Hashtable playerPropertiesTemp = new Hashtable();
        playerPropertiesTemp.Add("nickname", PhotonNetwork.NickName);
        playerPropertiesTemp.Add("Score", 0);
        playerPropertiesTemp.Add("Id", PhotonNetwork.LocalPlayer.UserId);
        playerPropertiesTemp.Add("PlayerOrder", PhotonNetwork.LocalPlayer.ActorNumber);

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerPropertiesTemp); // recebe a Hashtable do jogador e envia para a Photon essa Hashtable criada quando chamar o metodo
    }

    public void GetReady(bool ready)
    {
        object playersReadyCount = 0;
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayersReady", out playersReadyCount))
        {
            int playerReady = (int)playersReadyCount;
            if (ready)
            {
                playerReady++;
            }
            else
            {
                playerReady--;
            }
            Hashtable newProperties = new Hashtable();
            newProperties.Add("PlayersReady", playerReady);
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties);
            }
        }
    }

public enum Telas { Engajamento = 0, Usuario = 1,Lobby = 2, Salas = 3 }
