using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist2;

public class Melt : MonoBehaviour
{
    private VolumetricFog fog;


    private void OnTriggerStay(Collider other)
    {
        fog = other.GetComponent<VolumetricFog>();
        if (fog == null)
            return;
        Debug.Log("Hello");
        var albedo = Vector3.Distance(other.transform.position, transform.position);
        albedo = Math.Clamp(albedo * 0.5f, 0f, 1f);
        albedo = albedo * albedo;
        
        fog.albedo.a = albedo;
    }

    private void OnTriggerExit(Collider other)
    {
        fog = other.GetComponent<VolumetricFog>();
        if (fog == null)
            return;
        Debug.Log("Goodbye");
        fog.albedo.a = fog.profile.albedo.a;
    }
}
