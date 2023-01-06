using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public float destroyTime = 5;
    public float chargeAmount = 200;

    public WeaponTorch torch;

    private void Awake()
    {
        torch = FindObjectOfType<WeaponTorch>();
    }
    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (torch != null )
            {
                torch.ChargeTorch(chargeAmount);
                gameObject.SetActive(false);
            }
        }
    }
}
