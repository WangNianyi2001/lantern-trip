using System.Collections;
using System.Collections.Generic;
using LanternTrip;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public UnityEvent onAce;
    private List<Enermy> enemys = new List<Enermy>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            enemys.Add(transform.GetChild(i).GetComponent<Enermy>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckAce();
    }

    void CheckAce()
    {
        int i;
        for (i = 0; i < enemys.Count ; i++)
        {
            if (enemys[i].curHp.Value > 0f)
            {
                return;
            }
        }

        onAce?.Invoke();
        Debug.Log("怪全死了");
    }


    
}