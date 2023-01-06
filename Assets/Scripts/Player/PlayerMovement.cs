using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerMovement : MonoBehaviour, IPlayer
{
    private float health;
    private float healthMax;
    private float x, y;

    private Animator playerAnimator;
    private Transform playerPosition;
    private Rigidbody2D playerRigidbody;
    private AudioSource audioSource;
    public AudioClip dashAudio;

    private readonly int count = 1;

    private float speed;

    private Transform[] tpoints;

    private float dashSpeed;

    private float dashTime;

    private float currentDashTime;

    public float Health { get => health; set => health = value; }
    public float HealthMax { get => healthMax; set => healthMax = value; }
    public float Speed { get => speed; set => speed = value; }
    public float DashSpeed { get => dashSpeed; set => dashSpeed = value; }
    public float DashTime { get => dashTime; set => dashTime = value; }
    public Animator PlayerAnimator { get => playerAnimator; set => playerAnimator = value; }
    public Transform PlayerPosition { get => playerPosition; set => playerPosition = value; }
    public Rigidbody2D PlayerRigidbody { get => playerRigidbody; set => playerRigidbody = value; }

    public abstract void InitialiseStart();
    public virtual void UpdateHealth(float damage) { }

    private void Start()
    {
        audioSource= GetComponent<AudioSource>();

        InitialiseStart();
    }

    public void OnMovement(InputValue readMove)
    {
        //Debug.Log("value : " + readMove.Get<Vector2>().normalized);
        x = readMove.Get<Vector2>().x;
        y = readMove.Get<Vector2>().y;
    }

    public void OnDash(InputValue readJump)
    {
        Debug.Log("Jump pressed");
        StartCoroutine(nameof(Dash));
        audioSource.PlayOneShot(dashAudio);
    }

    void Update()
    {
        SetAnims();
    }

    private void FixedUpdate()
    {
        playerPosition.position += speed * Time.deltaTime * new Vector3(x, y);
    }

    public void SetAnims()
    {
        if (x == 1)
        {
            playerAnimator.Play("walk_right");
            //Debug.Log("Playing right anim");
        }
        else if (x == -1)
        {
            playerAnimator.Play("walk_left");
            //Debug.Log("Playing left anim");
        }
        else if (y == 1)
        {
            playerAnimator.Play("walk_forward");
            //Debug.Log("Playing forward anim");
        }
        else if (y == -1)
        {
            playerAnimator.Play("walk_down");
            //Debug.Log("Playing down anim");
        }
        else
        {
            //playeranim.Play("idle");
            //Debug.Log("Playing idle anim");
        }
    }

    private IEnumerator Dash()
    {
        currentDashTime = dashTime;

        while (currentDashTime > 0f)
        {
            currentDashTime -= Time.deltaTime;
            playerRigidbody.velocity = (new Vector2(x, y) * 5f) * dashSpeed;
            yield return null;
        }

        playerRigidbody.velocity = Vector2.zero;
    }

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Player has enetered into " + collision.name);

        if (collision.CompareTag("spawn1"))
        {
            Debug.Log("Teleporting player.....");
            yield return StartCoroutine(nameof(WaitForTeleport));
            playerPosition.position = tpoints[0].position;
        } else if (collision.CompareTag("spawn2"))
        {
            Debug.Log("Teleporting player.....");
            yield return StartCoroutine(nameof(WaitForTeleport));
            playerPosition.position = tpoints[1].position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("spawn1") || collision.CompareTag("spawn2"))
        {
            Debug.Log("Player has exited........");
            StopCoroutine(nameof(WaitForTeleport));
        }
    }

    IEnumerator WaitForTeleport()
    {
        yield return new WaitForSeconds(count);
    }
}
