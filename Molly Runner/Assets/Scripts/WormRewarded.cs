using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormRewarded : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkManager.instance.Invoke("ShowRewarded", 0f);
            GameManager.instance.hasAds = true;
        }
    }
}