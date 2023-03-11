using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LanternTrip
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        public void Attack(TinderType element, Character hit)
        {
            // damage
            if (element - hit.element == 0)
            {
                hit.HP -= 100;
            }
            else
            {
                hit.HP -= 50;
            }

            if (hit.HP <= 0)
            {
                Kill(hit);
            }
        }

        public void Kill(Character tar)
        {
            tar.gameObject.SetActive(false);
        }
    }
}
