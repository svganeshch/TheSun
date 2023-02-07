using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerEvent : UnityEvent<float> { }

public class Player : PlayerMovement
{
    public PlayerEvent playerEvent;
    private UIManager uimanager;

    public override void Awake()
    {
        PlayerAnimator = GetComponent<Animator>();
        PlayerPosition = GetComponent<Transform>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        uimanager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        playerEvent = new PlayerEvent();
        playerEvent.AddListener(UpdateHealth);
        playerEvent.AddListener(uimanager.SetPlayerHealthBar);

        base.Awake();
    }

    public override void Start()
    {
        HealthMax = 1000;
        Health = HealthMax;
        Speed = 75;
        DashSpeed = 15f;
        DashTime = 0.5f;

        base.Start();
    }

    public override void UpdateHealth(float damage)
    {
        if (Health > 0)
        {
            Health -= damage;
            Debug.Log("Player health decreased" + Health);
        }

        if (Health <= 0) SceneManager.LoadScene("GameOver");
    }
}
