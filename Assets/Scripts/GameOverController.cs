using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameOverController : MonoBehaviour
{
    [SerializeField]
    TMP_Text textoResultados;
    [SerializeField]
    public TMP_Text textoWon;
    // Start is called before the first frame update
    void Start()
    {
        MostrarResultados();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void MostrarResultados()
    {
        //textoWon.text = GameController.instance.nomeWon + " Venceu!";
        object pWon;
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Won", out pWon))
        {
            textoWon.text += (string)pWon + " Venceu! ";
        }

        string textoFinal = "";
        Player[] playersList = PhotonNetwork.PlayerList;
        foreach (Player playerAtual in playersList)
        {
            /*object idPlayer;
            if(playerAtual.CustomProperties.TryGetValue("Id", out idPlayer))
            {
                textoFinal += (string)idPlayer + " - ";
            }
            else
            {
                textoFinal += "0000: ";
                }*/

            object nicknamePlayer;
            if(playerAtual.CustomProperties.TryGetValue("Nick", out nicknamePlayer))
            {
                textoFinal += (string)nicknamePlayer+" : ";
            }
            else
            {
                textoFinal += "nonName: ";;
            }

            object scorePlayer;
            if  (playerAtual.CustomProperties.TryGetValue("Score", out scorePlayer))
            {
                textoFinal += (int)scorePlayer + "points \n";
            }
            else
            {
                textoFinal += "0 points \n";
            }
        }
        textoResultados.text = textoFinal;
    }
    public void BackMenu()
    {
        GameController.instance.ResetSingleton();
        PhotonNetwork.Disconnect();

        SceneManager.LoadScene(0);
    }
}
