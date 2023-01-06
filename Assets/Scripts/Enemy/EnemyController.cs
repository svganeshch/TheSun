using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class EnemyController : MonoBehaviour, IEnemy
{
    private Player playerReference;
    private Vector3 playerPosition;
    private LayerMask playerLayer;
    private WeaponTorch torch;

    public float enemyMaxHealth;
    public float damageAmount;
    public float speed;
    public float detectionRange;

    private float enemyHealth;
    private RaycastHit2D enemyRay;
    private Animator enemyAnimator;
    private float attackDistance = 0;
    private float attackTime = 1.5f;
    private float chargeTime = 2;
    private float currentChargeTime;
    private float currentAttackTime;
    private BoxCollider2D enemyCollider;
    private Rigidbody2D enemyRB;
    public AudioClip[] enemyMedia;
    private AudioSource audioSource;

    public Image enemyHealthBar;
    private float currentEnemyHealth;
    private float currentEnemyHealthPercentage;

    public UIManager uimanager;
    public GameObject battery;
    private bool batteryDropped = false;
    public EnemySpawner enemySpawner;

    private bool isSpawnDone = false;

    private FovDetector fovDetector;

    AIPath enemyAI;
    AIDestinationSetter enemyAIDestination;

    private Transform getAwayDestination;
    private List<Transform> MyGetAwayPoints;
    private bool isMovingToDestination = false;

    private float currentRegenTime;
    private float regenTime = 2f;
    private bool isAtDestination;

    public float HealthMax { get => enemyMaxHealth; set => enemyMaxHealth = value; }
    public float Health { get => enemyHealth; set => enemyHealth = value; }
    public float Speed { get => speed; set => speed = value; }
    public float DetectionRange { get => detectionRange; set => detectionRange = value; }
    public float DamageAmount { get => damageAmount; set => damageAmount = value; }

    public abstract void InitialiseStart();

    public void SetSpawnDone()
    {
        enemyAnimator.SetBool("isSpawnDone", true);
        isSpawnDone = true;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<BoxCollider2D>();
        enemyRB = GetComponentInParent<Rigidbody2D>();

        playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerLayer = LayerMask.GetMask("Player");
        torch = FindObjectOfType<WeaponTorch>();

        uimanager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        enemySpawner = GameObject.Find("Spawner").GetComponent<EnemySpawner>();

        enemyAI = GetComponentInParent<AIPath>();
        enemyAIDestination = GetComponentInParent<AIDestinationSetter>();

        fovDetector = GetComponentInChildren<FovDetector>();

        MyGetAwayPoints = FindObjectOfType<AccumulatePoints>().getAwayPoints;
    }

    public void Start()
    {
        playerPosition = playerReference.transform.position;

        Health = HealthMax;

        gameObject.tag = "Enemy";
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        enemyAI.maxSpeed = speed;
        enemyAIDestination.target = playerReference.transform;

        InitialiseStart();
    }

    public void Update()
    {
        playerPosition = playerReference.transform.position;

        if (isSpawnDone)
        {
            TrackPlayer();
            FlipSprite();
            EnemyColliderTrigger();

            EvadeCheck();
        }
    }

    private void EvadeCheck()
    {
        if ((enemyHealth / enemyMaxHealth) <= 0.4 || enemyAnimator.GetBool("isCharging"))
        {
            if (!isMovingToDestination)
            {
                EvadePlayer();
            }
            else
            {
                RegenHealth();
            }
        }
        else
        {
            isMovingToDestination = false;
            enemyAIDestination.target = playerReference.transform;
        }
    }

    public void TrackPlayer()
    {
        if (!isMovingToDestination) enemyAI.maxSpeed = speed;

        if (isAtDestination)
        {
            //Idle
            enemyAnimator.SetBool("isRunning", false);
            enemyAnimator.SetBool("isAttacking", false);
        }
        else
        {
            // Running
            enemyAnimator.SetBool("isAttacking", false);
            enemyAnimator.SetBool("isRunning", true);
            Debug.Log("State : Run");
        }

        //Charging
        if (currentAttackTime > attackTime)
        {
            currentChargeTime += Time.deltaTime;
            enemyAnimator.SetBool("isChargingDone", false);
            enemyAnimator.SetBool("isAttacking", false);
            enemyAnimator.SetBool("isCharging", true);

            enemyAI.maxSpeed = speed * 1.5f;

            if (currentChargeTime > chargeTime)
            {
                currentChargeTime = 0;
                currentAttackTime = 0;

                enemyAnimator.SetBool("isCharging", false);
                enemyAnimator.SetBool("isChargingDone", true);
            }
        }
    }

    private void EvadePlayer()
    {
        int randIndex = Random.Range(0, MyGetAwayPoints.Count);
        Transform currentGetAwayPoint = MyGetAwayPoints[randIndex];
        //currentGetAwayPoint.position = new Vector3(currentGetAwayPoint.position.x * 2, currentGetAwayPoint.position.y * 2, 0);

        getAwayDestination = currentGetAwayPoint;

        Debug.Log("Getting Away!!");
        enemyAIDestination.target = getAwayDestination;
        enemyAI.maxSpeed = speed * 2f;
        isMovingToDestination = true;
    }

    public virtual void EnemyColliderTrigger()
    {
        Collider2D[] outerCollider= Physics2D.OverlapCircleAll(transform.position, detectionRange, playerLayer);

        if (outerCollider.Length == 0) return;
        
        foreach (Collider2D collider in outerCollider)
        {
            if (collider.CompareTag("Player"))
            {
                if (enemyHealth > 0) AttackPlayer();

                enemyRay = Physics2D.Raycast(transform.position, collider.transform.position - transform.position, detectionRange, playerLayer);
                if (enemyRay.collider != null)
                {
                    attackDistance = enemyRay.distance;
                }
            }
        }
    }
   
    public virtual void AttackPlayer()
    {
        //Attack
        if (attackDistance < detectionRange-1 && currentAttackTime < attackTime && fovDetector.hasEntered)
        {
            currentAttackTime += Time.deltaTime;

            enemyAnimator.SetBool("isCharging", false);
            enemyAnimator.SetBool("isRunning", false);
            enemyAnimator.SetBool("isAttacking", true);

            //audioSource.PlayOneShot(enemyMedia[0]);

            Debug.Log("State : Attack");
            //Debug.Log("attack current time" + currentAttackTime);
        }
    }

    public virtual void FlipSprite()
    {
        Vector3 refPos;

        if (isAtDestination)
            refPos = playerPosition;
        else if (isMovingToDestination)
        {
            refPos = getAwayDestination.position;
        }
        else
            refPos = playerPosition;

        if (refPos.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (refPos.x < transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);

        //if (enemyRB.velocity.x >= 0.01f)
        //    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        //else if (enemyRB.velocity.x <= -0.01f)
        //    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
    }

    public void SetEnemyHealthBar()
    {
        if (enemyHealth <= 0) return;

        currentEnemyHealth = enemyHealth;
        currentEnemyHealthPercentage = currentEnemyHealth / enemyMaxHealth;
        enemyHealthBar.fillAmount = currentEnemyHealthPercentage;

        Debug.Log("current enemy health : " + currentEnemyHealth + "percent : " + currentEnemyHealthPercentage);
    }

    public void UpdateHealth(float damage)
    {
        enemyHealth -= damage;
        SetEnemyHealthBar();

        //enemyAnimator.SetBool("isHit", true);

        if (enemyHealth <= 0) DestroyEnemyObject();
    }

    private void RegenHealth()
    {
        currentRegenTime += Time.deltaTime;
        if (currentRegenTime > regenTime)
        {
            enemyHealth += 50;
            currentRegenTime = 0;

            Debug.Log("Regen health " + enemyHealth);
            SetEnemyHealthBar();
        }
    }

    public virtual void DestroyEnemyObject()
    {
        audioSource.PlayOneShot(enemyMedia[1]);

        enemyCollider.enabled= false;
        enemyAnimator.SetBool("isDead", true);

        uimanager.SetScoreText();

        if (!batteryDropped) {
            bool toDrop = Random.value < 0.5f;

            if (torch.TorchCharge <= 250) toDrop= true;

            if (toDrop)
            {
                Instantiate(battery, transform.position, Quaternion.identity);
                batteryDropped = true;
            }
        }

        if (enemySpawner!= null)
            enemySpawner.EnemyDead();

        Destroy(transform.parent.gameObject, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerReference.playerEvent.Invoke(damageAmount);
            Debug.Log("player got attacked");
        }
        else if (collision.CompareTag("GetAwayPoints"))
        {
            if (isMovingToDestination)
            {
                if (getAwayDestination.name == collision.name)
                {
                    isAtDestination = true;
                    Debug.Log("reached destination");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("GetAwayPoints"))
        {
            isAtDestination = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawRay(transform.position, playerPosition - transform.position);
    }
}
