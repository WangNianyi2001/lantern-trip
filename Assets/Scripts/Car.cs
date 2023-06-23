using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using LanternTrip;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class Car : MonoBehaviour
{
    public UnityEvent onGetOff;
    
    private bool _isTriggered = false;
    private bool _isUsed = false;
    
    private Protagonist protagonist;
    private CinemachineDollyCart DollyCart;

    private Rigidbody _rb;
    
    private float carSpeed = 0.0f;

    private void Start()
    {
        DollyCart = GetComponent<CinemachineDollyCart>();
    }

    private void Update()
    {
        if (Mathf.Abs(DollyCart.m_Position - DollyCart.m_Path.PathLength) < 0.2f)
        {
            GetOff();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isTriggered && collision.collider.CompareTag("Player"))
        {
            _isTriggered = true;
            protagonist = collision.collider.GetComponent<Protagonist>();
            if (protagonist == null)
            {
                Debug.LogError("Protagonist is null");
                return;
            }

            _rb = protagonist.GetComponent<Rigidbody>();

            StartCoroutine(DisableProtagonist());
            SpeedUpTo(100.0f);
        }
    }

    
    private void GetOff()
    {
        if (_rb == null)
        {
            return;
        }

        if (_isUsed)
        {
            return;
        }

        _isUsed = true;
        
        Debug.Log("下车");
        onGetOff?.Invoke();
        
        _rb.useGravity = true;
        _rb.isKinematic = false;
        protagonist.enabled = true;
    }

    public void SpeedUpTo(float maxSpeed)
    {
        StartCoroutine(SpeedUpCoroutine(maxSpeed));
    }

    IEnumerator DisableProtagonist()
    {
        yield return null;
        protagonist.enabled = false;
        yield return null;
        
        _rb.useGravity = false;
        _rb.isKinematic = true;
    }

    IEnumerator SpeedUpCoroutine(float maxSpeed)
    {
        while (carSpeed <= maxSpeed)
        {
            carSpeed += Time.deltaTime * 3.6f;
            DollyCart.m_Speed = carSpeed;
            yield return null;
        }
        
    }
    
}
