using System.Collections;
using System.Collections.Generic;
using LanternTrip;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public UnityEvent onAce;
    private List<Enermy> enemys = new List<Enermy>(8);
    private List<EnemyManager> _enemyManagers = new List<EnemyManager>(8);

    private bool _isTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var go = transform.GetChild(i);
            var childEnemy = go.GetComponent<Enermy>();
            var childEnemyGroup = go.GetComponent<EnemyManager>();
            if (childEnemy)
            {
                enemys.Add(childEnemy);
            }
            if (childEnemyGroup)
            {
                _enemyManagers.Add(childEnemyGroup);
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

        foreach (var enemyi in enemys)
        {
            if (enemyi.curHp.Value > 0f)
            {
                return;
            }
        }

        foreach (var enemyGroupi in _enemyManagers)
        {
            if (!enemyGroupi._isTriggered) 
            { 
                return;
            }
        }

        _isTriggered = true;
        onAce?.Invoke();
        Debug.Log("怪全死了");
    }


    
}