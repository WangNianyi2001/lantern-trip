using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumblingSphere : MonoBehaviour
{
    public float TimeInterval = 0.5f;
    
    public GameObject Sphere;
    private Transform Slot1;
    private Transform Slot2;
    private Transform Slot3;
    
    void Start()
    {
        Slot1 = transform.GetChild(0);
        Slot2 = transform.GetChild(1);
        Slot3 = transform.GetChild(2);
        if (Sphere != null && Slot1 != null && Slot2 != null && Slot3 != null)
        {
            
        }
        StartCoroutine(SpawnSphere());
    }

    void Update()
    {
        
    }

    IEnumerator SpawnSphere()
    {
        while (Sphere!=null)
        {
            var go = GameObject.Instantiate(Sphere);
            go.transform.position = Slot1.position;
            yield return new WaitForSeconds(TimeInterval);
            
            go = GameObject.Instantiate(Sphere);
            go.transform.position = Slot2.position;
            yield return new WaitForSeconds(TimeInterval);
            
            go = GameObject.Instantiate(Sphere);
            go.transform.position = Slot3.position;
            yield return new WaitForSeconds(TimeInterval);
        }
    }
}
