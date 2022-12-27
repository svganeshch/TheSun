using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Battery : MonoBehaviour
{
    float destroyTime = 3;
    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
