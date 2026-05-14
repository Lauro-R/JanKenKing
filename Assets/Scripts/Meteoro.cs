using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteoro : MonoBehaviour
{


    public void SetDirection(float velY)
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, velY);
    }

}
