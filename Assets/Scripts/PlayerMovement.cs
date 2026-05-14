using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    PhotonView myPhotonview;

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

}
