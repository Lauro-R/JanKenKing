using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerMovement : MonoBehaviour
{
    PhotonView myPhotonview;

    int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        myPhotonview = GetComponent<PhotonView>();

        if (myPhotonview.IsMine)
        {
            GerarCor();
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (myPhotonview.IsMine)
        {
            Move();
        }
    }

    void Move()
    {
        //float dirX, dirY;
         //dirX = Input.GetAxis("Horizontal");
         //dirY = Input.GetAxis("Vertical");

         //transform.Translate(new Vector2(dirX, dirY) * Time.deltaTime * 8f);

        Vector2 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(newPos.x , newPos.y);

        GetComponent<Rigidbody2D>().velocity = new Vector2(newPos.x , newPos.y);

    }

    public void GerarCor()
    {
        byte r, g, b;
        r = (byte)Random.Range(0, 255);
        g = (byte)Random.Range(0, 255);
        b = (byte)Random.Range(0, 255);

        myPhotonview.RPC("GerarCor_RPC", RpcTarget.AllBuffered, r, g, b);
    }

    [PunRPC]
    public void GerarCor_RPC(byte r, byte g, byte b)
    {

        GetComponent<Renderer>().material.color = new Color32(r, g, b, 255);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (myPhotonview.IsMine && collision.gameObject.tag == "Meteoro")
        {

            PhotonNetwork.Destroy(collision.gameObject);
            UpdateScore(1);

            //PhotonNetwork.Destroy(collision.gameObject.GetPhotonView());

            /*if (PhotonNetwork.IsMasterClient)
            {
                GoGameOver();
            }
            else
            {
                GetComponent<PhotonView>().RPC("GoGameOver", RpcTarget.MasterClient);
                }*/
        }
    }

    void UpdateScore(int value)
    {
        object currentScore;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Score", out currentScore)) //se tiver um valor Score ele entra no if com o valor na mão já
        {
            int newScore = (int)currentScore + value;
            Hashtable updatedScore = new Hashtable();
            updatedScore.Add("Score", newScore);
            PhotonNetwork.LocalPlayer.SetCustomProperties(updatedScore);
        }
    }

    [PunRPC]
    public void GoGameOver()
    {
        PhotonNetwork.LoadLevel(2);
    }

}
