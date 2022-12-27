using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyController : MonoBehaviour, IEnemy
{
    private Player playerReference;
    private Vector3 playerPosition;
    private float detectionRange;
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float enemyHealth;
    private float enemyMaxHealth;
    private SpriteRenderer enemyColor;
    private RaycastHit2D enemyRay;

    private LayerMask playerLayer;

    public UIManager uimanager;
    public GameObject battery;
    private bool batteryDropped = false;

    public float Health { get => enemyHealth; set => enemyHealth = value; }
    public float Speed { get => speed; set => speed = value; }
    public float DetectionRange { get => detectionRange; set => detectionRange = value; }

    public void Start()
    {
        gameObject.tag = "Enemy";
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        enemyMaxHealth = Health;
        playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerLayer = LayerMask.GetMask("Player");
        uimanager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        InitialiseStart();
    }

    public void FixedUpdate()
    {
        EnemyColliderTrigger();
    }

    private void Update()
    {
        
    }

    public abstract void InitialiseStart();

    public virtual void EnemyColliderTrigger()
    {
        Collider2D[] outerCollider= Physics2D.OverlapCircleAll(transform.position, detectionRange,playerLayer);
        
        foreach (Collider2D collider in outerCollider)
        {
            if (collider.CompareTag("Player"))
            {
                playerPosition = collider.transform.position;
                TrackHealth();
                ChasePlayer();

                Debug.Log("This is enemy " + gameObject.name + " health : " + enemyHealth);

                enemyRay = Physics2D.Raycast(transform.position, collider.transform.position - transform.position, detectionRange, playerLayer);
                if (enemyRay.collider != null)
                {
                    Debug.Log("Distance to player : " + enemyRay.distance);
                    if (enemyRay.distance < 0.5f)
                    {
                        playerReference.playerEvent.Invoke();
                    }
                }
            }
        }
    }
   
    public virtual void ChasePlayer()
    {
        if (Vector2.Distance(transform.position, playerPosition) > 4)
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
    }

    public virtual void EvadePlayer()
    {
        Debug.Log("Distance : " + Vector2.Distance(transform.position, playerPosition));
        if (Vector2.Distance(transform.position, playerPosition) < 2)
        {
            Debug.Log("Getting Away!!");
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, -speed * Time.deltaTime);
        }
    }

    public void TrackHealth()
    {
        enemyColor = gameObject.GetComponent<SpriteRenderer>();
        var enemyAlpha = enemyColor.color;
        enemyAlpha.a = (enemyHealth / enemyMaxHealth) * 1;
        enemyColor.color = enemyAlpha;
        Debug.Log("calc color alpha : " + enemyAlpha.a);
        Debug.Log("enemy color alpha : " + enemyColor.color.a);

        if (enemyHealth <= 100)
        {
            EvadePlayer();
        }

        if (enemyHealth <= 0) DestroyEnemyObject();
    }

    public virtual void DestroyEnemyObject()
    {
        Destroy(gameObject, 0.3f);

        if (!batteryDropped) {
            Instantiate(battery, transform.position, Quaternion.identity);
            batteryDropped = true;
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Enemy destroyed");
        uimanager.SetScoreText();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawRay(transform.position, playerPosition - transform.position);
    }
}
