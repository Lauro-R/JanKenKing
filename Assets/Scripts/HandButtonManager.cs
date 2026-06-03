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
    int results;

    [SerializeField]
    GameController _GameSingleton; //Singleton que gerencia o main menu e quando ele é clicado

    public void saveChoice()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _GameSingleton.MasterChoice = valorBotao;
            Debug.Log(valorBotao + " " + _GameSingleton.MasterChoice + "Master");
            StartCoroutine(CountdownDecision());
        }
        else
        {
            _GameSingleton.ChallengerChoice = valorBotao;
            Debug.Log(valorBotao  + " " +   _GameSingleton.ChallengerChoice + "Challenger");
        }
    }

    [PunRPC]
    public void saveChoice_RPC()
    {

    }

    public IEnumerator CountdownDecision()
    {
        yield return new WaitForSeconds(3);
        results = _GameSingleton.VerificarResultado(_GameSingleton.MasterChoice, _GameSingleton.ChallengerChoice);
        _GameSingleton.UpdateScore(results);

        Debug.Log("Resultado saiu!! " + results);
        _GameSingleton.Vencedor(_GameSingleton.MasterChoice, _GameSingleton.ChallengerChoice);
        Debug.Log("Vencedor foi chamado");
    }
}
