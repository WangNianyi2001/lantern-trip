using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CartController : MonoBehaviour
{

    //public CinemachineVirtualCamera cam;
    public CinemachineDollyCart cart;
    //public float speed;

    public void changeCartSpeed(float speed)
    {
        cart.m_Speed = speed;
    }
}
