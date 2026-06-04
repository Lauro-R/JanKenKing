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
    public static GameOverController _GameOverSingleton;

    private void Awake()
    {
        if (_GameOverSingleton != null && _GameOverSingleton != this)
        {

            Destroy(this.gameObject);
        }
        else
        {

            _GameOverSingleton = this;
        }
    }

    [SerializeField]
    TMP_Text textoResultados;
    [SerializeField]
    public TMP_Text textoWon;
    // Start is called before the first frame update
    void Start()
    {
        int Resultado = GameController._GameSingleton.VerificarResultadoDefault();

        MostrarResultados(Resultado);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MostrarResultados(int ResultadoFinal)
    {
        switch (ResultadoFinal)
        {
            case 0:
            textoWon.text = "Empatou!";
                break;
            case 1:
            textoWon.text = "Player 1 Venceu!";
                break;
            case 2:
            textoWon.text = "Player 2 Venceu!";
                break;
        }
    }

    public void BackMenu()
    {
        GameController._GameSingleton.ResetSingleton();
        PhotonNetwork.Disconnect();
        Debug.Log("2. BackMenu - Disconectado");
        SceneManager.LoadScene(0);
    }
}
