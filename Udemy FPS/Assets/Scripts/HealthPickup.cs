using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount;
    private bool collected = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !collected)
        {
            PlayerHealthController.instance.HealPlayer(healAmount);
            Destroy(gameObject);
        }
    }
}
