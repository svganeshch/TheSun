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

    private readonly int count = 1;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private Transform[] tpoints;

    [SerializeField]
    private float dashSpeed = 1f;

    [SerializeField]
    private float dashTime = 1f;

    private float currentDashTime;

    public float Health { get => health; set => health = value; }
    public float HealthMax { get => healthMax; set => healthMax = value; }
    public float Speed { get => speed; set => speed = value; }
    public float DashSpeed { get => dashSpeed; set => dashSpeed = value; }
    public float DashTime { get => dashTime; set => dashTime = value; }
    public Animator PlayerAnimator { get => playerAnimator; set => playerAnimator = value; }
    public Transform PlayerPosition { get => playerPosition; set => playerPosition = value; }
    public Rigidbody2D PlayerRigidbody { get => playerRigidbody; set => playerRigidbody = value; }

    public virtual void UpdateHealth() { }

    private void Start()
    {

    }

    void Update()
    {
        setAnims();
    }

    public void OnMovement(InputValue readMove)
    {
        //Debug.Log("value : " + readMove.Get<Vector2>());
        x = readMove.Get<Vector2>().x;
        y = readMove.Get<Vector2>().y;
    }

    public void OnJump(InputValue readJump)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Debug.Log("Jump pressed");
        StartCoroutine(Dash(mousePos - playerPosition.position));
    }

    private void FixedUpdate()
    {
        //if (x != 0 && y != 0)
        //    return;
        playerPosition.position += new Vector3(x, y) * speed * Time.deltaTime;
    }

    public void setAnims()
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

    private IEnumerator Dash(Vector3 direction)
    {
        currentDashTime = dashTime;

        while (currentDashTime > 0f)
        {
            currentDashTime -= Time.deltaTime;
            playerRigidbody.velocity = direction * dashSpeed;
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
