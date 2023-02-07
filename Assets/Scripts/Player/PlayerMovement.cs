using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerMovement : MonoBehaviour, IPlayer
{
    private float health;
    private float healthMax;
    private Vector2 movementDirection;

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

    private MyInputActions inputActions;
    private InputAction movementAction;
    private InputAction dashAction;

    public float Health { get => health; set => health = value; }
    public float HealthMax { get => healthMax; set => healthMax = value; }
    public float Speed { get => speed; set => speed = value; }
    public float DashSpeed { get => dashSpeed; set => dashSpeed = value; }
    public float DashTime { get => dashTime; set => dashTime = value; }
    public Animator PlayerAnimator { get => playerAnimator; set => playerAnimator = value; }
    public Transform PlayerPosition { get => playerPosition; set => playerPosition = value; }
    public Rigidbody2D PlayerRigidbody { get => playerRigidbody; set => playerRigidbody = value; }

    public virtual void UpdateHealth(float damage) { }

    public virtual void Awake()
    {
        inputActions = new MyInputActions();
    }

    public virtual void Start()
    {
        audioSource= GetComponent<AudioSource>();
    }

    void Update()
    {
        movementDirection = movementAction.ReadValue<Vector2>();
        SetAnims();
    }

    private void FixedUpdate()
    {
        playerPosition.position += speed * Time.deltaTime * new Vector3(movementDirection.x, movementDirection.y);
    }

    public void DashAction(InputAction.CallbackContext context)
    {
        Debug.Log("Jump pressed");
        StartCoroutine(nameof(Dash));
        audioSource.PlayOneShot(dashAudio);
    }

    public void SetAnims()
    {
        if (movementDirection.x == 1)
        {
            playerAnimator.Play("walk_right");
            //Debug.Log("Playing right anim");
        }
        else if (movementDirection.x == -1)
        {
            playerAnimator.Play("walk_left");
            //Debug.Log("Playing left anim");
        }
        else if (movementDirection.y == 1)
        {
            playerAnimator.Play("walk_forward");
            //Debug.Log("Playing forward anim");
        }
        else if (movementDirection.y == -1)
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
            playerRigidbody.velocity = (new Vector2(movementDirection.x, movementDirection.y) * 5f) * dashSpeed;
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

    private void OnEnable()
    {
        movementAction = inputActions.Player.Movement;
        movementAction.Enable();

        dashAction = inputActions.Player.Dash;
        dashAction.Enable();
        dashAction.performed += DashAction;
    }

    private void OnDisable()
    {
        movementAction.Disable();
        dashAction.Disable();
    }
}
