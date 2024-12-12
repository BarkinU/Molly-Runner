using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingButton : MonoBehaviour
{
    [SerializeField] private Animation myAnim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnim.Play();
        }
    }
}
