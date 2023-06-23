using System.Collections;
using System.Collections.Generic;
using LanternTrip;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public UnityEvent onAce;
    private List<Enermy> enemys;
    private List<EnemyManager> enemyGroups;

    private bool _isTriggered = false;

    public bool IsTriggered
    {
        get
        {
            return _isTriggered;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Enermy childEnemy = transform.GetChild(i).GetComponent<Enermy>();
            EnemyManager childEnemyManager = transform.GetChild(i).GetComponent<EnemyManager>();

            if (childEnemy != null)
            {
                enemys.Add(childEnemy);
            }

            if (childEnemyManager != null)
            {
                enemyGroups.Add(childEnemyManager);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isTriggered)
        {
            return;
        }
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

        for (int j = 0; j < enemyGroups.Count; j++)
        {
            if (!enemyGroups[i].IsTriggered)
            {
                return;
            }
        }

        _isTriggered = true;
        onAce?.Invoke();
        Debug.Log("怪全死了");
    }


    
}