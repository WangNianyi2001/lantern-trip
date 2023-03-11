using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LanternTrip
{
    public class Setup : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GameMgr.Instance.Init();

        }
    }
}
