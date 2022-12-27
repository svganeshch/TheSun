using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : PlayerMovement
{
    public UnityEvent playerEvent;
    public UIManager uimanager;
    void Start()
    {
        HealthMax = 1000;
        Health = HealthMax;
        Speed= 5f;
        DashSpeed= 0.5f;
        DashTime= 0.5f;

        PlayerAnimator = GetComponent<Animator>();
        PlayerPosition = GetComponent<Transform>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();

        playerEvent.AddListener(UpdateHealth);
        playerEvent.AddListener(uimanager.SetHealthBar);

    }

    public override void UpdateHealth()
    {
        if (Health > 0)
        {
            Health -= 10;
            Debug.Log("Player health decreased" + Health);
        }
    }
}
