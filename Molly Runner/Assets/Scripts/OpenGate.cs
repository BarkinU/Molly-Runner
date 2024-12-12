using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGate : MonoBehaviour
{
    private Animation myAnim;
    void Start()
    {
        myAnim = transform.GetChild(0).GetComponent<Animation>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnim.Play();
        }
    }
}
