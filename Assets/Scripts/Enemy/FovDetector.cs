using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovDetector : MonoBehaviour
{
    public bool hasEntered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hasEntered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hasEntered = false;
        }
    }
}
